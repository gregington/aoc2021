using System;
using MathNet.Numerics.LinearAlgebra;

namespace aoc19;

public sealed class Beacon {

    public double X => Vector[0];
    public double Y => Vector[1];

    public double Z => Vector[2];

    public Vector<double> Vector { get; }

    public Beacon(int x, int y, int z) : this(CreateVector.DenseOfArray(new double[] { x, y, z })) {
    }

    public Beacon(Vector<double> vector) {
        this.Vector = vector;
    }

    public double DistanceTo(Beacon other) {
        return Math.Sqrt(Math.Pow((X - other.X), 2) + Math.Pow((Y - other.Y), 2) + Math.Pow((Z - other.Z), 2));
    }

    public Beacon Translate(Vector<double> translation) {
        var translated = Vector.Add(translation);
        return new Beacon(Vector.Add(translation));
    }

    public override bool Equals(object? obj) => Vector.Equals(obj);

    public override int GetHashCode() => Vector.GetHashCode();
}