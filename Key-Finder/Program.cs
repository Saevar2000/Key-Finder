using NBitcoin;
using System;

namespace Key_Finder
{
    class Program
    {
        static void Main(string[] args)
        {
            // 5HpHagT65TZzG1PH3CSu63k8DbpvD8s5ip4nEB3kEsreAnchuDf -> 1EHNa6Q4Jz2uvNExL497mE43ikXhwF6kZm
            string privateKey = "5HpHagT65TZzG1PH3CSu63k8DbpvD8s5ip4nEB3kEsreAnchuDf8";

            Validator validator = new Validator();
            if (validator.ValidatePrivateKey(privateKey)) // Validate the private key first because converting it is much more heavy
            {
                Console.WriteLine("Converting private key to a bitcoin address...");
                BitcoinSecret secret = new BitcoinSecret(privateKey);
                var bitcoinAddressKey = secret.PrivateKey.PubKey.GetAddress(Network.Main);
                string bitcoinAddress = bitcoinAddressKey.ToString();
                Console.WriteLine($"{privateKey} -> {bitcoinAddress}");
                
                validator.ValidateBitcoinAddress(bitcoinAddress);
            }

            Console.ReadLine();
        }
    }
}
