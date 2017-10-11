using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler
{
    class ParseWebsite
    {

        // Downloads the website
        public HtmlDocument DownloadSite(string url)
        {
            string data = new MyWebClient().DownloadString(url);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(data);

            if (data == "Please refresh the page to continue...")
            {
                Console.WriteLine("Timed out, sleeping for 5 seconds and trying again");
                Thread.Sleep(5000);
                DownloadSite(url);
            }

            return document;
        }

        public List<string> GetLinks(HtmlDocument document)
        {
            List<string> validLinks = new List<string>();

            foreach (HtmlNode link in document.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = link.Attributes["href"];
                foreach (var link2 in att.Value.Split(' '))
                {
                    if (validLinks.Contains(link2))
                    {
                        if (link2.StartsWith("http"))
                        {
                            validLinks.Add(link2);
                            Console.WriteLine(link2);
                        }
                        else
                        {
                            // TODO: make this url + link2
                            validLinks.Add(link2);
                            Console.WriteLine(link2);
                        }
                    }
                }
            }

            return validLinks;
        }

        public List<string> GetKeys(HtmlDocument document)
        {
            List<string> possibleKeys = new List<string>();

            try
            {
                Directory.CreateDirectory("Scan-Me");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            string uniqueFileName = $@"Scan-Me/{Guid.NewGuid()}.txt";

            try
            {   
                document.DocumentNode.Descendants()
                .Where(n => n.Name == "script" || n.Name == "style" || n.Name == "#comment")
                .ToList()
                .ForEach(n => n.Remove());

                using (TextWriter tw = new StreamWriter(uniqueFileName))
                {
                    foreach (var node in document.DocumentNode.SelectNodes("//text()"))
                    {
                        if (!string.IsNullOrWhiteSpace(node.InnerText))
                        {
                            if (node.InnerText.Length > 48) // All keys are bigger than 48
                            {
                                tw.WriteLine(node.InnerText.Trim());
                            }
                        }
                    }
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }

            var tempFileName = Path.GetTempFileName();
            try
            {
                using (var streamReader = new StreamReader(uniqueFileName))
                using (var streamWriter = new StreamWriter(tempFileName))
                {
                    string line;
                    while ((line = streamReader.ReadLine()) != null)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            string[] words = line.Split(' ');
                            foreach (string word in words)
                            {
                                if (word.Length > 48)
                                {
                                    if (!possibleKeys.Contains(word)) // Prevent duplicates in current document
                                    {
                                        possibleKeys.Add(word);
                                        streamWriter.WriteLine(word);
                                        Console.WriteLine(word);
                                    }
                                }
                            }
                        }
                    }
                }
                File.Copy(tempFileName, uniqueFileName, true);

                // Delete file if nothing is written to it
                FileInfo f = new FileInfo(uniqueFileName);
                if (f.Length == 0)
                {
                    f.Delete();
                }
            }
            finally
            {
                File.Delete(tempFileName);
            }

            return possibleKeys;
        }
    }
}
