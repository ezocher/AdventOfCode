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
        private HashSet<string> visitedStates;
        private Queue<GameState> gameStates;
        private int lowestCost;

        private struct Room
        {
            public char Top;
            public char Bottom;

            public Room(char top, char bottom)
            {
                Top = top; Bottom = bottom;
            }

        }

        private static int[] MoveCost = new int[] { 1, 10, 100, 1000 };
        private static int[,] MoveSpaces = new int[,] { { 3, 2, 2, 4, 6, 8, 9 },
                                                    { 5, 4, 2, 2, 4, 6, 7 },
                                                    { 7, 6, 4, 2, 2, 4, 5 },
                                                    { 9, 8, 6, 4, 2, 2, 3 } };
        private class GameState
        {
            public const int HallSlots = 7;
            public const int NumRooms = 4;
            public const char EmptySlot = '.';
            public const char Amber = 'A';

            char[] Hall;
            Room[] Rooms;
            public int CumulativeCost;

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
                CumulativeCost = 0;
            }

            public GameState(GameState gs)
            {
                Hall = new char[HallSlots];
                Rooms = new Room[NumRooms];
                Array.Copy(gs.Hall, Hall, HallSlots);
                Array.Copy(gs.Rooms, Rooms, NumRooms);
                CumulativeCost = gs.CumulativeCost;
            }

            public bool Move(int from, int to)
            {
                char moving = GetSlotContent(from);

                if (moving == EmptySlot)
                    throw new ArgumentException("Move attempted from empty space", $"{from} -> {to}");
                if (GetSlotContent(to) != EmptySlot)
                    throw new ArgumentException("Move attempted to occupied space", $"{from} -> {to}");
                if ((from < HallSlots) && (to < HallSlots))
                    throw new ArgumentException("Move attempted from Hallway to Hallway", $"{from} -> {to}");

                SetSlotContent(to, moving);
                SetSlotContent(from, EmptySlot);

                CumulativeCost += SpacesMoved(from, to) * CostPerSpace(moving);

                return Solved();
            }

            public int CostPerSpace(char c) => MoveCost[c - Amber];

            public int SpacesMoved(int from, int to)
            {
                int spaces = 0;
                if (!IsHallSlot(from) && !IsHallSlot(to))
                {
                    int roomsMoved = Math.Abs(RoomIndexOf(from) - RoomIndexOf(to));
                    spaces = (roomsMoved * 2) + 2;
                }
                else
                {
                    int hallIndex = Math.Min(to, from);
                    int roomIndex = RoomIndexOf(Math.Max(to, from));
                    spaces = MoveSpaces[roomIndex, hallIndex];
                }

                if (IsBottomSlot(from)) spaces++;
                if (IsBottomSlot(to)) spaces++;

                return spaces;
            }

            public char GetSlotContent(int slot)
            {
                if (slot < HallSlots)
                    return Hall[slot];
                else
                {
                    int room = RoomIndexOf(slot);
                    return (slot % 2 == 1) ? Rooms[room].Top : Rooms[room].Bottom;
                }
            }

            public void SetSlotContent(int slot, char c)
            {
                if (IsHallSlot(slot))
                    Hall[slot] = c;
                else if (IsBottomSlot(slot))
                    Rooms[RoomIndexOf(slot)].Bottom = c;
                else
                    Rooms[RoomIndexOf(slot)].Top = c;
            }

            public string MinString()
            {
                char[] roomChars = new char[NumRooms * 2];
                for (int i = 0; i < NumRooms; i++)
                {
                    roomChars[i] = Rooms[i].Top;
                    roomChars[i + 4] = Rooms[i].Bottom;
                }
                return new string(Hall) + new string(roomChars);
            }

            public override string ToString()
            {
                return $"#{Hall[0]}{Hall[1]}^{Hall[2]}^{Hall[3]}^{Hall[4]}^{Hall[5]}{Hall[6]}# | " +
                       $"#{Rooms[0].Top}#{Rooms[1].Top}#{Rooms[2].Top}#{Rooms[3].Top}# | " +
                       $"#{Rooms[0].Bottom}#{Rooms[1].Bottom}#{Rooms[2].Bottom}#{Rooms[3].Bottom}#";
            }

            public string ToWriteString()
            {
                return "#############\n" +
                       $"#{Hall[0]}{Hall[1]}.{Hall[2]}.{Hall[3]}.{Hall[4]}.{Hall[5]}{Hall[6]}#\n" +
                       $"###{Rooms[0].Top}#{Rooms[1].Top}#{Rooms[2].Top}#{Rooms[3].Top}###\n" +
                       $"  #{Rooms[0].Bottom}#{Rooms[1].Bottom}#{Rooms[2].Bottom}#{Rooms[3].Bottom}#\n" +
                        "  #########";
            }
            public bool Solved()
            {
                for (int i = 0; i < HallSlots; i++)
                    if (Hall[i] != EmptySlot)
                        return false;

                for (int i = 0; i < NumRooms; i++)
                    if ((Rooms[i].Top != (char)(Amber + i)) || (Rooms[i].Bottom != (char)(Amber + i)))
                        return false;

                return true;
            }

            // Find and return all legal moves from the current GameState
            public List<(int, int)> GetPossibleMoves()
            {
                List<(int, int)> possibleMoves = new();

                // Possible moves from hall to destination room
                for (int i = 0; i < HallSlots; i++)
                    if (Hall[i] != EmptySlot)
                    {
                        int destRoom = RoomIndexOf(Hall[i]);
                        int availableSlot = RoomAvailableForMove(destRoom);
                        if ((availableSlot > RoomUnavailable) && (PathIsUnblocked(i, destRoom)))
                            possibleMoves.Add((i, availableSlot));
                    }

                for (int roomIndex = 0; roomIndex < NumRooms; roomIndex++)
                    if (!RoomIsSolved(roomIndex))
                    {
                        int topmostOccupiedSlot = TopFilledSlot(roomIndex);
                        if (!(topmostOccupiedSlot == RoomEmpty))
                        {
                            // --- Room to Room ---
                            // Possible moves from topmost full slot of room to destination room
                            if (RoomIndexOf(GetSlotContent(topmostOccupiedSlot)) != roomIndex)
                            {
                                int destRoom = RoomIndexOf(GetSlotContent(topmostOccupiedSlot));
                                int availableSlot = RoomAvailableForMove(destRoom);
                                if ((availableSlot > RoomUnavailable) && (PathIsUnblocked(topmostOccupiedSlot, destRoom)))
                                    possibleMoves.Add((topmostOccupiedSlot, availableSlot));
                            }

                            // --- Room to Hall ---
                            // Possible moves from topmost full slot of room to hall
                            // Need to consider moves from "final destination room" if there is another slot filled underneath with a different letter
                            if ((IsBottomSlot(topmostOccupiedSlot) && (RoomIndexOf(Rooms[roomIndex].Bottom) != roomIndex)) 
                            //    (RoomIndexOf(Rooms[roomIndex].Bottom) != roomIndex))
                            //{
                            //    List<int> openHallSlots = AvailableReachableHallSlots(roomIndex);
                            //    if (openHallSlots.Count > 0)
                            //        foreach (int destSlot in openHallSlots)
                            //            possibleMoves.Add((topmostOccupiedSlot, destSlot));
                            //}
                        }

                    }

                return possibleMoves;
            }

            public List<int> AvailableReachableHallSlots(int roomIndex)
            {
                List<int> availSlots = new();
                int roomRelative = roomIndex + 1;

                for (int i = roomRelative; i >= 0; i--)
                {
                    if (Hall[i] == EmptySlot)
                        availSlots.Add(i);
                    else
                        break;
                }

                for (int i = roomRelative + 1; i < HallSlots; i++)
                {
                    if (Hall[i] == EmptySlot)
                        availSlots.Add(i);
                    else
                        break;
                }

                return availSlots;
            }

            public bool RoomIsSolved(int roomIndex) => ((RoomIndexOf(Rooms[roomIndex].Top) == roomIndex) && (RoomIndexOf(Rooms[roomIndex].Bottom) == roomIndex));
            public static int RoomIndexOf(char c) => (int)(c - Amber);
            public static int RoomIndexOf(int slotNumber) => (slotNumber - HallSlots) / 2;
            public static bool IsHallSlot(int slotNumber) => (slotNumber < HallSlots);
            public static bool IsBottomSlot(int slotNumber)
            {
                if (IsHallSlot(slotNumber))
                    return false;
                else
                    return (slotNumber % 2 == 0);
            }
            public int TopSlotNumber(int roomIndex) => roomIndex * 2 + HallSlots;
            public int BottomSlotNumber(int roomIndex) => roomIndex * 2 + HallSlots + 1;

            const int RoomEmpty = -1;
            public int TopFilledSlot(int roomIndex)
            {
                if (Rooms[roomIndex].Top != EmptySlot)
                    return TopSlotNumber(roomIndex);
                else if (Rooms[roomIndex].Bottom != EmptySlot)
                    return BottomSlotNumber(roomIndex);
                else
                    return RoomEmpty;
            }

            const int RoomUnavailable = -1;
            // Returns index of slot if available
            public int RoomAvailableForMove(int roomIndex)
            {
                if ((Rooms[roomIndex].Top != EmptySlot) || (Rooms[roomIndex].Bottom != (char)(Amber + roomIndex)))
                    return RoomUnavailable;
                else if (Rooms[roomIndex].Bottom == EmptySlot)
                    return BottomSlotNumber(roomIndex);
                else
                    return TopSlotNumber(roomIndex);
            }

            public bool PathIsUnblocked(int from, int destRoomIndex)
            {
                if (from > HallSlots)
                {
                    // Convert room positions to proper hall positions to get correct checks for empty slots
                    int roomIndex = RoomIndexOf(from);
                    if (roomIndex == 0)
                        from = 1;
                    else if (roomIndex == 3)
                        from = 5;
                    else if (destRoomIndex > roomIndex)
                        from = roomIndex + 1;
                    else
                        from = roomIndex + 2;
                }

                // Check edges and "slide in" if unblocked
                if (from == 0)
                    if (Hall[1] != EmptySlot)
                        return false;
                    else
                        from = 1;
                if (from == 6)
                    if (Hall[5] != EmptySlot)
                        return false;
                    else
                        from = 5;

                int roomRelative = destRoomIndex + 1;

                if (from <= roomRelative)
                {
                    // Moving right
                    if (from == roomRelative)
                        return true;
                    for (int i = from + 1; i <= roomRelative; i++)
                        if (Hall[i] != EmptySlot)
                            return false;
                    return true;
                }
                else
                {
                    // Moving left
                    if (from - 1 == roomRelative)
                        return true;
                    for (int i = from - 1; i > roomRelative; i--)
                        if (Hall[i] != EmptySlot)
                            return false;
                    return true;
                }

            }
        }

        public Puzzle23(string input) : base(input) { Name = "Amphipod"; }

        public override void Setup()
        {
            List<string> lines = Tools.GetLines(Input);

            startingState = new GameState(lines[2], lines[3]);

            visitedStates = new();
            visitedStates.Add(startingState.MinString());

            gameStates = new();
            gameStates.Enqueue(startingState);

            lowestCost = int.MaxValue;
        }

        [Description("What is the least energy required to organize the amphipods?")]
        public override string SolvePart1()
        {
            List<(int, int)> possibleMoves = null;

            while (gameStates.Count > 0)
            {
                GameState gs = gameStates.Dequeue();
                possibleMoves = gs.GetPossibleMoves();
                
                // If a GameState has no legal moves it will return a zero lenght possible moves
                if (possibleMoves.Count > 0)
                    foreach ((int from, int to) in possibleMoves)
                    {
                        if ((visitedStates.Count % 1000) == 0)
                            Console.WriteLine($"Visited = {visitedStates.Count}, Queued = {gameStates.Count}");
                        GameState nextMove = new GameState(gs);
                        bool solved = nextMove.Move(from, to);
                        if (!visitedStates.Contains(nextMove.MinString()))
                        {
                            visitedStates.Add(nextMove.MinString());
                            if (solved)
                            {
                                if (nextMove.CumulativeCost < lowestCost)
                                    lowestCost = nextMove.CumulativeCost;
                            }
                            else
                            {
                                gameStates.Enqueue(nextMove);
                            }
                        }
                    }
            }
            return lowestCost.ToString();
        }

        [Description("What is the least energy required to organize the amphipods?")]
        public override string SolvePart2()
        {
            Setup();    // Remove if Part 2 builds on output of Part 1

            return string.Empty;
        }
    }
}