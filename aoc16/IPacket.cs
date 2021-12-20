using System.Collections.Immutable;

namespace aoc16;

public interface IPacket {
    public int Version { get; }

    public int TypeId { get; }

    public ImmutableArray<IPacket> Children { get; }
}