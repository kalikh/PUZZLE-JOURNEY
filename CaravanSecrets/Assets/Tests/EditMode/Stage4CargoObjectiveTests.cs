using System.Collections.Generic;
using CaravanSecrets.Game.Board;
using NUnit.Framework;

namespace CaravanSecrets.Game.Tests
{
    public sealed class Stage4CargoObjectiveTests
    {
        [Test]
        public void Cargo_DeliversOnlyToMatchingTypedDestination()
        {
            var game = new BoardGame(CargoLevel(CargoType.Spices, CargoType.Spices));
            Assert.That(game.MoveCargo("cargo"), Is.EqualTo(CargoMoveResult.Success));
            Assert.That(game.MoveCargo("cargo"), Is.EqualTo(CargoMoveResult.Delivered));
            Assert.That(game.State.GetCargo("cargo").IsDelivered, Is.True);
            Assert.That(game.State.IsComplete, Is.True);
        }

        [Test]
        public void WrongCargoDestination_Fails_AndUndoRestoresFullState()
        {
            var game = new BoardGame(CargoLevel(CargoType.Spices, CargoType.Water));
            Assert.That(game.MoveCargo("cargo"), Is.EqualTo(CargoMoveResult.Success));
            Assert.That(game.MoveCargo("cargo"), Is.EqualTo(CargoMoveResult.WrongDestination));
            Assert.That(game.State.HasFailed, Is.True);
            Assert.That(game.Undo(), Is.True);
            Assert.That(game.State.HasFailed, Is.False);
            Assert.That(game.State.GetCargo("cargo").Position, Is.EqualTo(new GridPosition(1, 0)));
            Assert.That(game.State.MoveCount, Is.EqualTo(1));
            game.Restart();
            Assert.That(game.State.GetCargo("cargo").Position, Is.EqualTo(new GridPosition(0, 0)));
            Assert.That(game.State.MoveCount, Is.Zero);
        }

        [Test]
        public void Cargo_AndCart_BlockEachOther()
        {
            var level = new LevelDefinition("collision", 4, 1,
                new Dictionary<GridPosition, CellType> { [new GridPosition(3, 0)] = CellType.Exit },
                new[] { new CartDefinition("cart", new GridPosition(1, 0), Direction.Right) }, null,
                "market", 11, cargo: new[] { new CargoDefinition("cargo", new GridPosition(0, 0), Direction.Right, CargoType.Fabrics) },
                cargoDestinations: new Dictionary<GridPosition, CargoType> { [new GridPosition(2, 0)] = CargoType.Fabrics });
            var game = new BoardGame(level);
            Assert.That(game.MoveCargo("cargo"), Is.EqualTo(CargoMoveResult.Blocked));
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.MoveCargo("cargo"), Is.EqualTo(CargoMoveResult.Success));
        }

        [Test]
        public void MultipleTypedObjectives_ReportProgressAndCompletion()
        {
            var level = new LevelDefinition("objectives", 3, 2,
                new Dictionary<GridPosition, CellType> { [new GridPosition(2, 1)] = CellType.Exit },
                new[] { new CartDefinition("cart", new GridPosition(1, 1), Direction.Right) }, null,
                "market", 12, cargo: new[] { new CargoDefinition("cargo", new GridPosition(1, 0), Direction.Right, CargoType.Tools) },
                cargoDestinations: new Dictionary<GridPosition, CargoType> { [new GridPosition(2, 0)] = CargoType.Tools },
                objectives: new[]
                {
                    new ObjectiveDefinition("carts", ObjectiveType.ExitAllCarts),
                    new ObjectiveDefinition("cargo", ObjectiveType.DeliverAllCargo)
                });
            var game = new BoardGame(level);
            Assert.That(game.State.IsComplete, Is.False);
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.State.GetObjectiveProgress(level.Objectives[0]), Is.EqualTo(1));
            Assert.That(game.State.IsComplete, Is.False);
            Assert.That(game.MoveCargo("cargo"), Is.EqualTo(CargoMoveResult.Delivered));
            Assert.That(game.State.IsComplete, Is.True);
        }

        [Test]
        public void Solver_FindsMixedCartAndCargoMinimumSequence()
        {
            var level = new LevelDefinition("solver", 3, 2,
                new Dictionary<GridPosition, CellType> { [new GridPosition(2, 1)] = CellType.Exit },
                new[] { new CartDefinition("cart", new GridPosition(1, 1), Direction.Right) }, null,
                "market", 13, cargo: new[] { new CargoDefinition("cargo", new GridPosition(0, 0), Direction.Right, CargoType.Scrolls) },
                cargoDestinations: new Dictionary<GridPosition, CargoType> { [new GridPosition(2, 0)] = CargoType.Scrolls });
            var result = LevelSolver.Solve(level);
            Assert.That(result.Status, Is.EqualTo(SolverStatus.Solved));
            Assert.That(result.MinimumMoves, Is.EqualTo(3));
            Assert.That(result.Moves, Does.Contain("cart"));
            Assert.That(result.Moves, Does.Contain("cargo"));
        }

        [Test]
        public void Validator_RejectsMissingCargoDestinationAndDuplicateObjectId()
        {
            var level = new LevelDefinition("invalid-cargo", 3, 1,
                new Dictionary<GridPosition, CellType> { [new GridPosition(2, 0)] = CellType.Exit },
                new[] { new CartDefinition("same", new GridPosition(1, 0), Direction.Right) }, null,
                "market", 14, cargo: new[] { new CargoDefinition("same", new GridPosition(0, 0), Direction.Right, CargoType.Food) });
            var issues = LevelValidator.Validate(level);
            Assert.That(issues, Has.Some.Contains("Duplicate object ID"));
            Assert.That(issues, Has.Some.Contains("no matching destination"));
        }

        private static LevelDefinition CargoLevel(CargoType cargoType, CargoType destinationType) =>
            new("cargo", 3, 1, new Dictionary<GridPosition, CellType>(), new CartDefinition[0], null,
                "market", 11, cargo: new[] { new CargoDefinition("cargo", new GridPosition(0, 0), Direction.Right, cargoType) },
                cargoDestinations: new Dictionary<GridPosition, CargoType> { [new GridPosition(2, 0)] = destinationType },
                objectives: new[] { new ObjectiveDefinition("deliver", ObjectiveType.DeliverAllCargo) });
    }
}
