using System.Collections.Generic;
using CaravanSecrets.Game.Board;
using CaravanSecrets.Game.Boosters;
using CaravanSecrets.Game.Results;
using NUnit.Framework;

namespace CaravanSecrets.Game.Tests
{
    public sealed class Stage4GateCTests
    {
        [Test]
        public void Compass_SuggestsSolutionMove_WithoutChangingBoard()
        {
            var game = new BoardGame(SimpleExitLevel(recommendedMoves: 2));
            var result = new CompassBooster().Use(new BoosterRequest(game));

            Assert.That(result.Status, Is.EqualTo(BoosterStatus.Applied));
            Assert.That(result.SuggestedObjectId, Is.EqualTo("cart"));
            Assert.That(game.State.MoveCount, Is.Zero);
            Assert.That(game.State.GetCart("cart").Position, Is.EqualTo(new GridPosition(0, 0)));
            Assert.That(game.HasUsedBooster, Is.True);
        }

        [Test]
        public void Compass_UsesCurrentStateAndReturnsNextProgressingMove()
        {
            var game = new BoardGame(SimpleExitLevel(recommendedMoves: 2));
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));

            var result = new CompassBooster().Use(new BoosterRequest(game));

            Assert.That(result.SuggestedObjectId, Is.EqualTo("cart"));
            Assert.That(game.State.MoveCount, Is.EqualTo(1));
        }

        [Test]
        public void Rope_RemovesOnlyRock_UndoRestoresEffectButNotUsage_AndRestartClearsUsage()
        {
            var rock = new GridPosition(1, 0);
            var level = new LevelDefinition("rope", 3, 1,
                new Dictionary<GridPosition, CellType> { [rock] = CellType.Rock, [new GridPosition(2, 0)] = CellType.Exit },
                new[] { new CartDefinition("cart", new GridPosition(0, 0), Direction.Right) });
            var game = new BoardGame(level);
            var rope = new RopeBooster();

            Assert.That(rope.Use(new BoosterRequest(game, new GridPosition(0, 0))).Status, Is.EqualTo(BoosterStatus.Ineligible));
            Assert.That(rope.Use(new BoosterRequest(game, rock)).Applied, Is.True);
            Assert.That(game.State.GetCell(rock), Is.EqualTo(CellType.Empty));
            Assert.That(game.Undo(), Is.True);
            Assert.That(game.State.GetCell(rock), Is.EqualTo(CellType.Rock));
            Assert.That(game.HasUsedBooster, Is.True);
            game.Restart();
            Assert.That(game.State.GetCell(rock), Is.EqualTo(CellType.Rock));
            Assert.That(game.HasUsedBooster, Is.False);
        }

        [Test]
        public void Rope_CannotRemoveGateOrSameRockTwice()
        {
            var rock = new GridPosition(1, 0);
            var gate = new GridPosition(2, 0);
            var level = new LevelDefinition("rope_rules", 4, 1,
                new Dictionary<GridPosition, CellType> { [rock] = CellType.Rock, [new GridPosition(3, 0)] = CellType.Exit },
                new[] { new CartDefinition("cart", new GridPosition(0, 0), Direction.Right) },
                gates: new[] { new GateDefinition("gate", gate) });
            var game = new BoardGame(level);
            var rope = new RopeBooster();

            Assert.That(rope.CanUse(new BoosterRequest(game, gate)), Is.False);
            Assert.That(rope.Use(new BoosterRequest(game, rock)).Applied, Is.True);
            Assert.That(rope.Use(new BoosterRequest(game, rock)).Status, Is.EqualTo(BoosterStatus.Ineligible));
        }

        [Test]
        public void ExtraSpace_AddsOneCapacity_UndoAndRestartRestoreOriginal()
        {
            var storage = new GridPosition(1, 0);
            var level = new LevelDefinition("space", 3, 2,
                new Dictionary<GridPosition, CellType>(),
                new[]
                {
                    new CartDefinition("a", new GridPosition(0, 0), Direction.Right),
                    new CartDefinition("b", new GridPosition(1, 1), Direction.Down)
                }, storageSlots: new[] { new StorageDefinition("store", storage, 1) });
            var game = new BoardGame(level);
            var booster = new ExtraSpaceBooster();

            Assert.That(game.Move("a"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Move("b"), Is.EqualTo(MoveResult.Blocked));
            Assert.That(booster.Use(new BoosterRequest(game, storage)).Applied, Is.True);
            Assert.That(game.State.GetStorageCapacity(storage), Is.EqualTo(2));
            Assert.That(game.Move("b"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Undo(), Is.True);
            Assert.That(game.Undo(), Is.True);
            Assert.That(game.State.GetStorageCapacity(storage), Is.EqualTo(1));
            Assert.That(game.HasUsedBooster, Is.True);
            game.Restart();
            Assert.That(game.State.GetStorageCapacity(storage), Is.EqualTo(1));
            Assert.That(game.HasUsedBooster, Is.False);
        }

        [Test]
        public void ExtraSpace_RequiresTypedStorage_AndCanApplyOnlyOnce()
        {
            var storage = new GridPosition(1, 0);
            var level = new LevelDefinition("space_rules", 2, 1, new Dictionary<GridPosition, CellType>(),
                new[] { new CartDefinition("cart", new GridPosition(0, 0), Direction.Right) },
                storageSlots: new[] { new StorageDefinition("store", storage, 1) });
            var game = new BoardGame(level);
            var booster = new ExtraSpaceBooster();

            Assert.That(booster.CanUse(new BoosterRequest(game, new GridPosition(0, 0))), Is.False);
            Assert.That(booster.Use(new BoosterRequest(game, storage)).Applied, Is.True);
            Assert.That(booster.Use(new BoosterRequest(game, storage)).Status, Is.EqualTo(BoosterStatus.Ineligible));
        }

        [TestCase(2, false, 3)]
        [TestCase(1, false, 2)]
        [TestCase(2, true, 2)]
        [TestCase(1, true, 1)]
        public void Stars_AreDeterministicForMovesAndBoosterUse(int recommendedMoves, bool useBooster, int expectedStars)
        {
            var game = new BoardGame(SimpleExitLevel(recommendedMoves));
            if (useBooster) Assert.That(new CompassBooster().Use(new BoosterRequest(game)).Applied, Is.True);
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));
            var result = LevelResultCalculator.Calculate(game, recommendedMoves);
            Assert.That(result.Stars, Is.EqualTo(expectedStars));
            Assert.That(result.MoveCount, Is.EqualTo(2));
            Assert.That(result.LevelId, Is.EqualTo("stars"));
        }

        [Test]
        public void IncompleteResultHasZeroStars_AndContainsNoRewardMutation()
        {
            var game = new BoardGame(SimpleExitLevel(recommendedMoves: 2));
            var result = LevelResultCalculator.Calculate(game, 2);
            Assert.That(result.IsComplete, Is.False);
            Assert.That(result.Stars, Is.Zero);
            Assert.That(typeof(LevelResult).GetProperty("RewardCoins"), Is.Null);
        }

        private static LevelDefinition SimpleExitLevel(int recommendedMoves) =>
            new("stars", 3, 1,
                new Dictionary<GridPosition, CellType> { [new GridPosition(2, 0)] = CellType.Exit },
                new[] { new CartDefinition("cart", new GridPosition(0, 0), Direction.Right) },
                recommendedMoves: recommendedMoves);
    }
}
