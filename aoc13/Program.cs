using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace aoc13;

class Program {
    static void Main(string[] args) {
        var (points, folds) = ReadData();

        for (var i = 0; i < folds.Count(); i++) {
            var fold = folds[i];
            points = PerformFold(fold, points);
            Console.WriteLine($"Fold {i + 1}, Points: {points.Count()}");
        }
        PrintPoints(points);
    }

    private static IEnumerable<Point> PerformFold(Fold fold, IEnumerable<Point> points) {
        switch (fold.Direction) {
            case "x":
                return FoldLeft(fold.Position, points);
            case "y":
                return FoldUp(fold.Position, points);
            default:
                throw new ArgumentException($"Unknown fold direction {fold.Direction}");
        }
    }

    private static IEnumerable<Point> FoldLeft(int position, IEnumerable<Point> points) {
        return points.Select(p => {
                if (p.X < position) {
                    return p as Point?;
                }
                if (p.X > position) {
                    return new Point(position - (p.X - position), p.Y);
                }
                return null;
            })
            .Where(p => p.HasValue)
            .Select(p => p.Value)
            .Where(p => p.X >= 0)
            .ToImmutableHashSet();
    }

    private static IEnumerable<Point> FoldUp(int position, IEnumerable<Point> points) {
        return points.Select(p => {
                if (p.Y < position) {
                    return p as Point?;
                }
                if (p.Y > position) {
                    return new Point(p.X, position - (p.Y - position));
                }
                return null;
            })
            .Where(p => p.HasValue)
            .Select(p => p.Value)
            .Where(p => p.Y >= 0)
            .ToImmutableHashSet();
    }

    private static void PrintPoints(IEnumerable<Point> points) {
        var xLen = points.Select(p => p.X).Max() + 1;
        var yLen = points.Select(p => p.Y).Max() + 1;

        var grid = new bool[yLen][];
        for (var i = 0; i < yLen; i++) {
            grid[i] = new bool[xLen];
        }

        foreach(var point in points) {
            grid[point.Y][point.X] = true;
        }

        Console.WriteLine();
        var lines = grid.Select(row => string.Join("", row.Select(v => v ? "#" : " ")));
        foreach(var line in lines) {
            Console.WriteLine(line);
        }
    }

    private static (IEnumerable<Point> Points, ImmutableArray<Fold> folds) ReadData() {
        var points = ImmutableArray<Point>.Empty;
        var folds = ImmutableArray<Fold>.Empty;

        var pointRegex = new Regex("^(?<x>[0-9]+),(?<y>[0-9]+)$");
        var foldRegex = new Regex("^fold along (?<direction>x|y)=(?<distance>[0-9]+)$");

        foreach(var line in File.ReadLines("input.txt")) {
            var match = pointRegex.Match(line);
            if (match.Success) {
                points = points.Add(new Point(Convert.ToInt32(match.Groups["x"].Value), Convert.ToInt32(match.Groups["y"].Value)));
                continue;
            }

            match = foldRegex.Match(line);
            if (match.Success) {
                folds = folds.Add(new Fold(match.Groups["direction"].Value, Convert.ToInt32(match.Groups["distance"].Value)));
            }
        }

        return (points, folds);
    }
}