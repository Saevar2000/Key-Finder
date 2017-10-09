using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler
{
    class Program
    {
        // Keeps track of what has been scanned in each session
        static List<string> scannedList = new List<string>();

        static void Main(string[] args)
        {
            ParseWebsite parseWebsite = new ParseWebsite();
            string startingURL = "https://pastebin.com/LxBhVLPM";

            // Manually parse startingURL so it doesn't get skipped
            parseWebsite.Parse(startingURL);

            // Start crawling from startingURL and parse everything from there
            CrawlAndParse(startingURL);

            Console.ReadKey();
        }

        private static void CrawlAndParse(string url)
        {
            ParseWebsite parseWebsite = new ParseWebsite();

            var data = new MyWebClient().DownloadString(url);
            var document = new HtmlDocument();
            document.LoadHtml(data);

            if (data == "Please refresh the page to continue...")
            {
                Console.WriteLine("Timed out, sleeping for 5 seconds and trying again");
                Thread.Sleep(5000);
                CrawlAndParse(url); // Try again
            }

            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                foreach (var link2 in att.Value.Split(' '))
                {
                    if (link2.StartsWith("http"))
                    {
                        if (scannedList.Contains(link2))
                        {
                            Console.WriteLine($"Already scanned: {link2}");
                        }
                        else
                        {
                            scannedList.Add(link2);

                            Console.WriteLine($"Scanning: {link2}");
                            parseWebsite.Parse(link2);
                            CrawlAndParse(link2); 

                            Console.WriteLine("Waiting 3 seconds");
                            Thread.Sleep(3000);
                        }
                    }
                    else
                    {
                        if (scannedList.Contains(link2))
                        {
                            Console.WriteLine($"Already scanned: {url}{link2}");
                        }
                        else
                        {
                            Console.WriteLine($"Scanning: {url}{link2}");
                            CrawlAndParse(url + link2);
                        }
                    }
                }
            }
        }
    }
}
