using System;
using System.Collections.Generic;
using CaravanSecrets.Game.Board;
using UnityEngine;

namespace CaravanSecrets.Data.Levels
{
    [CreateAssetMenu(menuName = "Caravan Secrets/Level", fileName = "Level")]
    public sealed class LevelAsset : ScriptableObject
    {
        [SerializeField] private string levelId = "desert_01";
        [SerializeField] private string regionId = "desert";
        [SerializeField, Min(0)] private int levelNumber;
        [SerializeField, Min(0)] private int recommendedMoves;
        [SerializeField, Min(0)] private int rewardCoins;
        [SerializeField, Min(2)] private int width = 4;
        [SerializeField, Min(1)] private int height = 3;
        [SerializeField] private List<CellEntry> cells = new();
        [SerializeField] private List<CartEntry> carts = new();
        [SerializeField] private List<DestinationEntry> destinations = new();
        [SerializeField] private List<CargoEntry> cargo = new();
        [SerializeField] private List<CargoDestinationEntry> cargoDestinations = new();
        [SerializeField] private List<ObjectiveEntry> objectives = new();
        [SerializeField] private List<GateEntry> gates = new();
        [SerializeField] private List<SwitchEntry> linkedSwitches = new();
        [SerializeField] private List<StorageEntry> storageSlots = new();
        [SerializeField] private List<DirectionTileEntry> directionTiles = new();

        public string LevelId => levelId;
        public string RegionId => regionId;
        public int LevelNumber => levelNumber;
        public int RecommendedMoves => recommendedMoves;
        public int RewardCoins => rewardCoins;
        public IReadOnlyList<CellEntry> Cells => cells;
        public IReadOnlyList<CartEntry> Carts => carts;
        public IReadOnlyList<DestinationEntry> Destinations => destinations;
        public IReadOnlyList<CargoEntry> Cargo => cargo;
        public IReadOnlyList<CargoDestinationEntry> CargoDestinations => cargoDestinations;
        public IReadOnlyList<ObjectiveEntry> Objectives => objectives;
        public IReadOnlyList<GateEntry> Gates => gates;
        public IReadOnlyList<SwitchEntry> LinkedSwitches => linkedSwitches;
        public IReadOnlyList<StorageEntry> StorageSlots => storageSlots;
        public IReadOnlyList<DirectionTileEntry> DirectionTiles => directionTiles;

        public void Configure(string id, int boardWidth, int boardHeight, IEnumerable<CellEntry> cellData, IEnumerable<CartEntry> cartData,
            IEnumerable<DestinationEntry> destinationData = null)
        {
            levelId = id;
            width = boardWidth;
            height = boardHeight;
            cells = new List<CellEntry>(cellData);
            carts = new List<CartEntry>(cartData);
            destinations = destinationData == null ? new List<DestinationEntry>() : new List<DestinationEntry>(destinationData);
        }

        public void ConfigureMetadata(string region, int number, int targetMoves, int coins)
        {
            regionId = region ?? string.Empty;
            levelNumber = number;
            recommendedMoves = targetMoves;
            rewardCoins = coins;
        }

        public void ConfigureExpansion(IEnumerable<CargoEntry> cargoData,
            IEnumerable<CargoDestinationEntry> cargoDestinationData, IEnumerable<ObjectiveEntry> objectiveData)
        {
            cargo = cargoData == null ? new List<CargoEntry>() : new List<CargoEntry>(cargoData);
            cargoDestinations = cargoDestinationData == null ? new List<CargoDestinationEntry>() : new List<CargoDestinationEntry>(cargoDestinationData);
            objectives = objectiveData == null ? new List<ObjectiveEntry>() : new List<ObjectiveEntry>(objectiveData);
        }

        public void ConfigureMechanisms(IEnumerable<GateEntry> gateData, IEnumerable<SwitchEntry> switchData,
            IEnumerable<StorageEntry> storageData, IEnumerable<DirectionTileEntry> directionTileData)
        {
            gates = gateData == null ? new List<GateEntry>() : new List<GateEntry>(gateData);
            linkedSwitches = switchData == null ? new List<SwitchEntry>() : new List<SwitchEntry>(switchData);
            storageSlots = storageData == null ? new List<StorageEntry>() : new List<StorageEntry>(storageData);
            directionTiles = directionTileData == null ? new List<DirectionTileEntry>() : new List<DirectionTileEntry>(directionTileData);
        }

        public LevelDefinition ToDefinition()
        {
            var boardCells = new Dictionary<GridPosition, CellType>();
            foreach (var cell in cells) boardCells[new GridPosition(cell.Position.x, cell.Position.y)] = cell.Type;
            var boardCarts = new List<CartDefinition>();
            foreach (var cart in carts)
                boardCarts.Add(new CartDefinition(cart.Id, new GridPosition(cart.Position.x, cart.Position.y), cart.Direction));
            var boardDestinations = new Dictionary<string, GridPosition>();
            foreach (var destination in destinations)
                boardDestinations[destination.CartId] = new GridPosition(destination.Position.x, destination.Position.y);
            var boardCargo = new List<CargoDefinition>();
            foreach (var item in cargo)
                boardCargo.Add(new CargoDefinition(item.Id, new GridPosition(item.Position.x, item.Position.y), item.Direction, item.Type));
            var boardCargoDestinations = new Dictionary<GridPosition, CargoType>();
            foreach (var destination in cargoDestinations)
                boardCargoDestinations[new GridPosition(destination.Position.x, destination.Position.y)] = destination.Type;
            var boardObjectives = new List<ObjectiveDefinition>();
            foreach (var objective in objectives) boardObjectives.Add(new ObjectiveDefinition(objective.Id, objective.Type));
            var boardGates = new List<GateDefinition>();
            foreach (var gate in gates) boardGates.Add(new GateDefinition(gate.Id, new GridPosition(gate.Position.x, gate.Position.y), gate.InitiallyOpen));
            var boardSwitches = new List<SwitchDefinition>();
            foreach (var item in linkedSwitches)
                boardSwitches.Add(new SwitchDefinition(item.Id, new GridPosition(item.Position.x, item.Position.y), item.GateIds));
            var boardStorage = new List<StorageDefinition>();
            foreach (var storage in storageSlots)
                boardStorage.Add(new StorageDefinition(storage.Id, new GridPosition(storage.Position.x, storage.Position.y), storage.Capacity));
            var boardDirectionTiles = new List<DirectionTileDefinition>();
            foreach (var tile in directionTiles)
                boardDirectionTiles.Add(new DirectionTileDefinition(tile.Id, new GridPosition(tile.Position.x, tile.Position.y), tile.Direction));
            return new LevelDefinition(levelId, width, height, boardCells, boardCarts, boardDestinations,
                regionId, levelNumber, recommendedMoves, rewardCoins, boardCargo, boardCargoDestinations, boardObjectives,
                boardGates, boardSwitches, boardStorage, boardDirectionTiles);
        }
    }

    [Serializable]
    public struct CellEntry
    {
        public Vector2Int Position;
        public CellType Type;
        public CellEntry(Vector2Int position, CellType type) { Position = position; Type = type; }
    }

    [Serializable]
    public struct CartEntry
    {
        public string Id;
        public Vector2Int Position;
        public Direction Direction;
        public CartEntry(string id, Vector2Int position, Direction direction) { Id = id; Position = position; Direction = direction; }
    }

    [Serializable]
    public struct DestinationEntry
    {
        public string CartId;
        public Vector2Int Position;
        public DestinationEntry(string cartId, Vector2Int position) { CartId = cartId; Position = position; }
    }

    [Serializable]
    public struct CargoEntry
    {
        public string Id;
        public Vector2Int Position;
        public Direction Direction;
        public CargoType Type;
        public CargoEntry(string id, Vector2Int position, Direction direction, CargoType type)
        { Id = id; Position = position; Direction = direction; Type = type; }
    }

    [Serializable]
    public struct CargoDestinationEntry
    {
        public Vector2Int Position;
        public CargoType Type;
        public CargoDestinationEntry(Vector2Int position, CargoType type) { Position = position; Type = type; }
    }

    [Serializable]
    public struct ObjectiveEntry
    {
        public string Id;
        public ObjectiveType Type;
        public ObjectiveEntry(string id, ObjectiveType type) { Id = id; Type = type; }
    }

    [Serializable]
    public struct GateEntry
    {
        public string Id;
        public Vector2Int Position;
        public bool InitiallyOpen;
        public GateEntry(string id, Vector2Int position, bool initiallyOpen = false)
        { Id = id; Position = position; InitiallyOpen = initiallyOpen; }
    }

    [Serializable]
    public struct SwitchEntry
    {
        public string Id;
        public Vector2Int Position;
        public List<string> GateIds;
        public SwitchEntry(string id, Vector2Int position, IEnumerable<string> gateIds)
        { Id = id; Position = position; GateIds = gateIds == null ? new List<string>() : new List<string>(gateIds); }
    }

    [Serializable]
    public struct StorageEntry
    {
        public string Id;
        public Vector2Int Position;
        public int Capacity;
        public StorageEntry(string id, Vector2Int position, int capacity)
        { Id = id; Position = position; Capacity = capacity; }
    }

    [Serializable]
    public struct DirectionTileEntry
    {
        public string Id;
        public Vector2Int Position;
        public Direction Direction;
        public DirectionTileEntry(string id, Vector2Int position, Direction direction)
        { Id = id; Position = position; Direction = direction; }
    }
}
