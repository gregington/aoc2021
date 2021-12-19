using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace aoc15;
class Program {
    static void Main(string[] args) {
        var riskLevels = ReadData();
        var path = FindPath(riskLevels);
        Console.WriteLine($"Min risk: {path.Value.Risk}");
    }

    static Node? FindPath(ImmutableArray<ImmutableArray<int>> riskLevels) {
        var dest = new Point(riskLevels.Length - 1, riskLevels[0].Length - 1);

        var risks = Enumerable.Range(0, riskLevels.Length)
            .SelectMany(y => Enumerable.Range(0, riskLevels[0].Length).Select(x => new Point(x, y)))
            .ToDictionary(p => p, _ => int.MaxValue);

        var priorityQueue = new PriorityQueue<Node, int>();

        var startNode = new Node(new Point(0, 0), ImmutableArray.Create<Point>().Add(new Point(0, 0)), 0);
        priorityQueue.Enqueue(startNode, 0);

        while (priorityQueue.Count > 0) {
            var node = priorityQueue.Dequeue();
            if (node.Point == dest) {
                return node;
            }

            if (node.Risk > risks[node.Point]) {
                continue;
            }

            risks[node.Point] = node.Risk;

            var adjacent = Adjacent(node.Point, dest.X, dest.Y)
                .Select(a => node.MoveTo(a, riskLevels[a.Y][a.X]))
                .Where(x => x.Risk <= risks[x.Point])
                .Select(n => (n, n.Risk));

            priorityQueue.EnqueueRange(adjacent);
        }
        return null;
    }

    static IEnumerable<Point> Adjacent(Point point, int maxX, int maxY) {
        return new [] { 
                point with {X = point.X - 1, Y = point.Y},
                point with {X = point.X + 1, Y = point.Y},
                point with {X = point.X, Y = point.Y - 1},
                point with {X = point.X, Y = point.Y + 1}}
            .Where(p => p.X >= 0 && p.X <= maxX)
            .Where(p => p.Y >= 0 && p.Y <= maxY)
            .ToImmutableArray();
    }

    static ImmutableArray<ImmutableArray<int>> ReadData() {
        return File.ReadLines("input.txt")
            .Select(line => line.Select(c => Convert.ToInt32(c.ToString())).ToImmutableArray())
            .ToImmutableArray();
    }
}