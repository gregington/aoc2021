namespace aoc17;

public readonly record struct Point(int X, int Y) {

    public bool Within(TargetArea targetArea) =>
        X >= targetArea.XMin && X <= targetArea.XMax &&
        Y >= targetArea.YMin && Y <= targetArea.YMax;

    public bool Overshot(TargetArea targetArea) => 
        X > targetArea.XMax || Y < targetArea.YMin;

    public Point Step(Velocity velocity) => new Point(X + velocity.X, Y + velocity.Y);
}