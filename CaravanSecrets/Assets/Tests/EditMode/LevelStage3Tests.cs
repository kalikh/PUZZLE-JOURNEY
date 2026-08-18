using System.Collections.Generic;
using CaravanSecrets.Game.Board;
using CaravanSecrets.Data.Levels;
using NUnit.Framework;
using UnityEngine;

namespace CaravanSecrets.Game.Tests
{
    public sealed class LevelStage3Tests
    {
        [Test]
        public void Metadata_IsPreservedInRuntimeDefinition()
        {
            var level = Simple("meta", 7, 9, 25);
            Assert.That(level.RegionId, Is.EqualTo("desert"));
            Assert.That(level.LevelNumber, Is.EqualTo(7));
            Assert.That(level.RecommendedMoves, Is.EqualTo(9));
            Assert.That(level.RewardCoins, Is.EqualTo(25));
        }

        [Test]
        public void LevelAsset_ConvertsSchemaMetadataAndDestinationLinks()
        {
            var asset = ScriptableObject.CreateInstance<LevelAsset>();
            asset.Configure("asset", 3, 1,
                new[] { new CellEntry(new Vector2Int(2, 0), CellType.Exit) },
                new[] { new CartEntry("a", new Vector2Int(0, 0), Direction.Right) },
                new[] { new DestinationEntry("a", new Vector2Int(2, 0)) });
            asset.ConfigureMetadata("desert", 12, 2, 30);
            var definition = asset.ToDefinition();

            Assert.That(definition.RegionId, Is.EqualTo("desert"));
            Assert.That(definition.LevelNumber, Is.EqualTo(12));
            Assert.That(definition.RecommendedMoves, Is.EqualTo(2));
            Assert.That(definition.RewardCoins, Is.EqualTo(30));
            Assert.That(definition.Destinations["a"], Is.EqualTo(new GridPosition(2, 0)));
            Object.DestroyImmediate(asset);
        }

        [Test]
        public void Validator_ReportsMetadataAndDuplicateLevelNumbers()
        {
            var invalid = new LevelDefinition("bad", 3, 1,
                new Dictionary<GridPosition, CellType> { [new GridPosition(2, 0)] = CellType.Exit },
                new[] { new CartDefinition("a", new GridPosition(0, 0), Direction.Right) }, null, "", -1, -2, -3);
            Assert.That(LevelValidator.Validate(invalid), Has.Some.Contains("Region ID"));
            Assert.That(LevelValidator.Validate(invalid), Has.Some.Contains("negative"));
            var duplicate = LevelValidator.ValidateCollection(new[] { Simple("one", 2), Simple("two", 2) });
            Assert.That(duplicate, Has.Some.Contains("Duplicate level number"));
        }

        [Test]
        public void Solver_FindsMinimumMoveSequence()
        {
            var result = LevelSolver.Solve(Simple("solve", 1));
            Assert.That(result.Status, Is.EqualTo(SolverStatus.Solved));
            Assert.That(result.MinimumMoves, Is.EqualTo(2));
            Assert.That(result.Moves, Is.EqualTo(new[] { "a", "a" }));
        }

        [Test]
        public void Solver_ReportsUnsolvableAndLimitReached()
        {
            var blocked = new LevelDefinition("blocked", 3, 1, new Dictionary<GridPosition, CellType>
            {
                [new GridPosition(1, 0)] = CellType.Rock, [new GridPosition(2, 0)] = CellType.Exit
            }, new[] { new CartDefinition("a", new GridPosition(0, 0), Direction.Right) }, null, "desert", 1);
            Assert.That(LevelSolver.Solve(blocked).Status, Is.EqualTo(SolverStatus.Unsolvable));
            var limited = LevelSolver.Solve(Simple("limited", 1), new SolverOptions { MaxDepth = 1, MaxVisitedStates = 100 });
            Assert.That(limited.Status, Is.EqualTo(SolverStatus.LimitReached));
            var stateLimited = LevelSolver.Solve(Simple("limited-states", 1), new SolverOptions { MaxDepth = 20, MaxVisitedStates = 1 });
            Assert.That(stateLimited.Status, Is.EqualTo(SolverStatus.LimitReached));
        }

        [Test]
        public void Solver_ExplicitlyRejectsUnsupportedCellType()
        {
            var level = new LevelDefinition("unsupported", 3, 1, new Dictionary<GridPosition, CellType>
            {
                [new GridPosition(1, 0)] = (CellType)999, [new GridPosition(2, 0)] = CellType.Exit
            }, new[] { new CartDefinition("a", new GridPosition(0, 0), Direction.Right) }, null, "desert", 1);
            Assert.That(LevelSolver.Solve(level).Status, Is.EqualTo(SolverStatus.Unsupported));
        }

        private static LevelDefinition Simple(string id, int number, int moves = 2, int coins = 10) =>
            new(id, 3, 1, new Dictionary<GridPosition, CellType> { [new GridPosition(2, 0)] = CellType.Exit },
                new[] { new CartDefinition("a", new GridPosition(0, 0), Direction.Right) }, null,
                "desert", number, moves, coins);
    }
}
