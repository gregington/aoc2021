using System;
using System.Collections.Immutable;
using System.Reactive.Linq;

namespace aoc4;

static class Program {
    static void Main(string[] args) {
        var (drawnNumbers, boards) = ReadData();

        var drawnNums = drawnNumbers.ToObservable()
            .Scan(ImmutableArray<int>.Empty, (s, n) => s.Add(n));

        var (numsSelected, winningBoard) = drawnNums
            .Select(s => (DrawnNums: s, WinningBoard: boards.FirstOrDefault(b => b.Win(s.ToImmutableHashSet()))))
            .SkipWhile(x => x.WinningBoard == null)
            .Take(1)
            .FirstAsync().Wait();

        var lastNumDrawn = numsSelected.Last();
        var unmarkedNums = winningBoard.Unmarked(numsSelected.ToImmutableHashSet());
        var unmarkedNumSum = unmarkedNums.Sum();

        Console.WriteLine($"Last num drawn: {lastNumDrawn}");
        Console.WriteLine($"Winning Board:\n{winningBoard}");
        Console.WriteLine($"Unmarked nums: {string.Join(",", unmarkedNums.Select(n => n.ToString()))}");
        Console.WriteLine($"Unmarked sum: {unmarkedNumSum}");
        Console.WriteLine($"Score: {unmarkedNumSum * lastNumDrawn}");

        var (numsSelected2, winningBoards2) = drawnNums
            .Scan((DrawnNums: ImmutableArray<int>.Empty, Boards: boards), (x, n) => {
                if (x.Boards.Count() == 1) {
                    return (n, x.Boards);
                }
                return (n, x.Boards.Where(b => !b.Win(n.ToImmutableHashSet())));
            })
            .SkipWhile(x => x.Boards.Count() > 1)
            .SkipWhile(x => !x.Boards.First().Win(x.DrawnNums.ToImmutableHashSet()))
            .Take(1)
            .FirstAsync().Wait();

        var winningBoard2 = winningBoards2.First();
        var lastNumDrawn2 = numsSelected2.Last();
        var unmarkedNums2 = winningBoard2.Unmarked(numsSelected2.ToImmutableHashSet());
        var unmarkedNumSum2 = unmarkedNums2.Sum();

        Console.WriteLine();
        Console.WriteLine($"Last num drawn for last winning board: {lastNumDrawn2}");
        Console.WriteLine($"Last Winning Board:\n{winningBoard2}");
        Console.WriteLine($"Last Winning Board Unmarked nums: {string.Join(",", unmarkedNums2.Select(n => n.ToString()))}");
        Console.WriteLine($"Last Winning Board Unmarked sum: {unmarkedNumSum2}");
        Console.WriteLine($"Last Winning Board Score: {unmarkedNumSum2 * lastNumDrawn2}");
    }

    static (IEnumerable<int> DrawnNumbers, IEnumerable<Board> Boards) ReadData() {
        var lines = File.ReadAllLines("input.txt");

        var drawnNumbers = lines.First().Split(',').Select(n => Convert.ToInt32(n));

        var boards = ImmutableArray<Board>.Empty;
        var rows = ImmutableArray<ImmutableArray<int>>.Empty;
        foreach(var line in lines.Skip(1)) {
            var nums = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(n => Convert.ToInt32(n))
                .ToImmutableArray();
            if (nums.Length == 0 && rows.Length > 0) {
                boards = boards.Add(new Board(rows.ToImmutableArray()));
                rows = rows.Clear();
            } else if (nums.Length > 0) {
                rows = rows.Add(nums);
            }
        }

        return (drawnNumbers, boards);
    }
}

