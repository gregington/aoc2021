using System;
using System.Diagnostics.CodeAnalysis;

namespace aoc18;

public class Pair : IElement {

    private IElement x;
    private IElement y;

    [AllowNull]    
    private Pair parent;

    public Pair(IElement x, IElement y) {
        this.x = x;
        this.y = y;
        x.Parent = this;
        y.Parent = this;
    }

    public IElement X => x;
    public IElement Y => y;
    public Pair Parent {
        get {
            return parent;
        }
        set {
            this.parent = value;
        }
    }

    public static Pair operator +(Pair a, Pair b) {
        var newPair = new Pair(a, b);
        return newPair;
    }
/*
    public Explode() {
        if (!(x is Number)) {
            throw new ArgumentException("Expected x to be a Number");
        }
        if (!(y is Number)) {
            throw new ArgumentException("Expected y to be a Number");
        }
        var firstRegularNumberToLeft = FindFirstRegularNumberToLeft();
    }
*/

    public override string ToString() => $"[{X.ToString()},{Y.ToString()}]";

}