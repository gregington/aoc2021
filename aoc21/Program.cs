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

        var numWins = PlayQuantum(startingPositions);
        Console.WriteLine($"Most wins: {numWins.Max()}");
    }

    private static ImmutableArray<ulong> PlayQuantum(ImmutableArray<int> startingPositions) {
        var dicePossibilities = CalculateDiracDicePossibilities();

        var winCounts = new ulong[2];

        RollDiracDice(dicePossibilities, 0, 0, startingPositions[0], startingPositions[1], 0, 1, winCounts);

        return winCounts.ToImmutableArray();
    }

    public static void RollDiracDice(ImmutableDictionary<int, ulong> dicePossibilities, int p0Points, int p1Points, int p0Pos, int p1Pos, int turn, ulong universes, ulong[] winCounts) {
        const int winningScore = 21;
        if (p0Points >= winningScore || p1Points >= winningScore) {
            return;
        }
        
        var wins = new ulong[2];

        if (turn == 0) {
            foreach (var kvp in dicePossibilities) {
                var nextPosition = Move(p0Pos, kvp.Key);
                var newPoints = p0Points + nextPosition;
                if (newPoints < winningScore) {
                    RollDiracDice(dicePossibilities, newPoints, p1Points, nextPosition, p1Pos, 1, kvp.Value * universes, winCounts);
                } else {
                    winCounts[0] += universes * kvp.Value;
                }
            }
        } else {
            foreach (var kvp in dicePossibilities) {
                var nextPosition = Move(p1Pos, kvp.Key);
                var newPoints = p1Points + nextPosition;
                if (newPoints < winningScore) {
                    RollDiracDice(dicePossibilities, p0Points, newPoints, p0Pos, nextPosition, 0, kvp.Value * universes, winCounts);
                } else {
                    winCounts[1] += universes * kvp.Value;
                }
            }
        }
    }

    private static ImmutableDictionary<int, ulong> CalculateDiracDicePossibilities() {
        var dict = new Dictionary<int, ulong>();
        for (var i = 1; i <= 3; i++) {
            for (var j = 1; j <= 3; j++) {
                for (var k = 1; k <= 3; k++) {
                    var total = i + j + k;
                    var prevValue = dict.GetValueOrDefault(total, (ulong) 0);
                    dict[total] = prevValue + 1;
                }
            }
        }

        return dict.ToImmutableDictionary();
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
