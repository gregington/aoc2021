using System;
using System.Reactive;
using System.Reactive.Linq;

namespace aoc3;

class Program {
    static void Main(string[] args) {
        var (gamma, epsilon) = CalculateDiagnosticVariables(Diagnostics());
        Console.WriteLine($"Gamma = {gamma}, Epsilon = {epsilon}");
        Console.WriteLine($"Gamma * Epsilon = {gamma * epsilon}");

        var (generatorRating, scrubberRating) = CalculateLifeSupportRatings(Diagnostics());
        Console.WriteLine($"Oxygen Generator Rating = {generatorRating}, CO2 Scrubber Rating = {scrubberRating}");
        Console.WriteLine($"Life Support Rating = {generatorRating * scrubberRating}");
    }

    static IEnumerable<int[]> Diagnostics() {
        foreach (var line in File.ReadLines("input.txt")) {
            yield return line.Select(c => c - '0').ToArray();
        }
    }

    static (int Gamma, int Epsilon) CalculateDiagnosticVariables(IEnumerable<int[]> diagnostics) {
        var numDiagnostics = diagnostics.Count();
        var diagnosticChars = diagnostics.First().Length;

        var oneCounts = Observable.Aggregate(diagnostics.ToObservable(), new int[diagnosticChars], (c, x) => 
            c.Zip(x).Select(x => x.First + x.Second).ToArray())
            .FirstAsync().Wait();

        var gamma = oneCounts.Select(x => x > (numDiagnostics / 2) ? 1 : 0).ToArray();
        var epsilon = Invert(gamma);

        return (ToDecimal(gamma), ToDecimal(epsilon));
    }

    static (int GeneratorRating, int ScrubberRating) CalculateLifeSupportRatings(IEnumerable<int[]> diagnostics) =>
        (ToDecimal(LifeSupportRating(diagnostics, FilterGeneratorValues)), ToDecimal(LifeSupportRating(diagnostics, FilterScrubberValues)));

    static int[] LifeSupportRating(IEnumerable<int[]> diagnostics, Func<IEnumerable<int[]>, int, IEnumerable<int[]>> filter) {
        var numBits = diagnostics.First().Length;

        return Observable.Range(0, numBits)
            .Aggregate(diagnostics, (d, i) => {
                if (d.Count() == 1) {
                    return d;
                }
                return filter(d, i);
            })
            .FirstAsync().Wait().First();
    }

    static IEnumerable<int[]> FilterGeneratorValues(IEnumerable<int[]> values, int position) {
        var (mostCommonValue, mostCommonCount) = MostCommonValue(values, position);
        var (leastCommonValue, leastCommonCount) = LeastCommonValue(values, position);

        var countsEqual = mostCommonCount == leastCommonCount;
        var keepBit = countsEqual ? 1 : mostCommonValue;

        return values.Where(x => x[position] == keepBit);
    }

    static IEnumerable<int[]> FilterScrubberValues(IEnumerable<int[]> values, int position) {
        var (_, mostCommonCount) = MostCommonValue(values, position);
        var (leastCommonValue, leastCommonCount) = LeastCommonValue(values, position);

        var countsEqual = mostCommonCount == leastCommonCount;
        var keepBit = countsEqual ? 0 : leastCommonValue;

        return values.Where(x => x[position] == keepBit);
    }

    static (int Value, int Count) MostCommonValue(IEnumerable<int[]> values, int position) {
        var count = values.Count();
        var onesCount = values.Select(x => x[position])
            .Where(x => x == 1)
            .Count();

        var oneMoreCommon = onesCount > (count / 2);

        return oneMoreCommon ? (1, onesCount) : (0, count - onesCount);
    }

    static (int Value, int Count) LeastCommonValue(IEnumerable<int[]> values, int position) {
        var count = values.Count();
        var onesCount = values.Select(x => x[position])
            .Where(x => x == 1)
            .Count();

        var oneLessCommon = onesCount <= (count / 2);
        return oneLessCommon ? (1, onesCount) : (0, count - onesCount);
    }

    static int[] Invert(int[] num) => num.Select(x => x == 0 ? 1 : 0).ToArray();

    static int ToDecimal(IEnumerable<int> bits) =>
        bits.Reverse().ToObservable()
            .Aggregate((Radix: 0, Result: 0), (r, x) => (r.Radix + 1, r.Result + (x << r.Radix))).FirstAsync().Wait().Result;
    
}