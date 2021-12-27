using System;

namespace aoc23;

class Program {
    static void Main(string[] args) {
        var initialBoard = ReadData();

        var minCost = FindMinCost(initialBoard);
        if (minCost == null) {
            Console.WriteLine("Solve failed");
            return;
        }
        Console.WriteLine($"Minimum cost: {minCost}");
    }

    private static int? FindMinCost(Board initialBoard) {
        var costs = new Dictionary<Board, int>();
        costs.Add(initialBoard, 0);

        var priorityQueue = new PriorityQueue<Board, int>();
        priorityQueue.Enqueue(initialBoard, 0);

        while (priorityQueue.Count > 0) {
            var board = priorityQueue.Dequeue();
            var cost = costs[board];

            if (board.Complete) {
                return cost;
            }

            var nextMoves = board.Moves()
                .Select(x => (x.Board, Cost: x.Cost + cost))
                .Where(x => x.Cost < costs.GetValueOrDefault(x.Board, int.MaxValue));

            foreach (var move in nextMoves) {
                costs[move.Board] = move.Cost;
                priorityQueue.Enqueue(move.Board, move.Cost);
            }
        }

        return null;
    }

    private static Board ReadData() {
        var text = File.ReadAllLines("input.txt");
        var hallwayLine = text[1];
        var hallway = new string(hallwayLine.Where(c => c != '#').ToArray());
        var rooms = text.Skip(2).Reverse().Skip(1).Reverse()
            .Select(line => line.Where(c => c != '#' && c != ' ').ToArray())
            .ToArray();

        var room0 = new String(rooms.Select(x => x[0]).ToArray());
        var room1 = new String(rooms.Select(x => x[1]).ToArray());
        var room2 = new String(rooms.Select(x => x[2]).ToArray());
        var room3 = new String(rooms.Select(x => x[3]).ToArray());

        return new Board(hallway, room0, room1, room2, room3);
    }
}