namespace aoc16;

using System.Collections.Immutable;

public readonly record struct OperatorPacket(int Version, int TypeId, ImmutableArray<IPacket> Children) : IPacket {
    public override string ToString()
    {
        var childrenStr = string.Join(", ", Children.Select(c => c.ToString()));
        return $"{nameof(OperatorPacket)} {{ {nameof(Version)} = {Version}, {nameof(TypeId)} = {TypeId}, {nameof(Children)} = [ {childrenStr} ] }}";
    }
}