using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace aoc23;

public readonly record struct Board(string Hallway, string Room0, string Room1, string Room2, string Room3) {
    private const int HallwayWidth = 11;

    private static readonly ImmutableHashSet<int> ForbiddenHallwayIndexes = new [] {  2, 4, 6, 8 }.ToImmutableHashSet();

    private static readonly ImmutableDictionary<char, int> TargetHallwayIndices = new Dictionary<char, int> {
        ['A'] = 2,
        ['B'] = 4,
        ['C'] = 6,
        ['D'] = 8
    }.ToImmutableDictionary();

    private static readonly ImmutableDictionary<char, int> CostPerMove = new Dictionary<char, int> {
        ['A'] = 1,
        ['B'] = 10,
        ['C'] = 100,
        ['D'] = 1000
    }.ToImmutableDictionary();

    public bool Complete => Room0.All(c => c == 'A') && Room1.All(c => c == 'B') && Room2.All(c => c == 'C') && Room3.All(c => c == 'D');

    public int RoomSize => Room0.Length;

    public IEnumerable<(Board Board, int Cost)> Moves() {
        return MovesFromHallway().Concat(MovesFromRooms());
    }

    private IEnumerable<string> Rooms() {
        yield return Room0;
        yield return Room1;
        yield return Room2;
        yield return Room3;
    }

    private IEnumerable<(Board Board, int Cost)> MovesFromHallway() {
        var self = this;
        return Enumerable.Range(0, HallwayWidth)
            .Zip(Hallway, (a, b) => (FromIndex: a, Amphipod: b))
            .Where(x => x.Amphipod != '.')
            .Select(x => (x.FromIndex, ToIndex: TargetHallwayIndices[x.Amphipod], x.Amphipod, Room: self.RoomForAmphipod(x.Amphipod)))
            .Where(x => x.Room.All(c => c == '.' || c == x.Amphipod))
            .Where(x => self.HasClearPath(x.FromIndex, x.ToIndex))
            .Select(x => (x.FromIndex, HallwayDistance: Math.Abs(x.FromIndex - x.ToIndex), RoomDistance: x.Room.LastIndexOf('.') + 1, x.Amphipod))
            .Select(x => (x.FromIndex, Cost: (x.HallwayDistance + x.RoomDistance) * CostPerMove[x.Amphipod]))
            .Select(x => (Board: self.MoveToRoom(x.FromIndex), x.Cost));
    }

    private IEnumerable<(Board, int Cost)> MovesFromRooms() {
        var self = this;
        return Enumerable.Range(0, 4)
            .Zip(Rooms(), (i, r) => (RoomIndex: i, Room: r))
            .Select(x => (x.RoomIndex, x.Room, Amphipod: x.Room.Where(c => c != '.').FirstOrDefault('.')))
            .Where(x => x.Amphipod != '.')
            .Select(x => (x.RoomIndex, x.Room, AmphipodIndex: x.Room.IndexOf(x.Amphipod), x.Amphipod))
            .Select(x => (x.RoomIndex, x.Room, x.AmphipodIndex, x.Amphipod, HallwayIndexes: self.GetHallwayDestinations(x.RoomIndex)))
            .Select(x => x.HallwayIndexes.Select(y => (x.RoomIndex, x.Room, x.Amphipod, HallwayIndex: y)))
            .SelectMany(x => x)
            .Select(x => (x.RoomIndex, x.Room, x.Amphipod, x.HallwayIndex, HallwayDistance: Math.Abs(x.HallwayIndex - HallwayIndexForRoom(x.RoomIndex)), RoomDistance: x.Room.IndexOf(x.Amphipod) + 1))
            .Select(x => (Board: self.MoveToHallway(x.RoomIndex, x.HallwayIndex, x.Amphipod), Cost: (x.HallwayDistance + x.RoomDistance) * CostPerMove[x.Amphipod]));
    }

    private IEnumerable<int> GetHallwayDestinations(int roomIndex) {
        var fromIndex = HallwayIndexForRoom(roomIndex);

        var destinations = new List<int>();
        // go left
        for (int i = fromIndex - 1; i >= 0; i--) {
            if (Hallway[i] != '.') {
                break;
            }
            destinations.Add(i);
        }

        // go right
        for (int i = fromIndex + 1; i < Hallway.Length; i++) {
            if (Hallway[i] != '.') {
                break;
            }
            destinations.Add(i);
        }

        return destinations.Where(x => !ForbiddenHallwayIndexes.Contains(x));
    }

    private static int HallwayIndexForRoom(int roomIndex) => (roomIndex + 1) * 2;

    private Board MoveToRoom(int fromIndex) {
        var amphipod = Hallway[fromIndex];
        var hallwayChars = Hallway.Select(c => c).ToArray();
        hallwayChars[fromIndex] = '.';
        var newHallway = new string(hallwayChars);

        var rooms = Rooms().ToArray();
        var roomIndex = amphipod - 'A';
        var room = rooms[roomIndex];
        var freeIndex = room.LastIndexOf('.');
        var roomChars = room.Select(c => c).ToArray();
        roomChars[freeIndex] = amphipod;
        var newRoom = new string(roomChars);
        rooms[roomIndex] = newRoom;

        return new Board(newHallway, rooms[0], rooms[1], rooms[2], rooms[3]);
    }

    private Board MoveToHallway(int roomIndex, int hallwayIndex, char amphipod) {
        var rooms = Rooms().ToArray();
        var room = rooms[roomIndex];
        var indexInRoom = room.IndexOf(amphipod);
        var roomChars = room.Select(c => c).ToArray();
        roomChars[indexInRoom] = '.';
        rooms[roomIndex] = new string(roomChars);

        var hallwayChars = Hallway.Select(c => c).ToArray();
        hallwayChars[hallwayIndex] = amphipod;
        var newHallway = new string(hallwayChars);

        return new Board(newHallway, rooms[0], rooms[1], rooms[2], rooms[3]);
    }

    private string RoomForAmphipod(char amphipod) =>
        amphipod switch {
            'A' => Room0,
            'B' => Room1,
            'C' => Room2,
            'D' => Room3,
            _ => throw new ArgumentException($"Unknown amphipod {amphipod}")
        };

    private bool HasClearPath(int fromIndex, int toIndex) {
        var substr = fromIndex < toIndex 
            ? Hallway.Substring(fromIndex + 1, toIndex - fromIndex)
            : Hallway.Substring(toIndex, fromIndex - toIndex);

        return substr.All(c => c == '.');
    }
    public override string ToString() {
        var sb = new StringBuilder();
        
        var self = this;
        var roomsChars = Enumerable.Range(0, Room0.Length)
            .Select(i => new [] { self.Room0[i], self.Room1[i], self.Room2[i], self.Room3[i] })
            .ToArray();


        sb.Append(string.Join("", Enumerable.Repeat("#", HallwayWidth + 2)));
        sb.Append('\n');
        sb.Append('#');
        sb.Append(Hallway);
        sb.Append("#\n");
        sb.Append("###");
        sb.Append(roomsChars.Select(c => string.Join("#", c)).First());
        sb.Append("###\n");
        foreach (var line in roomsChars.Skip(1)) {
            sb.Append("  #");
            sb.Append(string.Join("#", line));
            sb.Append("#\n");
        }
        sb.Append("  #########");
        return sb.ToString();
    }
}