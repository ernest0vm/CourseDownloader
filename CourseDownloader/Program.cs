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
                string courseName;
                int courseNumber;
                int maxVideoIndex;
                
                Console.WriteLine("Input the course name:");
                courseName = Console.ReadLine();

                Console.WriteLine("Input the course index:");
                courseNumber = Convert.ToInt32(Console.ReadLine());

                Console.WriteLine("Input the max index:");
                maxVideoIndex = Convert.ToInt32(Console.ReadLine());
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
                    Client.DownloadFile(source, output);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{output} Downloaded successful.");
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
            }
        }
    }
}
