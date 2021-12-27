using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using MathNet.Numerics.LinearAlgebra;

namespace aoc19;

class Program {
    private static int MinBeaconsForOverlap = 12;

    static void Main(string[] args) {
        var scanners = ReadData();
        var numBeacons = LocateScanners(scanners)
            .SelectMany(s => s.GetBeaconsInWorld())
            .Distinct()
            .Count();

        Console.WriteLine(numBeacons);
    }

    private static ImmutableHashSet<Scanner> LocateScanners(IEnumerable<Scanner> input) {
        var scanners = new HashSet<Scanner>(input);
        var locatedScanners = new HashSet<Scanner>();
        var queue = new Queue<Scanner>();

        locatedScanners.Add(scanners.First());
        queue.Enqueue(scanners.First());

        scanners.Remove(scanners.First());

        while (queue.Any()) {
            var scannerA = queue.Dequeue();
            foreach (var scannerB in scanners) {
                var locatedScanner = TryToLocate(scannerA, scannerB);
                if (locatedScanner != null) {
                    locatedScanners.Add(locatedScanner.Value);
                    queue.Enqueue(locatedScanner.Value);

                    scanners.Remove(scannerB);
                }
            }
        }

        return locatedScanners.ToImmutableHashSet();
    }

    private static Scanner? TryToLocate(Scanner scannerA, Scanner scannerB) {
        var beaconsInA = scannerA.GetBeaconsInWorld().ToImmutableArray();

        foreach (var (beaconInA, beaconInB) in PotentialMatchingBeacons(scannerA, scannerB)) {
            foreach (var rotation in RotationMatrices()) {
                var rotatedB = scannerB.Rotate(rotation);
                var beaconRotatedInB = rotatedB.Transform(beaconInB);

                var locatedB = rotatedB.Translate(beaconInA - beaconRotatedInB);

                if (locatedB.GetBeaconsInWorld().Intersect(beaconsInA).Count() >= MinBeaconsForOverlap) {
                    return locatedB;
                }
            }
        }

        return null;
    }

    private static IEnumerable<(Vector<double> BeaconInA, Vector<double> BeaconInB)> PotentialMatchingBeacons(Scanner scannerA, Scanner scannerB) {
        foreach (var beaconInA in scannerA.GetBeaconsInWorld()) {
            var absA = AbsCoordinates(scannerA.Translate(beaconInA * -1)).ToHashSet();

            foreach (var beaconInB in scannerB.GetBeaconsInWorld()) {
                var absB = AbsCoordinates(scannerB.Translate(beaconInB * -1));

                if (absB.Count(d => absA.Contains(d)) >= 3 * MinBeaconsForOverlap) {
                    yield return (beaconInA, beaconInB);
                }
            }
        }

        IEnumerable<double> AbsCoordinates(Scanner scanner) =>
            scanner.GetBeaconsInWorld()
                .SelectMany(b => b)
                .Select(Math.Abs);
    }

    private static ImmutableArray<Scanner> ReadData() {
        var scannerRegex = new Regex("--- scanner (?<scanner>[0-9]+) ---");
        var beaconRegex = new Regex("(?<x>-?[0-9]+),(?<y>-?[0-9]+),(?<z>-?[0-9]+)");
        var dict = new Dictionary<int, List<Vector<double>>>();


        List<Vector<double>> beacons = null;
        foreach(var line in File.ReadLines("input.txt")) {
            var scannerMatch = scannerRegex.Match(line);
            if (scannerMatch.Success) {
                var scannerNum = Convert.ToInt32(scannerMatch.Groups["scanner"].Value);
                beacons = new List<Vector<double>>();
                dict.Add(scannerNum, beacons);
                continue;
            }
            var beaconMatch = beaconRegex.Match(line);
            if (beaconMatch.Success) {
                var x = Convert.ToDouble(beaconMatch.Groups["x"].Value);
                var y = Convert.ToDouble(beaconMatch.Groups["y"].Value);
                var z = Convert.ToDouble(beaconMatch.Groups["z"].Value);                
                beacons.Add(CreateVector.DenseOfArray<double>(new [] { x, y, z, }));
            }            
        }

        return dict.Select(kvp => (ScannerNum: kvp.Key, Beacons: kvp.Value.ToImmutableArray()))
            .OrderBy(x => x.ScannerNum)
            .Select(x => x.Beacons.ToImmutableArray())
            .Select(b => new Scanner(CreateVector.DenseOfArray<double>(new [] { 0d, 0d, 0d }), RotationMatrices().First(), b))
            .ToImmutableArray();
    }

    private static IEnumerable<Matrix<double>> RotationMatrices() {
        var xRotations = CalculateXRotations();
        var yRotations = CalculateYRotations();
        var zRotations = CalculateZRotations();

        foreach (var xRotation in xRotations) {
            foreach (var yRotation in yRotations) {
                foreach (var zRotation in zRotations) {
                    yield return xRotation.Multiply(yRotation).Multiply(zRotation);
                }
            }
        }
    }

    private static IEnumerable<Matrix<double>> CalculateXRotations() {
        foreach (var theta in RotationAngles()) {
            yield return CreateMatrix.DenseOfArray<double>(new double[,] {
                { 1, 0,                     0 },
                { 0, (int) Math.Cos(theta), (int) -Math.Sin(theta) },
                { 0, (int) Math.Sin(theta), (int) Math.Cos(theta) }
            });
        }
    }

    private static IEnumerable<Matrix<double>> CalculateYRotations() {
        foreach (var theta in RotationAngles()) {
            yield return CreateMatrix.DenseOfArray<double>(new double[,] {
                { (int) Math.Cos(theta),  0, (int) Math.Sin(theta) },
                { 0,                      1, 0 },
                { (int) -Math.Sin(theta), 0, (int) Math.Cos(theta) }
            });
        }
    }

    private static IEnumerable<Matrix<double>> CalculateZRotations() {
        foreach (var theta in RotationAngles()) {
            yield return CreateMatrix.DenseOfArray<double>(new double[,] {
                { (int) Math.Cos(theta), (int) -Math.Sin(theta), 0 },
                { (int) Math.Sin(theta), (int) Math.Cos(theta),  0 },
                { 0,                     0,                      1 }
            });
        }
    }
    

    private static IEnumerable<double> RotationAngles() {
        for (var i = 0; i < 4; i++) {
            yield return i * Math.PI / 2;
        }
    }
}
