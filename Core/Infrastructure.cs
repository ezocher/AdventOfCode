using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Core
{
    static class Infrastructure
    {
        public static string GetSolutionRootPath()
        {
            string runningExePath = AppDomain.CurrentDomain.BaseDirectory;
            return runningExePath.Substring(0, runningExePath.IndexOf(@"\bin\"));
        }

        // If no input, fetch puzzle input and create empty test input file
        // This does nothing and returns false if files have already been downloaded and/or generated
        public static bool DownloadInput(string session, int year, int day)
        {
            // Input files stored in rootPath plus \Y20XX\Inputs
            string solutionRootPath = GetSolutionRootPath();
            string inputFilesPath = Path.Combine(solutionRootPath, $@"Y{year}\Inputs");

            Directory.CreateDirectory(Path.Combine(solutionRootPath, $"Y{year}"));
            Directory.CreateDirectory(inputFilesPath);

            string[] existingInputFiles = Directory.GetFiles(inputFilesPath, $"puzzle{day:00}-*.txt");
            if (existingInputFiles.Length > 0)
            {
                return false;
            }
            else
            {
                // fetch puzzle input
                using (HttpClient web = AdventOfCodeClient(session))
                {
                    string html = DownloadHtml(web, $"https://adventofcode.com/{year}/day/{day}/input");
                    File.WriteAllText(Path.Combine(inputFilesPath, $@"puzzle{day:00}--.txt"), html.TrimEnd());
                }
                Runner.WriteLine($"Downloaded input for {year}-{day}", ConsoleColor.White);

                // Create empty test input file
                using (FileStream fs = File.Create(inputFilesPath + $@"\puzzle{day:00}---test.txt"))
                    fs.Close();
                Runner.WriteLine($"Generated empty test input file 'puzzle{day:00}---test.txt'", ConsoleColor.White);

                return true;
            }
        }

        private static HttpClient AdventOfCodeClient(string session)
        {
            HttpClient web = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.All });
            web.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("advent-of-code-grabber", "1.0"));
            web.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
            web.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
            web.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            web.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            web.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
            web.DefaultRequestHeaders.Add("cookie", $"session={session}");
            return web;
        }

        // Downloads or re-downloads puzzle description from aoc.com
        // Extracts Puzzle Title for insertion into code teplate
        public static (string, string) DownloadDescription(string session, int year, int day)
        {
            string solutionRootPath = GetSolutionRootPath();
            string html;

            using (HttpClient web = AdventOfCodeClient(session))
            {
                html = DownloadHtml(web, $"https://adventofcode.com/{year}/day/{day}");
            }

            // Directory.CreateDirectory(Path.Combine(path, $"Y{year}")); // This is done by DownloadInput()

            int index1 = html.IndexOf("<article", StringComparison.OrdinalIgnoreCase);
            int index2 = html.IndexOf("</article>", StringComparison.OrdinalIgnoreCase);
            int index5 = html.IndexOf("<article", index1 + 1, StringComparison.OrdinalIgnoreCase);
            int index6 = html.IndexOf("</article>", index2 + 1, StringComparison.OrdinalIgnoreCase);
            //if (index1 < 0 || index2 < 0)
            //{
            //    Console.WriteLine($"\nFailed to download Puzzle description {year}-{day}", ConsoleColor.Red);
            //    return;
            //}

            string dayHeader = $"<h2>--- Day {day}: ";
            int index3 = html.IndexOf(dayHeader);
            int index4 = html.IndexOf(" ---</h2>", index3);
            string dayTitle = html.Substring(index3 + dayHeader.Length, index4 - index3 - dayHeader.Length);

            // Search backward from the first </article> to find the puzzle question inside an <em></em> block
            // Remove the tags from inside the <em></em> block to get the raw text
            string startOfPuzzlePrompt = "<em>";
            string endOfPuzzlePrompt = "</em>";
            int indexFPPStart = html.LastIndexOf(startOfPuzzlePrompt, index2) + startOfPuzzlePrompt.Length;
            int indexFPPEnd = html.IndexOf(endOfPuzzlePrompt, indexFPPStart);
            string firstPuzzlePrompt = RemoveAllTags(html.Substring(indexFPPStart, indexFPPEnd - indexFPPStart));

            string secondPart = index5 > 0 ? html.Substring(index5, index6 - index5 + 10) : string.Empty;
            string descriptionHtml =
$@"<head>
<link rel=""stylesheet"" type=""text/css"" href=""../../style.css""/>
</head>
{html.Substring(index1, index2 - index1 + 10)}
{secondPart}".TrimEnd();

            Directory.CreateDirectory(Path.Combine(solutionRootPath, $@"Y{year}\Descriptions"));
            string descriptionPath = Path.Combine(solutionRootPath, $@"Y{year}\Descriptions\puzzle{day:00}.html");
            File.WriteAllText(descriptionPath, descriptionHtml);

            Runner.WriteLine($"Downloaded description for {year}-{day}", ConsoleColor.White);

            return (dayTitle, firstPuzzlePrompt);
        }

        // Download puzzle description and generate Puzzle class file
        // This does nothing if file has already been generated
        public static void GetInputGeneratePuzzleTemplate(string session, int year, int day)
        {
            if (!DownloadInput(session, year, day))
                return;

            (string dayTitle, string firstPuzzlePrompt) = DownloadDescription(session, year, day);

            string solutionRootPath = GetSolutionRootPath();

            // string html = DownloadHtml(web, $"https://adventofcode.com/{year}/day/{day}");

            string classPath = Path.Combine(solutionRootPath, $"Y{year}\\Puzzle{day:00}.cs");

            // TODO: Move template into a text file
            if (!File.Exists(classPath))
            {
                string templateClass =
@$"using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y{year}
{{
    public class Puzzle{day:00} : ASolver 
    {{
        private List<int> numbers;

        private List<(string, int)> commands;

        private List<IntLine> intLines;

        private const int PuzzThing = 1;
        private const int PuzzValue = 2;

        public Puzzle{day:00}(string input) : base(input) {{ Name = ""{dayTitle}""; }}

        public override void Setup()
        {{
            List<string> lines = Tools.GetLines(Input);

            numbers = new List<int>(Parse.LineOfDelimitedInts(lines[0], "",""));
            
            foreach (string line in lines)
                numbers.Add(Tools.ParseInt(line);

            commands = new List<(string, int)>();
            foreach (string line in lines)
            {{
                int separatorIndex = line.IndexOf(' ');
                commands.Add((line.Substring(0, separatorIndex), Tools.ParseInt(line, separatorIndex + 1)));
            }}

            intLines = new List<IntLine>();
            foreach (string inputLine in lines)
                intLines.Add(new IntLine(inputLine)); 
        }}

        [Description(""{firstPuzzlePrompt}"")]
        public override string SolvePart1()
        {{
            foreach (int number in numbers)
            {{
                
            }}
            return string.Empty;
        }}

        [Description(""{firstPuzzlePrompt}"")]
        public override string SolvePart2()
        {{
            Setup();    // Remove if Part 2 builds on output of Part 1

            return string.Empty;
        }}
    }}
}}";
                File.WriteAllText(classPath, templateClass);

                Runner.WriteLine($"Generated class template for {year}-{day}: {Path.GetFileName(classPath)}\n", ConsoleColor.White);
            }
        }

        // TODO: Bubble up exceptions (such as 404's) to callers who can handle them and have appropriate messages
        private static string DownloadHtml(HttpClient web, string url)
        {
            Task<string> contentTask;

            Task<HttpResponseMessage> responseTask = web.GetAsync(url);
            responseTask.Wait();
            HttpResponseMessage response = responseTask.Result.EnsureSuccessStatusCode();
            contentTask = response.Content.ReadAsStringAsync();
            contentTask.Wait();

            return contentTask.Result.ToString();
        }

        public static string RemoveAllTags(string html)
        {
            StringBuilder output = new StringBuilder(html);

            int searchIndex = 0;
            int openTagIndex;

            while ((openTagIndex = output.IndexOf('<', searchIndex)) != -1)
            {
                int closeTagIndex = output.IndexOf('>', openTagIndex + 1);
                if (closeTagIndex == -1)
                    break;
                output.Remove(openTagIndex, closeTagIndex - openTagIndex + 1);
                searchIndex = openTagIndex;
            }

            return output.ToString();
        }
    }
}
