using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace aoc4;
public sealed record Board(ImmutableArray<ImmutableArray<int>> Numbers) {

    public int NumRows => Numbers.Length;

    public int NumCols => Numbers[0].Length;

    public bool Win(ImmutableHashSet<int> drawnNumbers) =>
        LinesToCheck()
            .Any(l => l.All(n => drawnNumbers.Contains(n)));
    

    public IEnumerable<int> Unmarked(ImmutableHashSet<int> drawnNumbers) =>
        Numbers.SelectMany(n => n)
            .Where(n => !drawnNumbers.Contains(n));

    public override string ToString() {
        var sb = new StringBuilder();
        foreach (var row in Numbers) {
            sb.AppendLine(string.Join("", row.Select(n => $"{n,3}")));
        }
        return sb.ToString();
    }

    IEnumerable<ImmutableArray<int>> LinesToCheck() => Rows().Concat(Columns());

    IEnumerable<ImmutableArray<int>> Rows() {
        foreach (var r in Numbers) {
            yield return r;
        }
    }

    IEnumerable<ImmutableArray<int>> Columns() {
        for (var c = 0; c < NumCols; c++) {
            yield return Enumerable.Range(0, NumRows)
                .Select(r => Numbers[r][c])
                .ToImmutableArray();
        }
    }
}