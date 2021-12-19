using System.Collections.Generic;

namespace aoc15;

public class NodeComparer : IComparer<Node>
{
    public int Compare(Node x, Node y) => x.Risk.CompareTo(y.Risk);
}