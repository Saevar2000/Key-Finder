using Npgsql;
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
            var connString = "Host=127.0.0.1;Username=user;Password=pass;Database=db";

            using (var conn = new NpgsqlConnection(connString))
            {
                conn.Open();

                // Insert some data
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = "INSERT INTO data (some_field) VALUES (@p)";
                    cmd.Parameters.AddWithValue("p", "Hello world");
                    cmd.ExecuteNonQuery();
                }

                // Retrieve all rows
                using (var cmd = new NpgsqlCommand("SELECT some_field FROM data", conn))
                using (var reader = cmd.ExecuteReader())
                    while (reader.Read())
                        Console.WriteLine(reader.GetString(0));
            }

            Console.WriteLine("test");
            Console.ReadLine();

            CrawlParse c = new CrawlParse();

            List<string> alreadyScanned = new List<string>();
            List<string> toBeScanned = new List<string>();

            toBeScanned.AddRange(c.Parse("https://pastebin.com/LxBhVLPM"));

                for (int i = 0; i < toBeScanned.Count; i++)
                {
                    // Prevent duplicates
                    toBeScanned.RemoveAll(item => alreadyScanned.Contains(item));

                    Console.WriteLine(toBeScanned.ElementAt(i));
                    try
                    {
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
            

            Console.WriteLine("Done");
            Console.ReadLine();
        }
    }
}
