using System.Collections.Immutable;

namespace aoc16;

public readonly record struct LiteralPacket(int Version, int TypeId, long Value) : IPacket
{
    public ImmutableArray<IPacket> Children => ImmutableArray<IPacket>.Empty;

    public long Evaluate() => Value;

    public override string ToString()
    {
        return $"{nameof(LiteralPacket)} {{ {nameof(Version)} = {Version}, {nameof(TypeId)} = {TypeId}, {nameof(Value)} = {Value} }}";
    }
}