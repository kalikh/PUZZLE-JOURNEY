using System;

namespace CaravanSecrets.Game.Board
{
    public enum CellType { Empty, Rock, Exit, Switch, Storage }
    public enum Direction { Up, Right, Down, Left }
    public enum CargoType { Spices, Fabrics, Water, MetalParts, Artifacts, Tools, Food, Scrolls }
    public enum ObjectiveType { ExitAllCarts, DeliverAllCargo, ActivateAllSwitches }

    [Serializable]
    public readonly struct GridPosition : IEquatable<GridPosition>
    {
        public int X { get; }
        public int Y { get; }

        public GridPosition(int x, int y) { X = x; Y = y; }

        public GridPosition Step(Direction direction) => direction switch
        {
            Direction.Up => new GridPosition(X, Y + 1),
            Direction.Right => new GridPosition(X + 1, Y),
            Direction.Down => new GridPosition(X, Y - 1),
            Direction.Left => new GridPosition(X - 1, Y),
            _ => this
        };

        public bool Equals(GridPosition other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is GridPosition other && Equals(other);
        public override int GetHashCode() => HashCode.Combine(X, Y);
        public override string ToString() => $"({X}, {Y})";
    }
}
