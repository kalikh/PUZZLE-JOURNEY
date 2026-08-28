using System;
using System.Collections.Generic;
using System.Linq;

namespace CaravanSecrets.Game.Journey
{
    /// <summary>
    /// Pure, Unity-free description of one decorative landmark placed along a
    /// journey segment. PrefabKey must reference an existing VerticalSlice prefab
    /// (rock, gate, cart) so presentation never requires new art.
    /// </summary>
    public sealed class JourneyLandmarkPlan
    {
        public static readonly IReadOnlyList<string> AllowedPrefabKeys = new[] { "rock", "gate", "cart" };

        public string PrefabKey { get; }
        public float X { get; }
        public float Y { get; }
        public float Scale { get; }
        public float RotationDegrees { get; }

        public JourneyLandmarkPlan(string prefabKey, float x, float y, float scale, float rotationDegrees)
        {
            PrefabKey = prefabKey;
            X = x;
            Y = y;
            Scale = scale;
            RotationDegrees = rotationDegrees;
        }
    }

    /// <summary>
    /// One data-driven journey segment: a checkpoint, a travel road, the puzzle
    /// location bound to a level id, and the next checkpoint.
    /// </summary>
    public sealed class JourneyChainSegment
    {
        public string SegmentId { get; }
        public string LevelId { get; }
        public string StartCheckpointId { get; }
        public string PuzzleLocationId { get; }
        public string NextCheckpointId { get; }
        public float RoadBend { get; }
        public IReadOnlyList<JourneyLandmarkPlan> Landmarks { get; }
        private JourneySegmentDefinition _definition;

        public JourneyChainSegment(string segmentId, string levelId, string startCheckpointId,
            string nextCheckpointId, float roadBend, IReadOnlyList<JourneyLandmarkPlan> landmarks)
        {
            SegmentId = Require(segmentId, nameof(segmentId));
            LevelId = Require(levelId, nameof(levelId));
            StartCheckpointId = Require(startCheckpointId, nameof(startCheckpointId));
            NextCheckpointId = Require(nextCheckpointId, nameof(nextCheckpointId));
            PuzzleLocationId = LevelId + "_site";
            RoadBend = roadBend;
            Landmarks = landmarks ?? Array.Empty<JourneyLandmarkPlan>();
        }

        public JourneySegmentDefinition Definition =>
            _definition ??= new JourneySegmentDefinition(SegmentId, StartCheckpointId, PuzzleLocationId, NextCheckpointId);

        private static string Require(string value, string parameter) => !string.IsNullOrWhiteSpace(value)
            ? value
            : throw new ArgumentException("Journey identifiers cannot be empty.", parameter);
    }

    /// <summary>
    /// Ordered, validated chain of journey segments (e.g. the ten Desert Road stages).
    /// </summary>
    public sealed class JourneyChain
    {
        private readonly IReadOnlyList<JourneyChainSegment> _segments;

        public JourneyChain(IReadOnlyList<JourneyChainSegment> segments)
        {
            var errors = JourneyChainValidator.Validate(segments);
            if (errors.Count > 0)
                throw new ArgumentException("Invalid journey chain: " + string.Join("; ", errors), nameof(segments));
            _segments = segments;
        }

        public int Count => _segments.Count;
        public IReadOnlyList<JourneyChainSegment> Segments => _segments;
        public JourneyChainSegment this[int index] => _segments[index];

        public bool TryFindByLevelId(string levelId, out JourneyChainSegment segment)
        {
            if (!string.IsNullOrWhiteSpace(levelId))
                foreach (var candidate in _segments)
                    if (string.Equals(candidate.LevelId, levelId, StringComparison.Ordinal))
                    {
                        segment = candidate;
                        return true;
                    }
            segment = null;
            return false;
        }

        public int IndexOfLevel(string levelId)
        {
            if (!string.IsNullOrWhiteSpace(levelId))
                for (var index = 0; index < _segments.Count; index++)
                    if (string.Equals(_segments[index].LevelId, levelId, StringComparison.Ordinal)) return index;
            return -1;
        }
    }

    public static class JourneyChainValidator
    {
        public static IReadOnlyList<string> Validate(IReadOnlyList<JourneyChainSegment> segments)
        {
            var errors = new List<string>();
            if (segments == null || segments.Count == 0)
            {
                errors.Add("Journey chain must contain at least one segment.");
                return errors;
            }

            var segmentIds = new HashSet<string>(StringComparer.Ordinal);
            var levelIds = new HashSet<string>(StringComparer.Ordinal);
            for (var index = 0; index < segments.Count; index++)
            {
                var segment = segments[index];
                if (segment == null)
                {
                    errors.Add($"Segment {index} is null.");
                    continue;
                }
                if (!segmentIds.Add(segment.SegmentId)) errors.Add($"Duplicate segment id '{segment.SegmentId}'.");
                if (!levelIds.Add(segment.LevelId)) errors.Add($"Duplicate level id '{segment.LevelId}'.");
                if (segment.StartCheckpointId == segment.NextCheckpointId)
                    errors.Add($"Segment '{segment.SegmentId}' reuses checkpoint '{segment.StartCheckpointId}'.");
                if (index > 0 && segments[index - 1] != null &&
                    segments[index - 1].NextCheckpointId != segment.StartCheckpointId)
                    errors.Add($"Segment '{segment.SegmentId}' does not start at the previous segment's checkpoint.");
                foreach (var landmark in segment.Landmarks)
                {
                    if (landmark == null) { errors.Add($"Segment '{segment.SegmentId}' contains a null landmark."); continue; }
                    if (!JourneyLandmarkPlan.AllowedPrefabKeys.Contains(landmark.PrefabKey))
                        errors.Add($"Segment '{segment.SegmentId}' uses unknown landmark prefab '{landmark.PrefabKey}'.");
                    if (landmark.Scale < 0.1f || landmark.Scale > 2f)
                        errors.Add($"Segment '{segment.SegmentId}' landmark scale {landmark.Scale} is out of range.");
                    if (Abs(landmark.X) > 4f || Abs(landmark.Y) > 16f)
                        errors.Add($"Segment '{segment.SegmentId}' landmark is placed outside the journey corridor.");
                }
            }
            return errors;
        }

        private static float Abs(float value) => value < 0 ? -value : value;
    }
}
