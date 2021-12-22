using System;
using System.Collections.Immutable;
using System.Text;
using System.Text.RegularExpressions;

namespace aoc17;

class Program {
    static void Main(string[] args) {
        var targetArea = ReadData();
        Console.WriteLine(targetArea);

        const int minVx = 0;
        const int maxVx = 1000;
        const int minVy = -1000;
        const int maxVy = 1000;
        
        var bestY = int.MinValue;
        var bestVelocity = new Velocity(minVx, minVy);
        var bestTrajectory = ImmutableArray<Point>.Empty;

        for (var vx = minVx; vx < maxVx + 1; vx++) {
            for (var vy = minVy; vy < maxVy + 1; vy++) {
                var initialVelocity = new Velocity(vx, vy);
                var (hitTarget, trajectory) = Fire(initialVelocity, targetArea);
                if (hitTarget) {
                    var maxY = trajectory.Select(p => p.Y).Max();
                    if (maxY > bestY) {
                        bestY = maxY;
                        bestVelocity = initialVelocity;
                        bestTrajectory = trajectory;
                    }
                }
            }
        }

        PrintBoard(bestTrajectory, targetArea);
        Console.WriteLine($"Best Y: {bestY}, Best Velocity: {bestVelocity}");
    }

    private static (bool HitTarget, ImmutableArray<Point> Path) Fire(Velocity startingVelocity, TargetArea targetArea) {
        var point = new Point(0, 0);
        var velocity = startingVelocity;

        var points = new List<Point> { point };

        while (true) {
            point = point.Step(velocity);
            velocity = velocity.Step();
            points.Add(point);

            if (point.Within(targetArea)) {
                return (true, points.ToImmutableArray());
            }
            if (point.Overshot(targetArea)) {
                return (false, points.ToImmutableArray());
            }
        }
    }

    private static void PrintBoard(IEnumerable<Point> trajectory, TargetArea targetArea) {
        var maxX = Math.Max(trajectory.Select(p => p.X).Max(), targetArea.XMax);
        var maxY = Math.Max(trajectory.Select(p => p.Y).Max(), targetArea.YMax);
        var minY = Math.Min(trajectory.Select(p => p.Y).Min(), targetArea.YMin);

        var lines = new Dictionary<int, StringBuilder>();
        for (var y = maxY; y >= minY; y--) {
            var sb = new StringBuilder();
            for (var i = 0; i <= maxX; i++) {
                sb.Append('.');
            }
            lines[y] = sb;
        }

        for (var y = targetArea.YMax; y >= targetArea.YMin; y--) {
            for (var x = targetArea.XMin; x <= targetArea.XMax; x++) {
                lines[y][x] = 'T';
            }
        }

        foreach (var point in trajectory) {
            lines[point.Y][point.X] = '#';
        }

        lines[0][0] = 'S';

        for (var y = maxY; y >= minY; y--) {
            Console.WriteLine(lines[y].ToString());
        }
    }

    private static TargetArea ReadData() {
        var regex = new Regex("^target area: x=(?<xmin>-?[0-9]+)\\.\\.(?<xmax>-?[0-9]+), y=(?<ymin>-?[0-9]+)\\.\\.(?<ymax>-?[0-9]+)$");
        var line = File.ReadLines("input.txt").First();
        var match = regex.Match(line);
        var groups = match.Groups;
        return new TargetArea(
            Convert.ToInt32(groups["xmin"].Value),
            Convert.ToInt32(groups["xmax"].Value),
            Convert.ToInt32(groups["ymin"].Value),
            Convert.ToInt32(groups["ymax"].Value));
    }
}
