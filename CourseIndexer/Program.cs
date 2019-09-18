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
                tableCode.Append("| Index | Course Name | Category  | Time  | Videos |\n");
                tableCode.Append("| :---: | ----------- | :-------: | :---: | :----: |\n");

                for (int courseIndex = 1; courseIndex <= 80; courseIndex++)
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
                            var getCategory = Regex.Match(html, "<div.*class=.author-name.*(<\\s*a[^>]*>(.*?)<\\s*/\\s*a>)</div>");
                            var getTime = Regex.Match(html, "<div.*class=.total-lectures-time.*>(.*?)</div>");
                            var getVideos = Regex.Matches(html, "<span.*class=.lecture-title.*>(.*?)</span>");

                            //var title = Regex.Replace(getTitle.ToString(),"\\s{32}", string.Empty);
                            var title = getTitle.ToString().Trim();
                            var category = getCategory.Groups[2];
                            var time = getTime.Groups[1];
                            var videos = getVideos.Count;

                            if (getTitle.ToString() == string.Empty)
                            {
                                throw new Exception("The course don't exist!");
                            }

                            tableCode.Append($"| {courseIndex} | <a href=\"{url}\" target=\"_blank\">{title}</a> | {category} | {time} | {videos} |\n");

                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Url: {url}, Title: {title}, Index: {courseIndex}, Category: {category}, Time: {time}, Videos: {videos}");
                        }
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Error while read the web: {url}. {e.Message}");
                        tableCode.Append($"| {courseIndex} | {e.Message} | no category! | no time! | no videos! |\n");
                    }
                }

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Github table code:\n");
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
