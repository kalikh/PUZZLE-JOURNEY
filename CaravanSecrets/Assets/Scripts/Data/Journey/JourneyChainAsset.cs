using System;
using System.Collections.Generic;
using CaravanSecrets.Game.Journey;
using UnityEngine;

namespace CaravanSecrets.Data.Journey
{
    /// <summary>
    /// Serializable journey chain definition. The runtime converts this asset into
    /// the pure <see cref="JourneyChain"/> so presenters and controllers never
    /// hard-code level or checkpoint identifiers.
    /// </summary>
    [CreateAssetMenu(menuName = "Caravan Secrets/Journey Chain", fileName = "JourneyChain")]
    public sealed class JourneyChainAsset : ScriptableObject
    {
        [Serializable]
        public sealed class LandmarkData
        {
            public string PrefabKey = "rock";
            public float X;
            public float Y;
            public float Scale = 0.7f;
            public float RotationDegrees;
        }

        [Serializable]
        public sealed class SegmentData
        {
            public string SegmentId;
            public string LevelId;
            public string StartCheckpointId;
            public string NextCheckpointId;
            public float RoadBend = 1.05f;
            public LandmarkData[] Landmarks = Array.Empty<LandmarkData>();
        }

        [SerializeField] private SegmentData[] segments = Array.Empty<SegmentData>();

        public IReadOnlyList<SegmentData> Segments => segments;

        public JourneyChain ToChain()
        {
            var converted = new List<JourneyChainSegment>(segments.Length);
            foreach (var segment in segments)
            {
                var landmarks = new List<JourneyLandmarkPlan>();
                if (segment.Landmarks != null)
                    foreach (var landmark in segment.Landmarks)
                        if (landmark != null)
                            landmarks.Add(new JourneyLandmarkPlan(landmark.PrefabKey, landmark.X, landmark.Y,
                                landmark.Scale, landmark.RotationDegrees));
                converted.Add(new JourneyChainSegment(segment.SegmentId, segment.LevelId, segment.StartCheckpointId,
                    segment.NextCheckpointId, segment.RoadBend, landmarks));
            }
            return new JourneyChain(converted);
        }
    }
}
