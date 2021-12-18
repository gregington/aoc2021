using System.Collections.Immutable;

namespace aoc12;

public readonly record struct Cave(string name, ImmutableArray<Cave> Connections);