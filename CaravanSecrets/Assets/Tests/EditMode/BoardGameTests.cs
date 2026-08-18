using System.Collections.Generic;
using System.Linq;
using CaravanSecrets.Data.Levels;
using CaravanSecrets.Game.Board;
using NUnit.Framework;
using UnityEngine;

namespace CaravanSecrets.Game.Tests
{
    public sealed class BoardGameTests
    {
        [Test]
        public void Cart_MovesForward_AndCompletesOnExit()
        {
            var game = Game(3, new GridPosition(1, 0));
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.State.IsComplete, Is.True);
            Assert.That(game.State.MoveCount, Is.EqualTo(1));
        }

        [Test]
        public void Rock_BlocksMovement()
        {
            var game = Game(3, new GridPosition(2, 0), new GridPosition(1, 0));
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Blocked));
            Assert.That(game.State.MoveCount, Is.Zero);
        }

        [Test]
        public void Undo_RestoresFullState()
        {
            var game = Game(4, new GridPosition(3, 0));
            game.Move("cart");
            Assert.That(game.Undo(), Is.True);
            Assert.That(game.State.GetCart("cart").Position, Is.EqualTo(new GridPosition(0, 0)));
            Assert.That(game.State.MoveCount, Is.Zero);
        }

        [Test]
        public void Restart_RestoresInitialStateAndClearsHistory()
        {
            var game = Game(4, new GridPosition(3, 0));
            game.Move("cart");
            game.Restart();
            Assert.That(game.State.GetCart("cart").Position, Is.EqualTo(new GridPosition(0, 0)));
            Assert.That(game.CanUndo, Is.False);
        }

        [Test]
        public void PrototypeLevels_AreEightAndSolvable()
        {
            Assert.That(PrototypeLevels.All.Count, Is.EqualTo(8));
            foreach (var level in PrototypeLevels.All)
            {
                Assert.That(LevelValidator.Validate(level), Is.Empty, level.Id);
                var solution = LevelSolver.Solve(level);
                Assert.That(solution.Status, Is.EqualTo(SolverStatus.Solved), level.Id);
                var game = new BoardGame(level);
                foreach (var cartId in solution.Moves)
                    Assert.That(game.Move(cartId), Is.EqualTo(MoveResult.Success).Or.EqualTo(MoveResult.WrongExit), level.Id);
                Assert.That(game.State.IsComplete, Is.True, level.Id);
            }
        }

        [Test]
        public void FirstFiveResourceLevels_AreValidAndSolvable()
        {
            var levels = Resources.LoadAll<LevelAsset>("Levels")
                .OrderBy(asset => asset.LevelId).Take(5).Select(asset => asset.ToDefinition()).ToArray();
            Assert.That(levels.Length, Is.EqualTo(5));
            foreach (var level in levels)
            {
                Assert.That(LevelValidator.Validate(level), Is.Empty, level.Id);
                var game = new BoardGame(level);
                for (var pass = 0; pass < 40 && !game.State.IsComplete; pass++)
                    foreach (var cart in level.Carts.Reverse()) game.Move(cart.Id);
                Assert.That(game.State.IsComplete, Is.True, level.Id);
            }
        }

        [Test]
        public void DestinationMatching_WrongGateFails_AndUndoRestoresEverything()
        {
            var cells = new Dictionary<GridPosition, CellType>
            {
                [new GridPosition(2, 0)] = CellType.Exit,
                [new GridPosition(2, 1)] = CellType.Exit
            };
            var destinations = new Dictionary<string, GridPosition>
            {
                ["a"] = new GridPosition(2, 1), ["b"] = new GridPosition(2, 0)
            };
            var game = new BoardGame(new LevelDefinition("matching", 3, 2, cells, new[]
            {
                new CartDefinition("a", new GridPosition(1, 0), Direction.Right),
                new CartDefinition("b", new GridPosition(1, 1), Direction.Right)
            }, destinations));

            Assert.That(game.Move("a"), Is.EqualTo(MoveResult.WrongExit));
            Assert.That(game.State.HasFailed, Is.True);
            Assert.That(game.State.MoveCount, Is.EqualTo(1));
            Assert.That(game.Undo(), Is.True);
            Assert.That(game.State.HasFailed, Is.False);
            Assert.That(game.State.MoveCount, Is.Zero);
            Assert.That(game.State.GetCart("a").Position, Is.EqualTo(new GridPosition(1, 0)));
        }

        [Test]
        public void Storage_LocksCart_UntilAnotherCartActivatesSwitch_AndUndoRestoresLock()
        {
            var cells = new Dictionary<GridPosition, CellType>
            {
                [new GridPosition(1, 1)] = CellType.Storage,
                [new GridPosition(1, 0)] = CellType.Switch,
                [new GridPosition(3, 1)] = CellType.Exit,
                [new GridPosition(3, 0)] = CellType.Exit
            };
            var game = new BoardGame(new LevelDefinition("storage", 4, 2, cells, new[]
            {
                new CartDefinition("stored", new GridPosition(0, 1), Direction.Right),
                new CartDefinition("key", new GridPosition(0, 0), Direction.Right)
            }));

            Assert.That(game.Move("stored"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.State.StoredCartId, Is.EqualTo("stored"));
            Assert.That(game.Move("stored"), Is.EqualTo(MoveResult.Stored));
            Assert.That(game.Move("key"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.State.BarriersOpen, Is.True);
            Assert.That(game.Move("stored"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.State.StoredCartId, Is.Null);
            Assert.That(game.Undo(), Is.True);
            Assert.That(game.State.StoredCartId, Is.EqualTo("stored"));
            Assert.That(game.State.BarriersOpen, Is.True);
        }

        [Test]
        public void Switch_OpensRockBarrier_AndUndoClosesIt()
        {
            var cells = new Dictionary<GridPosition, CellType>
            {
                [new GridPosition(1, 0)] = CellType.Switch,
                [new GridPosition(2, 1)] = CellType.Rock,
                [new GridPosition(3, 0)] = CellType.Exit,
                [new GridPosition(3, 1)] = CellType.Exit
            };
            var game = new BoardGame(new LevelDefinition("switch", 4, 2, cells, new[]
            {
                new CartDefinition("key", new GridPosition(0,0), Direction.Right),
                new CartDefinition("blocked", new GridPosition(1,1), Direction.Right)
            }));
            Assert.That(game.Move("blocked"), Is.EqualTo(MoveResult.Blocked));
            Assert.That(game.Move("key"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.State.BarriersOpen, Is.True);
            Assert.That(game.Move("blocked"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Undo(), Is.True);
            Assert.That(game.Undo(), Is.True);
            Assert.That(game.State.BarriersOpen, Is.False);
        }

        [Test]
        public void DesertVerticalSlice_RequiresClearingTheCrossing()
        {
            var cells = new Dictionary<GridPosition, CellType>
            {
                [new GridPosition(4, 4)] = CellType.Exit,
                [new GridPosition(4, 0)] = CellType.Exit,
                [new GridPosition(3, 4)] = CellType.Rock,
                [new GridPosition(2, 0)] = CellType.Switch
            };
            var game = new BoardGame(new LevelDefinition("desert_01", 5, 5, cells, new[]
            {
                new CartDefinition("a", new GridPosition(0, 4), Direction.Right),
                new CartDefinition("b", new GridPosition(0, 0), Direction.Right)
            }));

            Assert.That(game.Move("a"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Move("a"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Move("a"), Is.EqualTo(MoveResult.Blocked));
            Assert.That(game.Move("b"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Move("b"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.State.BarriersOpen, Is.True);
            for (var step = 0; step < 2; step++) Assert.That(game.Move("a"), Is.EqualTo(MoveResult.Success));
            for (var step = 0; step < 2; step++) Assert.That(game.Move("b"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.State.IsComplete, Is.True);
            Assert.That(game.State.MoveCount, Is.EqualTo(8));
        }

        private static BoardGame Game(int width, GridPosition exit, GridPosition? rock = null)
        {
            var cells = new Dictionary<GridPosition, CellType> { [exit] = CellType.Exit };
            if (rock.HasValue) cells[rock.Value] = CellType.Rock;
            return new BoardGame(new LevelDefinition("test", width, 1, cells,
                new[] { new CartDefinition("cart", new GridPosition(0, 0), Direction.Right) }));
        }
    }
}
