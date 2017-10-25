using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawl
{
    class Program
    {
        static void Main(string[] args)
        {
            CrawlParse c = new CrawlParse();

            List<string> alreadyScanned = new List<string>();
            List<string> toBeScanned = new List<string>();

            toBeScanned.AddRange(c.Parse("https://pastebin.com/LxBhVLPM"));

            while(!Console.KeyAvailable)
            {
                for (int i = 0; i < toBeScanned.Count; i++)
                {
                    // Prevent duplicates
                    toBeScanned.RemoveAll(item => alreadyScanned.Contains(item));

                    Console.WriteLine(toBeScanned.ElementAt(i));
                    try
                    {
                        // Throws exception when it gets null, needs to be looked at,
                        // this does not need to be in a try
                        toBeScanned.AddRange(c.Parse(toBeScanned.ElementAt(i)));
                    }
                    catch (Exception e)
                    {
                        //Console.WriteLine(e.StackTrace);
                    }

                    toBeScanned.RemoveAt(i);

                    Console.WriteLine("toBeScanned: " + toBeScanned.Count);
                    Console.WriteLine("pages scraped: " + i);
                }
            }
            

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
