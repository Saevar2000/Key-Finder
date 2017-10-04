using NBitcoin;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace Key_Finder
{
    class Program
    {
        private static string[] words;

        [STAThread]
        static void Main(string[] args)
        {            
            ReadFile();

            // 5HpHagT65TZzG1PH3CSu63k8DbpvD8s5ip4nEB3kEsreAnchuDf -> 1EHNa6Q4Jz2uvNExL497mE43ikXhwF6kZm
            //string privateKey = "tada 5HpHagT65TZzG1PH3CSu63k8DbpvD8s5ip4nEB3kEsreAnchuDf";

            Validator validator = new Validator();
            foreach (string privateKey in words)
            {
                if (validator.ValidatePrivateKey(privateKey)) // Validate the private key first because converting it is much more heavy
                {
                    Console.WriteLine("Attempting to convert to a bitcoin address");
                    try {
                        BitcoinSecret secret = new BitcoinSecret(privateKey);
                        var bitcoinAddressKey = secret.PrivateKey.PubKey.GetAddress(Network.Main);
                        string bitcoinAddress = bitcoinAddressKey.ToString();
                        Console.WriteLine($"{privateKey} -> {bitcoinAddress}");

                        validator.ValidateBitcoinAddress(bitcoinAddress);
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Not a valid private key");
                        //Console.WriteLine(e);
                    }                      
                }
                else
                {
                    Console.WriteLine("Invalid key, not converting");
                }
            }

            Console.ReadKey();
        }

        private static void ReadFile() 
        {
            try
            {
                FolderBrowserDialog fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    foreach (var path in Directory.GetFiles(fbd.SelectedPath))
                    {
                        Console.WriteLine(path); // full path
                        var Files = Directory.EnumerateFiles(path, "*.txt");
                        using (StreamReader reader = new StreamReader(@"C:\Users\saevarg.h17\source\repos\Key-Finder\Key-Finder\TestFile1.txt"))
                        {
                            words = reader.ReadToEnd().Split(' ');
                        }
                    }
                }

                
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not read the file." + e);
            }
        }
    }
}
