using System;
using System.Collections.Generic;

namespace CaravanSecrets.Game.Board
{
    [Serializable]
    public sealed class CartDefinition
    {
        public string Id;
        public GridPosition Position;
        public Direction Direction;

        public CartDefinition(string id, GridPosition position, Direction direction)
        {
            Id = id;
            Position = position;
            Direction = direction;
        }
    }

    [Serializable]
    public sealed class CargoDefinition
    {
        public string Id { get; }
        public GridPosition Position { get; }
        public Direction Direction { get; }
        public CargoType Type { get; }

        public CargoDefinition(string id, GridPosition position, Direction direction, CargoType type)
        { Id = id; Position = position; Direction = direction; Type = type; }
    }

    [Serializable]
    public sealed class ObjectiveDefinition
    {
        public string Id { get; }
        public ObjectiveType Type { get; }

        public ObjectiveDefinition(string id, ObjectiveType type) { Id = id; Type = type; }
    }

    [Serializable]
    public sealed class GateDefinition
    {
        public string Id { get; }
        public GridPosition Position { get; }
        public bool InitiallyOpen { get; }
        public GateDefinition(string id, GridPosition position, bool initiallyOpen = false)
        { Id = id; Position = position; InitiallyOpen = initiallyOpen; }
    }

    [Serializable]
    public sealed class SwitchDefinition
    {
        public string Id { get; }
        public GridPosition Position { get; }
        public IReadOnlyList<string> GateIds { get; }
        public SwitchDefinition(string id, GridPosition position, IReadOnlyList<string> gateIds)
        { Id = id; Position = position; GateIds = gateIds ?? Array.Empty<string>(); }
    }

    [Serializable]
    public sealed class StorageDefinition
    {
        public string Id { get; }
        public GridPosition Position { get; }
        public int Capacity { get; }
        public StorageDefinition(string id, GridPosition position, int capacity = 1)
        { Id = id; Position = position; Capacity = capacity; }
    }

    [Serializable]
    public sealed class DirectionTileDefinition
    {
        public string Id { get; }
        public GridPosition Position { get; }
        public Direction Direction { get; }
        public DirectionTileDefinition(string id, GridPosition position, Direction direction)
        { Id = id; Position = position; Direction = direction; }
    }

    [Serializable]
    public sealed class LevelDefinition
    {
        public string Id { get; }
        public int Width { get; }
        public int Height { get; }
        public IReadOnlyDictionary<GridPosition, CellType> Cells { get; }
        public IReadOnlyList<CartDefinition> Carts { get; }
        public IReadOnlyDictionary<string, GridPosition> Destinations { get; }
        public string RegionId { get; }
        public int LevelNumber { get; }
        public int RecommendedMoves { get; }
        public int RewardCoins { get; }
        public IReadOnlyList<CargoDefinition> Cargo { get; }
        public IReadOnlyDictionary<GridPosition, CargoType> CargoDestinations { get; }
        public IReadOnlyList<ObjectiveDefinition> Objectives { get; }
        public IReadOnlyList<GateDefinition> Gates { get; }
        public IReadOnlyList<SwitchDefinition> Switches { get; }
        public IReadOnlyList<StorageDefinition> StorageSlots { get; }
        public IReadOnlyList<DirectionTileDefinition> DirectionTiles { get; }

        public LevelDefinition(string id, int width, int height,
            IReadOnlyDictionary<GridPosition, CellType> cells,
            IReadOnlyList<CartDefinition> carts,
            IReadOnlyDictionary<string, GridPosition> destinations = null,
            string regionId = "desert",
            int levelNumber = 0,
            int recommendedMoves = 0,
            int rewardCoins = 0,
            IReadOnlyList<CargoDefinition> cargo = null,
            IReadOnlyDictionary<GridPosition, CargoType> cargoDestinations = null,
            IReadOnlyList<ObjectiveDefinition> objectives = null,
            IReadOnlyList<GateDefinition> gates = null,
            IReadOnlyList<SwitchDefinition> switches = null,
            IReadOnlyList<StorageDefinition> storageSlots = null,
            IReadOnlyList<DirectionTileDefinition> directionTiles = null)
        {
            Id = id;
            Width = width;
            Height = height;
            Cells = cells;
            Carts = carts;
            Destinations = destinations ?? new Dictionary<string, GridPosition>();
            RegionId = regionId ?? string.Empty;
            LevelNumber = levelNumber;
            RecommendedMoves = recommendedMoves;
            RewardCoins = rewardCoins;
            Cargo = cargo ?? Array.Empty<CargoDefinition>();
            CargoDestinations = cargoDestinations ?? new Dictionary<GridPosition, CargoType>();
            Objectives = objectives ?? Array.Empty<ObjectiveDefinition>();
            Gates = gates ?? Array.Empty<GateDefinition>();
            Switches = switches ?? Array.Empty<SwitchDefinition>();
            StorageSlots = storageSlots ?? Array.Empty<StorageDefinition>();
            DirectionTiles = directionTiles ?? Array.Empty<DirectionTileDefinition>();
        }
    }
}
