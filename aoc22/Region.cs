namespace aoc22;

public readonly record struct Region(int XMin, int XMax, int YMin, int YMax, int ZMin, int ZMax) {
    
    public bool Contains(Region other) {
        return (other.XMax >= XMin && other.XMin <= XMax)
            && (other.YMax >= YMin && other.YMin <= YMax)
            && (other.ZMax >= ZMin && other.ZMin <= ZMax);
    }
    
    public Region Restrict(Region other) {
        return new Region(
            other.XMin < XMin ? XMin : other.XMin,
            other.XMax > XMax ? XMax : other.XMax,
            other.YMin < YMin ? YMin : other.YMin,
            other.YMax > YMax ? YMax : other.YMax,
            other.ZMin < ZMin ? ZMin : other.ZMin,
            other.ZMax > ZMax ? ZMax : other.ZMax
        );
    }

    public IEnumerable<Cube> Cubes() {
        for (var i = XMin; i <= XMax; i++) {
            for (var j = YMin; j <= YMax; j++) {
                for (var k = ZMin; k <= ZMax; k++) {
                    yield return new Cube(i, j, k);
                }
            }
        }
    }
}