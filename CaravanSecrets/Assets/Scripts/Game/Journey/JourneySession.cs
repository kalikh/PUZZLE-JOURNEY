using System;

namespace CaravanSecrets.Game.Journey
{
    public enum JourneyPhase
    {
        AtStartCheckpoint,
        TravellingToPuzzle,
        AtPuzzle,
        TravellingToNextCheckpoint,
        AtNextCheckpoint
    }

    public interface IJourneyProgress
    {
        string CurrentCheckpointId { get; }
        JourneyPhase Phase { get; }
    }

    public sealed class JourneySegmentDefinition
    {
        public string SegmentId { get; }
        public string StartCheckpointId { get; }
        public string PuzzleLocationId { get; }
        public string NextCheckpointId { get; }

        public JourneySegmentDefinition(string segmentId, string startCheckpointId, string puzzleLocationId, string nextCheckpointId)
        {
            SegmentId = Require(segmentId, nameof(segmentId));
            StartCheckpointId = Require(startCheckpointId, nameof(startCheckpointId));
            PuzzleLocationId = Require(puzzleLocationId, nameof(puzzleLocationId));
            NextCheckpointId = Require(nextCheckpointId, nameof(nextCheckpointId));
            if (StartCheckpointId == NextCheckpointId)
                throw new ArgumentException("Journey checkpoints must be distinct.", nameof(nextCheckpointId));
        }

        private static string Require(string value, string parameter) => !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Journey identifiers cannot be empty.", parameter);
    }

    public sealed class JourneySession : IJourneyProgress
    {
        public JourneySegmentDefinition Segment { get; }
        public string CurrentCheckpointId { get; private set; }
        public JourneyPhase Phase { get; private set; }

        public JourneySession(JourneySegmentDefinition segment)
        {
            Segment = segment ?? throw new ArgumentNullException(nameof(segment));
            CurrentCheckpointId = segment.StartCheckpointId;
            Phase = JourneyPhase.AtStartCheckpoint;
        }

        public void BeginApproach()
        {
            RequirePhase(JourneyPhase.AtStartCheckpoint);
            Phase = JourneyPhase.TravellingToPuzzle;
        }

        public void ArriveAtPuzzle()
        {
            RequirePhase(JourneyPhase.TravellingToPuzzle);
            Phase = JourneyPhase.AtPuzzle;
        }

        public void BeginDeparture()
        {
            RequirePhase(JourneyPhase.AtPuzzle);
            Phase = JourneyPhase.TravellingToNextCheckpoint;
        }

        public void ArriveAtNextCheckpoint()
        {
            RequirePhase(JourneyPhase.TravellingToNextCheckpoint);
            CurrentCheckpointId = Segment.NextCheckpointId;
            Phase = JourneyPhase.AtNextCheckpoint;
        }

        private void RequirePhase(JourneyPhase required)
        {
            if (Phase != required)
                throw new InvalidOperationException($"Journey phase {Phase} cannot transition as {required}.");
        }
    }
}
