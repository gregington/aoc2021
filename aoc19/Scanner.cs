using System;
using System.Collections.Immutable;
using MathNet.Numerics.LinearAlgebra;

namespace aoc19;

public readonly record struct Scanner(Vector<double> Centre, Matrix<double> Rotation, ImmutableArray<Vector<double>> BeaconsInLocal) {

    public Scanner Rotate(Matrix<double> newRotation) => this with { Rotation = newRotation };
    
    public Scanner Translate(Vector<double> translation) => this with { Centre = Centre + translation };

    public Vector<double> Transform(Vector<double> coord) {
        return Rotation * coord + Centre;
    }

    public IEnumerable<Vector<double>> GetBeaconsInWorld() => BeaconsInLocal.Select(Transform);
}