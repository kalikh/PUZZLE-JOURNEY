using System;
using System.Collections.Generic;
using System.Linq;

namespace CaravanSecrets.Game.Board
{
    public sealed class CartState
    {
        public string Id { get; }
        public GridPosition Position { get; internal set; }
        public Direction Direction { get; internal set; }
        public bool HasExited { get; internal set; }

        internal CartState(CartDefinition definition)
        {
            Id = definition.Id;
            Position = definition.Position;
            Direction = definition.Direction;
        }
    }

    public sealed class CargoState
    {
        public string Id { get; }
        public GridPosition Position { get; internal set; }
        public Direction Direction { get; internal set; }
        public CargoType Type { get; }
        public bool IsDelivered { get; internal set; }

        internal CargoState(CargoDefinition definition)
        { Id = definition.Id; Position = definition.Position; Direction = definition.Direction; Type = definition.Type; }
    }

    public sealed class BoardState
    {
        private readonly Dictionary<GridPosition, CellType> _cells;
        private readonly Dictionary<string, CartState> _carts;
        private readonly Dictionary<string, GridPosition> _destinations;
        private readonly Dictionary<string, CargoState> _cargo;
        private readonly Dictionary<GridPosition, CargoType> _cargoDestinations;
        private readonly IReadOnlyList<ObjectiveDefinition> _objectives;
        private readonly Dictionary<string, GateDefinition> _gates;
        private readonly Dictionary<GridPosition, GateDefinition> _gatesByPosition;
        private readonly Dictionary<GridPosition, SwitchDefinition> _switchesByPosition;
        private readonly Dictionary<GridPosition, StorageDefinition> _storageByPosition;
        private readonly Dictionary<GridPosition, DirectionTileDefinition> _directionTilesByPosition;
        private readonly HashSet<string> _activatedSwitchIds = new();
        private readonly HashSet<GridPosition> _clearedExits = new();
        private readonly HashSet<GridPosition> _temporarilyRemovedRocks = new();
        private readonly Dictionary<GridPosition, int> _temporaryStorageBonuses = new();

        public string LevelId { get; }
        public int Width { get; }
        public int Height { get; }
        public int MoveCount { get; internal set; }
        public bool BarriersOpen { get; internal set; }
        public string StoredCartId { get; internal set; }
        public bool HasFailed { get; internal set; }
        public IReadOnlyCollection<CartState> Carts => _carts.Values;
        public IReadOnlyCollection<CargoState> Cargo => _cargo.Values;
        public IReadOnlyCollection<string> ActivatedSwitchIds => _activatedSwitchIds;
        public IReadOnlyCollection<GridPosition> TemporarilyRemovedRocks => _temporarilyRemovedRocks;
        public bool AreAllSwitchesActivated => _switchesByPosition.Count > 0
            ? _switchesByPosition.Values.All(item => _activatedSwitchIds.Contains(item.Id))
            : BarriersOpen;
        public bool IsComplete => EffectiveObjectives.All(IsObjectiveComplete);
        private IEnumerable<ObjectiveDefinition> EffectiveObjectives => _objectives.Count > 0
            ? _objectives
            : new[] { new ObjectiveDefinition("exit_carts", ObjectiveType.ExitAllCarts), new ObjectiveDefinition("deliver_cargo", ObjectiveType.DeliverAllCargo) };

        public BoardState(LevelDefinition level)
        {
            if (level == null) throw new ArgumentNullException(nameof(level));
            LevelId = level.Id;
            Width = level.Width;
            Height = level.Height;
            _cells = new Dictionary<GridPosition, CellType>(level.Cells);
            _carts = level.Carts.ToDictionary(c => c.Id, c => new CartState(c));
            _destinations = new Dictionary<string, GridPosition>(level.Destinations);
            _cargo = level.Cargo.ToDictionary(item => item.Id, item => new CargoState(item));
            _cargoDestinations = new Dictionary<GridPosition, CargoType>(level.CargoDestinations);
            _objectives = level.Objectives;
            _gates = level.Gates.ToDictionary(item => item.Id);
            _gatesByPosition = level.Gates.ToDictionary(item => item.Position);
            _switchesByPosition = level.Switches.ToDictionary(item => item.Position);
            _storageByPosition = level.StorageSlots.ToDictionary(item => item.Position);
            _directionTilesByPosition = level.DirectionTiles.ToDictionary(item => item.Position);
        }

        public CartState GetCart(string id) => _carts.TryGetValue(id, out var cart) ? cart : null;
        public CargoState GetCargo(string id) => _cargo.TryGetValue(id, out var cargo) ? cargo : null;
        public bool IsInside(GridPosition position) => position.X >= 0 && position.Y >= 0 && position.X < Width && position.Y < Height;
        public CellType GetCell(GridPosition position)
        {
            var cell = _cells.TryGetValue(position, out var value) ? value : CellType.Empty;
            if (cell == CellType.Rock && _temporarilyRemovedRocks.Contains(position)) return CellType.Empty;
            if (cell == CellType.Exit && _clearedExits.Contains(position)) return CellType.Empty;
            return BarriersOpen && cell == CellType.Rock ? CellType.Empty : cell;
        }
        public CartState GetCartAt(GridPosition position) => _carts.Values.FirstOrDefault(c => !c.HasExited && c.Position.Equals(position));
        public CargoState GetCargoAt(GridPosition position) => _cargo.Values.FirstOrDefault(c => !c.IsDelivered && c.Position.Equals(position));
        public bool TryGetCargoDestination(GridPosition position, out CargoType type) => _cargoDestinations.TryGetValue(position, out type);
        public bool IsGateOpen(GridPosition position)
        {
            if (!_gatesByPosition.TryGetValue(position, out var gate)) return true;
            if (gate.InitiallyOpen) return true;
            return _switchesByPosition.Values.Any(item => _activatedSwitchIds.Contains(item.Id) && item.GateIds.Contains(gate.Id));
        }
        public bool TryGetGate(GridPosition position, out GateDefinition gate) => _gatesByPosition.TryGetValue(position, out gate);
        public bool TryGetSwitch(GridPosition position, out SwitchDefinition item) => _switchesByPosition.TryGetValue(position, out item);
        public bool TryGetStorage(GridPosition position, out StorageDefinition storage) => _storageByPosition.TryGetValue(position, out storage);
        public bool TryGetDirectionTile(GridPosition position, out DirectionTileDefinition tile) => _directionTilesByPosition.TryGetValue(position, out tile);
        public int GetStorageOccupancy(GridPosition position) =>
            _carts.Values.Count(item => !item.HasExited && item.Position.Equals(position)) +
            _cargo.Values.Count(item => !item.IsDelivered && item.Position.Equals(position));
        public int GetStorageCapacity(GridPosition position) => _storageByPosition.TryGetValue(position, out var storage)
            ? storage.Capacity + (_temporaryStorageBonuses.TryGetValue(position, out var bonus) ? bonus : 0)
            : 0;
        public bool IsTemporaryRockRemovalEligible(GridPosition position) =>
            IsInside(position) && _cells.TryGetValue(position, out var cell) && cell == CellType.Rock &&
            !_temporarilyRemovedRocks.Contains(position);
        public bool IsExtraSpaceEligible(GridPosition position) =>
            _storageByPosition.ContainsKey(position) && !_temporaryStorageBonuses.ContainsKey(position);
        internal bool TemporarilyRemoveRock(GridPosition position) =>
            IsTemporaryRockRemovalEligible(position) && _temporarilyRemovedRocks.Add(position);
        internal bool AddTemporaryStorageSpace(GridPosition position)
        {
            if (!IsExtraSpaceEligible(position)) return false;
            _temporaryStorageBonuses[position] = 1;
            return true;
        }
        public bool CanOccupy(GridPosition position, string movingId)
        {
            if (!IsGateOpen(position)) return false;
            var occupied = (_carts.Values.Any(item => item.Id != movingId && !item.HasExited && item.Position.Equals(position)) ? 1 : 0) +
                           (_cargo.Values.Any(item => item.Id != movingId && !item.IsDelivered && item.Position.Equals(position)) ? 1 : 0);
            return _storageByPosition.ContainsKey(position) ? occupied < GetStorageCapacity(position) : occupied == 0;
        }
        internal void ActivateSwitchAt(GridPosition position)
        {
            if (_switchesByPosition.TryGetValue(position, out var item)) _activatedSwitchIds.Add(item.Id);
        }
        public bool IsMatchingExit(string cartId, GridPosition position) => !_destinations.TryGetValue(cartId, out var destination) || destination.Equals(position);
        public bool TryGetDestination(string cartId, out GridPosition destination) => _destinations.TryGetValue(cartId, out destination);
        public bool HasDestination(string cartId) => _destinations.ContainsKey(cartId);
        public string GetDestinationCartId(GridPosition position) => _destinations.FirstOrDefault(pair => pair.Value.Equals(position)).Key;
        internal void ClearExit(GridPosition position) => _clearedExits.Add(position);

        public bool IsObjectiveComplete(ObjectiveDefinition objective) => objective.Type switch
        {
            ObjectiveType.ExitAllCarts => _carts.Values.All(cart => cart.HasExited),
            ObjectiveType.DeliverAllCargo => _cargo.Values.All(cargo => cargo.IsDelivered),
            ObjectiveType.ActivateAllSwitches => AreAllSwitchesActivated,
            _ => false
        };

        public float GetObjectiveProgress(ObjectiveDefinition objective) => objective.Type switch
        {
            ObjectiveType.ExitAllCarts => Ratio(_carts.Values.Count(cart => cart.HasExited), _carts.Count),
            ObjectiveType.DeliverAllCargo => Ratio(_cargo.Values.Count(cargo => cargo.IsDelivered), _cargo.Count),
            ObjectiveType.ActivateAllSwitches => _switchesByPosition.Count == 0
                ? (BarriersOpen ? 1f : 0f)
                : Ratio(_activatedSwitchIds.Count, _switchesByPosition.Count),
            _ => 0f
        };

        internal BoardSnapshot Snapshot() => new BoardSnapshot(MoveCount, BarriersOpen, StoredCartId, HasFailed, _clearedExits.ToArray(),
            _carts.Values.Select(c => new CartSnapshot(c.Id, c.Position, c.Direction, c.HasExited)).ToArray(),
            _cargo.Values.Select(c => new CargoSnapshot(c.Id, c.Position, c.Direction, c.IsDelivered)).ToArray(),
            _activatedSwitchIds.ToArray(), _temporarilyRemovedRocks.ToArray(),
            _temporaryStorageBonuses.Select(item => new StorageBonusSnapshot(item.Key, item.Value)).ToArray());

        internal void Restore(BoardSnapshot snapshot)
        {
            MoveCount = snapshot.MoveCount;
            BarriersOpen = snapshot.BarriersOpen;
            StoredCartId = snapshot.StoredCartId;
            HasFailed = snapshot.HasFailed;
            _activatedSwitchIds.Clear();
            foreach (var id in snapshot.ActivatedSwitchIds) _activatedSwitchIds.Add(id);
            _temporarilyRemovedRocks.Clear();
            foreach (var position in snapshot.TemporarilyRemovedRocks) _temporarilyRemovedRocks.Add(position);
            _temporaryStorageBonuses.Clear();
            foreach (var bonus in snapshot.StorageBonuses) _temporaryStorageBonuses[bonus.Position] = bonus.Amount;
            _clearedExits.Clear();
            foreach (var exit in snapshot.ClearedExits) _clearedExits.Add(exit);
            foreach (var item in snapshot.Carts)
            {
                var cart = _carts[item.Id];
                cart.Position = item.Position;
                cart.Direction = item.Direction;
                cart.HasExited = item.HasExited;
            }
            foreach (var item in snapshot.Cargo)
            {
                var cargo = _cargo[item.Id];
                cargo.Position = item.Position;
                cargo.Direction = item.Direction;
                cargo.IsDelivered = item.IsDelivered;
            }
        }

        private static float Ratio(int completed, int total) => total == 0 ? 1f : (float)completed / total;
    }

    internal readonly struct CartSnapshot
    {
        public string Id { get; }
        public GridPosition Position { get; }
        public bool HasExited { get; }
        public Direction Direction { get; }
        public CartSnapshot(string id, GridPosition position, Direction direction, bool hasExited)
        { Id = id; Position = position; Direction = direction; HasExited = hasExited; }
    }

    internal readonly struct CargoSnapshot
    {
        public string Id { get; }
        public GridPosition Position { get; }
        public bool IsDelivered { get; }
        public Direction Direction { get; }
        public CargoSnapshot(string id, GridPosition position, Direction direction, bool isDelivered)
        { Id = id; Position = position; Direction = direction; IsDelivered = isDelivered; }
    }

    internal sealed class BoardSnapshot
    {
        public int MoveCount { get; }
        public bool BarriersOpen { get; }
        public CartSnapshot[] Carts { get; }
        public string StoredCartId { get; }
        public bool HasFailed { get; }
        public GridPosition[] ClearedExits { get; }
        public CargoSnapshot[] Cargo { get; }
        public string[] ActivatedSwitchIds { get; }
        public GridPosition[] TemporarilyRemovedRocks { get; }
        public StorageBonusSnapshot[] StorageBonuses { get; }
        public BoardSnapshot(int moveCount, bool barriersOpen, string storedCartId, bool hasFailed, GridPosition[] clearedExits, CartSnapshot[] carts, CargoSnapshot[] cargo, string[] activatedSwitchIds, GridPosition[] temporarilyRemovedRocks, StorageBonusSnapshot[] storageBonuses)
        { MoveCount = moveCount; BarriersOpen = barriersOpen; StoredCartId = storedCartId; HasFailed = hasFailed; ClearedExits = clearedExits; Carts = carts; Cargo = cargo; ActivatedSwitchIds = activatedSwitchIds; TemporarilyRemovedRocks = temporarilyRemovedRocks; StorageBonuses = storageBonuses; }
    }

    internal readonly struct StorageBonusSnapshot
    {
        public GridPosition Position { get; }
        public int Amount { get; }
        public StorageBonusSnapshot(GridPosition position, int amount) { Position = position; Amount = amount; }
    }
}
