using System;
using System.Collections.Immutable;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;

namespace aoc14;

class Program {
    static void Main(string [] args) {
        var (template, insertionRules) = ReadData();
        Console.WriteLine(template);

        var expansion = template;
        for (int i = 0; i < 10; i++) {
            expansion = Expand(expansion, insertionRules);
        }

        var frequencies10 = expansion.GroupBy(x => x).Select(g => g.Count());
        var mostFrequent10 = frequencies10.Max();
        var leastFrequent10 = frequencies10.Min();
        Console.WriteLine($"10 Steps - Most Frequent: {mostFrequent10}, Least Frequent: {leastFrequent10}, Diff: {mostFrequent10 - leastFrequent10}");

        var (pairCounts, letterCounts) = CreateCounts(template, insertionRules, 40);
        var mostFrequent40 = letterCounts.Values.Max();
        var leastFrequent40 = letterCounts.Values.Min();
        Console.WriteLine($"40 Steps - Most Frequent: {mostFrequent40}, Least Frequent: {leastFrequent40}, Diff: {mostFrequent40 - leastFrequent40}");
    }

    private static (ImmutableDictionary<string, long>, ImmutableDictionary<char, long>) CreateCounts(string template, ImmutableDictionary<string, string> insertionRules, int numExpansions) {
        var pairs = template.ToObservable()
            .Buffer(2, 1)
            .Select(x => string.Join("", x.ToArray()))
            .Where(x => x.Length == 2)
            .ToEnumerable().ToImmutableArray();

        var pairCounts = pairs.GroupBy(x => x).ToImmutableDictionary(g => g.Key, g => (long) g.Count());
        var charCounts = template.GroupBy(c => c).ToImmutableDictionary(g => g.Key, g => (long) g.Count());

        for(var i = 0; i < numExpansions; i++) {
            (pairCounts, charCounts) = ExpandCounts(pairCounts, charCounts, insertionRules);
        }

        return (pairCounts, charCounts);
    }

    private static (ImmutableDictionary<string, long>, ImmutableDictionary<char, long>) ExpandCounts(ImmutableDictionary<string, long> initialCounts, ImmutableDictionary<char, long> initialCharCounts, ImmutableDictionary<string, string> insertionRules) {
        var newCounts = initialCounts.Aggregate((PairCounts: ImmutableDictionary.Create<string, long>(), CharCounts: initialCharCounts), (d, kvp) => {
            var c = insertionRules[kvp.Key][0];
            var newPairs = new [] { $"{kvp.Key[0]}{c}", $"{c}{kvp.Key[1]}"};
            var (pairCounts, charCounts) = d;
            foreach(var newPair in newPairs) {
                if (pairCounts.ContainsKey(newPair)) {
                    var newValue = pairCounts[newPair] + kvp.Value;
                    pairCounts = pairCounts.SetItem(newPair, newValue);
                } else {
                    pairCounts = pairCounts.SetItem(newPair, kvp.Value);
                }
            }
            var newCharCount = charCounts.GetValueOrDefault(c, 0) + kvp.Value;
            charCounts = charCounts.SetItem(c, newCharCount);
            return (pairCounts, charCounts);
        });
        return newCounts;
    }

    private static string Expand(string s, ImmutableDictionary<string, string> insertionRules) {
        var pairs = s.ToObservable().Buffer(2, 1)
            .Select(x => string.Join("", x.ToArray()))
            .Where(x => x.Length == 2)
            .ToEnumerable().ToImmutableArray();;

        var sb = new StringBuilder();

        for (var j = 0; j < pairs.Count(); j++) {
            var pair = pairs[j];
            if (j == 0) {
                sb.Append(pair[0]);
            }
            sb.Append(insertionRules[pair]);
            sb.Append(pair[1]);
        }
        return sb.ToString();
    }

    private static (string Template, ImmutableDictionary<string, string> InsertionRules) ReadData() {
        var lines = File.ReadAllLines("input.txt");

        var template = lines.Take(1).First();

        var insertionRules = lines.Skip(2)
            .Select(l => l.Split("->", StringSplitOptions.TrimEntries))
            .ToImmutableDictionary(x => x[0], x => x[1]);

        return (template, insertionRules);
    }
}
