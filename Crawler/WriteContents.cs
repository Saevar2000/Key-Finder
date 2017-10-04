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
    class WriteContents
    {
        public void Write(string url)
        {
            try
            {
                TextWriter tw = new StreamWriter("test-" + DateTime.Now.Ticks + ".txt ", true);
                var data = new MyWebClient().DownloadString(url);
                var doc = new HtmlDocument();
                doc.LoadHtml(data);
                // Regex.Replace(doc, @"<[^>]*(>|$)|&nbsp;|&zwnj;|&raquo;|&laquo;", string.Empty).Trim();


                foreach (var node in doc.DocumentNode.SelectNodes("//text()"))
                {
                    if (!string.IsNullOrWhiteSpace(node.InnerText))
                    {
                        tw.WriteLine(node.InnerText);
                        Console.WriteLine(node.InnerText);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }
    }
}
