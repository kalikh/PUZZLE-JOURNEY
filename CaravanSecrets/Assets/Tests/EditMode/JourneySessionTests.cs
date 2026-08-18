using System;
using CaravanSecrets.Game.Journey;
using CaravanSecrets.Data.Save;
using NUnit.Framework;
using UnityEngine;

namespace CaravanSecrets.Game.Tests
{
    public sealed class JourneySessionTests
    {
        [Test]
        public void RepresentativeSegment_AdvancesCheckpointOnlyAfterArrival()
        {
            var segment = new JourneySegmentDefinition("segment", "start", "puzzle", "next");
            var session = new JourneySession(segment);

            Assert.That(session.CurrentCheckpointId, Is.EqualTo("start"));
            Assert.That(session.Phase, Is.EqualTo(JourneyPhase.AtStartCheckpoint));

            session.BeginApproach();
            session.ArriveAtPuzzle();
            session.BeginDeparture();
            Assert.That(session.CurrentCheckpointId, Is.EqualTo("start"));

            session.ArriveAtNextCheckpoint();
            Assert.That(session.CurrentCheckpointId, Is.EqualTo("next"));
            Assert.That(session.Phase, Is.EqualTo(JourneyPhase.AtNextCheckpoint));
        }

        [Test]
        public void JourneySession_RejectsOutOfOrderTransitions()
        {
            var session = new JourneySession(new JourneySegmentDefinition("segment", "start", "puzzle", "next"));
            Assert.Throws<InvalidOperationException>(() => session.ArriveAtPuzzle());
            Assert.Throws<InvalidOperationException>(() => session.BeginDeparture());
            Assert.Throws<InvalidOperationException>(() => session.ArriveAtNextCheckpoint());
        }

        [Test]
        public void JourneySegment_RequiresDistinctValidCheckpointIds()
        {
            Assert.Throws<ArgumentException>(() => new JourneySegmentDefinition("segment", "same", "puzzle", "same"));
            Assert.Throws<ArgumentException>(() => new JourneySegmentDefinition("", "start", "puzzle", "next"));
        }

        [TestCase(JourneyPhase.AtStartCheckpoint, JourneyPhase.AtStartCheckpoint)]
        [TestCase(JourneyPhase.TravellingToPuzzle, JourneyPhase.AtStartCheckpoint)]
        [TestCase(JourneyPhase.AtPuzzle, JourneyPhase.AtPuzzle)]
        [TestCase(JourneyPhase.TravellingToNextCheckpoint, JourneyPhase.AtPuzzle)]
        public void RestoreStable_NormalizesIntermediatePhases(JourneyPhase saved, JourneyPhase expected)
        {
            var segment = new JourneySegmentDefinition("segment", "start", "puzzle", "next");
            var restored = JourneySession.RestoreStable(segment, "start", saved);
            Assert.That(restored.CurrentCheckpointId, Is.EqualTo("start"));
            Assert.That(restored.Phase, Is.EqualTo(expected));
        }

        [Test]
        public void RestoreStable_NextCheckpointAlwaysWinsOverStalePhase()
        {
            var segment = new JourneySegmentDefinition("segment", "start", "puzzle", "next");
            var restored = JourneySession.RestoreStable(segment, "next", JourneyPhase.TravellingToPuzzle);
            Assert.That(restored.CurrentCheckpointId, Is.EqualTo("next"));
            Assert.That(restored.Phase, Is.EqualTo(JourneyPhase.AtNextCheckpoint));
            Assert.Throws<InvalidOperationException>(() => restored.BeginDeparture());
        }

        [Test]
        public void PlayerSaveData_MissingJourneyFieldsRemainBackwardCompatible()
        {
            var oldJson = "{\"SaveVersion\":1,\"CurrentLevelId\":\"desert_01\",\"Coins\":9}";
            var data = JsonUtility.FromJson<PlayerSaveData>(oldJson);
            Assert.That(data.SaveVersion, Is.EqualTo(1));
            Assert.That(data.CurrentLevelId, Is.EqualTo("desert_01"));
            Assert.That(data.JourneyPuzzleCompleted, Is.False);
        }
    }
}
