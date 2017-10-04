using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    class Program
    {

        static void Main(string[] args)
        {
            WriteContents writeContents = new WriteContents();
            string url = "https://pastebin.com/XQWEefvc/";
            writeContents.Write(url);



            Console.ReadKey();
        }
    }
}
