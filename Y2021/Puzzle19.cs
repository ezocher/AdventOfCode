using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle19 : ASolver 
    {
        struct IntPoint3D
        {
            public int X, Y, Z;

            public IntPoint3D(string s)
            {
                List<int> n = new List<int>(Parse.LineOfDelimitedInts(s, ","));
                X = n[0]; Y = n[1]; Z = n[2];
            }

            public IntPoint3D(int x, int y, int z)
            {
                X = x; Y = y; Z = z;
            }

            public override string ToString() => $"{X},{Y},{Z}";
        }

        struct BeaconPair
        {
            public int IndexA;
            public int IndexB;
            IntPoint3D Delta;
            public float Distance;
            // Scanner ?

            public BeaconPair(int ia, int ib, IntPoint3D a, IntPoint3D b)
            {
                IndexA = ia;
                IndexB = ib;
                Delta = new IntPoint3D(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y), Math.Abs(a.Z - b.Z));
                Distance = (float)Math.Sqrt(Delta.X * Delta.X + Delta.Y * Delta.Y + Delta.Z * Delta.Z);
            }

            public static bool PairsMatch(BeaconPair pairA, BeaconPair pairB)
            {
                int[] deltasA = new int[3] { pairA.Delta.X, pairA.Delta.Y, pairA.Delta.Z };
                Array.Sort(deltasA);
                int[] deltasB = new int[3] { pairB.Delta.X, pairB.Delta.Y, pairB.Delta.Z };
                Array.Sort(deltasB);
                return ((deltasA[0] == deltasB[0]) && (deltasA[1] == deltasB[1]) && (deltasA[2] == deltasB[2]));
            }

            public override string ToString() => $"{Distance:F2}: [{IndexA}] <=> [{IndexB}]";
        }

        struct ScannerPair
        {
            int IndexAlpha;
            int IndexBeta;
            bool[] MatchedBeaconsAlpha;
            bool[] MatchedBeaconsBeta;
            List<(int, int, int, int)> MatchedBeaconPairsIndexes;
            public int MatchedBeaconCount;

            // TODO: Verify this
            public ScannerPair(int iAlpha, int iBeta, Scanner alpha, Scanner beta)
            {
                IndexAlpha = iAlpha; IndexBeta = iBeta;
                MatchedBeaconsAlpha = new bool[alpha.Beacons.Count];
                MatchedBeaconsBeta = new bool[beta.Beacons.Count];
                MatchedBeaconPairsIndexes = new();

                int iA = 0; int iB = 0;

                while ((iA < alpha.BeaconPairs.Count) && (iB < beta.BeaconPairs.Count))
                {
                    if (alpha.BeaconPairs[iA].Distance == beta.BeaconPairs[iB].Distance)
                    {
                        if (BeaconPair.PairsMatch(alpha.BeaconPairs[iA], beta.BeaconPairs[iB]))
                        {
                            int i1 = alpha.BeaconPairs[iA].IndexA;
                            int i2 = alpha.BeaconPairs[iA].IndexB;
                            int i3 = beta.BeaconPairs[iB].IndexA;
                            int i4 = beta.BeaconPairs[iB].IndexB;
                            MatchedBeaconPairsIndexes.Add((i1, i2, i3, i4));
                            MatchedBeaconsAlpha[i1] = MatchedBeaconsAlpha[i2] = true;
                            MatchedBeaconsBeta[i3] = MatchedBeaconsBeta[i4] = true;
                        }
                        iA++; iB++;
                    }
                    else if (alpha.BeaconPairs[iA].Distance > beta.BeaconPairs[iB].Distance)
                        iB++;
                    else
                        iA++;
                }

                MatchedBeaconCount = Math.Min(Tools.CountTrue(MatchedBeaconsAlpha), Tools.CountTrue(MatchedBeaconsBeta));
            }

            public override string ToString() => $"Scanners {IndexAlpha} & {IndexBeta}: {MatchedBeaconCount} matches";
        }

        class Scanner
        {
            public List<IntPoint3D> Beacons;
            public List<BeaconPair> BeaconPairs;
            public List<int> OverlappingScannerIndexes;

            // Direction

            // Beacon Distance Map

            public Scanner(List<IntPoint3D> beacons)
            {
                Beacons = beacons;

                BeaconPairs = new();
                for (int a = 0; a < Beacons.Count; a++)
                    for (int b = a + 1; b < Beacons.Count; b++)
                        BeaconPairs.Add(new BeaconPair(a, b, Beacons[a], Beacons[b]));
                BeaconPairs.Sort((s1, s2) => s1.Distance.CompareTo(s2.Distance));

                OverlappingScannerIndexes = new();
            }
        }

        private List<Scanner> scanners;
        private bool[] mappedScanners;
        private List<ScannerPair> overlappingScannerPairs;
        private HashSet<string> recordedBeacons;

        public Puzzle19(string input) : base(input) { Name = "Beacon Scanner"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            scanners = new();
            List<IntPoint3D> beacons = new();

            for (int i = 1; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("--- scanner"))
                {
                    scanners.Add(new Scanner(beacons));
                    beacons = new();
                }
                else if (lines[i] != string.Empty)
                    beacons.Add(new IntPoint3D(lines[i]));
            }
            scanners.Add(new Scanner(beacons));

            overlappingScannerPairs = new();
            mappedScanners = new bool[scanners.Count];
            recordedBeacons = new();
        }

        private void CompareScanners()
        {
            const int MatchThreshold = 12;

            for (int a = 0; a < scanners.Count; a++)
                for (int b = a + 1; b < scanners.Count; b++)
                {
                    ScannerPair sp = new ScannerPair(a, b, scanners[a], scanners[b]);
                    if (sp.MatchedBeaconCount >= MatchThreshold)
                    {
                        overlappingScannerPairs.Add(sp);
                        scanners[a].OverlappingScannerIndexes.Add(b);
                        scanners[b].OverlappingScannerIndexes.Add(a);
                    }
                }
        }

        private void AddBeacons(Scanner s)
        {
            foreach (IntPoint3D b in s.Beacons)
                recordedBeacons.Add(b.ToString());
        }

        [Description("How many beacons are there?")]
        public override string SolvePart1()
        {
            CompareScanners();
            //AddBeacons(scanners[0]);

            return recordedBeacons.Count.ToString();
        }

        [Description("How many beacons are there?")]
        public override string SolvePart2()
        {
            Setup();    // Remove if Part 2 builds on output of Part 1

            return string.Empty;
        }
    }
}