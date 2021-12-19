using System.Collections.Immutable;

namespace aoc15;

public readonly record struct Node(Point Point, int Risk) {
    public Node MoveTo(Point newPoint, int risk) {
        return new Node(newPoint, Risk + risk);
    }
}