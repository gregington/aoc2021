using System;
using System.IO;
using System.Reactive;
using System.Reactive.Linq;

namespace aoc1;
class Program {
    static void Main(string[] args) {
        var depths = Depths().ToObservable();
        var numIncreases = depths
            .Buffer(2, 1)
            .Select(x => x.Last() > x.First())
            .Where(x => x)
            .Count()
            .FirstAsync().Wait();

        Console.WriteLine($"Number of increases = {numIncreases}");

        var numSlidingWindowIncreases = depths
            .Buffer(3, 1)
            .Where(x => x.Count() == 3)
            .Select(x => x.Sum())
            .Buffer(2, 1)
            .Select(x => x.Last() > x.First())
            .Where(x => x)
            .Count()
            .FirstAsync().Wait();

        Console.WriteLine($"Number of sliding window increases = {numSlidingWindowIncreases}");

    }

    static IEnumerable<int> Depths() {
        foreach (var line in File.ReadLines("input")) {
            yield return Convert.ToInt32(line);
        }
    }
}

