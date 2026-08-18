using System.Collections.Generic;
using CaravanSecrets.Game.Board;
using NUnit.Framework;
using CaravanSecrets.Data.Levels;
using UnityEngine;

namespace CaravanSecrets.Game.Tests
{
    public sealed class Stage4GateBTests
    {
        [Test]
        public void LinkedSwitch_OpensOnlyItsGate_AndUndoClosesIt()
        {
            var level = MechanismLevel(
                gates: new[] { new GateDefinition("gate", new GridPosition(2, 1)) },
                switches: new[] { new SwitchDefinition("switch", new GridPosition(1, 0), new[] { "gate" }) });
            var game = new BoardGame(level);

            Assert.That(game.Move("target"), Is.EqualTo(MoveResult.Blocked));
            Assert.That(game.Move("key"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.State.IsGateOpen(new GridPosition(2, 1)), Is.True);
            Assert.That(game.Move("target"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Undo(), Is.True);
            Assert.That(game.Undo(), Is.True);
            Assert.That(game.State.IsGateOpen(new GridPosition(2, 1)), Is.False);
            Assert.That(game.State.ActivatedSwitchIds, Is.Empty);
        }

        [Test]
        public void SeparateSwitches_DoNotOpenUnlinkedGate()
        {
            var level = MechanismLevel(
                gates: new[]
                {
                    new GateDefinition("gate_a", new GridPosition(2, 1)),
                    new GateDefinition("gate_b", new GridPosition(3, 1))
                },
                switches: new[] { new SwitchDefinition("switch", new GridPosition(1, 0), new[] { "gate_a" }) });
            var game = new BoardGame(level);
            game.Move("key");
            Assert.That(game.State.IsGateOpen(new GridPosition(2, 1)), Is.True);
            Assert.That(game.State.IsGateOpen(new GridPosition(3, 1)), Is.False);
        }

        [Test]
        public void Storage_AllowsObjectsUpToCapacity_ThenReleasesThem()
        {
            var storage = new[] { new StorageDefinition("bay", new GridPosition(1, 0), 2) };
            var level = new LevelDefinition("storage-capacity", 3, 1, new Dictionary<GridPosition, CellType>(), new[]
            {
                new CartDefinition("a", new GridPosition(0, 0), Direction.Right),
                new CartDefinition("b", new GridPosition(2, 0), Direction.Left)
            }, null, "market", 15, storageSlots: storage,
                objectives: new[] { new ObjectiveDefinition("switch", ObjectiveType.ActivateAllSwitches) });
            var game = new BoardGame(level);

            Assert.That(game.Move("a"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Move("b"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.State.GetStorageOccupancy(new GridPosition(1, 0)), Is.EqualTo(2));
            Assert.That(game.Move("a"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.State.GetStorageOccupancy(new GridPosition(1, 0)), Is.EqualTo(1));
            Assert.That(game.Undo(), Is.True);
            Assert.That(game.State.GetStorageOccupancy(new GridPosition(1, 0)), Is.EqualTo(2));
        }

        [Test]
        public void DirectionTile_ChangesLogicalDirection_AndUndoRestoresIt()
        {
            var level = new LevelDefinition("turn", 3, 3,
                new Dictionary<GridPosition, CellType> { [new GridPosition(1, 2)] = CellType.Exit },
                new[] { new CartDefinition("cart", new GridPosition(0, 0), Direction.Right) }, null,
                "city", 21, directionTiles: new[]
                {
                    new DirectionTileDefinition("turn_up", new GridPosition(1, 0), Direction.Up)
                });
            var game = new BoardGame(level);
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.State.GetCart("cart").Direction, Is.EqualTo(Direction.Up));
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Undo(), Is.True);
            Assert.That(game.State.GetCart("cart").Position, Is.EqualTo(new GridPosition(1, 0)));
            Assert.That(game.State.GetCart("cart").Direction, Is.EqualTo(Direction.Up));
            Assert.That(game.Undo(), Is.True);
            Assert.That(game.State.GetCart("cart").Direction, Is.EqualTo(Direction.Right));
        }

        [Test]
        public void Cargo_ActivatesLinkedSwitchAndUsesDirectionTile()
        {
            var level = new LevelDefinition("cargo-mechanisms", 4, 3, new Dictionary<GridPosition, CellType>(),
                new CartDefinition[0], null, "city", 22,
                cargo: new[] { new CargoDefinition("cargo", new GridPosition(0, 0), Direction.Right, CargoType.Tools) },
                cargoDestinations: new Dictionary<GridPosition, CargoType> { [new GridPosition(2, 2)] = CargoType.Tools },
                objectives: new[] { new ObjectiveDefinition("cargo", ObjectiveType.DeliverAllCargo) },
                gates: new[] { new GateDefinition("gate", new GridPosition(2, 1)) },
                switches: new[] { new SwitchDefinition("switch", new GridPosition(1, 0), new[] { "gate" }) },
                directionTiles: new[] { new DirectionTileDefinition("up", new GridPosition(2, 0), Direction.Up) });
            var game = new BoardGame(level);
            Assert.That(game.MoveCargo("cargo"), Is.EqualTo(CargoMoveResult.Success));
            Assert.That(game.MoveCargo("cargo"), Is.EqualTo(CargoMoveResult.Success));
            Assert.That(game.State.IsGateOpen(new GridPosition(2, 1)), Is.True);
            Assert.That(game.State.GetCargo("cargo").Direction, Is.EqualTo(Direction.Up));
            Assert.That(game.MoveCargo("cargo"), Is.EqualTo(CargoMoveResult.Success));
            Assert.That(game.MoveCargo("cargo"), Is.EqualTo(CargoMoveResult.Delivered));
            Assert.That(game.State.IsComplete, Is.True);
        }

        [Test]
        public void Validator_RejectsBrokenLinksCapacityAndMechanismOverlap()
        {
            var level = new LevelDefinition("bad-mechanisms", 3, 2,
                new Dictionary<GridPosition, CellType> { [new GridPosition(2, 1)] = CellType.Exit },
                new[] { new CartDefinition("cart", new GridPosition(0, 1), Direction.Right) }, null,
                "city", 23,
                gates: new[] { new GateDefinition("gate", new GridPosition(1, 1)) },
                switches: new[] { new SwitchDefinition("switch", new GridPosition(1, 0), new[] { "missing" }) },
                storageSlots: new[] { new StorageDefinition("storage", new GridPosition(1, 0), 0) });
            var issues = LevelValidator.Validate(level);
            Assert.That(issues, Has.Some.Contains("missing gate"));
            Assert.That(issues, Has.Some.Contains("capacity"));
            Assert.That(issues, Has.Some.Contains("Unsupported mechanism overlap"));
        }

        [Test]
        public void Solver_HandlesLinkedGateAndDirectionState()
        {
            var result = LevelSolver.Solve(MechanismLevel(
                gates: new[] { new GateDefinition("gate", new GridPosition(2, 1)) },
                switches: new[] { new SwitchDefinition("switch", new GridPosition(1, 0), new[] { "gate" }) }));
            Assert.That(result.Status, Is.EqualTo(SolverStatus.Solved));
            Assert.That(result.Moves, Does.Contain("key"));
            Assert.That(result.Moves, Does.Contain("target"));
        }

        [Test]
        public void LevelAsset_ConvertsGateBMechanisms()
        {
            var asset = ScriptableObject.CreateInstance<LevelAsset>();
            asset.Configure("schema-b", 4, 2,
                new[] { new CellEntry(new Vector2Int(3, 1), CellType.Exit) },
                new[] { new CartEntry("cart", new Vector2Int(0, 1), Direction.Right) });
            asset.ConfigureMechanisms(
                new[] { new GateEntry("gate", new Vector2Int(2, 1)) },
                new[] { new SwitchEntry("switch", new Vector2Int(1, 0), new[] { "gate" }) },
                new[] { new StorageEntry("storage", new Vector2Int(2, 0), 2) },
                new[] { new DirectionTileEntry("turn", new Vector2Int(1, 1), Direction.Up) });
            var definition = asset.ToDefinition();
            Assert.That(definition.Gates[0].Id, Is.EqualTo("gate"));
            Assert.That(definition.Switches[0].GateIds, Is.EqualTo(new[] { "gate" }));
            Assert.That(definition.StorageSlots[0].Capacity, Is.EqualTo(2));
            Assert.That(definition.DirectionTiles[0].Direction, Is.EqualTo(Direction.Up));
            Object.DestroyImmediate(asset);
        }

        private static LevelDefinition MechanismLevel(IReadOnlyList<GateDefinition> gates, IReadOnlyList<SwitchDefinition> switches) =>
            new("mechanisms", 5, 2,
                new Dictionary<GridPosition, CellType>
                {
                    [new GridPosition(4, 0)] = CellType.Exit, [new GridPosition(4, 1)] = CellType.Exit
                },
                new[]
                {
                    new CartDefinition("key", new GridPosition(0, 0), Direction.Right),
                    new CartDefinition("target", new GridPosition(1, 1), Direction.Right)
                }, null, "city", 20, gates: gates, switches: switches);
    }
}
