using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace AdventOfCode.Y2021
{
    public class Puzzle23 : ASolver 
    {
        private GameState startingState;
        private struct Room
        {
            public char Top;
            public char Bottom;

            public Room(char top, char bottom)
            {
                Top = top; Bottom = bottom;
            }
        }


        private class GameState
        {
            public const int HallSlots = 7;
            public const int NumRooms = 4;
            public const char EmptySlot = '.';
            public const char Amber = 'A';
            public int[] MoveCost = new int[] { 1, 10, 100, 1000 };
            public int[,] MoveSpaces = new int[,] { { 3, 2, 2, 4, 6, 8, 9 },
                                                    { 5, 4, 2, 2, 4, 6, 7 },
                                                    { 7, 6, 4, 2, 2, 4, 5 },
                                                    { 9, 8, 6, 4, 2, 2, 3 } };

            char[] Hall;
            Room[] Rooms;

            public GameState(string topLine, string bottomLine)
            {
                Hall = new char[HallSlots];
                for (int i = 0; i < HallSlots; i++)
                    Hall[i] = EmptySlot;

                Rooms = new Room[NumRooms];
                string[] tops = topLine.Split(new char[] { ' ', '#' }, StringSplitOptions.RemoveEmptyEntries);
                string[] bottoms = bottomLine.Split(new char[] { ' ', '#' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < NumRooms; i++)
                    Rooms[i] = new Room(tops[i][0], bottoms[i][0]);
            }

            public int Move(int from, int to)
            {
                char moving = GetSlot(from);

                if (moving == EmptySlot)
                    throw new ArgumentException("Move attempted from empty space", $"{from} -> {to}");
                if (GetSlot(to) != EmptySlot)
                    throw new ArgumentException("Move attempted to occupied space", $"{from} -> {to}");
                if ((from < HallSlots) && (to < HallSlots))
                    throw new ArgumentException("Move attempted from Hallway to Hallway", $"{from} -> {to}");

                SetSlot(to, moving);
                SetSlot(from, EmptySlot);

                return SpacesMoved(from, to) * CostPerSpace(moving);
            }

            public int CostPerSpace(char c) => MoveCost[c - Amber];

            public int SpacesMoved(int from, int to)
            {
                int spaces = 0;
                if (!IsHallSlot(from) && !IsHallSlot(to))
                {
                    int roomsMoved = Math.Abs(RoomOf(from) - RoomOf(to));
                    spaces = (roomsMoved * 2) + 2;
                }
                else
                {
                    int hallIndex = Math.Min(to, from);
                    int roomIndex = RoomOf(Math.Max(to, from));
                    spaces = MoveSpaces[roomIndex, hallIndex];
                }

                if (IsBottomSlot(from)) spaces++;
                if (IsBottomSlot(to)) spaces++;

                return spaces;
            }

            public char GetSlot(int slot)
            {
                if (slot < HallSlots)
                    return Hall[slot];
                else
                {
                    int room = (slot - HallSlots) / 2;
                    return (slot % 2 == 1) ? Rooms[room].Top : Rooms[room].Bottom;
                }
            }

            public void SetSlot(int slot, char c)
            {
                if (IsHallSlot(slot))
                    Hall[slot] = c;
                else if (IsBottomSlot(slot))
                    Rooms[RoomOf(slot)].Bottom = c;
                else
                    Rooms[RoomOf(slot)].Top = c;
            }

            public static int RoomOf(int slot) => (slot - HallSlots) / 2;
            public static bool IsHallSlot(int slot) => (slot < HallSlots);
            public static bool IsBottomSlot(int slot)
            {
                if (IsHallSlot(slot))
                    return false;
                else
                    return (slot % 2 == 0);
            }

            public override string ToString()
            {
                return "#############\n" +
                       $"#{Hall[0]}{Hall[1]}.{Hall[2]}.{Hall[3]}.{Hall[4]}.{Hall[5]}{Hall[6]}#\n" +
                       $"###{Rooms[0].Top}#{Rooms[1].Top}#{Rooms[2].Top}#{Rooms[3].Top}###\n" +
                       $"  #{Rooms[0].Bottom}#{Rooms[1].Bottom}#{Rooms[2].Bottom}#{Rooms[3].Bottom}#\n" +
                        "  #########";
            }

        }

        public Puzzle23(string input) : base(input) { Name = "Amphipod"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            startingState = new GameState(lines[2], lines[3]);
            Console.WriteLine();
            Console.WriteLine(startingState);
            Console.WriteLine($"Move cost = {startingState.Move(11, 2)}");
            Console.WriteLine(startingState);
            Console.WriteLine($"Move cost = {startingState.Move(9, 11)}");
            Console.WriteLine(startingState);
            Console.WriteLine($"Move cost = {startingState.Move(10, 3)}");
            Console.WriteLine(startingState);
            Console.WriteLine($"Move cost = {startingState.Move(2, 10)}");
            Console.WriteLine(startingState);
            Console.WriteLine($"Move cost = {startingState.Move(7, 9)}");
            Console.WriteLine(startingState);
        }

        [Description("What is the least energy required to organize the amphipods?")]
        public override string SolvePart1()
        {
            return string.Empty;
        }

        [Description("What is the least energy required to organize the amphipods?")]
        public override string SolvePart2()
        {
            Setup();    // Remove if Part 2 builds on output of Part 1

            return string.Empty;
        }
    }
}