using System.Collections.Generic;
using System.IO;
using System.Linq;
using CaravanSecrets.Data.Levels;
using CaravanSecrets.Features.Gameplay;
using CaravanSecrets.Game.Board;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CaravanSecrets.Editor.LevelEditor
{
    public sealed class CaravanLevelEditorWindow : EditorWindow
    {
        private enum GridTool { PaintCell, PlaceCart, PlaceCargo, PlaceGate, PlaceSwitch, PlaceStorage, PlaceDirectionTile, Remove, LinkCartDestination, LinkCargoDestination }

        private const string LevelFolder = "Assets/Resources/Levels";
        private LevelAsset _asset;
        private string _levelId = "desert_new";
        private string _regionId = "desert";
        private int _levelNumber;
        private int _width = 5;
        private int _height = 5;
        private int _recommendedMoves;
        private int _rewardCoins;
        private readonly List<CellEntry> _cells = new();
        private readonly List<CartEntry> _carts = new();
        private readonly List<DestinationEntry> _destinations = new();
        private readonly List<CargoEntry> _cargo = new();
        private readonly List<CargoDestinationEntry> _cargoDestinations = new();
        private readonly List<ObjectiveEntry> _objectives = new();
        private readonly List<GateEntry> _gates = new();
        private readonly List<SwitchEntry> _linkedSwitches = new();
        private readonly List<StorageEntry> _storageSlots = new();
        private readonly List<DirectionTileEntry> _directionTiles = new();
        private GridTool _tool;
        private CellType _cellType = CellType.Rock;
        private string _cartId = "a";
        private Direction _cartDirection = Direction.Right;
        private string _destinationCartId = "a";
        private string _cargoId = "cargo_01";
        private Direction _cargoDirection = Direction.Right;
        private CargoType _cargoType = CargoType.Spices;
        private CargoType _cargoDestinationType = CargoType.Spices;
        private string _gateId = "gate_01";
        private bool _gateInitiallyOpen;
        private string _switchId = "switch_01";
        private string _switchGateIds = "gate_01";
        private string _storageId = "storage_01";
        private int _storageCapacity = 1;
        private string _directionTileId = "turn_01";
        private Direction _tileDirection = Direction.Up;
        private Vector2 _scroll;
        private IReadOnlyList<string> _issues = new List<string>();
        private SolverResult _solver;

        [MenuItem("Caravan Secrets/Levels/Caravan Level Editor")]
        public static void Open() => GetWindow<CaravanLevelEditorWindow>("Caravan Level Editor");

        private void OnGUI()
        {
            _scroll = EditorGUILayout.BeginScrollView(_scroll);
            EditorGUI.BeginChangeCheck();
            var selected = (LevelAsset)EditorGUILayout.ObjectField("Level Asset", _asset, typeof(LevelAsset), false);
            if (selected != _asset) Load(selected);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Level Metadata", EditorStyles.boldLabel);
            _levelId = EditorGUILayout.TextField("Level ID", _levelId);
            _regionId = EditorGUILayout.TextField("Region ID", _regionId);
            _levelNumber = EditorGUILayout.IntField("Level Number", _levelNumber);
            _width = Mathf.Clamp(EditorGUILayout.IntField("Columns", _width), 2, 20);
            _height = Mathf.Clamp(EditorGUILayout.IntField("Rows", _height), 1, 20);
            _recommendedMoves = Mathf.Max(0, EditorGUILayout.IntField("Recommended Moves", _recommendedMoves));
            _rewardCoins = Mathf.Max(0, EditorGUILayout.IntField("Reward Coins", _rewardCoins));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Grid Tool", EditorStyles.boldLabel);
            _tool = (GridTool)EditorGUILayout.EnumPopup("Mode", _tool);
            if (_tool == GridTool.PaintCell) _cellType = (CellType)EditorGUILayout.EnumPopup("Cell Type", _cellType);
            if (_tool == GridTool.PlaceCart)
            {
                _cartId = EditorGUILayout.TextField("Cart ID", _cartId);
                _cartDirection = (Direction)EditorGUILayout.EnumPopup("Direction", _cartDirection);
            }
            if (_tool == GridTool.PlaceCargo)
            {
                _cargoId = EditorGUILayout.TextField("Cargo ID", _cargoId);
                _cargoDirection = (Direction)EditorGUILayout.EnumPopup("Direction", _cargoDirection);
                _cargoType = (CargoType)EditorGUILayout.EnumPopup("Cargo Symbol/Type", _cargoType);
            }
            if (_tool == GridTool.LinkCartDestination)
                _destinationCartId = EditorGUILayout.TextField("Cart ID", _destinationCartId);
            if (_tool == GridTool.LinkCargoDestination)
                _cargoDestinationType = (CargoType)EditorGUILayout.EnumPopup("Cargo Symbol/Type", _cargoDestinationType);
            if (_tool == GridTool.PlaceGate)
            {
                _gateId = EditorGUILayout.TextField("Gate ID", _gateId);
                _gateInitiallyOpen = EditorGUILayout.Toggle("Initially Open", _gateInitiallyOpen);
            }
            if (_tool == GridTool.PlaceSwitch)
            {
                _switchId = EditorGUILayout.TextField("Switch ID", _switchId);
                _switchGateIds = EditorGUILayout.TextField("Linked Gate IDs", _switchGateIds);
                EditorGUILayout.HelpBox("Separate multiple gate IDs with commas.", MessageType.None);
            }
            if (_tool == GridTool.PlaceStorage)
            {
                _storageId = EditorGUILayout.TextField("Storage ID", _storageId);
                _storageCapacity = Mathf.Max(1, EditorGUILayout.IntField("Capacity", _storageCapacity));
            }
            if (_tool == GridTool.PlaceDirectionTile)
            {
                _directionTileId = EditorGUILayout.TextField("Direction Tile ID", _directionTileId);
                _tileDirection = (Direction)EditorGUILayout.EnumPopup("New Direction", _tileDirection);
            }

            DrawObjectives();

            DrawGrid();
            DrawDestinationLinks();
            DrawActions();
            DrawResults();
            EditorGUILayout.EndScrollView();
            if (EditorGUI.EndChangeCheck()) Repaint();
        }

        private void DrawGrid()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Board — coordinates are (column, row)", EditorStyles.boldLabel);
            for (var y = _height - 1; y >= 0; y--)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label($"y={y}", GUILayout.Width(32));
                for (var x = 0; x < _width; x++)
                {
                    var position = new Vector2Int(x, y);
                    if (GUILayout.Button(CellLabel(position), GUILayout.Width(86), GUILayout.Height(52))) ApplyTool(position);
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private string CellLabel(Vector2Int position)
        {
            var cell = _cells.LastOrDefault(item => item.Position == position);
            var cart = _carts.LastOrDefault(item => item.Position == position);
            var destination = _destinations.LastOrDefault(item => item.Position == position);
            var cargo = _cargo.LastOrDefault(item => item.Position == position);
            var cargoDestination = _cargoDestinations.LastOrDefault(item => item.Position == position);
            var gate = _gates.LastOrDefault(item => item.Position == position);
            var linkedSwitch = _linkedSwitches.LastOrDefault(item => item.Position == position);
            var storage = _storageSlots.LastOrDefault(item => item.Position == position);
            var tile = _directionTiles.LastOrDefault(item => item.Position == position);
            var parts = new List<string> { $"({position.x},{position.y})" };
            if (_cells.Any(item => item.Position == position)) parts.Add(cell.Type.ToString());
            if (_carts.Any(item => item.Position == position)) parts.Add($"Cart {cart.Id} {Arrow(cart.Direction)}");
            if (_destinations.Any(item => item.Position == position)) parts.Add($"→ {destination.CartId}");
            if (_cargo.Any(item => item.Position == position)) parts.Add($"Cargo {cargo.Id} {cargo.Type} {Arrow(cargo.Direction)}");
            if (_cargoDestinations.Any(item => item.Position == position)) parts.Add($"Cargo target: {cargoDestination.Type}");
            if (_gates.Any(item => item.Position == position)) parts.Add($"Gate {gate.Id} {(gate.InitiallyOpen ? "open" : "closed")}");
            if (_linkedSwitches.Any(item => item.Position == position)) parts.Add($"Switch {linkedSwitch.Id} → {string.Join(",", linkedSwitch.GateIds)}");
            if (_storageSlots.Any(item => item.Position == position)) parts.Add($"Storage {storage.Id} cap={storage.Capacity}");
            if (_directionTiles.Any(item => item.Position == position)) parts.Add($"Turn {Arrow(tile.Direction)}");
            return string.Join("\n", parts);
        }

        private void ApplyTool(Vector2Int position)
        {
            switch (_tool)
            {
                case GridTool.PaintCell:
                    _cells.RemoveAll(item => item.Position == position);
                    if (_cellType != CellType.Empty) _cells.Add(new CellEntry(position, _cellType));
                    break;
                case GridTool.PlaceCart:
                    if (string.IsNullOrWhiteSpace(_cartId)) return;
                    _carts.RemoveAll(item => item.Position == position || item.Id == _cartId);
                    _carts.Add(new CartEntry(_cartId.Trim(), position, _cartDirection));
                    break;
                case GridTool.PlaceCargo:
                    if (string.IsNullOrWhiteSpace(_cargoId)) return;
                    _cargo.RemoveAll(item => item.Position == position || item.Id == _cargoId);
                    _cargo.Add(new CargoEntry(_cargoId.Trim(), position, _cargoDirection, _cargoType));
                    break;
                case GridTool.Remove:
                    var removedIds = _carts.Where(item => item.Position == position).Select(item => item.Id).ToArray();
                    _cells.RemoveAll(item => item.Position == position);
                    _carts.RemoveAll(item => item.Position == position);
                    _destinations.RemoveAll(item => item.Position == position || removedIds.Contains(item.CartId));
                    _cargo.RemoveAll(item => item.Position == position);
                    _cargoDestinations.RemoveAll(item => item.Position == position);
                    RemoveMechanismAt(position);
                    break;
                case GridTool.LinkCartDestination:
                    if (!_carts.Any(item => item.Id == _destinationCartId)) return;
                    _cells.RemoveAll(item => item.Position == position);
                    _cells.Add(new CellEntry(position, CellType.Exit));
                    _destinations.RemoveAll(item => item.CartId == _destinationCartId);
                    _destinations.Add(new DestinationEntry(_destinationCartId, position));
                    break;
                case GridTool.LinkCargoDestination:
                    _cargoDestinations.RemoveAll(item => item.Position == position);
                    _cargoDestinations.Add(new CargoDestinationEntry(position, _cargoDestinationType));
                    break;
                case GridTool.PlaceGate:
                    if (string.IsNullOrWhiteSpace(_gateId)) return;
                    RemoveMechanismAt(position);
                    _gates.RemoveAll(item => item.Id == _gateId);
                    _gates.Add(new GateEntry(_gateId.Trim(), position, _gateInitiallyOpen));
                    break;
                case GridTool.PlaceSwitch:
                    if (string.IsNullOrWhiteSpace(_switchId)) return;
                    RemoveMechanismAt(position);
                    _linkedSwitches.RemoveAll(item => item.Id == _switchId);
                    var gateIds = _switchGateIds.Split(',').Select(item => item.Trim()).Where(item => item.Length > 0);
                    _linkedSwitches.Add(new SwitchEntry(_switchId.Trim(), position, gateIds));
                    break;
                case GridTool.PlaceStorage:
                    if (string.IsNullOrWhiteSpace(_storageId)) return;
                    RemoveMechanismAt(position);
                    _storageSlots.RemoveAll(item => item.Id == _storageId);
                    _storageSlots.Add(new StorageEntry(_storageId.Trim(), position, Mathf.Max(1, _storageCapacity)));
                    break;
                case GridTool.PlaceDirectionTile:
                    if (string.IsNullOrWhiteSpace(_directionTileId)) return;
                    RemoveMechanismAt(position);
                    _directionTiles.RemoveAll(item => item.Id == _directionTileId);
                    _directionTiles.Add(new DirectionTileEntry(_directionTileId.Trim(), position, _tileDirection));
                    break;
            }
            _issues = new List<string>();
            _solver = null;
        }

        private void RemoveMechanismAt(Vector2Int position)
        {
            _gates.RemoveAll(item => item.Position == position);
            _linkedSwitches.RemoveAll(item => item.Position == position);
            _storageSlots.RemoveAll(item => item.Position == position);
            _directionTiles.RemoveAll(item => item.Position == position);
        }

        private void DrawDestinationLinks()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Destination Links", EditorStyles.boldLabel);
            if (_destinations.Count == 0) EditorGUILayout.LabelField("No destination links. Unlinked carts may use any exit.");
            foreach (var link in _destinations)
                EditorGUILayout.LabelField($"Cart {link.CartId} → Exit ({link.Position.x}, {link.Position.y})");
            foreach (var link in _cargoDestinations)
                EditorGUILayout.LabelField($"Cargo {link.Type} → Target ({link.Position.x}, {link.Position.y})");
        }

        private void DrawObjectives()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Typed Objectives", EditorStyles.boldLabel);
            foreach (ObjectiveType type in System.Enum.GetValues(typeof(ObjectiveType)))
            {
                var enabled = _objectives.Any(item => item.Type == type);
                var requested = EditorGUILayout.ToggleLeft(type.ToString(), enabled);
                if (requested == enabled) continue;
                _objectives.RemoveAll(item => item.Type == type);
                if (requested) _objectives.Add(new ObjectiveEntry($"objective_{type.ToString().ToLowerInvariant()}", type));
            }
        }

        private void DrawActions()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("New")) NewDraft();
            if (GUILayout.Button("Save")) Save();
            if (GUILayout.Button("Duplicate")) Duplicate();
            if (GUILayout.Button("Validate")) ValidateDraft();
            if (GUILayout.Button("Solve")) SolveDraft();
            if (GUILayout.Button("Play-test")) Playtest();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawResults()
        {
            if (_issues.Count > 0)
            {
                EditorGUILayout.Space();
                foreach (var issue in _issues) EditorGUILayout.HelpBox(issue, MessageType.Error);
            }
            if (_solver != null)
            {
                var message = $"{_solver.Status}: {_solver.Message}\nVisited: {_solver.VisitedStates}";
                if (_solver.Status == SolverStatus.Solved)
                    message += $"\nMinimum moves: {_solver.MinimumMoves}\nSequence: {string.Join(" → ", _solver.Moves)}";
                EditorGUILayout.HelpBox(message, _solver.Status == SolverStatus.Solved ? MessageType.Info : MessageType.Warning);
            }
        }

        private void NewDraft()
        {
            _asset = null;
            _levelId = "desert_new";
            _regionId = "desert";
            _levelNumber = _recommendedMoves = _rewardCoins = 0;
            _width = _height = 5;
            _cells.Clear(); _carts.Clear(); _destinations.Clear(); _cargo.Clear(); _cargoDestinations.Clear(); _objectives.Clear();
            _gates.Clear(); _linkedSwitches.Clear(); _storageSlots.Clear(); _directionTiles.Clear();
            _issues = new List<string>(); _solver = null;
        }

        private void Load(LevelAsset asset)
        {
            _asset = asset;
            if (asset == null) { NewDraft(); return; }
            var definition = asset.ToDefinition();
            _levelId = definition.Id; _regionId = definition.RegionId; _levelNumber = definition.LevelNumber;
            _width = definition.Width; _height = definition.Height; _recommendedMoves = definition.RecommendedMoves; _rewardCoins = definition.RewardCoins;
            _cells.Clear(); _cells.AddRange(asset.Cells);
            _carts.Clear(); _carts.AddRange(asset.Carts);
            _destinations.Clear(); _destinations.AddRange(asset.Destinations);
            _cargo.Clear(); _cargo.AddRange(asset.Cargo);
            _cargoDestinations.Clear(); _cargoDestinations.AddRange(asset.CargoDestinations);
            _objectives.Clear(); _objectives.AddRange(asset.Objectives);
            _gates.Clear(); _gates.AddRange(asset.Gates);
            _linkedSwitches.Clear(); _linkedSwitches.AddRange(asset.LinkedSwitches);
            _storageSlots.Clear(); _storageSlots.AddRange(asset.StorageSlots);
            _directionTiles.Clear(); _directionTiles.AddRange(asset.DirectionTiles);
            _issues = new List<string>(); _solver = null;
        }

        private LevelDefinition DraftDefinition()
        {
            var cells = _cells.GroupBy(item => item.Position).ToDictionary(group => new GridPosition(group.Key.x, group.Key.y), group => group.Last().Type);
            var carts = _carts.Select(item => new CartDefinition(item.Id, new GridPosition(item.Position.x, item.Position.y), item.Direction)).ToArray();
            var destinations = _destinations.GroupBy(item => item.CartId).ToDictionary(group => group.Key,
                group => new GridPosition(group.Last().Position.x, group.Last().Position.y));
            var cargo = _cargo.Select(item => new CargoDefinition(item.Id, new GridPosition(item.Position.x, item.Position.y), item.Direction, item.Type)).ToArray();
            var cargoDestinations = _cargoDestinations.GroupBy(item => item.Position).ToDictionary(
                group => new GridPosition(group.Key.x, group.Key.y), group => group.Last().Type);
            var objectives = _objectives.Select(item => new ObjectiveDefinition(item.Id, item.Type)).ToArray();
            var gates = _gates.Select(item => new GateDefinition(item.Id, new GridPosition(item.Position.x, item.Position.y), item.InitiallyOpen)).ToArray();
            var switches = _linkedSwitches.Select(item => new SwitchDefinition(item.Id,
                new GridPosition(item.Position.x, item.Position.y), item.GateIds)).ToArray();
            var storage = _storageSlots.Select(item => new StorageDefinition(item.Id,
                new GridPosition(item.Position.x, item.Position.y), item.Capacity)).ToArray();
            var directionTiles = _directionTiles.Select(item => new DirectionTileDefinition(item.Id,
                new GridPosition(item.Position.x, item.Position.y), item.Direction)).ToArray();
            return new LevelDefinition(_levelId, _width, _height, cells, carts, destinations,
                _regionId, _levelNumber, _recommendedMoves, _rewardCoins, cargo, cargoDestinations, objectives,
                gates, switches, storage, directionTiles);
        }

        private void ValidateDraft()
        {
            var issues = new List<string>();
            foreach (var duplicate in _cells.GroupBy(item => item.Position).Where(group => group.Count() > 1))
                issues.Add($"Multiple cells occupy {duplicate.Key}.");
            foreach (var duplicate in _carts.GroupBy(item => item.Id).Where(group => group.Count() > 1))
                issues.Add($"Duplicate cart ID: {duplicate.Key}.");
            foreach (var duplicate in _carts.GroupBy(item => item.Position).Where(group => group.Count() > 1))
                issues.Add($"Multiple carts occupy {duplicate.Key}.");
            foreach (var duplicate in _destinations.GroupBy(item => item.CartId).Where(group => group.Count() > 1))
                issues.Add($"Cart {duplicate.Key} has multiple destination links.");
            foreach (var duplicate in _cargo.GroupBy(item => item.Id).Where(group => group.Count() > 1))
                issues.Add($"Duplicate cargo ID: {duplicate.Key}.");
            foreach (var duplicate in _cargo.GroupBy(item => item.Position).Where(group => group.Count() > 1))
                issues.Add($"Multiple cargo objects occupy {duplicate.Key}.");
            foreach (var duplicate in _cargoDestinations.GroupBy(item => item.Position).Where(group => group.Count() > 1))
                issues.Add($"Multiple cargo destinations occupy {duplicate.Key}.");
            var mechanismIds = _gates.Select(item => item.Id).Concat(_linkedSwitches.Select(item => item.Id))
                .Concat(_storageSlots.Select(item => item.Id)).Concat(_directionTiles.Select(item => item.Id));
            foreach (var duplicate in mechanismIds.GroupBy(id => id).Where(group => group.Count() > 1))
                issues.Add($"Duplicate mechanism ID: {duplicate.Key}.");

            var draft = DraftDefinition();
            issues.AddRange(LevelValidator.Validate(draft));
            var otherDefinitions = AssetDatabase.FindAssets("t:LevelAsset", new[] { LevelFolder })
                .Select(AssetDatabase.GUIDToAssetPath).Select(AssetDatabase.LoadAssetAtPath<LevelAsset>)
                .Where(asset => asset != null && asset != _asset).Select(asset => asset.ToDefinition()).Append(draft);
            issues.AddRange(LevelValidator.ValidateCollection(otherDefinitions)
                .Where(issue => issue.StartsWith("Duplicate level number")));
            _issues = issues.Distinct().ToArray();
            _solver = null;
        }

        private void SolveDraft()
        {
            ValidateDraft();
            if (_issues.Count == 0) _solver = LevelSolver.Solve(DraftDefinition(), new SolverOptions { MaxVisitedStates = 100000, MaxDepth = 200 });
        }

        private void Save()
        {
            ValidateDraft();
            if (_issues.Count > 0) return;
            Directory.CreateDirectory(LevelFolder);
            if (_asset == null)
            {
                var path = AssetDatabase.GenerateUniqueAssetPath($"{LevelFolder}/{_levelId}.asset");
                _asset = CreateInstance<LevelAsset>();
                AssetDatabase.CreateAsset(_asset, path);
            }
            Undo.RecordObject(_asset, "Save caravan level");
            _asset.Configure(_levelId, _width, _height, _cells, _carts, _destinations);
            _asset.ConfigureMetadata(_regionId, _levelNumber, _recommendedMoves, _rewardCoins);
            _asset.ConfigureExpansion(_cargo, _cargoDestinations, _objectives);
            _asset.ConfigureMechanisms(_gates, _linkedSwitches, _storageSlots, _directionTiles);
            EditorUtility.SetDirty(_asset);
            AssetDatabase.SaveAssets();
            Selection.activeObject = _asset;
        }

        private void Duplicate()
        {
            if (_asset == null) { Save(); if (_asset == null) return; }
            var source = AssetDatabase.GetAssetPath(_asset);
            var target = AssetDatabase.GenerateUniqueAssetPath(Path.Combine(Path.GetDirectoryName(source) ?? LevelFolder,
                $"{Path.GetFileNameWithoutExtension(source)}_copy.asset").Replace('\\', '/'));
            if (!AssetDatabase.CopyAsset(source, target)) return;
            AssetDatabase.SaveAssets();
            Load(AssetDatabase.LoadAssetAtPath<LevelAsset>(target));
            Selection.activeObject = _asset;
        }

        private void Playtest()
        {
            ValidateDraft();
            if (_issues.Count > 0) return;
            GameplayPlaytestOverride.PendingLevel = DraftDefinition();
            EditorSceneManager.OpenScene("Assets/Scenes/Gameplay/Gameplay.unity");
            EditorApplication.isPlaying = true;
        }

        private static string Arrow(Direction direction) => direction switch
        {
            Direction.Up => "↑", Direction.Right => "→", Direction.Down => "↓", Direction.Left => "←", _ => "?"
        };
    }
}
