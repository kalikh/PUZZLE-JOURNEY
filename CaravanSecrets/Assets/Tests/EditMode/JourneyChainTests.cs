using System;
using System.Collections.Generic;
using CaravanSecrets.Data.Journey;
using CaravanSecrets.Data.Save;
using CaravanSecrets.Game.Journey;
using NUnit.Framework;
using UnityEngine;

namespace CaravanSecrets.Game.Tests
{
    public sealed class JourneyChainTests
    {
        private static JourneyChainSegment Segment(string id, string levelId, string start, string next) =>
            new(id, levelId, start, next, 1.05f,
                new[] { new JourneyLandmarkPlan("rock", 1.8f, -6.5f, 0.6f, 12f) });

        private static List<JourneyChainSegment> ValidChain()
        {
            var segments = new List<JourneyChainSegment> { Segment("s1", "level_01", "cp_start", "cp_02") };
            for (var index = 2; index <= 4; index++)
                segments.Add(Segment($"s{index}", $"level_{index:00}", $"cp_{index:00}", $"cp_{index + 1:00}"));
            return segments;
        }

        [Test]
        public void Validator_AcceptsLinkedUniqueChain()
        {
            Assert.That(JourneyChainValidator.Validate(ValidChain()), Is.Empty);
        }

        [Test]
        public void Validator_RejectsEmptyBrokenOrDuplicateChains()
        {
            Assert.That(JourneyChainValidator.Validate(Array.Empty<JourneyChainSegment>()), Is.Not.Empty);

            var broken = ValidChain();
            broken[1] = Segment("s2", "level_02", "wrong_start", "cp_03");
            Assert.That(JourneyChainValidator.Validate(broken), Is.Not.Empty);

            var duplicateLevels = ValidChain();
            duplicateLevels[2] = Segment("s3b", "level_02", "cp_03", "cp_04");
            Assert.That(JourneyChainValidator.Validate(duplicateLevels), Is.Not.Empty);

            var sameCheckpoints = new List<JourneyChainSegment> { Segment("s1", "level_01", "cp", "cp") };
            Assert.That(JourneyChainValidator.Validate(sameCheckpoints), Is.Not.Empty);
        }

        [Test]
        public void Validator_RejectsUnknownLandmarkPrefabsAndOutOfRangePlacement()
        {
            var badPrefab = new List<JourneyChainSegment>
            {
                new JourneyChainSegment("s1", "level_01", "cp_start", "cp_02", 1f,
                    new[] { new JourneyLandmarkPlan("castle", 1f, -5f, 0.6f, 0f) })
            };
            Assert.That(JourneyChainValidator.Validate(badPrefab), Is.Not.Empty);

            var outOfRange = new List<JourneyChainSegment>
            {
                new JourneyChainSegment("s1", "level_01", "cp_start", "cp_02", 1f,
                    new[] { new JourneyLandmarkPlan("rock", 9f, -5f, 0.6f, 0f) })
            };
            Assert.That(JourneyChainValidator.Validate(outOfRange), Is.Not.Empty);
        }

        [Test]
        public void JourneyChain_LooksUpSegmentsByLevelId()
        {
            var chain = new JourneyChain(ValidChain());
            Assert.That(chain.Count, Is.EqualTo(4));
            Assert.That(chain.TryFindByLevelId("level_03", out var segment), Is.True);
            Assert.That(segment.StartCheckpointId, Is.EqualTo("cp_03"));
            Assert.That(chain.IndexOfLevel("level_03"), Is.EqualTo(2));
            Assert.That(chain.TryFindByLevelId("oasis_01", out _), Is.False);
            Assert.That(chain.IndexOfLevel(null), Is.EqualTo(-1));
        }

        [Test]
        public void JourneyChain_ThrowsOnInvalidSegments()
        {
            Assert.Throws<ArgumentException>(() => new JourneyChain(new List<JourneyChainSegment>
            {
                Segment("s1", "level_01", "cp", "cp")
            }));
        }

        [Test]
        public void ChainedSessions_PreviousNextCheckpointBecomesNextStart()
        {
            var chain = new JourneyChain(ValidChain());
            var first = new JourneySession(chain[0].Definition);
            first.BeginApproach();
            first.ArriveAtPuzzle();
            first.BeginDeparture();
            first.ArriveAtNextCheckpoint();
            Assert.That(first.CurrentCheckpointId, Is.EqualTo(chain[1].StartCheckpointId));

            // Arrival at the previous segment's checkpoint restores as the next
            // segment's start checkpoint, so the approach can play again.
            var second = JourneySession.RestoreStable(chain[1].Definition, first.CurrentCheckpointId,
                JourneyPhase.AtStartCheckpoint);
            Assert.That(second.Phase, Is.EqualTo(JourneyPhase.AtStartCheckpoint));
            second.BeginApproach();
            Assert.That(second.Phase, Is.EqualTo(JourneyPhase.TravellingToPuzzle));
        }

        [Test]
        public void DesertRoadChainAsset_IsCompleteValidAndLinkedToTenLevels()
        {
            var asset = Resources.Load<JourneyChainAsset>("Journey/DesertRoadJourney");
            Assert.That(asset, Is.Not.Null, "DesertRoadJourney asset must exist under Assets/Resources/Journey.");
            var chain = asset.ToChain();
            Assert.That(chain.Count, Is.EqualTo(10));
            Assert.That(chain[0].StartCheckpointId, Is.EqualTo("desert_start"));
            Assert.That(chain[0].LevelId, Is.EqualTo("desert_01"));
            Assert.That(chain[0].NextCheckpointId, Is.EqualTo("desert_checkpoint_02"));
            for (var index = 0; index < chain.Count; index++)
            {
                Assert.That(chain[index].LevelId, Is.EqualTo($"desert_{index + 1:00}"));
                Assert.That(chain[index].Landmarks, Is.Not.Empty, $"Segment {index} needs landmarks.");
                if (index > 0)
                    Assert.That(chain[index].StartCheckpointId, Is.EqualTo(chain[index - 1].NextCheckpointId));
            }
        }

        [Test]
        public void PlayerSaveData_ChainProgressionRemainsVersionOneCompatible()
        {
            var oldJson = "{\"SaveVersion\":1,\"CurrentLevelId\":\"desert_03\",\"JourneyCheckpointId\":\"desert_checkpoint_03\",\"JourneyPhase\":\"AtNextCheckpoint\"}";
            var data = JsonUtility.FromJson<PlayerSaveData>(oldJson);
            Assert.That(data.SaveVersion, Is.EqualTo(1));
            Assert.That(data.CurrentLevelId, Is.EqualTo("desert_03"));
            Assert.That(data.JourneyCheckpointId, Is.EqualTo("desert_checkpoint_03"));
            Assert.That(data.JourneyPuzzleCompleted, Is.False);
        }
    }
}
