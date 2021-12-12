namespace aoc2_2;

public readonly record struct Command(Direction Direction, int Distance) {

    public static Command FromText(string s) {
        var parts = s.Split(' ');
        return new Command(DirectionFromString(parts[0]), Convert.ToInt32(parts[1]));
    }

    private static Direction DirectionFromString(string s) {
        var enumStr = s.Substring(0, 1).ToUpper() + s.Substring(1).ToLower();
        return Enum.Parse<Direction>(enumStr);
    }

    public override string ToString() => $"{Direction} {Distance}";
}