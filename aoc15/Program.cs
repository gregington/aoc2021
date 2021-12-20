using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;

namespace aoc15;
class Program {
    static void Main(string[] args) {
        var riskLevels = ReadData();
        var node = FindPath(riskLevels);
        Console.WriteLine($"Min risk: {node.Value.Risk}");

        var expandedRiskLevels = ExpandRiskLevels(riskLevels);
        var expandedNode = FindPath(expandedRiskLevels);
        Console.WriteLine($"Min expanded risk: {expandedNode.Value.Risk}");
    }

    static Node? FindPath(ImmutableArray<ImmutableArray<int>> riskLevels) {
        var stopwatch = Stopwatch.StartNew();
        var dest = new Point(riskLevels.Length - 1, riskLevels[0].Length - 1);

        var risks = Enumerable.Range(0, riskLevels.Length)
            .Select(y => {
                var arr = new int[riskLevels[0].Length];
                Array.Fill(arr, int.MaxValue);
                return arr;
            }).ToArray();

        var priorityQueue = new PriorityQueue<Node, int>();

        var startNode = new Node(new Point(0, 0), 0);
        priorityQueue.Enqueue(startNode, 0);

        while (priorityQueue.Count > 0) {
            var node = priorityQueue.Dequeue();
            if (node.Point == dest) {
                Console.WriteLine($"{stopwatch.Elapsed}");
                return node;
            }

            if (node.Risk > risks[node.Point.Y][node.Point.X]) {
                continue;
            }

            var adjacent = Adjacent(node.Point, dest.X, dest.Y)
                .Select(a => node.MoveTo(a, riskLevels[a.Y][a.X]))
                .Where(n => n.Risk < risks[n.Point.Y][n.Point.X]);

            foreach (var adj in adjacent) {
                risks[adj.Point.Y][adj.Point.X] = adj.Risk;
                priorityQueue.Enqueue(adj, adj.Risk);
            }
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

    static ImmutableArray<ImmutableArray<int>> ExpandRiskLevels(ImmutableArray<ImmutableArray<int>> riskLevels) {
        const int multiplier = 5;
        var rows = riskLevels.Length;
        var cols = riskLevels[0].Length;
        var newRows = rows * multiplier;
        var newCols = cols * multiplier;

        var newRiskLevels = Enumerable.Range(0, newRows)
            .Select(r => new int[newCols])
            .ToArray();

        for (var tileRow = 0; tileRow < multiplier; tileRow++) {
            for (var tileCol = 0; tileCol < multiplier; tileCol++) {
                MakeCopy(riskLevels, tileRow * rows, tileCol * cols, tileRow + tileCol, newRiskLevels);
            }
        }

        return newRiskLevels.Select(r => r.ToImmutableArray()).ToImmutableArray();
    }

    static void MakeCopy(ImmutableArray<ImmutableArray<int>> riskLevels, int startRow, int startCol, int toAdd, int[][] newRiskLevels) {
        for (int r = 0; r < riskLevels.Length; r++) {
            for (int c = 0; c < riskLevels[0].Length; c++) {
                newRiskLevels[startRow + r][startCol + c] = ((riskLevels[r][c] - 1 + toAdd) % 9) + 1;
            }
        }
    }
    static ImmutableArray<ImmutableArray<int>> ReadData() {
        return File.ReadLines("input.txt")
            .Select(line => line.Select(c => Convert.ToInt32(c.ToString())).ToImmutableArray())
            .ToImmutableArray();
    }
}