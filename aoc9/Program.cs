using System;
using System.Collections.Immutable;
using System.IO;

namespace aoc9;

class Program {
    static void Main(string[] args) {
        var heights = ReadData();

        var adjacencies = Enumerable.Range(0, heights.Length)
            .SelectMany(r => Enumerable.Range(0, heights[0].Length).Select(c => new Point(r, c)))
            .Select(x => (Point: x, Height: heights[x.Row][x.Col], AdjacentPoints: AdjacentPoints(heights, x.Row, x.Col)));

        var lowPoints = adjacencies.Where(x => x.AdjacentPoints.All(a => heights[x.Point.Row][x.Point.Col] < heights[a.Row][a.Col]))
            .Select(x => x.Point);

        var riskLevels = lowPoints.Select(p => heights[p.Row][p.Col] + 1);

        Console.WriteLine($"Sum of Risk Levels: {riskLevels.Sum()}");
    }

    private static IEnumerable<Point> AdjacentPoints(ImmutableArray<ImmutableArray<int>> heightMap, int row, int col) {
        var numRows = heightMap.Length;
        var numCols = heightMap[0].Length;

        if (row > 0) {
            yield return new Point(row - 1, col);
        }
        if (row < numRows - 1) {
            yield return new Point(row + 1, col);
        }
        if (col > 0) {
            yield return new Point(row, col - 1);
        }
        if (col < numCols - 1) {
            yield return new Point(row, col + 1);
        }
    }

    private static IEnumerable<int> AdjacentHeights(ImmutableArray<ImmutableArray<int>> heightMap, int row, int col) {
        var numRows = heightMap.Length;
        var numCols = heightMap[0].Length;

        var builder = ImmutableArray.CreateBuilder<int>();

        if (row > 0) {
            builder.Add(heightMap[row - 1][col]);
        }
        if (row < numRows - 1) {
            builder.Add(heightMap[row + 1][col]);
        }
        if (col > 0) {
            builder.Add(heightMap[row][col - 1]);
        }
        if (col < numCols - 1) {
            builder.Add(heightMap[row][col + 1]);
        }
        return builder.ToImmutableArray();
    }

    private static ImmutableArray<ImmutableArray<int>> ReadData() {
        var builder = ImmutableArray.CreateBuilder<ImmutableArray<int>>();

        foreach (var line in File.ReadLines("input.txt")) {
            builder.Add(line.Select(c => Convert.ToInt32(c.ToString())).ToImmutableArray());
        }
        return builder.ToImmutableArray();
    }
}

