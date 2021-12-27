using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace aoc22;

class Program {
    static void Main() {
        var commands = ReadData();

        Part1(commands);
        Part2(commands);
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

    private static void Part2(ImmutableArray<Command> commands) {
        var onRegions = new List<Region>();
        var offRegions = new List<Region>();

        foreach (var command in commands) {
            // Intersections with on regions are added to off regions
            var newOffRegions = onRegions.Select(r => r.Intersect(command.Region))
                .Where(i => i.HasValue)
                .Select(i => i.Value)
                .ToArray();

            // Intersecions with off regions are added to on regions.
            var newOnRegions = offRegions.Select(r => r.Intersect(command.Region))
                .Where(i => i.HasValue)
                .Select(i => i.Value)
                .ToArray();

            offRegions.AddRange(newOffRegions);
            onRegions.AddRange(newOnRegions);

            if (command.Value == 1) {
                // Add new on range
                onRegions.Add(command.Region);                
            }
        }

        var onVolume = onRegions.Aggregate((ulong) 0, (t, r) => t + r.Volume());
        var offVolume = offRegions.Aggregate((ulong) 0, (t, r) => t + r.Volume());

        Console.WriteLine($"Total Volume On: {onVolume - offVolume}");
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