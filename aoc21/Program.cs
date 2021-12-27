using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace aoc21;

class Program {
    static void Main(string[] args) {
        var startingPositions = ReadData();

        var (scores, rolls) = Play(startingPositions);
        var losingScore = scores.Min();
        Console.WriteLine($"Losing score: {losingScore}, Total Rolls: {rolls}, losing score * rolls: {losingScore * rolls}");
    }

    private static (ImmutableArray<int> Scores, int NumRolls) Play(ImmutableArray<int> startingPositions) {
        const int winningScore = 1000;
        const int rollsPerTurn = 3;
        var dieRolls = DieRolls().GetEnumerator();
        var turns = CreateTurns(startingPositions.Length).GetEnumerator();
        var positions = startingPositions.ToArray();
        var scores = new int[positions.Length];
        var rollCount = 0;

        while (scores.All(s => s < winningScore)) {
            var player = NextTurn(turns);
            var rollTotal = SumRolls(dieRolls, rollsPerTurn);
            rollCount += rollsPerTurn;
            positions[player] = Move(positions[player], rollTotal);
            scores[player] += positions[player];
        }

        return (scores.ToImmutableArray(), rollCount);
    }

    private static int NextTurn(IEnumerator<int> turns) {
        turns.MoveNext();
        return turns.Current;
    }

    private static int Move(int currentPosition, int numSpaces) {
        const int totalSpaces = 10;
        return (((currentPosition - 1) + numSpaces) % totalSpaces) + 1;
    }

    private static int SumRolls(IEnumerator<int> dieRolls, int numRolls) {
        var sum = 0;
        for (int i = 0; i < numRolls; i++) {
            dieRolls.MoveNext();
            sum += dieRolls.Current;
        }
        return sum;
    }

    private static ImmutableArray<int> ReadData() {
        var regex = new Regex("Player (?<player>[0-9]+) starting position: (?<position>[0-9]+)");
        return File.ReadLines("input.txt")
            .Select(line => regex.Match(line))
            .Where(m => m.Success)
            .Select(m => m.Groups)
            .Select(g => (Player: Convert.ToInt32(g["player"].Value), Position: Convert.ToInt32(g["position"].Value)))
            .OrderBy(x => x.Player)
            .Select(x => x.Position)
            .ToImmutableArray();
    }

    private static IEnumerable<int> DieRolls() {
        while (true) {
            for (int i = 1; i <= 100; i++) {
                yield return i;
            }
        }
    }

    private static IEnumerable<int> CreateTurns(int numPlayers) {
        while (true) {
            for (int i = 0; i < numPlayers; i++) {
                yield return i;
            }
        }
    }
}
