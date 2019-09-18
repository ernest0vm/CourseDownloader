using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace CourseDownloader
{
    class Program
    {
        private static readonly string[] illegalChars = { "<", ">", ":", "\"", "/", "\\", "|", "¿", "?", "*" };
        private static string CourseName { get; set; } = string.Empty;

        static void Main(string[] args)
        {
            try
            {
                List<string> videoNames = new List<string>();
                int courseNumber;
                int maxVideoIndex;

                Console.Title = "clubacademy.mx course downloader";
                Console.WriteLine($"** Welcome to {Console.Title} **");
                Console.WriteLine();
                
                Console.WriteLine("Please input the course index: (only numbers)");
                courseNumber = ValidateNumber(Console.ReadLine());
                videoNames = GetVideos(courseNumber);
                maxVideoIndex = Convert.ToInt32(videoNames.Count);

                Console.WriteLine($"I found the following title for the course '{CourseName}', you want to keep it [y / n] default: no");
                if (Console.ReadKey().Key != ConsoleKey.Y )
                {
                    Console.WriteLine();
                    Console.WriteLine($"\nPlease input the course name: (don't use {string.Join(" ", illegalChars)})");
                    CourseName = ValidateString(Console.ReadLine());
                    
                }

                Console.WriteLine();
                string pathToSave = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{CourseName}";
                Console.WriteLine($"All videos are stored in '{pathToSave}'");
                Console.WriteLine();

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
                    catch (Exception e)
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

                Client.Dispose();
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

                        var getTitle = Regex.Match(html, "<h1.*class=.entry-title.*[^>](.*)</h1>").Groups[1];
                        var getVideos = Regex.Matches(html, "<span.*class=.lecture-title.*>(.*?)</span>");

                        CourseName = ValidateString(getTitle.ToString().Trim());

                        foreach (Match item in getVideos)
                        {
                            if (item.Groups.Count > 0)
                            {
                                videoList.Add(ValidateString(item.Groups[1].ToString().Trim()));
                            }
                        }

                        if(videoList.Count > 0)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine();
                            Console.WriteLine($"{videoList.Count} videos found in the course!");
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine();
                            Console.WriteLine($"Video list contains no values, please check the course index.");
                            Console.WriteLine("Press any key to exit.");
                            Console.ReadLine();
                            Environment.Exit(0);
                        }
                        
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

        private static string ValidateString(string data)
        {
            foreach (var illegalChar in illegalChars)
            {
                if (data.Contains(illegalChar))
                {
                    switch (illegalChar)
                    {
                        case "\"":
                            data = new string(data.Replace(illegalChar, "'"));
                            break;
                        case ":":
                            data = new string(data.Replace(illegalChar, " -"));
                            break;
                        default:
                            data = new string(data.Replace(illegalChar, string.Empty));
                            break;
                    }
                }
            }

            return data;
        }

        private static int ValidateNumber(string data)
        {
            var getData = Regex.Match(data, "[0-9]+").Groups[0];
            var isNumber = Regex.IsMatch(getData.ToString(), "[0-9]+");

            if (isNumber)
            {
                return Convert.ToInt32(getData.Value);
            }
            else
            {
                Console.WriteLine("Please input the course index: (only numbers)");
                var newNumber = ValidateNumber(Console.ReadLine());
                Console.WriteLine();
                return newNumber;
            }
        }
    }
}
