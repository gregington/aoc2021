using System;
using System.Collections.Immutable;
using System.IO;

namespace aoc9;

class Program {
    static void Main(string[] args) {
        var heights = ReadData();

        var adjacencies = Enumerable.Range(0, heights.Length)
            .SelectMany(r => Enumerable.Range(0, heights[0].Length).Select(c => new Point(r, c)))
            .Select(x => (Point: x, Height: heights[x.Row][x.Col], AdjacentPoints: AdjacentPoints(x, heights)));

        var lowPoints = adjacencies.Where(x => x.AdjacentPoints.All(a => heights[x.Point.Row][x.Point.Col] < heights[a.Row][a.Col]))
            .Select(x => x.Point);

        var riskLevels = lowPoints.Select(p => heights[p.Row][p.Col] + 1);

        Console.WriteLine($"Sum of Risk Levels: {riskLevels.Sum()}");

        var basins = lowPoints.Select(p => FindBasin(p, ImmutableHashSet<Point>.Empty, heights));

        var top3Basins = basins.Select(b => b.Count()).OrderByDescending(c => c).Take(3);

        Console.WriteLine($"Top 3 basins multiplied: {top3Basins.Aggregate(1, (a, b) => a * b)}");
    }

    static IEnumerable<Point> FindBasin(Point point, ImmutableHashSet<Point> seenPoints, ImmutableArray<ImmutableArray<int>> heights) {
        seenPoints = seenPoints.Add(point);

        foreach (var adj in AdjacentPoints(point, heights)) {
            if (seenPoints.Contains(adj)) {
                continue;
            }
            var height = heights[adj.Row][adj.Col];
            if (height < 9) {
                seenPoints = seenPoints.Union(FindBasin(adj, seenPoints, heights));
            }
        }

        return seenPoints;
    }

    private static IEnumerable<Point> AdjacentPoints(Point p, ImmutableArray<ImmutableArray<int>> heightMap) {
        var numRows = heightMap.Length;
        var numCols = heightMap[0].Length;

        if (p.Row > 0) {
            yield return new Point(p.Row - 1, p.Col);
        }
        if (p.Row < numRows - 1) {
            yield return new Point(p.Row + 1, p.Col);
        }
        if (p.Col > 0) {
            yield return new Point(p.Row, p.Col - 1);
        }
        if (p.Col < numCols - 1) {
            yield return new Point(p.Row, p.Col + 1);
        }
    }

    private static ImmutableArray<ImmutableArray<int>> ReadData() {
        var builder = ImmutableArray.CreateBuilder<ImmutableArray<int>>();

        foreach (var line in File.ReadLines("input.txt")) {
            builder.Add(line.Select(c => Convert.ToInt32(c.ToString())).ToImmutableArray());
        }
        return builder.ToImmutableArray();
    }
}

