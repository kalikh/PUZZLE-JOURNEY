using System;
using CaravanSecrets.Game.Journey;
using NUnit.Framework;

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
    }
}
