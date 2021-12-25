using System.Diagnostics.CodeAnalysis;

namespace aoc18;

public class Number : IElement {

    private int value;

    [AllowNull]
    private Pair parent;

    public Number(Pair parent, int value) {
        this.value = value;
    }

    public Pair Parent {
        get {
            return parent;
        }
        set {
            parent = value;
        }
    }


    public override string ToString() => value.ToString();
}