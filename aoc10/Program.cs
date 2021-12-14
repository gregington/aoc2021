using System;
using System.Collections.Immutable;
using System.IO;

namespace aoc10;

class Program {
    static void Main(string[] args) {
        var lines = ReadData();

        var corruptLines = lines.Select(l => (Line: l, CorruptPos: CorruptPosition(l)))
            .Where(x => x.CorruptPos > 0);

        var firstCorruptChar = corruptLines.Select(x => x.Line[x.CorruptPos]);
        var corruptScores = firstCorruptChar.Select(ScoreCorruptChar);
        var totalScore = corruptScores.Sum();

        Console.WriteLine($"Total Corrupt Score: {totalScore}");

        var incompleteLines = lines.Where(x => !corruptLines.Select(y => y.Line).Contains(x));

        var completions = incompleteLines.Select(x => (Line: x, Completion: Completion(x)));

        var completionScores = completions.Select(x => ScoreCompletion(x.Completion));

        var sortedScores = completionScores.OrderBy(s => s).ToImmutableArray();

        var middleScore = sortedScores[sortedScores.Length / 2];

        Console.WriteLine($"Middle Completion Score: {middleScore}");
    }

    static long ScoreCompletion(string completion) =>
        completion.Select(c => c)
            .Aggregate(0L, (s, c) => s * 5 + CompletionCharScore(c));
    
    static long CompletionCharScore(char c) {
        switch(c) {
            case ')':
                return 1;
            case ']':
                return 2;
            case '}':
                return 3;
            case '>':
                return 4;
            default:
                throw new ArgumentException($"Unexpected char {c}");
        }
    }

    static string Completion(string line) {
        var remainingStack =  Completion(line.Select(c => c).GetEnumerator(), ImmutableArray<char>.Empty);
        return string.Join("", remainingStack.Reverse().Select(ClosingFor));
    }

    static ImmutableArray<char> Completion(IEnumerator<char> chars, ImmutableArray<char> stack) {
        if (!chars.MoveNext()) {
            return stack;
        }
        var c = chars.Current;

        if (IsOpening(c)) {
            stack = stack.Add(c);
        } else {
            stack = stack.RemoveAt(stack.Length - 1);
        }

        return Completion(chars, stack);
    }

    static int ScoreCorruptChar(char c) {
        switch (c) {
            case ')':
                return 3;
            case ']':
                return 57;
            case '}':
                return 1197;
            case '>':
                return 25137;
            default:
                throw new ArgumentException($"Unexpected corrupt char {c}");
        }
    }

    static int CorruptPosition(string line) {
        return CorruptPosition(line.Select(c => c).GetEnumerator(), ImmutableArray<char>.Empty, 0);
    }

    static int CorruptPosition(IEnumerator<char> chars, ImmutableArray<char> stack, int pos) {
        if (!chars.MoveNext()) {
            return 0;
        }
        var c = chars.Current;

        if (IsOpening(c)) {
            stack = stack.Add(c);
        } else if (stack.Length == 0) {
            // incomplete
            return -1;
        } else {
            var opening = stack.Last();
            stack = stack.RemoveAt(stack.Length - 1);

            // Corrupt
            if (c != ClosingFor(opening)) {
                return pos;
            }
        }

        return CorruptPosition(chars, stack, pos + 1);
    }

    static bool IsOpening(char c) {
        return c == '(' || c == '[' || c == '{' || c == '<';
    }

    static char ClosingFor(char c) {
        switch (c) {
            case '(':
                return ')';
            case '[':
                return ']';
            case '{':
                return '}';
            case '<':
                return '>';
            default:
                throw new ArgumentException($"Invalid character {c}");
        }
    }

    static IEnumerable<string> ReadData() {
        foreach (var line in File.ReadLines("input.txt")) {
            yield return line;
        }
    }
}

