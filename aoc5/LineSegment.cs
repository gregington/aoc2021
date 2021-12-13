using System;
using System.Collections.Immutable;

namespace aoc5;

public readonly record struct LineSegment(Point P1, Point P2) {

    public bool IsHorizontal => P1.Y == P2.Y;

    public bool IsVertical => P1.X == P2.X;

    public IEnumerable<Point> Points() {
        if (IsHorizontal) {
            return HorizontalPoints();
        }
        if (IsVertical) {
            return VerticalPoints();
        }
        return DiagonalPoints();
    }

    private IEnumerable<Point> HorizontalPoints() {
        var minX = Math.Min(P1.X, P2.X);
        var maxX = Math.Max(P1.X, P2.X);
        var y = P1.Y;
        return Enumerable.Range(minX, maxX - minX + 1)
            .Select(x => new Point(x, y));
    }

    private IEnumerable<Point> VerticalPoints() {
        var minY = Math.Min(P1.Y, P2.Y);
        var maxY = Math.Max(P1.Y, P2.Y);
        var x = P1.X;
        return Enumerable.Range(minY, maxY - minY + 1)
            .Select(y => new Point(x, y));
    }

    private IEnumerable<Point> DiagonalPoints() {
        var pointWithMinX = P1.X < P2.X ? P1 : P2;
        var pointWithMaxX = P1.X > P2.X ? P1 : P2;

        var length = pointWithMaxX.X - pointWithMinX.X + 1;

        var yMultiplier = Math.Sign(pointWithMaxX.Y - pointWithMinX.Y);

        return Enumerable.Range(0, length)
            .Select(i => new Point(pointWithMinX.X + i, pointWithMinX.Y + (yMultiplier * i)));
    }

    public override string ToString() => $"{P1} -> {P2}";
}