using System;
using System.Collections.Immutable;

namespace aoc20;

class Program {
    static void Main(string[] args) {
        var (algorithm, image) = ReadData();
        var numIters = 2;
        image = Expand(image, numIters * 2);

        PrintImage(image);
        for (var i = 0; i < numIters; i++) {
            image = Enhance(image, algorithm);
            PrintImage(image);
        }

        image = Reduce(image, numIters);
        PrintImage(image);

        var numLitPixels = image.SelectMany(x => x).Where(x => x == 1).Count();
        Console.WriteLine($"Num lit pixels: {numLitPixels}");
    }

    private static ImmutableArray<ImmutableArray<int>> Enhance(ImmutableArray<ImmutableArray<int>> image, ImmutableArray<int> algorithm) {
        //image = Expand2(image);
        image = Convolve(image, algorithm);
        return image;
    }

    private static ImmutableArray<ImmutableArray<int>> Convolve(ImmutableArray<ImmutableArray<int>> image, ImmutableArray<int> algorithm) {
        var numRows = image.Length;
        var numCols = image[0].Length;

        return Enumerable.Range(0, numRows)
            .Select(row => Enumerable.Range(0, numCols).Select(col => algorithm[CalculateConvolveNumber(image, row, col)]).ToImmutableArray())
            .ToImmutableArray();
    }

    private static int CalculateConvolveNumber(ImmutableArray<ImmutableArray<int>> image, int row, int col) {
        var numRows = image.Length;
        var numCols = image[0].Length;

        return Enumerable.Range(row - 1, 3)
            .SelectMany(r => Enumerable.Range(col - 1, 3).Select(c => Convolve(r, c)))
            .Aggregate(0, (a, i) => (a << 1) | i);


        int Convolve(int row, int col) {
            if (row < 0 || row >= numRows || col < 0 || col >= numCols) {
                return 0;
            }
            return image[row][col];
        }
    }

    private static ImmutableArray<ImmutableArray<int>> Expand(ImmutableArray<ImmutableArray<int>> image, int pixels) {
        var emptyRows = Enumerable.Repeat(Enumerable.Repeat(0, image[0].Length + pixels * 2).ToImmutableArray(), pixels).ToImmutableArray();
        var emptyCols = Enumerable.Repeat(0, pixels).ToImmutableArray();

        return image.Select(row => row.InsertRange(0, emptyCols).AddRange(emptyCols))
            .ToImmutableArray()
            .InsertRange(0, emptyRows)
            .AddRange(emptyRows);
    }

    private static ImmutableArray<ImmutableArray<int>> Reduce(ImmutableArray<ImmutableArray<int>> image, int pixels) {
        var rows = image.Skip(pixels)
            .Reverse()
            .Skip(pixels)
            .Reverse();

        return rows.Select(c => c.Skip(pixels).Reverse().Skip(pixels).Reverse().ToImmutableArray()).ToImmutableArray();
    }

    private static void PrintImage(IEnumerable<ImmutableArray<int>> image) {
        var imageStrings = image.Select(row => new String(row.Select(px => px == 0 ? '.' : '#').ToArray()));

        Console.WriteLine();
        foreach (var str in imageStrings) {
            Console.WriteLine(str);
        }
    }

    private static (ImmutableArray<int> Algorithm, ImmutableArray<ImmutableArray<int>> Image) ReadData() {
        var lines = File.ReadAllLines("input.txt");

        var algorithm = lines.First()
            .Select(ToBinary)
            .ToImmutableArray();

        var image = lines.Skip(1)
            .Where(line => line.Contains('.') || line.Contains('#'))
            .Select(line => line.Select(ToBinary).ToImmutableArray())
            .ToImmutableArray();

        return (algorithm, image);

        int ToBinary(char c) => c == '.' ? 0 : 1;
    }
}