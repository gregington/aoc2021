using System;
using System.Collections.Immutable;

namespace aoc25;

class Program {
    static void Main(string[] args) {
        var (rows, cols, cucumbers) = ReadData();

        Part1(rows, cols, cucumbers);
    }

    static void Part1(int rowLength, int colLength, ImmutableDictionary<Point, Direction> cucumbers) {

        var currentState = cucumbers;
        ImmutableDictionary<Point, Direction> prevState;
        var step = 0;
        do {
            step++;
            prevState = currentState;
            currentState = MoveDirection(currentState, Direction.East);
            currentState = MoveDirection(currentState, Direction.South);
        } while (currentState.Except(prevState).Any());

        PrintState(currentState, rowLength, colLength);
        Console.Write($"Cucumbers stop in {step} steps.");


        ImmutableDictionary<Point, Direction> MoveDirection(ImmutableDictionary<Point, Direction> state, Direction direction) {
            var cucumbers = currentState.Where(kvp => kvp.Value == direction)
                .Select(kvp => kvp.Key);
            var destinations = cucumbers.Select(x => (Current: x, Destination: CalculateDestination(x, direction, rowLength, colLength)));
            var movements = destinations.Where(x => !state.ContainsKey(x.Destination));
            return Move(currentState, movements, direction);
        }
    }

    private static Point CalculateDestination(Point current, Direction direction, int rowLength, int colLength) {
        return direction switch {
            Direction.East => current with { Col = (current.Col + 1) % colLength },
            Direction.South => current with { Row = (current.Row + 1) % rowLength},
            _ => throw new ArgumentException($"Unknown direction {direction}")
        };
    }

    private static ImmutableDictionary<Point, Direction> Move(ImmutableDictionary<Point, Direction> state, IEnumerable<(Point Current, Point Destination)> movements, Direction direction) {
        state = state.RemoveRange(movements.Select(x => x.Current));
        state = state.AddRange(movements.Select(x => new KeyValuePair<Point, Direction>(x.Destination, direction)));
        return state;
    }

    private static void PrintState(ImmutableDictionary<Point, Direction> cucumbers, int rowLength, int colLength) {
        for (var row = 0; row < rowLength; row++) {
            for (var col = 0; col < colLength; col++) {
                var point = new Point(row, col);
                if (!cucumbers.ContainsKey(point)) {
                    Console.Write('.');
                } else {
                    Console.Write(cucumbers[point] switch {
                        Direction.East => '>',
                        _ => 'v'
                    });
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }

    private static (int Rows, int Cols, ImmutableDictionary<Point, Direction> Cucumbers) ReadData() {
        var lines = File.ReadAllLines("input.txt");

        var rows = lines.Length;
        var cols = lines[0].Length;

        var dict = ImmutableDictionary.CreateBuilder<Point, Direction>();
        for (var r = 0; r < rows; r++) {
            for (var c = 0; c < cols; c++) {
                var ch = lines[r][c];
                if (ch == '>') {
                    dict.Add(new Point(r, c), Direction.East);
                } else if (ch == 'v') {
                    dict.Add(new Point(r, c), Direction.South);
                }
            }
        }

        return (rows, cols, dict.ToImmutableDictionary());
    }
}