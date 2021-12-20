using System;
using System.Collections.Immutable;

namespace aoc16;

class Program {
    static void Main(string[] args) {
        var hex = ReadData();
        var packet = Parse(hex);

        Console.WriteLine(VersionSum(packet));
    }

    private static IPacket Parse(string hex) => Parse(new BitEnumerable(hex).GetEnumerator());

    private static IPacket Parse(IEnumerator<int> enumerator) {
        const int literalType = 4;
        var version = ReadInt(enumerator, 3);
        var typeId = ReadInt(enumerator, 3);

        return typeId switch {
            literalType => ParseLiteral(version, typeId, enumerator),
            _ => ParseOperator(version, typeId, enumerator)
        };
    }

    private static LiteralPacket ParseLiteral(int version, int typeId, IEnumerator<int> enumerator) {
        const int moreDataMask = 1 << 4;
        const int dataMask = 0x0F;

        var value = 0;
        bool hasMoreData;
        do {
            var dataChunk = ReadInt(enumerator, 5);
            hasMoreData = (dataChunk & moreDataMask) > 0;
            value = (value << 4) | (dataMask & dataChunk);
        } while (hasMoreData);
        
        return new LiteralPacket(version, typeId, value);
    }

    private static OperatorPacket ParseOperator(int version, int typeId, IEnumerator<int> enumerator) {
        const int bitLengthType = 0;
        
        var lengthType = ReadInt(enumerator, 1);
        ImmutableArray<IPacket> subPackets;
        if (lengthType == bitLengthType) {
            var length = ReadInt(enumerator, 15);
            var builder = ImmutableArray.CreateBuilder<int>();
            for (var i = 0; i < length; i++) {
                enumerator.MoveNext();
                builder.Add(enumerator.Current);
            }
            subPackets = ParseToEndOfEnumerator(builder.ToList().GetEnumerator());
        } else {
            var length = ReadInt(enumerator, 11);
            var builder = ImmutableArray.CreateBuilder<IPacket>();
            for (int i = 0; i < length; i++) {
                builder.Add(Parse(enumerator));
            }
            subPackets = builder.ToImmutableArray();
        }

        return new OperatorPacket(version, typeId, subPackets);        
    }

    private static ImmutableArray<IPacket> ParseToEndOfEnumerator(IEnumerator<int> enumerator) {
        var builder = ImmutableArray.CreateBuilder<IPacket>();
        var peekEnumerator = new PeekEnumerator<int>(enumerator);
        while (true) {
            try {
                var _ = peekEnumerator.Peek;
            } catch (InvalidOperationException) {
                break;
            }
            builder.Add(Parse(peekEnumerator));
        }

        return builder.ToImmutableArray();
    }

    private static int ReadInt(IEnumerator<int> enumerator, int numBits) {
        var value = 0;
        for (var i = 0; i < numBits; i++) {
            enumerator.MoveNext();
            value = (value << 1) | enumerator.Current; 
        }
        return value;
    }

    private static int VersionSum(IPacket packet) => packet.Children.Aggregate(packet.Version, (s, c) => s + VersionSum(c));

    private static string ReadData() => File.ReadLines("input.txt").First();
}
