using System.Linq;
using CaravanSecrets.Data.Levels;
using CaravanSecrets.Game.Board;
using NUnit.Framework;
using UnityEngine;

namespace CaravanSecrets.Game.Tests
{
    public sealed class Stage4GateDTests
    {
        private static LevelAsset[] LoadLevels() => Resources.LoadAll<LevelAsset>("Levels")
            .OrderBy(level => level.LevelId).ToArray();

        [Test]
        public void ProductionCampaign_HasThirtySequentialLevelsAndTenPerRegion()
        {
            var levels = LoadLevels();
            Assert.That(levels, Has.Length.EqualTo(30));
            Assert.That(levels.Select(level => level.LevelId),
                Is.EqualTo(Enumerable.Range(1, 30).Select(number => $"desert_{number:00}")));
            Assert.That(levels.Count(level => level.RegionId == "desert"), Is.EqualTo(10));
            Assert.That(levels.Count(level => level.RegionId == "oasis"), Is.EqualTo(10));
            Assert.That(levels.Count(level => level.RegionId == "city"), Is.EqualTo(10));
        }

        [Test]
        public void ProductionLevels_AreValidSolvableAndReplayTheirMinimumSolutions()
        {
            foreach (var asset in LoadLevels().Skip(5))
            {
                var level = asset.ToDefinition();
                Assert.That(LevelValidator.Validate(level), Is.Empty, asset.LevelId);
                var solution = LevelSolver.Solve(level, new SolverOptions { MaxVisitedStates = 250000, MaxDepth = 250 });
                Assert.That(solution.Status, Is.EqualTo(SolverStatus.Solved), asset.LevelId);
                Assert.That(asset.RecommendedMoves, Is.GreaterThanOrEqualTo(solution.MinimumMoves), asset.LevelId);

                var game = new BoardGame(level);
                foreach (var objectId in solution.Moves)
                    Assert.That(game.MoveObject(objectId), Is.True, $"{asset.LevelId}: {objectId}");
                Assert.That(game.State.IsComplete, Is.True, asset.LevelId);
            }
        }

        [Test]
        public void MechanicProgression_MatchesGateDBands()
        {
            var levels = LoadLevels().ToDictionary(level => int.Parse(level.LevelId.Substring(level.LevelId.Length - 2)));
            Assert.That(Enumerable.Range(11, 5).All(number => levels[number].Cargo.Count > 0), Is.True);
            Assert.That(Enumerable.Range(16, 5).All(number => levels[number].Gates.Count > 0 && levels[number].LinkedSwitches.Count > 0), Is.True);
            Assert.That(Enumerable.Range(21, 5).All(number => levels[number].DirectionTiles.Count > 0), Is.True);
            Assert.That(Enumerable.Range(26, 5).All(number =>
                (levels[number].Cargo.Count > 0 ? 1 : 0) +
                (levels[number].Gates.Count > 0 ? 1 : 0) +
                (levels[number].StorageSlots.Count > 0 ? 1 : 0) +
                (levels[number].DirectionTiles.Count > 0 ? 1 : 0) >= 2), Is.True);
        }
    }
}
