using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace CourseDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                List<string> videoNames = new List<string>();
                string courseName;
                int courseNumber;
                int maxVideoIndex;

                Console.Title = "clubacademy.mx course downloader";
                Console.WriteLine($"** Welcome to {Console.Title} **");
                Console.WriteLine();

                Console.WriteLine("Please input the course name:");
                courseName = Console.ReadLine();
                Console.WriteLine();

                Console.WriteLine("Please input the course index: (only numbers)");
                courseNumber = Convert.ToInt32(Console.ReadLine());

                videoNames = GetVideos(courseNumber);
                maxVideoIndex = Convert.ToInt32(videoNames.Count);

                string pathToSave = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{courseName}";

                WebClient Client = new WebClient();

                // check if exists a folder to save the course videos
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                for (int a = 1; a <= maxVideoIndex; a++)
                {
                    string videoName = $"{a}.- {videoNames[a - 1]}";
                    string source = $"http://54.147.183.92/CFTP2019/V{courseNumber}_{a}.mp4";
                    string output = pathToSave + $"\\{videoName}.mp4";

                    if (File.Exists(output))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.WriteLine($"{output} Already downloaded.");
                        continue;
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"Downloading: {source}");

                    try
                    {
                        Client.DownloadFile(source, output);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"{output} Downloaded successful.");
                    }
                    catch(Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Cannot download: {output}");
                        Console.WriteLine($"Error Message: {e.Message}");
                    }
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine("All videos has been downloaded successful, press any key to exit.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error ocurred: {ex.Message}.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }
        }

        private static List<string> GetVideos(int courseIndex)
        {
            List<string> videoList = new List<string>();

            try
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

                        var getVideos = Regex.Matches(html, "<span.*class=.lecture-title.*>(.*?)</span>");

                        foreach (Match item in getVideos)
                        {
                            if (item.Groups.Count > 0)
                                videoList.Add(item.Groups[1].ToString().Trim());
                        }

                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine();
                        Console.WriteLine("Video list obtained correctly!");
                    }
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error while read the web: {url}. {e.Message}");
                }

                Console.WriteLine();

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error ocurred: {ex.Message}.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }

            return videoList;
        }
    }
}
