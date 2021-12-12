using System;
using System.Reactive;
using System.Reactive.Linq;

namespace aoc2_2;
class Program {
    static void Main(string[] args) {
        var commands = Commands().ToObservable();
        var finalPosition = Observable.Aggregate(commands, new Position(), (p, c) => p.Apply(c)).FirstAsync().Wait();
        Console.WriteLine($"Final position: {finalPosition}");
        Console.WriteLine($"Horiz * Depth = {finalPosition.Horizontal * finalPosition.Depth}");

    }

    static IEnumerable<Command> Commands() {
        foreach (var line in File.ReadLines("input.txt")) {
            yield return Command.FromText(line);
        }
    }
}