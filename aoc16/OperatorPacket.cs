namespace aoc16;

using System.Collections.Immutable;

public readonly record struct OperatorPacket(int Version, int TypeId, ImmutableArray<IPacket> Children) : IPacket {

    public long Evaluate() {
        const int sum = 0;
        const int product = 1;
        const int minimum = 2;
        const int maximum = 3;
        const int greaterThan = 5;
        const int lessThan = 6;
        const int equal = 7;

        return TypeId switch {
            sum => Children.Select(c => c.Evaluate()).Sum(),
            product => Children.Aggregate(1L, (a, c) => a * c.Evaluate()),
            minimum => Children.Select(c => c.Evaluate()).Min(),
            maximum => Children.Select(c => c.Evaluate()).Max(),
            greaterThan => Children[0].Evaluate() > Children[1].Evaluate() ? 1 : 0,
            lessThan => Children[0].Evaluate() < Children[1].Evaluate() ? 1 : 0,
            equal => Children[0].Evaluate() == Children[1].Evaluate() ? 1 : 0,
            _ => throw new ArgumentException($"Unknown TypeId {TypeId}")
        };
    }

    public override string ToString()
    {
        var childrenStr = string.Join(", ", Children.Select(c => c.ToString()));
        return $"{nameof(OperatorPacket)} {{ {nameof(Version)} = {Version}, {nameof(TypeId)} = {TypeId}, {nameof(Children)} = [ {childrenStr} ] }}";
    }
}