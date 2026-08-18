using System.Collections.Generic;

namespace CaravanSecrets.Game.Board
{
    public enum MoveResult { Success, CartNotFound, AlreadyExited, Blocked, OutOfBounds, WrongExit, Stored }
    public enum CargoMoveResult { Success, CargoNotFound, AlreadyDelivered, Blocked, OutOfBounds, WrongDestination, Delivered }

    public sealed class BoardGame
    {
        private readonly LevelDefinition _level;
        private readonly Stack<BoardSnapshot> _history = new();
        private readonly HashSet<string> _usedBoosterIds = new();
        public BoardState State { get; private set; }
        public bool CanUndo => _history.Count > 0;
        public IReadOnlyCollection<string> UsedBoosterIds => _usedBoosterIds;
        public bool HasUsedBooster => _usedBoosterIds.Count > 0;

        public BoardGame(LevelDefinition level)
        {
            _level = level;
            State = new BoardState(level);
        }

        public MoveResult Move(string cartId)
        {
            var cart = State.GetCart(cartId);
            if (cart == null) return MoveResult.CartNotFound;
            if (cart.HasExited) return MoveResult.AlreadyExited;
            if (State.HasFailed) return MoveResult.Blocked;
            if (State.StoredCartId == cartId && !State.BarriersOpen) return MoveResult.Stored;

            var next = cart.Position.Step(cart.Direction);
            if (!State.IsInside(next)) return MoveResult.OutOfBounds;
            if (State.GetCell(next) == CellType.Rock || !State.CanOccupy(next, cartId)) return MoveResult.Blocked;

            _history.Push(State.Snapshot());
            cart.Position = next;
            State.MoveCount++;
            if (State.StoredCartId == cartId) State.StoredCartId = null;
            if (State.GetCell(next) == CellType.Storage && !State.TryGetStorage(next, out _)) State.StoredCartId = cartId;
            if (State.GetCell(next) == CellType.Switch) State.BarriersOpen = true;
            State.ActivateSwitchAt(next);
            if (State.TryGetDirectionTile(next, out var directionTile)) cart.Direction = directionTile.Direction;
            if (State.GetCell(next) == CellType.Exit && !State.IsMatchingExit(cartId, next))
            {
                State.HasFailed = true;
                return MoveResult.WrongExit;
            }
            if (State.GetCell(next) == CellType.Exit)
            {
                cart.HasExited = true;
                if (State.HasDestination(cartId)) State.ClearExit(next);
            }
            return MoveResult.Success;
        }

        public CargoMoveResult MoveCargo(string cargoId)
        {
            var cargo = State.GetCargo(cargoId);
            if (cargo == null) return CargoMoveResult.CargoNotFound;
            if (cargo.IsDelivered) return CargoMoveResult.AlreadyDelivered;
            if (State.HasFailed) return CargoMoveResult.Blocked;
            var next = cargo.Position.Step(cargo.Direction);
            if (!State.IsInside(next)) return CargoMoveResult.OutOfBounds;
            if (State.GetCell(next) == CellType.Rock || !State.CanOccupy(next, cargoId))
                return CargoMoveResult.Blocked;

            _history.Push(State.Snapshot());
            cargo.Position = next;
            State.MoveCount++;
            if (State.GetCell(next) == CellType.Switch) State.BarriersOpen = true;
            State.ActivateSwitchAt(next);
            if (State.TryGetDirectionTile(next, out var directionTile)) cargo.Direction = directionTile.Direction;
            if (!State.TryGetCargoDestination(next, out var destinationType)) return CargoMoveResult.Success;
            if (destinationType != cargo.Type)
            {
                State.HasFailed = true;
                return CargoMoveResult.WrongDestination;
            }
            cargo.IsDelivered = true;
            return CargoMoveResult.Delivered;
        }

        public bool MoveObject(string objectId)
        {
            if (State.GetCart(objectId) != null)
            {
                var result = Move(objectId);
                return result == MoveResult.Success || result == MoveResult.WrongExit;
            }
            if (State.GetCargo(objectId) != null)
            {
                var result = MoveCargo(objectId);
                return result == CargoMoveResult.Success || result == CargoMoveResult.Delivered || result == CargoMoveResult.WrongDestination;
            }
            return false;
        }

        public bool Undo()
        {
            if (_history.Count == 0) return false;
            State.Restore(_history.Pop());
            return true;
        }

        public bool TryRemoveTemporaryRock(GridPosition position, string boosterId)
        {
            if (!State.IsTemporaryRockRemovalEligible(position)) return false;
            _history.Push(State.Snapshot());
            if (!State.TemporarilyRemoveRock(position)) { _history.Pop(); return false; }
            RecordBoosterUse(boosterId);
            return true;
        }

        public bool TryAddTemporaryStorageSpace(GridPosition position, string boosterId)
        {
            if (!State.IsExtraSpaceEligible(position)) return false;
            _history.Push(State.Snapshot());
            if (!State.AddTemporaryStorageSpace(position)) { _history.Pop(); return false; }
            RecordBoosterUse(boosterId);
            return true;
        }

        public void RecordBoosterUse(string boosterId)
        {
            if (!string.IsNullOrWhiteSpace(boosterId)) _usedBoosterIds.Add(boosterId);
        }

        internal BoardGame Fork()
        {
            var copy = new BoardGame(_level);
            copy.State.Restore(State.Snapshot());
            foreach (var id in _usedBoosterIds) copy._usedBoosterIds.Add(id);
            return copy;
        }

        public void Restart()
        {
            State = new BoardState(_level);
            _history.Clear();
            _usedBoosterIds.Clear();
        }
    }
}
