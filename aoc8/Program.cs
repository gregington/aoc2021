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

        Console.WriteLine($"Num times unique segment counts appear: {outputsWithUniqueSegmentCounts}");

        var outputSum = data.Select(Solve).Sum();
        Console.WriteLine($"Sum of solutions: {outputSum}");
    }

    private static int Solve(Data data) {
        var segmentMappings = MapSegments(data.Input);

        return GetNumber(segmentMappings, data.Output);
    }

    private static int GetNumber(ImmutableArray<string> segmentMap, IEnumerable<string> output) {
        var mapBySegment = segmentMap.Zip(Enumerable.Range(0, segmentMap.Length))
            .ToImmutableDictionary(x => x.First, x => x.Second.ToString());

        var orderedOutput = output.Select(x => string.Join("", x.Select(c => c).OrderBy(c => c)));

        return Convert.ToInt32(string.Join("", orderedOutput.Select(x => mapBySegment[x])));
    }

    private static ImmutableArray<string> MapSegments(IEnumerable<string> input) {
        var segmentPossibilities = CreateSegmentPossibilities(input);

        // 1, 4, 7, 8 solved
        // Solve 9. Is the only one out of the 0, 6, 9 that shares segments with 4.

        segmentPossibilities = Solve9(segmentPossibilities);

        // 1, 4, 7, 8, 9 solved
        // Solve 0 and 6. 0 is the only possibility that has all segments in common with 1.

        segmentPossibilities = Solve0And6(segmentPossibilities);

        // 0, 1, 4, 6, 7, 8, 9 solved
        // Solve 3. 3 is the only possibility that has all segments in common with 7.

        segmentPossibilities = Solve3(segmentPossibilities);

        // 0, 1, 3, 4, 6, 7, 8, 9 solved.
        // Solve 2 and 5. All segments in 5 are contained in 6.

        segmentPossibilities = Solve2And5(segmentPossibilities);

        return segmentPossibilities.Select(x => x.First())
            .Select(x => string.Join("", x.Select(c => c).OrderBy(c => c)))
            .ToImmutableArray();
    }

    private static ImmutableArray<ImmutableHashSet<string>> Solve9(ImmutableArray<ImmutableHashSet<string>> segmentPossibilities) {
        var segmentsFor9 = ContainsAllSegments(segmentPossibilities[9], segmentPossibilities[4].First());

        return segmentPossibilities.SetItem(9, ImmutableHashSet.Create(segmentsFor9))
            .SetItem(0, segmentPossibilities[0].Remove(segmentsFor9))
            .SetItem(6, segmentPossibilities[6].Remove(segmentsFor9));
    }

    private static ImmutableArray<ImmutableHashSet<string>> Solve0And6(ImmutableArray<ImmutableHashSet<string>> segmentPossibilities) {
        var segmentsFor0 = ContainsAllSegments(segmentPossibilities[0], segmentPossibilities[1].First());

        return segmentPossibilities.SetItem(0, ImmutableHashSet.Create(segmentsFor0))
            .SetItem(6, segmentPossibilities[6].Remove(segmentsFor0));
    }

    private static ImmutableArray<ImmutableHashSet<string>> Solve3(ImmutableArray<ImmutableHashSet<string>> segmentPossibilities) {
        var segmentsFor3 = ContainsAllSegments(segmentPossibilities[3], segmentPossibilities[7].First());

        return segmentPossibilities.SetItem(3, ImmutableHashSet.Create(segmentsFor3))
            .SetItem(2, segmentPossibilities[2].Remove(segmentsFor3))
            .SetItem(5, segmentPossibilities[5].Remove(segmentsFor3));
    }

    private static ImmutableArray<ImmutableHashSet<string>> Solve2And5(ImmutableArray<ImmutableHashSet<string>> segmentPossibilities) {
        var charsFor6 = segmentPossibilities[6].First().Select(c => c).ToImmutableHashSet();
        
        var segmentsFor5 = segmentPossibilities[5].Select(s => (Segments: s, Chars: s.Select(c => c).ToImmutableHashSet()))
            .Where(x => x.Chars.Intersect(charsFor6).Count() == x.Chars.Count())
            .Select(x => x.Segments)
            .First();
        
        return segmentPossibilities.SetItem(5, ImmutableHashSet.Create(segmentsFor5))
            .SetItem(2, segmentPossibilities[2].Remove(segmentsFor5));
    }

    private static string ContainsAllSegments(IEnumerable<string> segmentsToCheck, string mustContain) {
        IEnumerable<char> mustContainChar = mustContain.Select(x => x);

        var segs = segmentsToCheck.Where(x => mustContainChar.All(c => x.Contains(c)));
        return segs.First();
    }

    private static ImmutableArray<ImmutableHashSet<string>> CreateSegmentPossibilities(IEnumerable<string> scrambledSegs) {
        return new [] {
            FindStringsOfLength(6, scrambledSegs), // 0
            FindStringsOfLength(2, scrambledSegs), // 1
            FindStringsOfLength(5, scrambledSegs), // 2
            FindStringsOfLength(5, scrambledSegs), // 3
            FindStringsOfLength(4, scrambledSegs), // 4
            FindStringsOfLength(5, scrambledSegs), // 5
            FindStringsOfLength(6, scrambledSegs), // 6
            FindStringsOfLength(3, scrambledSegs), // 7
            FindStringsOfLength(7, scrambledSegs), // 8
            FindStringsOfLength(6, scrambledSegs), // 9
        }.ToImmutableArray();
    }

    private static ImmutableHashSet<string> FindStringsOfLength(int length, IEnumerable<string> segments) =>
        segments.Where(x => x.Length == length).ToImmutableHashSet();

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
