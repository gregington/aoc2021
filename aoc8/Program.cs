using System;
using System.Collections.Immutable;
using System.IO;

namespace aoc8;

class Program {
    static void Main(string[] args) {
        var data = ReadData();

        var numSegmentsPerDigit = CreateNumSegmentsPerDigit();
        var uniqueSegmentCount = numSegmentsPerDigit
            .Aggregate(ImmutableDictionary<int, int>.Empty, (d, x) => {
                if (d.ContainsKey(x)) {
                    var count = d[x];
                    return d.SetItem(x, count + 1);
                }
                return d.SetItem(x, 1);
            })
            .Where(kvp => kvp.Value == 1)
            .Select(kvp => kvp.Key)
            .ToImmutableHashSet();

        var outputsWithUniqueSegmentCounts = data.SelectMany(x => x.Output)
            .Select(x => x.Length)
            .Where(x => uniqueSegmentCount.Contains(x))
            .Count();

        Console.WriteLine($"Num timed unique segment counts appear: {outputsWithUniqueSegmentCounts}")        ;    
    }

    private static IEnumerable<Data> ReadData() {
        foreach (var line in File.ReadLines("input.txt")) {
            var components = line.Split('|');
            var inputs = components[0].Trim().Split(' ').ToImmutableArray();
            var outputs = components[1].Trim().Split(' ').ToImmutableArray();
            yield return new Data(inputs, outputs);
        }
    }

    private static ImmutableArray<int> CreateNumSegmentsPerDigit() {
        return new int[] { 6, 2, 5, 5, 4, 5, 6, 3, 7, 6 }.ToImmutableArray();
    }
}
