using System.Collections.Generic;
using System.Linq;
using CaravanSecrets.Data.Levels;
using CaravanSecrets.Game.Board;
using NUnit.Framework;
using UnityEngine;

namespace CaravanSecrets.Game.Tests
{
    /// <summary>
    /// Level-quality gates for Stage 4 remediation (Phase 1 exemplars + Phase 2 campaign 6–30).
    /// </summary>
    public sealed class LevelQualityExemplarTests
    {
        private static readonly string[] ExemplarIds =
        {
            "desert_06", "desert_08", "desert_12", "desert_16", "desert_21"
        };

        private static readonly string[] CampaignIds = Enumerable.Range(6, 25)
            .Select(i => $"desert_{i:00}")
            .ToArray();

        private static LevelAsset Load(string levelId)
        {
            var asset = Resources.Load<LevelAsset>($"Levels/{levelId}");
            Assert.That(asset, Is.Not.Null, levelId);
            return asset;
        }

        private static SolverResult Solve(LevelDefinition level) =>
            LevelSolver.Solve(level, new SolverOptions { MaxVisitedStates = 250000, MaxDepth = 250 });

        private static float InterleaveRatio(IReadOnlyList<string> moves)
        {
            if (moves == null || moves.Count < 2) return 0f;
            var changes = 0;
            for (var i = 1; i < moves.Count; i++)
                if (moves[i] != moves[i - 1]) changes++;
            return (float)changes / (moves.Count - 1);
        }

        [Test]
        public void Exemplars_AreValidSolvableAndNotSingleObjectCorridors()
        {
            foreach (var levelId in ExemplarIds)
            {
                var level = Load(levelId).ToDefinition();
                Assert.That(LevelValidator.Validate(level), Is.Empty, levelId);

                var solution = Solve(level);
                Assert.That(solution.Status, Is.EqualTo(SolverStatus.Solved), $"{levelId}: {solution.Message}");
                Assert.That(solution.Moves.Distinct().Count(), Is.GreaterThan(1),
                    $"{levelId} must require more than one movable object.");
                Assert.That(InterleaveRatio(solution.Moves), Is.GreaterThan(0.2f),
                    $"{levelId} minimum solution should interleave objects (ratio={InterleaveRatio(solution.Moves):0.00}).");

                var game = new BoardGame(level);
                foreach (var objectId in solution.Moves)
                    Assert.That(game.MoveObject(objectId), Is.True, $"{levelId}: {objectId}");
                Assert.That(game.State.IsComplete, Is.True, levelId);
            }
        }

        [Test]
        public void Phase2Campaign_Levels6To30_AreMultiObjectDependencyPuzzles()
        {
            foreach (var levelId in CampaignIds)
            {
                var level = Load(levelId).ToDefinition();
                Assert.That(LevelValidator.Validate(level), Is.Empty, levelId);

                var solution = Solve(level);
                Assert.That(solution.Status, Is.EqualTo(SolverStatus.Solved), $"{levelId}: {solution.Message}");
                Assert.That(solution.Moves.Distinct().Count(), Is.GreaterThan(1),
                    $"{levelId} must require more than one movable object.");
                Assert.That(InterleaveRatio(solution.Moves), Is.GreaterThanOrEqualTo(0.2f),
                    $"{levelId} minimum solution should interleave objects (ratio={InterleaveRatio(solution.Moves):0.00}).");

                var game = new BoardGame(level);
                foreach (var objectId in solution.Moves)
                    Assert.That(game.MoveObject(objectId), Is.True, $"{levelId}: {objectId}");
                Assert.That(game.State.IsComplete, Is.True, levelId);
            }
        }

        [Test]
        public void Exemplar06_WrongReleaseOrder_DoesNotCompleteImmediately()
        {
            var level = Load("desert_06").ToDefinition();
            var game = new BoardGame(level);
            Assert.That(game.Move("b"), Is.EqualTo(MoveResult.Blocked));
            Assert.That(game.State.IsComplete, Is.False);
        }

        [Test]
        public void Exemplar12_FabricCannotPassClosedGateBeforeWater()
        {
            var level = Load("desert_12").ToDefinition();
            var game = new BoardGame(level);
            Assert.That(game.MoveCargo("fabric"), Is.EqualTo(CargoMoveResult.Success));
            Assert.That(game.MoveCargo("fabric"), Is.EqualTo(CargoMoveResult.Success));
            Assert.That(game.MoveCargo("fabric"), Is.EqualTo(CargoMoveResult.Blocked));
            Assert.That(game.State.IsComplete, Is.False);
        }

        [Test]
        public void Exemplar16_CartCannotFinishBeforeKeyOpensGate()
        {
            var level = Load("desert_16").ToDefinition();
            var game = new BoardGame(level);
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Blocked));
            Assert.That(game.State.IsComplete, Is.False);
        }

        [Test]
        public void Exemplar21_TurnedCartCannotExitThroughClosedGate()
        {
            var level = Load("desert_21").ToDefinition();
            var game = new BoardGame(level);
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Blocked));
            Assert.That(game.State.IsComplete, Is.False);
        }

        [Test]
        public void Phase2_11_SpicesCannotPassClosedGateBeforeWater()
        {
            var level = Load("desert_11").ToDefinition();
            var game = new BoardGame(level);
            Assert.That(game.MoveCargo("spices"), Is.EqualTo(CargoMoveResult.Success));
            Assert.That(game.MoveCargo("spices"), Is.EqualTo(CargoMoveResult.Success));
            Assert.That(game.MoveCargo("spices"), Is.EqualTo(CargoMoveResult.Blocked));
            Assert.That(game.State.IsComplete, Is.False);
        }

        [Test]
        public void Phase2_15_ArtifactBlockedUntilCartReleasesLane()
        {
            var level = Load("desert_15").ToDefinition();
            var game = new BoardGame(level);
            Assert.That(game.MoveCargo("artifact"), Is.EqualTo(CargoMoveResult.Success)); // (1,1)
            Assert.That(game.MoveCargo("artifact"), Is.EqualTo(CargoMoveResult.Blocked)); // cart at (1,2)
            Assert.That(game.State.IsComplete, Is.False);
        }

        [Test]
        public void Phase2_17_HelperBlockedUntilMainReleases()
        {
            var level = Load("desert_17").ToDefinition();
            var game = new BoardGame(level);
            Assert.That(game.Move("helper"), Is.EqualTo(MoveResult.Blocked));
            Assert.That(game.State.IsComplete, Is.False);
        }

        [Test]
        public void Phase2_22_ScrollBlockedAtGateUntilKeyOpens()
        {
            var level = Load("desert_22").ToDefinition();
            var game = new BoardGame(level);
            Assert.That(game.MoveCargo("scroll"), Is.EqualTo(CargoMoveResult.Success));
            Assert.That(game.MoveCargo("scroll"), Is.EqualTo(CargoMoveResult.Success));
            Assert.That(game.MoveCargo("scroll"), Is.EqualTo(CargoMoveResult.Success)); // turn + to (2,2)
            Assert.That(game.MoveCargo("scroll"), Is.EqualTo(CargoMoveResult.Blocked)); // gate_scroll
            Assert.That(game.State.IsComplete, Is.False);
        }

        [Test]
        public void Phase2_25_CartBlockedAtGateUntilPartsOpen()
        {
            var level = Load("desert_25").ToDefinition();
            var game = new BoardGame(level);
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success));
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success)); // turn up
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Success)); // (3,2)
            Assert.That(game.Move("cart"), Is.EqualTo(MoveResult.Blocked)); // gate at (3,3)
            Assert.That(game.State.IsComplete, Is.False);
        }

        [Test]
        public void Phase2_30_CaravanBlockedUntilArtifactOpensGate()
        {
            var level = Load("desert_30").ToDefinition();
            var game = new BoardGame(level);
            Assert.That(game.Move("caravan"), Is.EqualTo(MoveResult.Success)); // (3,3)
            Assert.That(game.Move("caravan"), Is.EqualTo(MoveResult.Blocked)); // closed gate_caravan
            Assert.That(game.State.IsComplete, Is.False);
        }

        [Test]
        public void FrozenLevels1To5_RetainAcceptedSha256Hashes()
        {
            var expected = new Dictionary<string, string>
            {
                ["desert_01"] = "4382644B2B05A103D73DF920EF5C985E752DB2F87DDA5FF9BCF6A820B0A455F0",
                ["desert_02"] = "FBF73996F18718B8A3B978A1B82C76DA989B9A5C32B2F5D2CBD35516D88CB73E",
                ["desert_03"] = "15B3CFD092185C14098F0B89E610FEBD336AFBD0A4EEDAE85000B3CB098C92EA",
                ["desert_04"] = "06ECDECC4AAFEE9EE311FB1DC00A1A18DA841C94233F1DF60D065815B105042E",
                ["desert_05"] = "992EAFD617483F7C4790411D465D9892EE2DD6760C2B4ABFDF05654D1FF283CC"
            };

            foreach (var pair in expected)
            {
                var path = System.IO.Path.GetFullPath(
                    System.IO.Path.Combine(Application.dataPath, "Resources", "Levels", $"{pair.Key}.asset"));
                Assert.That(System.IO.File.Exists(path), Is.True, path);
                using var sha = System.Security.Cryptography.SHA256.Create();
                var hash = sha.ComputeHash(System.IO.File.ReadAllBytes(path));
                var hex = string.Concat(hash.Select(b => b.ToString("X2")));
                Assert.That(hex, Is.EqualTo(pair.Value), pair.Key);
            }
        }
    }
}
