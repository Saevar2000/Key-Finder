using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Crawler
{
    class ParseWebsite
    {
        public void Parse(string link)
        {
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
                var data = new MyWebClient().DownloadString(link);
                var document = new HtmlDocument();
                document.LoadHtml(data);

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
                            if (node.InnerText.Length > 48)
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
                                    streamWriter.WriteLine(word);
                                    Console.WriteLine(word);
                                }
                            }
                        }
                    }
                }
                File.Copy(tempFileName, uniqueFileName, true);
            }
            finally
            {
                File.Delete(tempFileName);
            }
        }
    }
}
