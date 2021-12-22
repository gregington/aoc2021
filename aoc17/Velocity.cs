namespace aoc17;

public readonly record struct Velocity(int X, int Y) {
    public Velocity Step() => new Velocity(StepX(), StepY());
    
    private int StepX() =>
        X switch {
            < 0 => X + 1,
            > 0 => X - 1,
            _ => X
        };

    private int StepY() => Y - 1;
    
}