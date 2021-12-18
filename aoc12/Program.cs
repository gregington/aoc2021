using System;
using System.Collections.Immutable;

namespace aoc12;
class Program {
    static void Main(string[] args) {
        var caves = ReadData();
        var smallCavesOnce = FindPathsSmallCavesOnce(caves);
        Console.WriteLine($"Numer of paths, small caves once: {smallCavesOnce.Count()} paths");

        var singleSmallCaveTwice = FindPathsSingleSmallCaveTwice(caves);
        Console.WriteLine($"Numer of paths, single small cave twice: {singleSmallCaveTwice.Count()} paths");
    }

    static ImmutableArray<ImmutableArray<string>> FindPathsSingleSmallCaveTwice(ImmutableDictionary<string, ImmutableHashSet<string>> caves) {
        return FindPathsSingleSmallCaveTwice(caves, "start", ImmutableArray<string>.Empty, ImmutableArray<ImmutableArray<string>>.Empty);
    }

    static ImmutableArray<ImmutableArray<string>> FindPathsSingleSmallCaveTwice(ImmutableDictionary<string, ImmutableHashSet<string>> caves, string currentCave,
        ImmutableArray<string> path, ImmutableArray<ImmutableArray<string>> pathsToEnd) {
            var newPath = path.Add(currentCave);
            if (currentCave == "end") {
                return pathsToEnd.Add(newPath);
            }
            if (currentCave == "start" && path.Contains("start")) {
                return pathsToEnd;
            }
            var numSmallCaveVisits = newPath.Where(IsSmall)
                .GroupBy(x => x)
                .ToImmutableDictionary(g => g.Key, g => g.Count());
            if (numSmallCaveVisits.Any(x => x.Value > 2)
                || numSmallCaveVisits.Where(kvp => kvp.Value > 1).Count() > 1) {
                return pathsToEnd;
            }
            foreach(var nextCave in caves[currentCave]) {
                pathsToEnd = FindPathsSingleSmallCaveTwice(caves, nextCave, newPath, pathsToEnd);
            }
            return pathsToEnd;
    }

    static ImmutableArray<ImmutableArray<string>> FindPathsSmallCavesOnce(ImmutableDictionary<string, ImmutableHashSet<string>> caves) {
        return FindPathsSmallCavesOnce(caves, "start", ImmutableArray<string>.Empty, ImmutableArray<ImmutableArray<string>>.Empty);
    }

    static ImmutableArray<ImmutableArray<string>> FindPathsSmallCavesOnce(ImmutableDictionary<string, ImmutableHashSet<string>> caves, string currentCave,
        ImmutableArray<string> path, ImmutableArray<ImmutableArray<string>> pathsToEnd) {
            var newPath = path.Add(currentCave);
            if (currentCave == "end") {
                return pathsToEnd.Add(newPath);
            }
            if (IsSmall(currentCave) && path.Contains(currentCave)) {
                return pathsToEnd;
            }
            foreach(var nextCave in caves[currentCave]) {
                pathsToEnd = FindPathsSmallCavesOnce(caves, nextCave, newPath, pathsToEnd);
            }
            return pathsToEnd;
    }

    static bool IsSmall(string cave) => Char.IsLower(cave[0]);

    static ImmutableDictionary<string, ImmutableHashSet<string>> ReadData() {
        return File.ReadLines("input.txt").Aggregate(ImmutableDictionary<string, ImmutableHashSet<string>>.Empty, (d, l) => {
            var route = l.Split('-');
            foreach(var cave in route) {
                if (!d.ContainsKey(cave)) {
                    d = d.Add(cave, ImmutableHashSet<string>.Empty);
                }
            }
            d = d.SetItem(route[0], d[route[0]].Add(route[1]));
            d = d.SetItem(route[1], d[route[1]].Add(route[0]));
            return d;
        });
    }
}
