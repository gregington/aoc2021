using System;
using System.Collections.Immutable;
using System.IO;

namespace aoc6;

class Program {
    static void Main(string[] args) {
        var initialState = ReadState();

        Console.WriteLine($"18 Days: {Simulate(initialState, 18).Sum()}");
        Console.WriteLine($"80 Days: {Simulate(initialState, 80).Sum()}");
        Console.WriteLine($"256 Days: {Simulate(initialState, 256).Sum()}");
    }

    private static ImmutableArray<long> Simulate(ImmutableArray<long> initialState, int days) {
        return Enumerable.Range(0, days).Aggregate(initialState, (s, _) => {
            var expiredTimers = s.First();
            var newState = s.Skip(1).ToImmutableArray().Add(expiredTimers);
            return newState.SetItem(6, newState[6] + expiredTimers);
        });
    }

    private static ImmutableArray<long> ReadState() {
        var lines = File.ReadAllLines("input.txt");

        var timers = lines[0].Split(',')
            .Select(x => Convert.ToInt32(x));

        return timers.Aggregate(ImmutableArray.CreateRange<long>(Enumerable.Repeat<long>(0, 9)), 
            (s, t) => s.SetItem(t, s[t] + 1));
    }
}