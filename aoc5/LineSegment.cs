using System;
using System.Collections.Immutable;

namespace aoc5;

public readonly record struct LineSegment(Point P1, Point P2) {

    public bool IsHorizontal => P1.Y == P2.Y;

    public bool IsVertical => P1.X == P2.X;

    public IEnumerable<Point> Points() {
        if (IsHorizontal) {
            var minX = Math.Min(P1.X, P2.X);
            var maxX = Math.Max(P1.X, P2.X);
            var y = P1.Y;
            return Enumerable.Range(minX, maxX - minX + 1)
                .Select(x => new Point(x, y))
                .ToImmutableArray();
        }
        if (IsVertical) {
            var minY = Math.Min(P1.Y, P2.Y);
            var maxY = Math.Max(P1.Y, P2.Y);
            var x = P1.X;
            return Enumerable.Range(minY, maxY - minY + 1)
                .Select(y => new Point(x, y))
                .ToImmutableArray();
        }
        return ImmutableArray<Point>.Empty;
    }

    public override string ToString() => $"{P1} -> {P2}";
}