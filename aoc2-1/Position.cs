namespace aoc2;

public readonly record struct Position(int Horizontal = 0, int Depth = 0) {

    public Position Apply(Command command) {
        switch (command.Direction) {
            case Direction.Forward:
                return this with { Horizontal = this.Horizontal + command.Distance };
            case Direction.Down:
                return this with { Depth = this.Depth + command.Distance };
            case Direction.Up:
                return this with { Depth = this.Depth - command.Distance };
            default:
                throw new ArgumentException($"Unknown direction {command.Direction}.");
        }
    }

    public override string ToString() => $"Horizontal = {Horizontal}, Depth = {Depth}";
}