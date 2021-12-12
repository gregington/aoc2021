using System;
using System.Reactive;
using System.Reactive.Linq;

namespace aoc3;

class Program {
    static void Main(string[] args) {
        var (gamma, epsilon) = CalculateDiagnosticVariables(Diagnostics());
        Console.WriteLine($"Gamma = {gamma}, Epsilon = {epsilon}");
        Console.WriteLine($"Gamma * Epsilon = {gamma * epsilon}");
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

    static int[] Invert(int[] num) => num.Select(x => x == 0 ? 1 : 0).ToArray();

    static int ToDecimal(IEnumerable<int> bits) =>
        bits.Reverse().ToObservable()
            .Aggregate((Radix: 0, Result: 0), (r, x) => (r.Radix + 1, r.Result + (x << r.Radix))).FirstAsync().Wait().Result;
    
}