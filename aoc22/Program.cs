using System;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace aoc22;

class Program {
    static void Main() {
        var commands = ReadData();

        Part1(commands);
    }

    private static void Part1(ImmutableArray<Command> commands) {
        var limit = new Region(-50, 50, -50, 50, -50, 50);

        var limitedCommands = commands
            .Where(c => limit.Contains(c.Region))
            .Select(c => c with { Region = limit.Restrict(c.Region) })
            .ToImmutableArray();

        var onCubes = Execute(limitedCommands);
        var numOnCubes = onCubes.Count();
        Console.WriteLine($"Num cubes on: {numOnCubes}");
    }

    private static ImmutableHashSet<Cube> Execute(IEnumerable<Command> commands) {
        var onCubes = new HashSet<Cube>();

        foreach (var command in commands) {
            if (command.Value == 0) {
                foreach(var cube in command.Region.Cubes()) {
                    onCubes.Remove(cube);
                }
            } else {
                foreach(var cube in command.Region.Cubes()) {
                    onCubes.Add(cube);
                }
            }
        }

        return onCubes.ToImmutableHashSet();
    }

    private static ImmutableArray<Command> ReadData() {
        var regex = new Regex("(?<value>on|off) x=(?<xmin>-?\\d+)\\.\\.(?<xmax>-?\\d+),y=(?<ymin>-?\\d+)\\.\\.(?<ymax>-?\\d+),z=(?<zmin>-?\\d+)\\.\\.(?<zmax>-?\\d+)");
        
        return File.ReadLines("input.txt")
            .Select(line => regex.Match(line))
            .Where(m => m.Success)
            .Select(m => m.Groups)
            .Select(g => new Command(new Region(
                Convert.ToInt32(g["xmin"].Value),
                Convert.ToInt32(g["xmax"].Value),
                Convert.ToInt32(g["ymin"].Value),
                Convert.ToInt32(g["ymax"].Value),
                Convert.ToInt32(g["zmin"].Value),
                Convert.ToInt32(g["zmax"].Value)
            ), g["value"].Value == "off" ? 0 : 1))
            .ToImmutableArray();
    }
}