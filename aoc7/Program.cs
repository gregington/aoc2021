using System;
using System.Collections.Immutable;
using System.IO;

namespace aoc7;

class Program {
    static void Main(string[] args) {
        var positions = ReadPositions();

        var fuelCostsConstant = Enumerable.Range(0, positions.Length)
            .Select(p => (Position: p, Cost: CalculateConstantCost(p, positions)));

        var (minConstantCostPos, minConstantCost) = fuelCostsConstant.MinBy(x => x.Cost);

        Console.WriteLine($"Min Constant Cost Pos: {minConstantCostPos}, Min Constant Cost: {minConstantCost}");

        var fuelCostsVariable = Enumerable.Range(0, positions.Length)
            .Select(p => (Position: p, Cost: CalculateVariableCost(p, positions)));

        var (minVariableCostPos, minVariableCost) = fuelCostsVariable.MinBy(x => x.Cost);

        Console.WriteLine($"Min Variable Cost Pos: {minVariableCostPos}, Min Variable Cost: {minVariableCost}");
    }

    private static int CalculateConstantCost(int idx, ImmutableArray<int> initialPositions) =>
        Enumerable.Range(0, initialPositions.Length)
            .Zip(initialPositions, (a, b) => (Index: a, NumCrabs: b))
            .Select(x => x.NumCrabs * (Math.Abs(idx - x.Index)))
            .Sum();

    private static int CalculateVariableCost(int idx, ImmutableArray<int> initialPositions) =>
        Enumerable.Range(0, initialPositions.Length)
            .Zip(initialPositions, (a, b) => (Index: a, NumCrabs: b))
            .Select(x => {
                var dist = Math.Abs(idx - x.Index);
                return x.NumCrabs * dist * (dist + 1) / 2;
            })
            .Sum();

    private static ImmutableArray<int> ReadPositions() {
        var lines = File.ReadAllLines("input.txt");

        var crabPositions = lines[0].Split(',').Select(x => Convert.ToInt32(x));
        var maxPosition = crabPositions.Max();

        return crabPositions.Aggregate(Enumerable.Repeat(0, maxPosition + 1).ToImmutableArray(),
            (p, i) => p.SetItem(i, p[i] + 1));
    }
}