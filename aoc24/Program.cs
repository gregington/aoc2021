using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace aoc24;

// Really had no idea with this one. Solution from lots of Reddit comments.
class Program {
    static void Main(string[] args) {
        var constants = ReadData();

        var highest = Solve(constants, Enumerable.Repeat(9, 14).ToArray());
        var lowest = Solve(constants, Enumerable.Repeat(1, 14).ToArray());

        var highestValidation = Run(constants, highest.ToString().Select(c => Convert.ToInt32(c)).ToArray());
        var lowestValidation = Run(constants, lowest.ToString().Select(c => Convert.ToInt32(c)).ToArray());

        Console.WriteLine($"Highest: {highest}. Validated: {highestValidation}");
        Console.WriteLine($"Lowest: {lowest}. Validated: {lowestValidation}");
    }

    private static bool Run(ImmutableArray<Constants> constantsList, int[] input) {
        var w = 0;
        var x = 0;
        var y = 0;

        var stack = new Stack<int>();
        stack.Push(0);

        for (int i = 0; i < input.Length; i++) {
            var constants = constantsList[i];
            w = input[i];
            x = stack.Peek();
            if (constants.A == 26) {
                stack.Pop();
            }

            x += constants.B;

            if (x != w) {
                y = w + constants.C;
                stack.Push(y);
            }            
        }

        return stack.Peek() == 0;
    }

    private static long Solve(ImmutableArray<Constants> constantsList, int[] input) {
        var stack = new Stack<(int, int)>();

        for (var i = 0; i < input.Length; i++) {
            var (div, chk, add) = constantsList[i];

            if (div == 1) {
                stack.Push((i, add));
            } else {
                var (j, ad) = stack.Pop();
                input[i] = input[j] + ad + chk;
                if (input[i] > 9) {
                    input[j] -= input[i] - 9;
                    input[i] = 9;
                }

                if (input[i] < 1) {
                    input[j] += 1 - input[i];
                    input[i] = 1;
                }
            }
        }
        return input.Aggregate(0L, (a, x) => (a * 10) + x);
    }

    private static ImmutableArray<Constants> ReadData() {
        var lines = File.ReadAllLines("input.txt");

        var divRegex = new Regex("div z (-?[0-9]+)");
        var divs = lines.Select(x => divRegex.Match(x))
            .Where(m => m.Success)
            .Select(m => Convert.ToInt32(m.Groups[1].Value));

        var addXRegex = new Regex("add x (-?[0-9]+)");
        var addXs = lines.Select(x => addXRegex.Match(x))
            .Where(m => m.Success)
            .Select(m => Convert.ToInt32(m.Groups[1].Value));

        var addYRegex = new Regex("add y (-?[0-9]+)");
        var addYs = Enumerable.Range(0, lines.Length)
            .Zip(lines)
            .Where(x => x.Second == "add y w")
            .Select(x => x.First + 1)
            .Select(x => lines[x])
            .Select(x => addYRegex.Match(x))
            .Where(m => m.Success)
            .Select(m => Convert.ToInt32(m.Groups[1].Value));


        return divs.Zip(addXs, (a, b) => (A: a, B: b))
            .Zip(addYs, (a, b) => (a.A, a.B, C: b))
            .Select(x => new Constants(x.A, x.B, x.C))
            .ToImmutableArray();
    }
}