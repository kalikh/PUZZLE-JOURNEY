using System.Collections.Generic;
using System.Linq;

namespace CaravanSecrets.Game.Board
{
    public static class LevelValidator
    {
        public static IReadOnlyList<string> Validate(LevelDefinition level)
        {
            var issues = new List<string>();
            if (level == null) { issues.Add("Level is null."); return issues; }
            if (string.IsNullOrWhiteSpace(level.Id)) issues.Add("Level ID is required.");
            if (string.IsNullOrWhiteSpace(level.RegionId)) issues.Add("Region ID is required.");
            if (level.LevelNumber < 0) issues.Add("Level number cannot be negative.");
            if (level.RecommendedMoves < 0) issues.Add("Recommended moves cannot be negative.");
            if (level.RewardCoins < 0) issues.Add("Reward coins cannot be negative.");
            if (level.Width < 2 || level.Height < 1) issues.Add("Board size is invalid.");
            if (level.Carts.Count > 0 && level.Cells.Count(pair => pair.Value == CellType.Exit) == 0)
                issues.Add("At least one cart exit is required.");

            var ids = new HashSet<string>();
            foreach (var cart in level.Carts)
            {
                if (string.IsNullOrWhiteSpace(cart.Id)) issues.Add("Cart ID is required.");
                else if (!ids.Add(cart.Id)) issues.Add($"Duplicate cart ID: {cart.Id}.");
                if (!Inside(level, cart.Position)) issues.Add($"Cart {cart.Id} is outside the board.");
                if (level.Cells.TryGetValue(cart.Position, out var cell) && cell != CellType.Empty)
                    issues.Add($"Cart {cart.Id} overlaps {cell}.");
            }

            foreach (var cargo in level.Cargo)
            {
                if (string.IsNullOrWhiteSpace(cargo.Id)) issues.Add("Cargo ID is required.");
                else if (!ids.Add(cargo.Id)) issues.Add($"Duplicate object ID: {cargo.Id}.");
                if (!Inside(level, cargo.Position)) issues.Add($"Cargo {cargo.Id} is outside the board.");
                if (!System.Enum.IsDefined(typeof(Direction), cargo.Direction)) issues.Add($"Cargo {cargo.Id} has invalid direction.");
                if (!System.Enum.IsDefined(typeof(CargoType), cargo.Type)) issues.Add($"Cargo {cargo.Id} has unsupported type.");
                if (level.Cells.TryGetValue(cargo.Position, out var cell) && cell != CellType.Empty)
                    issues.Add($"Cargo {cargo.Id} overlaps {cell}.");
                if (level.Carts.Any(cart => cart.Position.Equals(cargo.Position))) issues.Add($"Cargo {cargo.Id} overlaps a cart.");
            }

            foreach (var cell in level.Cells)
            {
                if (!Inside(level, cell.Key)) issues.Add($"Cell {cell.Key} is outside the board.");
                if (!System.Enum.IsDefined(typeof(CellType), cell.Value)) issues.Add($"Cell {cell.Key} has unsupported type {cell.Value}.");
            }
            foreach (var destination in level.Destinations)
            {
                if (!ids.Contains(destination.Key)) issues.Add($"Destination references missing cart: {destination.Key}.");
                if (!Inside(level, destination.Value)) issues.Add($"Destination for {destination.Key} is outside the board.");
                else if (!level.Cells.TryGetValue(destination.Value, out var cell) || cell != CellType.Exit)
                    issues.Add($"Destination for {destination.Key} is not an exit.");
            }
            foreach (var destination in level.CargoDestinations)
            {
                if (!Inside(level, destination.Key)) issues.Add($"Cargo destination {destination.Key} is outside the board.");
                if (!System.Enum.IsDefined(typeof(CargoType), destination.Value)) issues.Add($"Cargo destination {destination.Key} has unsupported type.");
            }
            if (level.Cargo.Count > 0)
            {
                foreach (var type in level.Cargo.Select(item => item.Type).Distinct())
                    if (!level.CargoDestinations.Values.Contains(type)) issues.Add($"Cargo type {type} has no matching destination.");
            }
            var objectiveIds = new HashSet<string>();
            foreach (var objective in level.Objectives)
            {
                if (string.IsNullOrWhiteSpace(objective.Id)) issues.Add("Objective ID is required.");
                else if (!objectiveIds.Add(objective.Id)) issues.Add($"Duplicate objective ID: {objective.Id}.");
                if (!System.Enum.IsDefined(typeof(ObjectiveType), objective.Type)) issues.Add($"Objective {objective.Id} has unsupported type.");
                if (objective.Type == ObjectiveType.DeliverAllCargo && level.Cargo.Count == 0)
                    issues.Add($"Objective {objective.Id} requires cargo.");
            }
            var gateIds = new HashSet<string>();
            foreach (var gate in level.Gates)
            {
                if (string.IsNullOrWhiteSpace(gate.Id)) issues.Add("Gate ID is required.");
                else if (!gateIds.Add(gate.Id) || !ids.Add(gate.Id)) issues.Add($"Duplicate object ID: {gate.Id}.");
                if (!Inside(level, gate.Position)) issues.Add($"Gate {gate.Id} is outside the board.");
            }
            var switchIds = new HashSet<string>();
            foreach (var item in level.Switches)
            {
                if (string.IsNullOrWhiteSpace(item.Id)) issues.Add("Switch ID is required.");
                else if (!switchIds.Add(item.Id) || !ids.Add(item.Id)) issues.Add($"Duplicate object ID: {item.Id}.");
                if (!Inside(level, item.Position)) issues.Add($"Switch {item.Id} is outside the board.");
                if (item.GateIds.Count == 0) issues.Add($"Switch {item.Id} does not link to a gate.");
                foreach (var gateId in item.GateIds)
                    if (!level.Gates.Any(gate => gate.Id == gateId)) issues.Add($"Switch {item.Id} references missing gate {gateId}.");
            }
            foreach (var storage in level.StorageSlots)
            {
                if (string.IsNullOrWhiteSpace(storage.Id)) issues.Add("Storage ID is required.");
                else if (!ids.Add(storage.Id)) issues.Add($"Duplicate object ID: {storage.Id}.");
                if (!Inside(level, storage.Position)) issues.Add($"Storage {storage.Id} is outside the board.");
                if (storage.Capacity < 1) issues.Add($"Storage {storage.Id} capacity must be at least 1.");
            }
            foreach (var tile in level.DirectionTiles)
            {
                if (string.IsNullOrWhiteSpace(tile.Id)) issues.Add("Direction tile ID is required.");
                else if (!ids.Add(tile.Id)) issues.Add($"Duplicate object ID: {tile.Id}.");
                if (!Inside(level, tile.Position)) issues.Add($"Direction tile {tile.Id} is outside the board.");
                if (!System.Enum.IsDefined(typeof(Direction), tile.Direction)) issues.Add($"Direction tile {tile.Id} has invalid direction.");
            }
            var mechanismPositions = level.Gates.Select(item => (item.Position, item.Id))
                .Concat(level.Switches.Select(item => (item.Position, item.Id)))
                .Concat(level.StorageSlots.Select(item => (item.Position, item.Id)))
                .Concat(level.DirectionTiles.Select(item => (item.Position, item.Id)));
            foreach (var group in mechanismPositions.GroupBy(item => item.Position).Where(group => group.Count() > 1))
                issues.Add($"Unsupported mechanism overlap at {group.Key}: {string.Join(", ", group.Select(item => item.Id))}.");
            return issues;
        }

        public static IReadOnlyList<string> ValidateCollection(IEnumerable<LevelDefinition> levels)
        {
            var issues = new List<string>();
            if (levels == null) { issues.Add("Level collection is null."); return issues; }
            var materialized = levels.Where(level => level != null).ToArray();
            foreach (var group in materialized.Where(level => level.LevelNumber > 0)
                         .GroupBy(level => (level.RegionId, level.LevelNumber)).Where(group => group.Count() > 1))
                issues.Add($"Duplicate level number {group.Key.LevelNumber} in region {group.Key.RegionId}.");
            foreach (var level in materialized)
                foreach (var issue in Validate(level)) issues.Add($"{level.Id}: {issue}");
            return issues;
        }

        private static bool Inside(LevelDefinition level, GridPosition position) =>
            position.X >= 0 && position.Y >= 0 && position.X < level.Width && position.Y < level.Height;
    }
}
