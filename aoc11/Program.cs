using System;
using System.Collections.Immutable;
using System.IO;
using aco11;

namespace aoc11;

class Program {
    static void Main(string[] args) {
        var adj = Adjacent(new Point(1, 1), 3, 3);
        var energyLevels = ReadInput();
        var flashCount = 0;
        int steps = 100;
        for (var i = 0 ; i < steps; i++) {
            (energyLevels, var flashes) = PerformStep(energyLevels);
            flashCount += flashes;
        }
        PrintEnergyLevels(energyLevels);
        Console.WriteLine($"Steps: {steps}, Flashes: {flashCount}");
    }

    static (ImmutableArray<ImmutableArray<int>> EnergyLevels, int NumFlashed) PerformStep(ImmutableArray<ImmutableArray<int>> energyLevels) {
        energyLevels = IncrementEnergyLevels(energyLevels);
        return Flash(energyLevels);
    }

    static ImmutableArray<ImmutableArray<int>> IncrementEnergyLevels(ImmutableArray<ImmutableArray<int>> energyLevels) {
        return energyLevels.Select(r => r.Select(c => c + 1).ToImmutableArray()).ToImmutableArray();
    }

    static (ImmutableArray<ImmutableArray<int>> EnergyLevels, int NumFlashed) Flash(ImmutableArray<ImmutableArray<int>> energyLevels) {
        ImmutableHashSet<Point> flashed = ImmutableHashSet<Point>.Empty;
        var numRows = energyLevels.Length;
        var numCols = energyLevels[0].Length;
        while (true) {
            var highEnergyLevels = FindHighEnergyLevels(energyLevels);
            var pointsToFlash = highEnergyLevels.Except(flashed);
            if (pointsToFlash.IsEmpty) {
                break;
            }

            foreach(var p in pointsToFlash) {
                var adjacent = Adjacent(p, numRows, numCols);
                foreach(var a in adjacent) {
                    var energyLevel = energyLevels[a.Row][a.Col];
                    var newRow = energyLevels[a.Row].SetItem(a.Col, energyLevel + 1);
                    energyLevels = energyLevels.SetItem(a.Row, newRow);
                }
            }

            flashed = flashed.Union(pointsToFlash);
        }

        // Reset flashed to zero
        foreach(var f in flashed) {
            var newRow = energyLevels[f.Row].SetItem(f.Col, 0);
            energyLevels = energyLevels.SetItem(f.Row, newRow);
        }

        return (energyLevels, flashed.Count);
    }

    static IEnumerable<Point> Adjacent(Point point, int numRows, int numCols) {
        return Enumerable.Range(-1, 3)
            .SelectMany(r => Enumerable.Range(-1, 3).Select(c => new Point(r, c)))
            .Select(o => new Point(point.Row + o.Row, point.Col + o.Col))
            .Where(p => p != point)
            .Where(p => p.Row >= 0 && p.Row < numRows)
            .Where(p => p.Col >= 0 && p.Col < numCols);
    }

    static ImmutableHashSet<Point> FindHighEnergyLevels(ImmutableArray<ImmutableArray<int>> energyLevels) {
        var points = energyLevels.Zip(Enumerable.Range(0, energyLevels.Length),  (a, b) => (Row: b, Vals: a))
            .SelectMany(x => x.Vals.Zip(Enumerable.Range(0, x.Vals.Length), (a, b) => (Point: new Point(x.Row, b), Energy: a)));
        return points.Where(p => p.Energy > 9)
            .Select(p => p.Point)
            .ToImmutableHashSet();
    }

    static ImmutableArray<ImmutableArray<int>> ReadInput() {
        return File.ReadLines("input.txt")
            .Select(l => l.Select(c => Convert.ToInt32(c.ToString())).ToImmutableArray())
            .ToImmutableArray();
    }

    static void PrintEnergyLevels(ImmutableArray<ImmutableArray<int>> energyLevels) {
        foreach (var row in energyLevels) {
            Console.WriteLine(string.Join("", row.Select(x => x.ToString())));
        }
    }

}
