using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace AdventOfCode.Y2021
{
    public class Puzzle12 : ASolver 
    {
        private class Cave
        {
            public string Name;
            public List<Cave> Links;

            public Cave(string s)
            {
                Name = s;
                Links = new List<Cave>();
            }

            public void AddLink(Cave c) => Links.Add(c);

            public bool IsLarge() => (Char.IsUpper(Name[0]));

            public override string ToString()
            {
                return $"{Name} ({Links.Count} ->)";
            }

        }

        private Cave AddIfNew(string s)
        {
            if (caveSystem.ContainsKey(s))
                return caveSystem[s];
            else
            {
                Cave c = new Cave(s);
                caveSystem.Add(s, c);
                return c;
            }
        }

        private Dictionary<string, Cave> caveSystem;
        List<List<Cave>> paths;

        private void EnumerateAllPaths(List<Cave> path, bool hasDupSmall, Cave cave)
        {
            List<Cave>[] pathForks;

            if (cave.Name == "end")
            {
                path.Add(cave);
                paths.Add(path);
                return;
            }
            else if (!cave.IsLarge() && path.Contains(cave))
            {
                return;
            }
            else
            {
                path.Add(cave);
                pathForks = new List<Cave>[cave.Links.Count];
                pathForks[0] = path;
                for (int i = 1; i < cave.Links.Count; i++)
                    pathForks[i] = new List<Cave>(path);
                for (int i = 0; i < cave.Links.Count; i++)
                    EnumerateAllPaths(pathForks[i], cave.Links[i]);
            }
        }

        private void EnumerateAllPaths(List<Cave> path, Cave cave)
        {
            List<Cave>[] pathForks;

            if (cave.Name == "end")
            {
                path.Add(cave);
                paths.Add(path);
                return;
            }
            else if (!cave.IsLarge() && path.Contains(cave))
            {
                return;
            }
            else
            {
                path.Add(cave);
                pathForks = new List<Cave>[cave.Links.Count];
                pathForks[0] = path;
                for (int i = 1; i < cave.Links.Count; i++)
                    pathForks[i] = new List<Cave>(path);
                for (int i = 0; i < cave.Links.Count; i++)
                    EnumerateAllPaths(pathForks[i], cave.Links[i]);
            }
        }

        private string Concat(List<Cave> path)
        {
            StringBuilder s = new StringBuilder();

            foreach (Cave c in path)
                s.Append((c.Name == "end") ? c.Name : $"{c.Name}, ");

            return s.ToString();
        }

        private void EnumerateAllPathsPart2(List<Cave> path, bool hasSmallDup, Cave cave)
        {
            List<Cave>[] pathForks;

            if (cave.Name == "end")
            {
                path.Add(cave);
                paths.Add(path);
                return;
            }
            else if (!cave.IsLarge() && path.Contains(cave) && hasSmallDup)
            {
                return;
            }
            else
            {
                if ((cave.Name == "start") && path.Count > 0)
                    return;
                if (!cave.IsLarge() && path.Contains(cave))
                    hasSmallDup = true;
                path.Add(cave);
                pathForks = new List<Cave>[cave.Links.Count];
                pathForks[0] = path;
                for (int i = 1; i < cave.Links.Count; i++)
                    pathForks[i] = new List<Cave>(path);
                for (int i = 0; i < cave.Links.Count; i++)
                    EnumerateAllPathsPart2(pathForks[i], hasSmallDup, cave.Links[i]);
            }
        }

        public Puzzle12(string input) : base(input) { Name = "Passage Pathing"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            caveSystem = new Dictionary<string, Cave>();
            paths = new List<List<Cave>>();

            foreach (string line in lines)
            {
                string[] nodes = line.Split('-');
                Cave one = AddIfNew(nodes[0]);
                Cave two = AddIfNew(nodes[1]);
                one.AddLink(two);
                two.AddLink(one);
            }
        }

        [Description("How many paths through this cave system are there that visit small caves at most once?")]
        public override string SolvePart1()
        {
            List<Cave> path = new List<Cave>();
            EnumerateAllPaths(path, caveSystem["start"]);
            
            return paths.Count.ToString();
        }



        [Description("Given these new rules, how many paths through this cave system are there?")]
        public override string SolvePart2()
        {
            paths = new List<List<Cave>>();

            List<Cave> path = new List<Cave>();
            EnumerateAllPathsPart2(path, false, caveSystem["start"]);

            return paths.Count.ToString();
        }
    }
}