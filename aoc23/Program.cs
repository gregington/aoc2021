using System;
using System.Collections.Immutable;

namespace aoc23;

class Program {
    static void Main(string[] args) {
        var initialBoard = ReadData();

        Part1(initialBoard);
        Part2(initialBoard);
    }

    private static void Part1(string[] lines) {
        var initialBoard = BoardFromLines(lines);
        var (minCost, history) = FindMinCost(initialBoard);
        if (minCost == null) {
            Console.WriteLine("Solve failed");
            return;
        }
        Console.WriteLine($"Minimum cost: {minCost}");
    }

    private static void Part2(string[] lines) {
        var linesList = lines.ToList();
        linesList.Insert(3, "  #D#C#B#A#");
        linesList.Insert(4, "  #D#B#A#C#");
        
        var initialBoard = BoardFromLines(linesList.ToArray());
        var (minCost, history) = FindMinCost(initialBoard);
        if (minCost == null) {
            Console.WriteLine("Solve failed");
            return;
        }
        Console.WriteLine($"Minimum cost, expanded board: {minCost}");
    }

    private static (int? Cost, ImmutableArray<Board> History) FindMinCost(Board initialBoard) {
        var costs = new Dictionary<Board, int>();
        costs.Add(initialBoard, 0);

        var priorityQueue = new PriorityQueue<Board, int>();
        priorityQueue.Enqueue(initialBoard, 0);

        var history = new Dictionary<Board, ImmutableArray<Board>>();
        history.Add(initialBoard, ImmutableArray.Create<Board>().Add(initialBoard));

        while (priorityQueue.Count > 0) {
            var board = priorityQueue.Dequeue();
            var cost = costs[board];

            if (board.Complete) {
                return (cost, history[board]);
            }

            var nextMoves = board.Moves()
                .Select(x => (x.Board, Cost: x.Cost + cost))
                .Where(x => x.Cost < costs.GetValueOrDefault(x.Board, int.MaxValue));

            foreach (var move in nextMoves) {
                costs[move.Board] = move.Cost;
                priorityQueue.Enqueue(move.Board, move.Cost);
                history[move.Board] = history[board].Add(move.Board);
            }
        }

        return (null, ImmutableArray<Board>.Empty);
    }

    private static Board BoardFromLines(string[] lines) {
        var hallwayLine = lines[1];
        var hallway = new string(hallwayLine.Where(c => c != '#').ToArray());
        var rooms = lines.Skip(2).Reverse().Skip(1).Reverse()
            .Select(line => line.Where(c => c != '#' && c != ' ').ToArray())
            .ToArray();

        var room0 = new String(rooms.Select(x => x[0]).ToArray());
        var room1 = new String(rooms.Select(x => x[1]).ToArray());
        var room2 = new String(rooms.Select(x => x[2]).ToArray());
        var room3 = new String(rooms.Select(x => x[3]).ToArray());

        return new Board(hallway, room0, room1, room2, room3);
    }
    private static string[] ReadData() {
        return File.ReadAllLines("input.txt");
    }
}