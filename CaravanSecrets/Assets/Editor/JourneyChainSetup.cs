using System.Collections.Generic;
using System.IO;
using CaravanSecrets.Data.Journey;
using CaravanSecrets.Data.Levels;
using CaravanSecrets.Game.Journey;
using UnityEditor;
using UnityEngine;

namespace CaravanSecrets.Editor
{
    /// <summary>
    /// Generates the data-driven Desert Road journey chain (levels 1-10) from the
    /// existing level assets. Deterministic output; running it again rewrites the
    /// same asset without touching level data.
    /// </summary>
    public static class JourneyChainSetup
    {
        private const string AssetPath = "Assets/Resources/Journey/DesertRoadJourney.asset";
        private const int SegmentCount = 10;

        [MenuItem("Caravan Secrets/Journey/Create or Update Desert Road Chain")]
        public static void CreateOrUpdate()
        {
            var levelIds = new List<string>();
            for (var index = 1; index <= SegmentCount; index++)
            {
                var levelId = $"desert_{index:00}";
                var levelAsset = AssetDatabase.LoadAssetAtPath<LevelAsset>($"Assets/Resources/Levels/{levelId}.asset");
                if (levelAsset == null) throw new System.InvalidOperationException($"Missing level asset for {levelId}.");
                levelIds.Add(levelId);
            }

            var asset = AssetDatabase.LoadAssetAtPath<JourneyChainAsset>(AssetPath);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<JourneyChainAsset>();
                Directory.CreateDirectory("Assets/Resources/Journey");
                AssetDatabase.CreateAsset(asset, AssetPath);
            }

            var serialized = new SerializedObject(asset);
            var segments = serialized.FindProperty("segments");
            segments.arraySize = SegmentCount;
            for (var index = 0; index < SegmentCount; index++)
            {
                var element = segments.GetArrayElementAtIndex(index);
                element.FindPropertyRelative("SegmentId").stringValue = $"desert_road_{index + 1:00}";
                element.FindPropertyRelative("LevelId").stringValue = levelIds[index];
                element.FindPropertyRelative("StartCheckpointId").stringValue =
                    index == 0 ? "desert_start" : $"desert_checkpoint_{index + 1:00}";
                element.FindPropertyRelative("NextCheckpointId").stringValue = $"desert_checkpoint_{index + 2:00}";
                element.FindPropertyRelative("RoadBend").floatValue = (index % 2 == 0 ? -1.05f : 1.05f) * (1f + (index % 3) * 0.08f);
                WriteLandmarks(element.FindPropertyRelative("Landmarks"), index);
            }
            serialized.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();

            var errors = JourneyChainValidator.Validate(asset.ToChain().Segments);
            if (errors.Count > 0)
                throw new System.InvalidOperationException("Journey chain invalid: " + string.Join("; ", errors));
            Debug.Log($"CARAVAN_JOURNEY_CHAIN_READY segments={SegmentCount} path={AssetPath}");
        }

        private static void WriteLandmarks(SerializedProperty landmarks, int segmentIndex)
        {
            const int count = 4;
            landmarks.arraySize = count;
            for (var landmark = 0; landmark < count; landmark++)
            {
                var element = landmarks.GetArrayElementAtIndex(landmark);
                var seed = segmentIndex * 7 + landmark * 3;
                var side = (seed % 2 == 0) ? -1f : 1f;
                element.FindPropertyRelative("PrefabKey").stringValue = landmark == 3 ? "gate" : "rock";
                element.FindPropertyRelative("X").floatValue = side * (1.6f + (seed % 5) * 0.32f);
                element.FindPropertyRelative("Y").floatValue = -11.5f + landmark * 7.1f + (seed % 3) * 0.45f;
                element.FindPropertyRelative("Scale").floatValue = landmark == 3 ? 0.9f : 0.52f + (seed % 4) * 0.11f;
                element.FindPropertyRelative("RotationDegrees").floatValue = (seed * 13) % 47 - 23;
            }
        }
    }
}
