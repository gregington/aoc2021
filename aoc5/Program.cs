using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Immutable;

namespace aoc5;

class Program {
    static void Main(string[] args) {
        var lineSegments = ReadLineSegments();

        var horizontalAndVerticalLineSegments = lineSegments
            .Where(x => x.IsHorizontal || x.IsVertical);

        var pointFrequency = horizontalAndVerticalLineSegments
            .SelectMany(x => x.Points())
            .Aggregate(ImmutableDictionary<Point, int>.Empty, (d, p) => {
                if (d.ContainsKey(p)) {
                    var count = d[p] + 1;
                    return d.SetItem(p, count + 1);
                }
                return d.Add(p, 1);
            });            

        var dangerousPoints = pointFrequency
            .Where(kvp => kvp.Value > 1)
            .Select(kvp => kvp.Key);

        Console.WriteLine($"Num points: {pointFrequency.Count()}");
        Console.WriteLine($"Num dangerous points: {dangerousPoints.Count()}");
    }

    static IEnumerable<LineSegment> ReadLineSegments() {
        var regex = new Regex("^(?<x1>\\d+),(?<y1>\\d+) -> (?<x2>\\d+),(?<y2>\\d+)$");
        foreach(var line in File.ReadLines("input.txt")) {
            var match = regex.Match(line);
            if (!match.Success) {
                continue;
            }
            var groups = match.Groups;
            var p1 = new Point(Convert.ToInt32(groups["x1"].Value), Convert.ToInt32(groups["y1"].Value));
            var p2 = new Point(Convert.ToInt32(groups["x2"].Value), Convert.ToInt32(groups["y2"].Value));
            yield return new LineSegment(p1, p2);
        }
    }
}