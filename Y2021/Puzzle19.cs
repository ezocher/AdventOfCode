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

            public IntPoint3D ApplyTransform(Transform transform)
            {
                return new(this.X, this.Y, this.Z);
            }

            public override string ToString() => $"{X},{Y},{Z}";
        }

        struct Transform
        {
            public int I;

            public Transform(int i)
            {
                I = i;
            }
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

        class Scanner
        {
            public List<IntPoint3D> Beacons;
            public List<BeaconPair> BeaconPairs;
            public List<ScannerPair> OverlappingScanners;
            public bool OverlappingScannersRemapped = false;
            public int Index;

            // Direction

            // Beacon Distance Map

            public Scanner(List<IntPoint3D> beacons, int index)
            {
                Beacons = beacons;
                Index = index;

                BeaconPairs = new();
                for (int a = 0; a < Beacons.Count; a++)
                    for (int b = a + 1; b < Beacons.Count; b++)
                        BeaconPairs.Add(new BeaconPair(a, b, Beacons[a], Beacons[b]));
                BeaconPairs.Sort((s1, s2) => s1.Distance.CompareTo(s2.Distance));

                OverlappingScanners = new();
            }

            public void AddBeaconsToMasterList(HashSet<string> masterList)
            {
                foreach (IntPoint3D b in this.Beacons)
                    masterList.Add(b.ToString());
            }

            public void TransformAllBeacons(Transform transform)
            {
                for (int i = 0; i < Beacons.Count; i++)
                    Beacons[i] = Beacons[i].ApplyTransform(transform);
            }
        }

        struct ScannerPair
        {
            public int IndexAlpha;
            public int IndexBeta;
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

        private List<Scanner> scanners;
        private bool[] remappedScanners;
        private HashSet<string> recordedBeacons;

        private const int CoordinateSystemBaseScannerIndex = 0;

        public Puzzle19(string input) : base(input) { Name = "Beacon Scanner"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            scanners = new();
            List<IntPoint3D> beacons = new();
            int scannerIndex = 0;

            for (int i = 1; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("--- scanner"))
                {
                    scanners.Add(new Scanner(beacons, scannerIndex++));
                    beacons = new();
                }
                else if (lines[i] != string.Empty)
                    beacons.Add(new IntPoint3D(lines[i]));
            }
            scanners.Add(new Scanner(beacons, scannerIndex));

            remappedScanners = new bool[scanners.Count];
            recordedBeacons = new();
        }

        private void FindOverlappingScannerPairs()
        {
            const int MatchThreshold = 12;

            for (int a = 0; a < scanners.Count; a++)
                for (int b = a + 1; b < scanners.Count; b++)
                {
                    ScannerPair sp = new ScannerPair(a, b, scanners[a], scanners[b]);
                    if (sp.MatchedBeaconCount >= MatchThreshold)
                    {
                        scanners[a].OverlappingScanners.Add(sp);
                        scanners[b].OverlappingScanners.Add(sp);
                    }
                }
        }

        private void RemapCoordinates(ScannerPair sp, Scanner baseScanner)
        {
            bool alphaIsBaseScanner = (sp.IndexAlpha == baseScanner.Index); 
            Scanner remapScanner = alphaIsBaseScanner ? scanners[sp.IndexBeta] : scanners[sp.IndexAlpha];

            if (!remappedScanners[remapScanner.Index])
            {
                Transform transform = FindOffsetAndRotation(baseScanner, remapScanner);

                remapScanner.TransformAllBeacons(transform);

                remappedScanners[remapScanner.Index] = true;
            }
        }

        private Transform FindOffsetAndRotation(Scanner baseScanner, Scanner remapScanner)
        {
            return new Transform(666);
        }

        private void RemapAllScannerCoordinates(Scanner startingScanner)
        {
            Scanner baseScanner = startingScanner;

            while (Tools.CountTrue(remappedScanners) < scanners.Count)
            {
                foreach (var scannerPair in baseScanner.OverlappingScanners)
                    RemapCoordinates(scannerPair, baseScanner);
                baseScanner.OverlappingScannersRemapped = true;

                int i = 0;
                while (!remappedScanners[i] || scanners[i].OverlappingScannersRemapped)
                    i++;
                baseScanner = scanners[i];
            }
        }


        [Description("How many beacons are there?")]
        public override string SolvePart1()
        {
            // Setup() reads scanners from input and Scanner() constructor builds beacon pairs

            // Use a specific scanner as coordinate system base
            remappedScanners[CoordinateSystemBaseScannerIndex] = true;

            FindOverlappingScannerPairs();

            RemapAllScannerCoordinates(scanners[CoordinateSystemBaseScannerIndex]);

            foreach (var scanner in scanners)   
                scanner.AddBeaconsToMasterList(recordedBeacons);

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