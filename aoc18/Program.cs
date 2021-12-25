using System.Collections.Immutable;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace aoc18;

class Program {
    static void Main(string[] args) {
        var lines = ReadData().ToImmutableArray();
        var init = lines.First();
        var sum = lines.Skip(1).Aggregate(init, (x, y) => Add(x, y));
        Console.WriteLine(Magnitude(sum));

        var mag = Pairs(lines).Select(x => (x.Left, x.Right, Magnitude: Magnitude(Add(x.Left, x.Right))))
            .MaxBy(x => x.Magnitude);
        Console.WriteLine(mag);
    }

    private static IEnumerable<(string Left, string Right)> Pairs(ImmutableArray<string> lines) {
        for (var i = 0; i < lines.Length; i++) {
            for (var j = 0; j < lines.Length; j++) {
                if (i == j) {
                    continue;
                }
                yield return (lines[i], lines[j]);
                yield return (lines[j], lines[i]);
            }
        }
    }

    private static string Add(string left, string right) {
        return Reduce($"[{left},{right}]");
    }

    private static int Magnitude(string snailNum) {
        while (snailNum.Contains('[')) {
            var (pos, len, left, right) = FindSimplePairs(snailNum)[0];
            var mag = PairMagnitude(left, right);
            var strLeft = snailNum.Substring(0, pos);
            var strRight = snailNum.Substring(pos + len);
            snailNum = strLeft + mag.ToString() + strRight;
        }
        return Convert.ToInt32(snailNum);
    }

    private static int PairMagnitude(int left, int right) {
        return 3 * left + 2 * right;
    }

    private static string Reduce(string snailNum) {
        while (true) {
            (snailNum, var didExplode) = Explode(snailNum);
            if (didExplode) {
                continue;
            }
            (snailNum, var didSplit) = Split(snailNum);
            if (!didSplit) {
                break;
            }
        }
        return snailNum;
    }

    private static (string SnailNum, bool didExplode) Explode(string snailNum) {
        var simplePairs = FindSimplePairs(snailNum);
        foreach (var simplePair in simplePairs) {
            var depth = FindDepth(snailNum, simplePair);
            if (depth >= 5) {
                var (pos, len, left, right) = simplePair;
                var offsetLeft = 0;
                string leftChunk;
                string rightChunk;
                var (lefterPos, lefterLen, lefterNum) = FindNext(snailNum, pos, 0, true);
                if (lefterPos != -1) {
                    var newLeftNum = simplePair.Left + lefterNum;
                    var newLeftStr = newLeftNum.ToString();
                    offsetLeft += newLeftStr.Length - lefterLen;
                    leftChunk = snailNum.Substring(0, lefterPos);
                    rightChunk = snailNum.Substring(lefterPos + lefterLen);
                    snailNum = leftChunk + newLeftStr + rightChunk;
                }
                leftChunk = snailNum.Substring(0, simplePair.Pos + offsetLeft);
                rightChunk = snailNum.Substring(simplePair.Pos + simplePair.Len + offsetLeft);
                var offsetMid = 1 - simplePair.Len;
                snailNum = leftChunk + "0" + rightChunk;
                var (righterPos, righterLen, righterNum) = FindNext(snailNum, simplePair.Pos + simplePair.Len, offsetLeft + offsetMid, false);
                if (righterPos != -1) {
                    var newRightNum = simplePair.Right + righterNum;
                    var righterNumStr = newRightNum.ToString();
                    leftChunk = snailNum.Substring(0, righterPos);
                    rightChunk = snailNum.Substring(righterPos + righterLen);
                    snailNum = leftChunk + righterNumStr + rightChunk;
                }
                return (snailNum, true);
            }
        }
        return (snailNum, false);
    }

    private static (string SnailNum, bool didSplit) Split(string snailNum) {
        var regex = new Regex("(?<num>\\d\\d+)");
        var match = regex.Match(snailNum);
        if (!match.Success) {
            return (snailNum, false);
        }
        var numStr = match.Groups["num"].Value;
        var num = Convert.ToInt32(numStr);
        var leftNum = num / 2;
        var rightNum = (int) (Math.Ceiling((float) num / 2.0));
        snailNum = snailNum.Substring(0, match.Index) + $"[{leftNum},{rightNum}]" + snailNum.Substring(match.Index + match.Length);
        return (snailNum, true);
    }

    private static int FindDepth(string snailNum, (int Pos, int Len, int Left, int Right) simplePair) {
        return snailNum.Substring(0, simplePair.Pos + 1).Aggregate(0, (a, c) => a + c switch {
            '[' => 1,
            ']' => -1,
            _ => 0
        });
    }

    private static (int Pos, int Len, int Num) FindNext(string snailNum, int index, int offset, bool left) {
        var regex = new Regex("([0-9]+)");

        var substr = left ? new String(snailNum.Substring(0, index + offset).Reverse().ToArray()) : snailNum.Substring(index + offset);

        var match = regex.Match(substr);
        if (!match.Success) {
            return (-1, -1, -1);
        }
        string numString = substr.Substring(match.Index, match.Length);
        var startPos = match.Index;
        var length = match.Length;
        if (left) {
            numString = new String(numString.Reverse().ToArray());
            return (index - (length + startPos) + offset, match.Length, Convert.ToInt32(numString));
        }
        return (index + startPos + offset, match.Length, Convert.ToInt32(numString));
    }

    private static ImmutableArray<(int Pos, int Len, int Left, int Right)> FindSimplePairs(string snailNum) {
        var regex = new Regex("(\\[(?<left>[0-9]+),(?<right>[0-9]+)\\])");
        var matches = regex.Matches(snailNum);
        return matches.Select(m => (m.Index, m.Length, Convert.ToInt32(m.Groups["left"].Value), Convert.ToInt32(m.Groups["right"].Value)))
            .ToImmutableArray();
    }

    private static IEnumerable<string> ReadData() {
        return File.ReadLines("input.txt");
    }
}