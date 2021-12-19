using System.Collections.Immutable;

namespace aoc15;

public readonly record struct Node(Point Point, ImmutableArray<Point> Path, int Risk) {
    public Node MoveTo(Point newPoint, int risk) {
        return new Node(newPoint, Path.Add(newPoint), Risk + risk);
    }
}