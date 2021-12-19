namespace aoc15;

public readonly record struct Point(int X, int Y) {
    public override string ToString() => $"({X},{Y})";
}