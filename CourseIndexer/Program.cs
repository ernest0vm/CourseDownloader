using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace CourseIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                StringBuilder tableCode = new StringBuilder();
                tableCode.Append("| Course Name | Course Index |\n");
                tableCode.Append("| ----------- | ------------ |\n");

                for (int courseIndex = 1; courseIndex <= 100; courseIndex++)
                {
                    Uri url = new Uri($"https://clubacademy.mx/detalle?id={courseIndex}");

                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                    try
                    {
                        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                        using (Stream stream = response.GetResponseStream())
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            string html = reader.ReadToEnd();

                            var getTitle = Regex.Match(html, "<h1.*class=.entry-title.*[^>](.*)</h1>").Groups[1];
                            //var title = Regex.Replace(getTitle.ToString(),"\\s{32}", string.Empty);
                            var title = getTitle.ToString().Trim();

                            if (getTitle.ToString() == string.Empty)
                            {
                                throw new Exception("The course don't exist!");
                            }

                            tableCode.Append($"| <a href=\"{url}\" target=\"_blank\">{title}</a> | {courseIndex} |\n");

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Url: {url}, Title: {title}, Index: {courseIndex}");  
                        }
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error while read the web: {url}. {e.Message}");
                        tableCode.Append($"| {e.Message} | {courseIndex} |\n");
                    }
                }

                Console.WriteLine(tableCode);

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error ocurred: {ex.Message}.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }
        }
    }
}
