using System;
using System.Collections.Immutable;
using System.IO;

namespace aoc7;

class Program {
    static void Main(string[] args) {
        var positions = ReadPositions();

        var fuelCosts = Enumerable.Range(0, positions.Length)
            .Select(p => (Position: p, Cost: CalculateCost(p, positions)));

        var (minPos, minCost) = fuelCosts.MinBy(x => x.Cost);

        Console.WriteLine($"Min Pos: {minPos}, Min Cost: {minCost}");
    }

    private static int CalculateCost(int idx, ImmutableArray<int> initialPositions) =>
        Enumerable.Range(0, initialPositions.Length)
            .Zip(initialPositions, (a, b) => (Index: a, NumCrabs: b))
            .Select(x => x.NumCrabs * (Math.Abs(idx - x.Index)))
            .Sum();

    private static ImmutableArray<int> ReadPositions() {
        var lines = File.ReadAllLines("input.txt");

        var crabPositions = lines[0].Split(',').Select(x => Convert.ToInt32(x));
        var maxPosition = crabPositions.Max();

        return crabPositions.Aggregate(Enumerable.Repeat(0, maxPosition + 1).ToImmutableArray(),
            (p, i) => p.SetItem(i, p[i] + 1));

    }
}