using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Crawl
{
    class CrawlParse
    {
        Validator validator = new Validator();

        public List<string> Parse(string url)
        {
            var baseUrl = new Uri(url);

            string data = new MyWebClient().DownloadString(url);
            HtmlDocument document = new HtmlDocument();

            document.LoadHtml(data);

            if (data == "Please refresh the page to continue...")
            {
                Console.WriteLine("rip");
                //return null;
            }

            // Get Keys
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
                                        if (validator.ValidatePrivateKey(word))
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

            // Prevent crash if site doesn't contain a link
            if (document.DocumentNode.SelectNodes("//a[@href]") == null)
            {
                Console.WriteLine("Site doesn't contain a link");
                return null;
            }

            // Get links
            List<string> links = new List<string>();

            foreach (var node in document.DocumentNode.SelectNodes("//a[@href]"))
            {
                HtmlAttribute att = node.Attributes["href"];
                foreach (var link2 in att.Value.Split(' '))
                {
                    if (!links.Contains(link2))
                    {
                        if (link2.StartsWith("http"))
                        {
                            Uri finalURL = new Uri(baseUrl, link2); 
                            // Console.WriteLine(finalURL);
                            links.Add(finalURL.ToString());
                        }
                        else
                        {
                            Uri finalURL = new Uri(baseUrl, link2);
                            // Console.WriteLine(finalURL);
                            links.Add(finalURL.ToString());
                        }
                    }
                }
            }

            return links;
        }
    }
}
