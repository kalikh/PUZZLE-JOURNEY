using System.Collections.Generic;

namespace CaravanSecrets.Game.Board
{
    public static class PrototypeLevels
    {
        public static IReadOnlyList<LevelDefinition> All { get; } = new[]
        {
            Level("desert_01", 4, 3,
                Cells(E(3,1)), C("a",0,1,Direction.Right)),

            Level("desert_02", 5, 4,
                Cells(E(4,1)), C("a",0,1,Direction.Right)),

            Level("desert_03", 5, 5,
                Cells(E(2,4)), C("a",2,0,Direction.Up)),

            // The vertical cart blocks the horizontal route and must leave first.
            Level("desert_04", 5, 5,
                Cells(E(4,2), E(2,4)),
                C("a",0,2,Direction.Right), C("b",2,2,Direction.Up)),

            // A three-cart chain introduces a clear C, B, A release order.
            Level("desert_05", 5, 5,
                Cells(E(4,2), E(2,4), E(4,3)),
                C("a",0,2,Direction.Right), C("b",2,2,Direction.Up), C("c",2,3,Direction.Right)),

            Level("desert_06", 5, 5,
                Cells(S(1,2), E(4,2), R(3,2), E(2,4), E(4,3)),
                C("a",0,2,Direction.Right), C("b",2,2,Direction.Up), C("c",2,3,Direction.Right)),

            Level("desert_07", 6, 6,
                Cells(S(4,0), E(0,0), R(2,2), R(4,3), E(5,2), E(0,3), E(3,5)),
                C("a",5,0,Direction.Left), C("b",0,2,Direction.Right), C("c",5,3,Direction.Left), C("d",3,0,Direction.Up)),

            Level("desert_08", 6, 6,
                Cells(S(1,2), E(5,2), R(4,2), E(2,5), E(5,3), E(4,5)),
                C("a",0,2,Direction.Right), C("b",2,2,Direction.Up), C("c",2,3,Direction.Right), C("d",4,0,Direction.Up))
        };

        private static LevelDefinition Level(string id, int width, int height,
            IReadOnlyDictionary<GridPosition, CellType> cells, params CartDefinition[] carts) =>
            new(id, width, height, cells, carts);

        private static Dictionary<GridPosition, CellType> Cells(params KeyValuePair<GridPosition, CellType>[] entries)
        {
            var result = new Dictionary<GridPosition, CellType>();
            foreach (var entry in entries) result[entry.Key] = entry.Value;
            return result;
        }

        private static KeyValuePair<GridPosition, CellType> E(int x, int y) => new(new GridPosition(x,y), CellType.Exit);
        private static KeyValuePair<GridPosition, CellType> R(int x, int y) => new(new GridPosition(x,y), CellType.Rock);
        private static KeyValuePair<GridPosition, CellType> S(int x, int y) => new(new GridPosition(x,y), CellType.Switch);
        private static CartDefinition C(string id, int x, int y, Direction direction) => new(id, new GridPosition(x,y), direction);
    }
}
