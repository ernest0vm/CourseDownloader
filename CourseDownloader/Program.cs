using System;
using System.IO;
using System.Net;

namespace CourseDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string courseName = "Android Oreo y Kotlin";
                int courseNumber = 35;
                int maxVideoIndex = 140;

                Console.Title = "clubacademy.mx course downloader";
                Console.WriteLine($"** Welcome to {Console.Title} **");
                Console.WriteLine();

                string pathToSave = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + $"\\{courseName}";

                WebClient Client = new WebClient();

                // check if exists a folder to save the course videos
                if (!Directory.Exists(pathToSave))
                {
                    Directory.CreateDirectory(pathToSave);
                }

                for (int a = 1; a <= maxVideoIndex; a++)
                {
                    string source = $"http://54.147.183.92/CFTP2019/V{courseNumber}_{a}.mp4";
                    string output = pathToSave + $"\\V{courseNumber}_{a}.mp4";

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
                    catch
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"Cannot download: {output}");
                    }
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine();
                Console.WriteLine("All videos has been downloaded successful, press any key to exit.");
                Console.ReadLine();
            }
            catch(Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error ocurred: {ex.Message}.");
                Console.WriteLine("Press any key to exit.");
                Console.ReadLine();
            }
        }
    }
}
