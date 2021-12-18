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

        const int numExpansions = 10;

        var expansion = template;
        for (int i = 1; i <= numExpansions; i++) {
            expansion = Expand(expansion, insertionRules);
        }

        var frequencies = expansion.GroupBy(x => x).Select(g => g.Count());
        var mostFrequent = frequencies.Max();
        var leastFrequent = frequencies.Min();
        Console.WriteLine($"Most Frequent: {mostFrequent}, Least Frequent: {leastFrequent}, Diff: {mostFrequent - leastFrequent}");
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
