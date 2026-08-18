using System.Collections.Generic;
using System.IO;
using System.Linq;
using CaravanSecrets.Data.Levels;
using CaravanSecrets.Game.Board;
using CaravanSecrets.Editor.LevelEditor;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace CaravanSecrets.Editor
{
    public static class LevelTools
    {
        private const string LevelFolder = "Assets/Resources/Levels";

        [MenuItem("Caravan Secrets/Levels/Generate Prototype Levels")]
        public static void GeneratePrototypeLevels()
        {
            Directory.CreateDirectory(LevelFolder);
            Generate(PrototypeLevels.All);
            Debug.Log($"CARAVAN_LEVELS_GENERATED count={PrototypeLevels.All.Count}");
        }

        public static void GenerateFirstFiveLevels()
        {
            Directory.CreateDirectory(LevelFolder);
            Generate(PrototypeLevels.All.Take(5));
            Debug.Log("CARAVAN_FIRST_FIVE_LEVELS_GENERATED");
        }

        [MenuItem("Caravan Secrets/Levels/Generate Stage 4 Production Levels")]
        public static void GenerateStage4ProductionLevels()
        {
            Directory.CreateDirectory(LevelFolder);
            foreach (var definition in Stage4ProductionCatalog.Create())
            {
                var result = LevelSolver.Solve(definition, new SolverOptions { MaxVisitedStates = 250000, MaxDepth = 250 });
                if (result.Status != SolverStatus.Solved)
                    throw new BuildFailedException($"[{definition.Id}] cannot be generated: {result.Status} — {result.Message}");

                var path = $"{LevelFolder}/{definition.Id}.asset";
                var asset = AssetDatabase.LoadAssetAtPath<LevelAsset>(path);
                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance<LevelAsset>();
                    AssetDatabase.CreateAsset(asset, path);
                }
                asset.Configure(definition.Id, definition.Width, definition.Height,
                    definition.Cells.Select(pair => new CellEntry(new Vector2Int(pair.Key.X, pair.Key.Y), pair.Value)),
                    definition.Carts.Select(cart => new CartEntry(cart.Id, new Vector2Int(cart.Position.X, cart.Position.Y), cart.Direction)),
                    definition.Destinations.Select(pair => new DestinationEntry(pair.Key, new Vector2Int(pair.Value.X, pair.Value.Y))));
                asset.ConfigureMetadata(definition.RegionId, definition.LevelNumber, result.MinimumMoves + 1, definition.RewardCoins);
                asset.ConfigureExpansion(
                    definition.Cargo.Select(item => new CargoEntry(item.Id, new Vector2Int(item.Position.X, item.Position.Y), item.Direction, item.Type)),
                    definition.CargoDestinations.Select(pair => new CargoDestinationEntry(new Vector2Int(pair.Key.X, pair.Key.Y), pair.Value)),
                    definition.Objectives.Select(item => new ObjectiveEntry(item.Id, item.Type)));
                asset.ConfigureMechanisms(
                    definition.Gates.Select(item => new GateEntry(item.Id, new Vector2Int(item.Position.X, item.Position.Y), item.InitiallyOpen)),
                    definition.Switches.Select(item => new SwitchEntry(item.Id, new Vector2Int(item.Position.X, item.Position.Y), item.GateIds)),
                    definition.StorageSlots.Select(item => new StorageEntry(item.Id, new Vector2Int(item.Position.X, item.Position.Y), item.Capacity)),
                    definition.DirectionTiles.Select(item => new DirectionTileEntry(item.Id, new Vector2Int(item.Position.X, item.Position.Y), item.Direction)));
                EditorUtility.SetDirty(asset);
                Debug.Log($"STAGE4_LEVEL_GENERATED id={definition.Id} region={definition.RegionId} number={definition.LevelNumber} minimumMoves={result.MinimumMoves} solution={string.Join(",", result.Moves)}", asset);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            ValidateAll();
        }

        private static void Generate(IEnumerable<LevelDefinition> definitions)
        {
            foreach (var definition in definitions)
            {
                var path = $"{LevelFolder}/{definition.Id}.asset";
                var asset = AssetDatabase.LoadAssetAtPath<LevelAsset>(path);
                if (asset == null)
                {
                    asset = ScriptableObject.CreateInstance<LevelAsset>();
                    AssetDatabase.CreateAsset(asset, path);
                }

                var cells = definition.Cells.Select(pair =>
                    new CellEntry(new Vector2Int(pair.Key.X, pair.Key.Y), pair.Value));
                var carts = definition.Carts.Select(cart =>
                    new CartEntry(cart.Id, new Vector2Int(cart.Position.X, cart.Position.Y), cart.Direction));
                var destinations = definition.Destinations.Select(pair =>
                    new DestinationEntry(pair.Key, new Vector2Int(pair.Value.X, pair.Value.Y)));
                asset.Configure(definition.Id, definition.Width, definition.Height, cells, carts, destinations);
                EditorUtility.SetDirty(asset);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Caravan Secrets/Levels/Validate All")]
        public static void ValidateAll()
        {
            var assets = LoadAll();
            var issueCount = 0;
            foreach (var asset in assets)
            {
                var issues = LevelAssetValidation.Validate(asset, assets);
                foreach (var issue in issues)
                {
                    issueCount++;
                    Debug.LogError($"[{asset.LevelId}] {issue}", asset);
                }
                if (issues.Count == 0)
                {
                    var result = LevelSolver.Solve(asset.ToDefinition());
                    if (result.Status != SolverStatus.Solved)
                    {
                        issueCount++;
                        Debug.LogError($"[{asset.LevelId}] Solver: {result.Status} — {result.Message}", asset);
                    }
                    else Debug.Log($"[{asset.LevelId}] SOLVED minimumMoves={result.MinimumMoves} visited={result.VisitedStates} solution={string.Join(",", result.Moves)}", asset);
                }
            }
            if (issueCount > 0) throw new BuildFailedException($"Level validation failed with {issueCount} issue(s).");
            Debug.Log($"CARAVAN_LEVEL_VALIDATION_PASSED count={assets.Count}");
        }

        public static void GenerateAndValidate()
        {
            GeneratePrototypeLevels();
            ValidateAll();
        }

        public static void ValidateForBuild() => ValidateAll();

        private static List<LevelAsset> LoadAll() => AssetDatabase.FindAssets("t:LevelAsset", new[] { LevelFolder })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<LevelAsset>)
            .Where(asset => asset != null)
            .OrderBy(asset => asset.LevelId)
            .ToList();
    }
}
