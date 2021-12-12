namespace aoc2_2;

public readonly record struct Position(int Horizontal = 0, int Depth = 0, int Aim = 0) {

    public Position Apply(Command command) {
        switch (command.Direction) {
            case Direction.Forward:
                return this with { Horizontal = this.Horizontal + command.Distance, Depth = this.Depth + (this.Aim * command.Distance) };
            case Direction.Down:
                return this with { Aim = this.Aim + command.Distance };
            case Direction.Up:
                return this with { Aim = this.Aim - command.Distance };
            default:
                throw new ArgumentException($"Unknown direction {command.Direction}.");
        }
    }

    public override string ToString() => $"Horizontal = {Horizontal}, Depth = {Depth}";
}