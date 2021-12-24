using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Y2021
{
    public partial class Puzzle23 : ASolver
    {
        private class Room2
        {
            public const int RoomSize = 4;

            // Top is index 0
            public char[] Slots;

            public Room2(char[] contents)
            {
                Slots = new char[RoomSize];
                for (int i = 0; i < RoomSize; i++)
                    Slots[i] = contents[i];
            }
            public Room2()
            {
                Slots = new char[RoomSize];
            }
        }

        // TODO: Remove Done/Unchanged/New comments from refactoring work
        private class GameState2
        {
            public const int HallSlots = 7;
            public const int NumRooms = 4;
            public const char EmptySlot = '.';
            public const char Amber = 'A';
            public const char CompletedSlot = '*';
            private static int[] MoveCost = new int[] { 1, 10, 100, 1000 };
            private static int[,] MoveSpaces = new int[,] { { 3, 2, 2, 4, 6, 8, 9 },
                                                            { 5, 4, 2, 2, 4, 6, 7 },
                                                            { 7, 6, 4, 2, 2, 4, 5 },
                                                            { 9, 8, 6, 4, 2, 2, 3 } };
            char[] Hall;
            Room2[] Rooms;
            public int CumulativeCost;

            public GameState2(string[] lines) // Done
            {
                Hall = new char[HallSlots];
                for (int i = 0; i < HallSlots; i++)
                    Hall[i] = EmptySlot;

                Rooms = new Room2[NumRooms];
                List<string[]> splitLines = new();
                foreach (string line in lines)
                    splitLines.Add(line.Split(new char[] { ' ', '#' }, StringSplitOptions.RemoveEmptyEntries));

                for (int room = 0; room < NumRooms; room++)
                {
                    Rooms[room] = new Room2(new char[] { splitLines[0][room][0], splitLines[1][room][0], splitLines[2][room][0], splitLines[3][room][0] });

                    for (int slot = Room2.RoomSize - 1; slot >= 0; slot--)
                    {
                        if (InDestinationRoom(room, Rooms[room].Slots[slot]))
                            Rooms[room].Slots[slot] = CompletedSlot;
                        else
                            break;
                    }
                }

                CumulativeCost = 0;
            }

            public GameState2(GameState2 gs) // Done
            {
                Hall = new char[HallSlots];
                Rooms = new Room2[NumRooms];
                Array.Copy(gs.Hall, Hall, HallSlots);
                for (int room = 0; room < NumRooms; room++)
                {
                    Rooms[room] = new();
                    Array.Copy(gs.Rooms[room].Slots, Rooms[room].Slots, Room2.RoomSize);
                }
                CumulativeCost = gs.CumulativeCost;
            }

            public bool Move(int from, int to) // Done
            {
                char pieceMoving = GetSlotContent(from);

                if (pieceMoving == EmptySlot)
                    throw new ArgumentException("Move attempted from empty space", $"{from} -> {to}");
                if (GetSlotContent(to) != EmptySlot)
                    throw new ArgumentException("Move attempted to occupied space", $"{from} -> {to}");
                if ((from < HallSlots) && (to < HallSlots))
                    throw new ArgumentException("Move attempted from Hallway to Hallway", $"{from} -> {to}");

                CumulativeCost += SpacesMoved(from, to) * CostPerSpace(pieceMoving);

                SetSlotContent(from, EmptySlot);

                if (IsRoomSlot(to))
                {
                    if (RoomIndexOf(pieceMoving) != RoomIndexOf(to))
                        throw new ArgumentException("Move attempted to non-matching destination room", $"{from} -> {to}");
                    pieceMoving = CompletedSlot;
                }

                SetSlotContent(to, pieceMoving);

                return Solved();
            }

            public int CostPerSpace(char c) => MoveCost[c - Amber]; // Unchanged

            public int SpacesMoved(int from, int to) // Done
            {
                int spaces = 0;
                if (IsRoomSlot(from) && IsRoomSlot(to))
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

                if (IsRoomSlot(from))
                    spaces += RoomSlotIndexOf(from);
                if (IsRoomSlot(to))
                    spaces += RoomSlotIndexOf(to);
                return spaces;
            }

            public char GetSlotContent(int slot) // Done
            {
                if (slot < HallSlots)
                    return Hall[slot];
                else
                {
                    (int room, int roomSlot) = RoomAndSlotIndex(slot);
                    return Rooms[room].Slots[roomSlot];
                }
            }

            public void SetSlotContent(int slot, char c) // Done
            {
                if (IsHallSlot(slot))
                    Hall[slot] = c;
                else
                {
                    (int room, int roomSlot) = RoomAndSlotIndex(slot);
                    Rooms[room].Slots[roomSlot] = c;
                }
            }

            public string ToMinString() // Done
            {
                char[] roomChars = new char[NumRooms * Room2.RoomSize];
                for (int room = 0; room < NumRooms; room++)
                    for (int slot = 0; slot < Room2.RoomSize; slot++)
                        roomChars[room + (slot * NumRooms)] = Rooms[room].Slots[slot];

                return new string(Hall) + new string(roomChars);
            }

            public override string ToString() //Done
            {
                string first2Lines =
                    $"#{Hall[0]}{Hall[1]}^{Hall[2]}^{Hall[3]}^{Hall[4]}^{Hall[5]}{Hall[6]}# | " +
                    $"#{Rooms[0].Slots[0]}#{Rooms[1].Slots[0]}#{Rooms[2].Slots[0]}#{Rooms[3].Slots[0]}# |";

                string roomLines = "";
                for (int slot = 1; slot < Room2.RoomSize; slot++)
                    roomLines += $"#{Rooms[0].Slots[slot]}#{Rooms[1].Slots[slot]}#{Rooms[2].Slots[slot]}#{Rooms[3].Slots[slot]}# | ";

                return first2Lines + roomLines;
            }

            public string ToWriteString() // Done
            {
                string first3Lines = "#############\n" +
                       $"#{Hall[0]}{Hall[1]}.{Hall[2]}.{Hall[3]}.{Hall[4]}.{Hall[5]}{Hall[6]}#\n" +
                       $"###{Rooms[0].Slots[0]}#{Rooms[1].Slots[0]}#{Rooms[2].Slots[0]}#{Rooms[3].Slots[0]}###\n";

                string roomLines = "";
                for (int slot = 1; slot < Room2.RoomSize; slot++)
                    roomLines += $"  #{Rooms[0].Slots[slot]}#{Rooms[1].Slots[slot]}#{Rooms[2].Slots[slot]}#{Rooms[3].Slots[slot]}#\n";

                return first3Lines + roomLines + "  #########\n";
            }

            public bool Solved() //Done
            {
                for (int room = 0; room < NumRooms; room++)
                    for (int slot = 0; slot < Room2.RoomSize; slot++)
                        if (Rooms[room].Slots[slot] != CompletedSlot)
                            return false;

                return true;
            }

            // Find and return all legal moves from the current GameState
            public List<(int, int)> GetPossibleMoves() // Done
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
                        if (!(topmostOccupiedSlot == RoomEmpty) && (GetSlotContent(topmostOccupiedSlot) != CompletedSlot))
                        {
                            // --- Room to Room ---
                            // Possible moves from topmost full slot of room to destination room
                            if (GetSlotContent(topmostOccupiedSlot) != CompletedSlot)
                            {
                                int destRoom = RoomIndexOf(GetSlotContent(topmostOccupiedSlot));
                                int availableSlot = RoomAvailableForMove(destRoom);
                                if ((availableSlot > RoomUnavailable) && (PathIsUnblocked(topmostOccupiedSlot, destRoom)))
                                    possibleMoves.Add((topmostOccupiedSlot, availableSlot));
                            }

                            // --- Room to Hall ---
                            // Possible moves from topmost full slot of room to hall
                            List<int> openHallSlots = AvailableReachableHallSlots(roomIndex);
                            if (openHallSlots.Count > 0)
                                foreach (int destSlot in openHallSlots)
                                    possibleMoves.Add((topmostOccupiedSlot, destSlot));
                        }
                    }

                return possibleMoves;
            }

            public List<int> AvailableReachableHallSlots(int roomIndex) // Unchanged
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

            public static (int, int) RoomAndSlotIndex(int slotNumber) => (RoomIndexOf(slotNumber), RoomSlotIndexOf(slotNumber)); // New

            public static int RoomSlotIndexOf(int slotNumber) => slotNumber - HallSlots - (RoomIndexOf(slotNumber) * Room2.RoomSize); // New

            public bool RoomIsSolved(int roomIndex) // Done
            {
                for (int slot = 0; slot < Room2.RoomSize; slot++)
                    if (Rooms[roomIndex].Slots[slot] != CompletedSlot)
                        return false;

                return true;
            }
            public bool InDestinationRoom(int roomIndex, char c) => (RoomIndexOf(c) == roomIndex); // Done
            public static int RoomIndexOf(char c) => (int)(c - Amber);  // Done
            public static int RoomIndexOf(int slotNumber) => (slotNumber - HallSlots) / Room2.RoomSize; // Done
            
            public static bool IsHallSlot(int slotNumber) => (slotNumber < HallSlots);  // Done
            public static bool IsRoomSlot(int slotNumber) => (slotNumber >= HallSlots); // Done

            public static bool IsBottomSlot(int slotNumber) => IsRoomSlot(slotNumber) && (slotNumber % Room2.RoomSize == 2); // Done
            public static bool IsTopSlot(int slotNumber) => IsRoomSlot(slotNumber) && (slotNumber % Room2.RoomSize == 3); // Done
            public int TopSlotNumber(int roomIndex) => roomIndex * Room2.RoomSize + HallSlots; //Done
            public int BottomSlotNumber(int roomIndex) => roomIndex * Room2.RoomSize + HallSlots + 3; //Done

            const int RoomEmpty = -1;
            public int TopFilledSlot(int roomIndex) //Done
            {
                int topSlotIndex = TopSlotNumber(roomIndex);

                for (int i = 0; i < Room2.RoomSize; i++)
                {
                    if (Rooms[roomIndex].Slots[i] != EmptySlot)
                        return topSlotIndex + i;
                }
                return RoomEmpty;
            }

            const int RoomUnavailable = -1;
            // Returns index of slot if available
            public int RoomAvailableForMove(int roomIndex)  // Done
            {
                int topFilledSlot = TopFilledSlot(roomIndex);

                if (topFilledSlot == RoomEmpty)
                    return BottomSlotNumber(roomIndex);
                else if (topFilledSlot == TopSlotNumber(roomIndex))
                    return RoomUnavailable;

                for (int i = topFilledSlot; i <= BottomSlotNumber(roomIndex); i ++)
                {
                    if (GetSlotContent(i) != CompletedSlot)
                        return RoomUnavailable;
                }
                return topFilledSlot - 1;
            }

            public bool PathIsUnblocked(int from, int destRoomIndex)  // Unchanged
            {
                if (IsRoomSlot(from))
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
    }
}
