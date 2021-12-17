using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle16 : ASolver 
    {
        private static bool[] bitArray;
        private static int index;

        enum TypeID { OpSum = 0, OpProduct = 1, OpMinimum = 2, OpMaximum = 3, LiteralValue = 4, 
            OpGreaterThan = 5, OpLessThan = 6, OpEqualTo = 7 }
        class Packet
        {
            public int Version;
            public TypeID Type;
            public int Value;
            public int LengthTypeID;
            public int TotalSubPacketLength;
            public int NumberOfSubPackets;
            public List<Packet> SubPackets;

            public Packet()
            {
                Version = GetNextBits(3);
                Type = (TypeID)GetNextBits(3);

                if (Type == TypeID.LiteralValue)
                {
                    TotalSubPacketLength = NumberOfSubPackets = 0;
                    SubPackets = null;
                    List<bool> n = new();
                    bool lastGroup;
                    do
                    {
                        lastGroup = !bitArray[index++];
                        for (int i = 1; i <= 4; i++)
                            n.Add(bitArray[index++]);
                    } while (!lastGroup);
                    Value = BitsToValue(n);
                }
                else
                {
                    LengthTypeID = GetNextBits(1);
                    if (LengthTypeID == 0)
                        TotalSubPacketLength = GetNextBits(15);
                    else
                        NumberOfSubPackets = GetNextBits(11);
                    SubPackets = new();
                }
            }

            public int Evaluate()
            {
                if (Type == TypeID.LiteralValue)
                    return Value;

                List<int> parameters = new();

                foreach (Packet s in SubPackets)
                    parameters.Add(s.Evaluate());

                if (Type <= TypeID.OpMaximum)
                    return ArtithmeticOp(Type, parameters);
                else
                    return BooleanOp(Type, parameters[0], parameters[1]);
            }

            private int ArtithmeticOp(TypeID type, List<int> parameters)
            {
                throw new NotImplementedException();
            }

            private int BooleanOp(TypeID type, int v1, int v2)
            {
                throw new NotImplementedException();
            }
        }



        public Puzzle16(string input) : base(input) { Name = "Packet Decoder"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            string transmission = lines[0].ToUpper();
            bitArray = new bool[transmission.Length * 4];

            int bitIndex = 0;
            for (int i = 0; i < transmission.Length; i++, bitIndex += 4)
                DecodeHex(transmission[i], bitIndex);

            index = 0;
        }

        private void DecodeHex(char c, int bi)
        {
            int value = HexValue(c);
            for (int d = 3; d >= 0; d--)
            {
                bitArray[bi + d] = ((value % 2) == 1);
                value >>= 1;
            }
        }

        public static int HexValue(char hex) => (int)hex - ((int)hex <= (int)'9' ? (int)'0' : ((int)'A' - 10));

        private static int GetNextBits(int numBits)
        {
            int result = 0;

            for (int i = 0; i < numBits; i++)
                result = (result << 1) + (bitArray[index + i] ? 1 : 0);

            index += numBits;
            return result;
        }

        private void SkipPadding()
        {
            if (index % 8 != 0)
                index += (8 - index % 8);
        }

        private static int BitsToValue(List<bool> bits)
        {
            int result = 0;

            foreach (bool bit in bits)
                result = (result << 1) + (bit ? 1 : 0);

            return result;
        }

        private int ProcessSubPackets(Packet p)
        {
            int totalVersionNumbers = 0;

            if (p.LengthTypeID == 0)
            {
                int endingIndex = index + p.TotalSubPacketLength;
                do
                {
                    Packet subPacket = new Packet();
                    p.SubPackets.Add(subPacket);
                    totalVersionNumbers += subPacket.Version;
                    if (subPacket.Type != TypeID.LiteralValue)
                        totalVersionNumbers += ProcessSubPackets(subPacket);
                } while (index < endingIndex);
            }
            else
            {
                for (int i = 1; i <= p.NumberOfSubPackets; i++)
                {
                    Packet subPacket = new Packet();
                    p.SubPackets.Add(subPacket);
                    totalVersionNumbers += subPacket.Version;
                    if (subPacket.Type != TypeID.LiteralValue)
                        totalVersionNumbers += ProcessSubPackets(subPacket);
                }
            }

            return totalVersionNumbers;
        }

        [Description("what do you get if you add up the version numbers in all packets?")]
        public override string SolvePart1()
        {
            int totalVersionNumbers = 0;

            while (index < bitArray.Length)
            {
                Packet p = new Packet();
                if ((p.NumberOfSubPackets > 0) || (p.TotalSubPacketLength > 0))
                    totalVersionNumbers += ProcessSubPackets(p);
                SkipPadding();

                totalVersionNumbers += p.Version;
            }

            return totalVersionNumbers.ToString();
        }

        [Description("What do you get if you evaluate the expression represented by your hexadecimal-encoded BITS transmission?")]
        public override string SolvePart2()
        {

            return p.Evaluate();
        }
    }
}