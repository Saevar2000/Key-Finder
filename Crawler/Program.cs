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
        // Keeps track of what has been scanned in each session.
        static List<string> scannedList = new List<string>();

        static void Main(string[] args)
        {
            ParseWebsite parseWebsite = new ParseWebsite();
            List<string> scannedLinks = new List<string>();
            List<string> toBeScannedLinks = new List<string>();
            string startingURL = "https://pastebin.com/LxBhVLPM";

            // Download site from url.
            HtmlDocument startDocument = parseWebsite.DownloadSite(startingURL);

            // get keys from the starting website here so it doesn't get skipped.
            parseWebsite.GetKeys(startDocument);

            foreach (var item in parseWebsite.GetLinks(startDocument))
            {
                parseWebsite.GetKeys(startDocument);
                scannedLinks.Add(item);
            }

            CrawlAndParse(toBeScannedLinks); 

            Console.ReadKey();
        }

        private static void CrawlAndParse(List<string> toBeScannedLinks)
        {
            ParseWebsite parseWebsite = new ParseWebsite();
            List<string> linksFound = new List<string>();

            foreach (var url in toBeScannedLinks)
            {
                HtmlDocument document = parseWebsite.DownloadSite(url);
                linksFound.AddRange(parseWebsite.GetLinks(document));
                parseWebsite.GetKeys(document);
            }

            CrawlAndParse(linksFound);
        }
    }
}
