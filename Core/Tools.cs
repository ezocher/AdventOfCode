﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
namespace AdventOfCode.Core {
    public static partial class Tools {
        private static string GetSolutionRootPath() {
            string runningExePath = AppDomain.CurrentDomain.BaseDirectory;
            return runningExePath.Substring(0, runningExePath.IndexOf(@"\bin\"));
        }

        // If no input, fetch puzzle input and generate empty test input file
        // This does nothing if files have already been downloaded/generated
        public static void DownloadInput(string session, int year, int day)
        {
            string path = GetSolutionRootPath();
        }

        // Downloads or re-downloads puzzle description from aoc.com
        public static string DownloadDescription(string session, int year, int day)
        {
            string path = GetSolutionRootPath();

            return String.Empty;
        }

        // Download puzzle description and generate Puzzle class file
        // This does nothing if file has already been generated
        public static void GeneratePuzzleTemplate(string session, int year, int day) {
            Console.WriteLine($"Generating Template for {year}-{day}");

            string path = GetSolutionRootPath();

            using (HttpClient web = new HttpClient(new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.All })) {
                web.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("advent-of-code-grabber", "1.0"));
                web.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("deflate"));
                web.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("gzip"));
                web.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                web.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                web.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue() { NoCache = true };
                web.DefaultRequestHeaders.Add("cookie", $"session={session}");

                string html = DownloadHtml(web, $"https://adventofcode.com/{year}/day/{day}");
                int index1 = html.IndexOf("<article", StringComparison.OrdinalIgnoreCase);
                int index2 = html.IndexOf("</article>", StringComparison.OrdinalIgnoreCase);
                int index5 = html.IndexOf("<article", index1 + 1, StringComparison.OrdinalIgnoreCase);
                int index6 = html.IndexOf("</article>", index2 + 1, StringComparison.OrdinalIgnoreCase);
                if (index1 < 0 || index2 < 0) {
                    Console.WriteLine($"Failed to download Puzzle data {year}-{day}", ConsoleColor.Red);
                    return;
                }

                string dayHeader = $"<h2>--- Day {day}: ";
                int index3 = html.IndexOf(dayHeader);
                int index4 = html.IndexOf(" ---</h2>", index3);
                string dayTitle = html.Substring(index3 + dayHeader.Length, index4 - index3 - dayHeader.Length);

                Directory.CreateDirectory(Path.Combine(path, $"Y{year}"));
                Directory.CreateDirectory(Path.Combine(path, $"Y{year}\\Inputs"));
                Directory.CreateDirectory(Path.Combine(path, $"Y{year}\\Descriptions"));

                string classPath = Path.Combine(path, $"Y{year}\\Puzzle{day:00}.cs");
                if (!File.Exists(classPath)) {
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

        [Description(""What is the answer?"")]
        public override string SolvePart1()
        {{
            foreach (int number in numbers)
            {{
                
            }}
            return string.Empty;
        }}

        [Description(""What is the answer?"")]
        public override string SolvePart2()
        {{
            return string.Empty;
        }}
    }}
}}";
                    File.WriteAllText(classPath, templateClass);
                }

                string secondPart = index5 > 0 ? html.Substring(index5, index6 - index5 + 10) : string.Empty;
                string descriptionHtml =
$@"<head>
<link rel=""stylesheet"" type=""text/css"" href=""../../style.css""/>
</head>
{html.Substring(index1, index2 - index1 + 10)}
{secondPart}".TrimEnd();

                string descriptionPath = Path.Combine(path, $@"Y{year}\Descriptions\puzzle{day:00}.html");
                File.WriteAllText(descriptionPath, descriptionHtml);

                string[] files = Directory.GetFiles(@$"Y{year}\Inputs\", $"puzzle{day:00}*.txt", SearchOption.TopDirectoryOnly);
                if (files.Length == 0) {
                    html = DownloadHtml(web, $"https://adventofcode.com/{year}/day/{day}/input");
                    File.WriteAllText(Path.Combine(path, $@"Y{year}\Inputs\puzzle{day:00}--.txt"), html.TrimEnd());
                }
            }
        }
        private static string DownloadHtml(HttpClient web, string url) {
            Task<HttpResponseMessage> responseTask = web.GetAsync(url);
            responseTask.Wait();
            HttpResponseMessage response = responseTask.Result.EnsureSuccessStatusCode();
            Task<string> contentTask = response.Content.ReadAsStringAsync();
            contentTask.Wait();
            return contentTask.Result.ToString();
        }
        public static string GetInput(string filePath) {
            if (!File.Exists(filePath)) { return string.Empty; }

            using (StreamReader sr = new StreamReader(filePath)) {
                return sr.ReadToEnd();
            }
        }
        public static List<string> GetLines(string input) {
            List<string> lines = new List<string>();
            StringBuilder line = new StringBuilder();
            for (int i = 0; i < input.Length; i++) {
                char c = input[i];
                if (c == '\n') {
                    lines.Add(line.ToString());
                    line.Clear();
                } else if (c != '\r') {
                    line.Append(c);
                }
            }
            lines.Add(line.ToString());
            return lines;
        }
        public static List<string> GetSections(string input, char newLineReplacement = '\n') {
            List<string> sections = new List<string>();
            StringBuilder section = new StringBuilder();
            char last = '\0';
            for (int i = 0; i < input.Length; i++) {
                char c = input[i];
                if (c == '\n' && last == '\n') {
                    sections.Add(section.ToString());
                    section.Clear();
                    last = '\0';
                } else if (c != '\r') {
                    if (c != '\n') {
                        if (last == '\n') {
                            section.Append(newLineReplacement);
                        }
                        section.Append(c);
                    }
                    last = c;
                }
            }
            sections.Add(section.ToString());
            return sections;
        }
        public static int[] GetNumbers(string input, char splitChar = '\n') {
            List<int> numbers = new List<int>();
            int startIndex = 0;
            for (int i = 0; i < input.Length; i++) {
                char c = input[i];
                if (c == splitChar) {
                    numbers.Add(ParseInt(input, startIndex, i - startIndex));
                    startIndex = i + 1;
                }
            }
            numbers.Add(ParseInt(input, startIndex));
            return numbers.ToArray();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ParseUInt(string number) {
            return (uint)ParseLong(number, 0, number.Length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ParseUInt(string number, int startIndex) {
            return (uint)ParseLong(number, startIndex, number.Length - startIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ParseUInt(string number, int startIndex, int length) {
            return (uint)ParseLong(number, startIndex, length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ParseUInt(string number, string startFrom, int startIndex = 0) {
            return (uint)ParseLong(number, startFrom, startIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint ParseUInt(string number, string startFrom, string endOn, int startIndex = 0) {
            return (uint)ParseLong(number, startFrom, endOn, startIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseInt(string number) {
            return (int)ParseLong(number, 0, number.Length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseInt(string number, int startIndex) {
            return (int)ParseLong(number, startIndex, number.Length - startIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseInt(string number, int startIndex, int length) {
            return (int)ParseLong(number, startIndex, length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseInt(string number, string startFrom, int startIndex = 0) {
            return (int)ParseLong(number, startFrom, startIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int ParseInt(string number, string startFrom, string endOn, int startIndex = 0) {
            return (int)ParseLong(number, startFrom, endOn, startIndex);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long ParseLong(string number) {
            return ParseLong(number, 0, number.Length);
        }
        public static long ParseLong(string number, string startFrom, int startIndex = 0) {
            int index1 = number.IndexOf(startFrom, startIndex);
            if (index1 < 0) { return 0; }

            return ParseLong(number, index1 + startFrom.Length, number.Length - index1 - startFrom.Length);
        }
        public static long ParseLong(string number, string startFrom, string endOn, int startIndex = 0) {
            int index1 = number.IndexOf(startFrom, startIndex);
            if (index1 < 0) { return 0; }

            int index2 = number.IndexOf(endOn, index1 + startFrom.Length);
            if (index2 < 0) { index2 = number.Length; }

            return ParseLong(number, index1 + startFrom.Length, index2 - index1 - startFrom.Length);
        }
        public static long ParseLong(string number, int startIndex, int length) {
            if (length <= 0) { return 0; }

            int i = 0;
            while (i < length && !char.IsDigit(number[i + startIndex])) {
                i++;
            }

            bool isNegative = i > 0 ? number[i + startIndex - 1] == '-' : false;
            long result = 0;
            while (i < length) {
                byte value = (byte)number[i + startIndex];
                if (value >= 0x30 && value <= 0x39) {
                    result = result * 10 + value - 0x30;
                }
                i++;
            }
            return isNegative ? -result : result;
        }
        public static void For<T>(IEnumerable<T> items, Action<T> action) {
            foreach (T item in items) {
                action(item);
            }
        }
        public static IEnumerable<T[]> Permute<T>(T[] items) {
            return DoPermute(items, 0, items.Length - 1);
        }
        private static IEnumerable<T[]> DoPermute<T>(T[] items, int start, int end) {
            if (start == end) {
                yield return items;
            } else {
                for (var i = start; i <= end; i++) {
                    Swap(ref items[start], ref items[i]);
                    foreach (T[] seq in DoPermute(items, start + 1, end)) {
                        yield return seq;
                    }
                    Swap(ref items[start], ref items[i]);
                }
            }
        }
        private static void Swap<T>(ref T a, ref T b) {
            var temp = a;
            a = b;
            b = temp;
        }
        public static IEnumerable<List<T>> Permute<T>(List<T> items) {
            return DoPermute(items, 0, items.Count - 1);
        }
        private static IEnumerable<List<T>> DoPermute<T>(List<T> items, int start, int end) {
            if (start == end) {
                yield return items;
            } else {
                for (var i = start; i <= end; i++) {
                    Swap(items, start, i);
                    foreach (List<T> seq in DoPermute(items, start + 1, end)) {
                        yield return seq;
                    }
                    Swap(items, start, i);
                }
            }
        }
        private static void Swap<T>(List<T> items, int a, int b) {
            var temp = items[a];
            items[a] = items[b];
            items[b] = temp;
        }
        public static int GCD(int a, int b) {
            if (a == 0 && b == 0) { return 1; }
            a = a < 0 ? -a : a;
            b = b < 0 ? -b : b;
            if (a == 0) { return b; }
            if (b == 0) { return a; }

            int r;
            if (b > a) {
                r = b % a;
                if (r == 0) { return a; }
                if (r == 1) { return 1; }
                b = r;
            }
            while (true) {
                r = a % b;
                if (r == 0) { return b; }
                if (r == 1) { return 1; }
                a = b;
                b = r;
            }
        }
        public static long GCD(long a, long b) {
            if (a == 0 && b == 0) { return 1; }
            a = a < 0 ? -a : a;
            b = b < 0 ? -b : b;
            if (a == 0) { return b; }
            if (b == 0) { return a; }

            long r;
            if (b > a) {
                r = b % a;
                if (r == 0) { return a; }
                if (r == 1) { return 1; }
                b = r;
            }
            while (true) {
                r = a % b;
                if (r == 0) { return b; }
                if (r == 1) { return 1; }
                a = b;
                b = r;
            }
        }
    }
}