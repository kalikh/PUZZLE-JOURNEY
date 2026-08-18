using System.Collections.Generic;
using System.Collections;
using System.Linq;
using CaravanSecrets.Data.Levels;
using CaravanSecrets.Game.Board;
using CaravanSecrets.Game.Boosters;
using CaravanSecrets.Game.Journey;
using CaravanSecrets.Features.Journey;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Localization.Settings;

namespace CaravanSecrets.Features.Gameplay
{
    public sealed class GameplayController : MonoBehaviour
    {
        private const float CellSize = 1.25f;
        private readonly List<GameObject> _visuals = new();
        private readonly Dictionary<string, SpriteRenderer> _cartRenderers = new();
        private readonly Dictionary<string, SpriteRenderer> _cargoRenderers = new();
        private readonly List<SpriteRenderer> _rockRenderers = new();
        private readonly List<SpriteRenderer> _gateRenderers = new();
        private readonly Dictionary<GridPosition, SpriteRenderer> _gateByPosition = new();
        private readonly Dictionary<GridPosition, SpriteRenderer> _linkedGateByPosition = new();
        private readonly List<SpriteRenderer> _switchRenderers = new();
        private BoardGame _game;
        private Camera _camera;
        private Sprite _squareSprite;
        private Sprite _arrowSprite;
        private Sprite _circleSprite;
        private GameObject _cartPrefab;
        private GameObject _rockPrefab;
        private GameObject _gatePrefab;
        private GameObject _roadPrefab;
        private GameObject _roadStripPrefab;
        private GameObject _switchPrefab;
        private GameObject _backgroundPrefab;
        private GameObject _backgroundInstance;
        private GameObject _hudPrefab;
        private GameplayHudView _hud;
        private GameplayFeedback _feedback;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        private DevelopmentLevelBrowser _levelBrowser;
#endif
        private string _selectedObjectId;
        private bool _isPaused;
        private bool _switchActivatedVisual;
        private IReadOnlyList<LevelDefinition> _levels;
        private int _levelIndex;
        private string _message = string.Empty;
        private bool _isAnimating;
        private bool _journeyTransitioning;
        private RepresentativeJourneyPresenter _journeyPresenter;

        private static readonly Color Sand = new(0.86f, 0.70f, 0.42f);
        private static readonly Color SandDark = new(0.66f, 0.47f, 0.24f);
        private static readonly Color Turquoise = new(0.12f, 0.67f, 0.66f);
        private static readonly Color CartGold = new(0.93f, 0.58f, 0.16f);

        private void Awake()
        {
            _feedback = gameObject.AddComponent<GameplayFeedback>();
            _camera = Camera.main;
            if (_camera == null)
            {
                var cameraObject = new GameObject("Main Camera", typeof(Camera));
                cameraObject.tag = "MainCamera";
                _camera = cameraObject.GetComponent<Camera>();
            }
            _camera.orthographic = true;
            _camera.transform.position = new Vector3(0, 0, -10);
            _camera.backgroundColor = new Color(0.10f, 0.16f, 0.20f);
            _squareSprite = CreateSquareSprite();
            _arrowSprite = CreateArrowSprite();
            _circleSprite = CreateCircleSprite();
            _cartPrefab = Resources.Load<GameObject>("VerticalSlice/Cart");
            _rockPrefab = Resources.Load<GameObject>("VerticalSlice/Rock");
            _gatePrefab = Resources.Load<GameObject>("VerticalSlice/Gate");
            _roadPrefab = Resources.Load<GameObject>("VerticalSlice/RoadTile");
            _roadStripPrefab = Resources.Load<GameObject>("VerticalSlice/RoadStrip");
            _switchPrefab = Resources.Load<GameObject>("VerticalSlice/DesertSwitch");
            _backgroundPrefab = Resources.Load<GameObject>("VerticalSlice/DesertBackground");
            _hudPrefab = Resources.Load<GameObject>("VerticalSlice/GameplayHUD");
            var assets = Resources.LoadAll<LevelAsset>("Levels").OrderBy(asset => asset.LevelId).ToArray();
            _levels = assets.Length > 0 ? assets.Select(asset => asset.ToDefinition()).ToArray() : PrototypeLevels.All;
#if UNITY_EDITOR
            if (GameplayPlaytestOverride.PendingLevel != null)
            {
                _levels = new[] { GameplayPlaytestOverride.PendingLevel };
                GameplayPlaytestOverride.PendingLevel = null;
            }
#endif
            Debug.Log($"CARAVAN_LEVELS_LOADED count={_levels.Count}");
            LoadLevel(0);
            CreateHud();
            if (_levels.Count > 1 && _levels[0].Id == "desert_01")
            {
                _journeyPresenter = gameObject.AddComponent<RepresentativeJourneyPresenter>();
                _journeyPresenter.Initialize(_camera, _camera.orthographicSize);
                StartCoroutine(BeginRepresentativeJourney());
            }
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            _levelBrowser = gameObject.AddComponent<DevelopmentLevelBrowser>();
#endif
        }

        private void Update()
        {
            if (TryGetPointerDown(out var screenPosition))
            {
                HandleTap(screenPosition);
                RenderHud();
            }
        }

        private void LoadLevel(int index)
        {
            _levelIndex = Mathf.Clamp(index, 0, _levels.Count - 1);
            _game = new BoardGame(_levels[_levelIndex]);
            _message = string.Empty;
            BuildBoard();
            if (index != 0 && _camera != null)
                _camera.transform.position = new Vector3(0, 0, -10);
            RenderHud();
        }

        private void CreateHud()
        {
            if (_hudPrefab == null) return;
            _hud = Instantiate(_hudPrefab).GetComponent<GameplayHudView>();
            _hud.Bind(TogglePause, UndoFromHud, MoveSelectedObject, RestartFromHud, HintFromHud);
            RenderHud();
        }

        private void RenderHud()
        {
            if (_hud == null || _game == null) return;
            string objective;
            if (!string.IsNullOrEmpty(_message)) objective = _message;
            else if (_isPaused) objective = GameplayStrings.Get("status.paused");
            else objective = GameplayObjectiveText.Resolve(_levelIndex, _levels[_levelIndex].Objectives, key => GameplayStrings.Get(key));
            _hud.Render(_levelIndex + 1, _levels.Count, _game.State.MoveCount, objective, _isPaused, _game.State.IsComplete);
        }

        private void TogglePause()
        {
            _isPaused = !_isPaused;
            Time.timeScale = _isPaused ? 0 : 1;
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            if (_isPaused) _levelBrowser?.Show(_levels.Count, _levelIndex, OpenLevelFromBrowser, ToggleLanguageFromBrowser, UseCompassFromBrowser);
            else _levelBrowser?.Hide();
#endif
            RenderHud();
        }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
        private void OpenLevelFromBrowser(int index)
        {
            _isPaused = false;
            Time.timeScale = 1;
            _levelBrowser?.Hide();
            LoadLevel(index);
        }

        private void ToggleLanguageFromBrowser()
        {
            var next = GameplayStrings.IsArabic ? "en" : "ar";
            var locale = LocalizationSettings.AvailableLocales.GetLocale(next);
            if (locale != null) LocalizationSettings.SelectedLocale = locale;
            _message = string.Empty;
            _levelBrowser?.RefreshLocalizedChrome();
            RenderHud();
        }

        private void UseCompassFromBrowser()
        {
            if (_game == null || _game.State.IsComplete) return;
            var result = new CompassBooster().Use(new BoosterRequest(_game));
            _message = result.Applied && !string.IsNullOrEmpty(result.SuggestedObjectId)
                ? GameplayStrings.Get("status.compass_hint", result.SuggestedObjectId)
                : GameplayStrings.Get("status.compass_unavailable");
            RenderHud();
        }
#endif

        private void UndoFromHud()
        {
            if (_journeyTransitioning || _isAnimating || !_game.Undo()) return;
            _message = string.Empty;
            RefreshCarts();
            RenderHud();
        }

        private void RestartFromHud()
        {
            if (_journeyTransitioning || _isAnimating) return;
            LoadLevel(_levelIndex);
        }

        private void HintFromHud()
        {
            if (_game.State.IsComplete)
            {
                if (_levelIndex == 0 && _journeyPresenter != null &&
                    _journeyPresenter.Session.Phase == JourneyPhase.AtPuzzle)
                {
                    StartCoroutine(CompleteRepresentativeJourney());
                    return;
                }
                LoadLevel((_levelIndex + 1) % _levels.Count);
                return;
            }
            var hintIndex = Mathf.Clamp(_levelIndex, 0, 4);
            _message = GameplayStrings.Get($"hint.{hintIndex + 1}");
            RenderHud();
        }

        private void BuildBoard()
        {
            foreach (var visual in _visuals) Destroy(visual);
            _visuals.Clear();
            _cartRenderers.Clear();
            _cargoRenderers.Clear();
            _rockRenderers.Clear();
            _gateRenderers.Clear();
            _gateByPosition.Clear();
            _linkedGateByPosition.Clear();
            _switchRenderers.Clear();
            _switchActivatedVisual = false;

            var state = _game.State;
            CreateRoadStrip(state);
            for (var y = 0; y < state.Height; y++)
            for (var x = 0; x < state.Width; x++)
            {
                var position = new GridPosition(x, y);
                switch (state.GetCell(position))
                {
                    case CellType.Rock:
                        var rock = CreateArtVisual(_rockPrefab, "Rock", position, new Vector2(CellSize * 0.78f, CellSize * 0.78f), 15, SandDark);
                        rock.transform.rotation = Quaternion.Euler(0, 0, (x * 31 + y * 17) % 25 - 12);
                        _rockRenderers.Add(rock.GetComponent<SpriteRenderer>());
                        break;
                    case CellType.Exit:
                        CreateGate(position);
                        break;
                    case CellType.Switch:
                        CreateSwitch(position);
                        break;
                    case CellType.Storage:
                        CreateStorage(position);
                        break;
                }
            }

            var definition = _levels[_levelIndex];
            foreach (var gate in definition.Gates) CreateLinkedGate(gate);
            foreach (var item in definition.Switches) CreateLinkedSwitch(item);
            foreach (var storage in definition.StorageSlots) CreateCapacityStorage(storage);
            foreach (var tile in definition.DirectionTiles) CreateDirectionTile(tile);

            foreach (var cart in state.Carts)
            {
                var renderer = CreateCart(cart);
                _cartRenderers[cart.Id] = renderer;
            }
            foreach (var destination in _levels[_levelIndex].CargoDestinations)
                CreateCargoDestination(destination.Key, destination.Value);
            foreach (var cargo in state.Cargo)
                _cargoRenderers[cargo.Id] = CreateCargo(cargo);

            var width = state.Width * CellSize;
            var height = state.Height * CellSize;
            var halfHeightNeeded = height * 0.5f + CellSize * 0.8f;
            var halfWidthNeeded = width * 0.5f / Mathf.Max(0.2f, _camera.aspect) + CellSize * 0.4f;
            _camera.orthographicSize = Mathf.Max(halfHeightNeeded, halfWidthNeeded);
            FitBackground();
        }

        private void CreateRoadStrip(BoardState state)
        {
            if (_roadStripPrefab == null) return;
            foreach (var cart in state.Carts)
            {
                var end = state.TryGetDestination(cart.Id, out var destination) ? destination : cart.Direction switch
                {
                    Direction.Up => new GridPosition(cart.Position.X, state.Height - 1),
                    Direction.Right => new GridPosition(state.Width - 1, cart.Position.Y),
                    Direction.Down => new GridPosition(cart.Position.X, 0),
                    Direction.Left => new GridPosition(0, cart.Position.Y),
                    _ => cart.Position
                };
                CreateTrack(ToWorld(cart.Position), ToWorld(end), CellSize * 0.56f);
            }
            foreach (var cargo in state.Cargo)
            {
                var end = cargo.Direction switch
                {
                    Direction.Up => new GridPosition(cargo.Position.X, state.Height - 1),
                    Direction.Right => new GridPosition(state.Width - 1, cargo.Position.Y),
                    Direction.Down => new GridPosition(cargo.Position.X, 0),
                    Direction.Left => new GridPosition(0, cargo.Position.Y),
                    _ => cargo.Position
                };
                CreateTrack(ToWorld(cargo.Position), ToWorld(end), CellSize * 0.46f);
            }
        }

        private void CreateTrack(Vector3 start, Vector3 end, float thickness)
        {
            var item = Instantiate(_roadStripPrefab, Vector3.zero, Quaternion.identity, transform);
            item.name = "Natural Desert Track";
            var renderer = item.GetComponent<SpriteRenderer>();
            if (renderer != null && renderer.sprite != null)
            {
                var size = renderer.sprite.bounds.size;
                var length = Vector3.Distance(start, end) + CellSize * 1.08f;
                item.transform.localScale = new Vector3(length / Mathf.Max(0.01f, size.x), thickness / Mathf.Max(0.01f, size.y), 1);
                renderer.sortingOrder = 0;
            }
            item.transform.position = (start + end) * 0.5f;
            var direction = end - start;
            item.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
            _visuals.Add(item);
        }

        private SpriteRenderer CreateCart(CartState cart)
        {
            var bodyObject = CreateArtVisual(_cartPrefab, cart.Id, cart.Position, new Vector2(CellSize * 0.86f, CellSize * 0.68f), 30, CartGold);
            var body = bodyObject.GetComponent<SpriteRenderer>();
            body.transform.rotation = Quaternion.Euler(0, 0, Rotation(cart.Direction));
            AddChild(body.transform, "Direction Arrow", _arrowSprite, Color.white, new Vector3(0.15f, 0.02f, -0.1f), new Vector3(0.38f, 0.52f, 1), 35);
            AddMatchMarker(body.transform, cart.Id, new Vector3(-0.18f, -0.17f, -0.12f), 38);
            return body;
        }

        private SpriteRenderer CreateCargo(CargoState cargo)
        {
            var bodyObject = CreateArtVisual(_cartPrefab, cargo.Id, cargo.Position,
                new Vector2(CellSize * 0.68f, CellSize * 0.54f), 31, CargoColor(cargo.Type));
            var body = bodyObject.GetComponent<SpriteRenderer>();
            body.transform.rotation = Quaternion.Euler(0, 0, Rotation(cargo.Direction));
            AddChild(body.transform, "Direction Arrow", _arrowSprite, Color.white,
                new Vector3(0.13f, 0.02f, -0.1f), new Vector3(0.32f, 0.45f, 1), 35);
            AddCargoSymbol(body.transform, cargo.Type, 39);
            return body;
        }

        private void CreateCargoDestination(GridPosition position, CargoType type)
        {
            var root = new GameObject($"Cargo Destination {type}");
            root.transform.SetParent(transform, false);
            root.transform.position = ToWorld(position);
            _visuals.Add(root);
            AddChild(root.transform, "Destination Ring", _circleSprite, new Color(0.08f, 0.25f, 0.22f, 0.75f),
                Vector3.zero, new Vector3(CellSize * 0.66f, CellSize * 0.66f, 1), 8);
            AddCargoSymbol(root.transform, type, 12);
        }

        private void AddCargoSymbol(Transform parent, CargoType type, int order)
        {
            var count = 1 + (int)type % 4;
            var sprite = (int)type % 2 == 0 ? _circleSprite : _squareSprite;
            for (var index = 0; index < count; index++)
            {
                var angle = count == 1 ? 0f : index * Mathf.PI * 2f / count;
                var offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), -0.15f) * (count == 1 ? 0f : 0.13f);
                AddChild(parent, $"Cargo Symbol {type} {index + 1}", sprite, Color.white, offset,
                    Vector3.one * (count == 1 ? 0.22f : 0.14f), order);
            }
        }

        private static Color CargoColor(CargoType type) => type switch
        {
            CargoType.Spices => new Color(0.78f, 0.24f, 0.12f), CargoType.Fabrics => new Color(0.52f, 0.18f, 0.62f),
            CargoType.Water => new Color(0.10f, 0.55f, 0.82f), CargoType.MetalParts => new Color(0.38f, 0.42f, 0.45f),
            CargoType.Artifacts => new Color(0.88f, 0.65f, 0.12f), CargoType.Tools => new Color(0.55f, 0.32f, 0.16f),
            CargoType.Food => new Color(0.28f, 0.65f, 0.22f), _ => new Color(0.82f, 0.76f, 0.62f)
        };

        private void CreateGate(GridPosition position)
        {
            if (_gatePrefab != null)
            {
                var gate = CreateArtVisual(_gatePrefab, "Exit Gate", position, new Vector2(CellSize * 0.9f, CellSize * 0.95f), 20, Turquoise);
                var gateRenderer = gate.GetComponent<SpriteRenderer>();
                _gateRenderers.Add(gateRenderer);
                _gateByPosition[position] = gateRenderer;
                AddMatchMarker(gate.transform, _game.State.GetDestinationCartId(position), new Vector3(0, 0.34f, -0.12f), 40);
                return;
            }
            var root = new GameObject("Exit Gate");
            root.transform.SetParent(transform, false);
            root.transform.position = ToWorld(position);
            _visuals.Add(root);
            AddChild(root.transform, "Portal Glow", _squareSprite, new Color(0.08f, 0.85f, 0.82f, 0.38f), Vector3.zero, new Vector3(CellSize * 0.62f, CellSize * 0.68f, 1), 1);
            AddChild(root.transform, "Left Pillar", _squareSprite, Turquoise, new Vector3(-0.38f, -0.02f, -0.05f), new Vector3(0.18f, 0.82f, 1), 2);
            AddChild(root.transform, "Right Pillar", _squareSprite, Turquoise, new Vector3(0.38f, -0.02f, -0.05f), new Vector3(0.18f, 0.82f, 1), 2);
            AddChild(root.transform, "Gate Top", _squareSprite, Turquoise, new Vector3(0, 0.39f, -0.05f), new Vector3(0.94f, 0.18f, 1), 2);
            AddMatchMarker(root.transform, _game.State.GetDestinationCartId(position), new Vector3(0, 0.2f, -0.1f), 5);
        }

        private void AddMatchMarker(Transform parent, string cartId, Vector3 position, int order)
        {
            if (string.IsNullOrEmpty(cartId)) return;
            var sprite = cartId == "a" ? _circleSprite : cartId == "b" ? _squareSprite : _arrowSprite;
            var color = cartId == "a" ? new Color(1f, 0.78f, 0.12f) : cartId == "b" ? new Color(0.9f, 0.22f, 0.16f) : new Color(0.1f, 0.9f, 0.88f);
            AddChild(parent, $"Destination {cartId}", sprite, color, position, new Vector3(0.2f, 0.2f, 1), order);
        }

        private void CreateStorage(GridPosition position)
        {
            if (_switchPrefab != null)
            {
                var item = CreateArtVisual(_switchPrefab, "Temporary Caravan Bay", position, new Vector2(CellSize * 0.72f, CellSize * 0.52f), 16, SandDark);
                var renderer = item.GetComponent<SpriteRenderer>();
                if (renderer != null) renderer.color = new Color(0.62f, 0.72f, 0.68f, 1f);
                return;
            }
            CreateVisual("Temporary Caravan Bay", position, new Color(0.35f, 0.55f, 0.48f), new Vector3(CellSize * 0.7f, CellSize * 0.55f, 1), 16);
        }

        private void CreateLinkedGate(GateDefinition gate)
        {
            var item = CreateArtVisual(_gatePrefab, $"Linked Gate {gate.Id}", gate.Position,
                new Vector2(CellSize * 0.82f, CellSize * 0.9f), 24, new Color(0.12f, 0.48f, 0.52f));
            var renderer = item.GetComponent<SpriteRenderer>();
            if (renderer != null) _linkedGateByPosition[gate.Position] = renderer;
            AddMatchMarker(item.transform, gate.Id, new Vector3(0, 0.3f, -0.12f), 41);
        }

        private void CreateLinkedSwitch(SwitchDefinition item)
        {
            var visual = CreateArtVisual(_switchPrefab, $"Linked Switch {item.Id}", item.Position,
                new Vector2(CellSize * 0.62f, CellSize * 0.46f), 17, new Color(0.16f, 0.72f, 0.42f));
            var renderer = visual.GetComponent<SpriteRenderer>();
            if (renderer != null) _switchRenderers.Add(renderer);
        }

        private void CreateCapacityStorage(StorageDefinition storage)
        {
            CreateStorage(storage.Position);
            var root = new GameObject($"Storage Capacity {storage.Capacity}");
            root.transform.SetParent(transform, false);
            root.transform.position = ToWorld(storage.Position);
            _visuals.Add(root);
            for (var index = 0; index < storage.Capacity; index++)
                AddChild(root.transform, $"Capacity Slot {index + 1}", _circleSprite, Color.white,
                    new Vector3((index - (storage.Capacity - 1) * 0.5f) * 0.16f, -0.24f, -0.12f), Vector3.one * 0.09f, 22);
        }

        private void CreateDirectionTile(DirectionTileDefinition tile)
        {
            var root = new GameObject($"Direction Tile {tile.Id}");
            root.transform.SetParent(transform, false);
            root.transform.position = ToWorld(tile.Position);
            _visuals.Add(root);
            AddChild(root.transform, "Direction Tile Base", _circleSprite, new Color(0.12f, 0.32f, 0.58f, 0.82f),
                Vector3.zero, Vector3.one * (CellSize * 0.5f), 13);
            var arrow = AddChild(root.transform, $"Direction {tile.Direction}", _arrowSprite, Color.white,
                new Vector3(0, 0, -0.12f), Vector3.one * 0.34f, 18);
            arrow.transform.rotation = Quaternion.Euler(0, 0, Rotation(tile.Direction));
        }

        private GameObject CreateArtVisual(GameObject prefab, string name, GridPosition position, Vector2 targetSize, int order, Color fallbackColor)
        {
            if (prefab == null) return CreateVisual(name, position, fallbackColor, new Vector3(targetSize.x, targetSize.y, 1), order);
            var item = Instantiate(prefab, ToWorld(position), Quaternion.identity, transform);
            item.name = name;
            var renderer = item.GetComponent<SpriteRenderer>();
            if (renderer != null && renderer.sprite != null)
            {
                var size = renderer.sprite.bounds.size;
                item.transform.localScale = new Vector3(targetSize.x / Mathf.Max(0.01f, size.x), targetSize.y / Mathf.Max(0.01f, size.y), 1);
                renderer.sortingOrder = order;
            }
            _visuals.Add(item);
            return item;
        }

        private void FitBackground()
        {
            if (_backgroundPrefab == null) return;
            if (_backgroundInstance == null) _backgroundInstance = Instantiate(_backgroundPrefab, Vector3.zero, Quaternion.identity);
            var renderer = _backgroundInstance.GetComponent<SpriteRenderer>();
            if (renderer == null || renderer.sprite == null) return;
            var visibleHeight = _camera.orthographicSize * 2f;
            var visibleWidth = visibleHeight * _camera.aspect;
            var spriteSize = renderer.sprite.bounds.size;
            var scale = Mathf.Max(visibleWidth / spriteSize.x, visibleHeight / spriteSize.y);
            _backgroundInstance.transform.localScale = Vector3.one * scale;
            _backgroundInstance.transform.position = new Vector3(0, 0, 5);
        }

        private void CreateSwitch(GridPosition position)
        {
            if (_switchPrefab != null)
            {
                var item = CreateArtVisual(_switchPrefab, "Desert Pressure Switch", position, new Vector2(CellSize * 0.82f, CellSize * 0.62f), 18, Turquoise);
                var renderer = item.GetComponent<SpriteRenderer>();
                if (renderer != null) _switchRenderers.Add(renderer);
                return;
            }
            var root = new GameObject("Barrier Switch");
            root.transform.SetParent(transform, false);
            root.transform.position = ToWorld(position);
            _visuals.Add(root);
            AddChild(root.transform, "Switch Base", _circleSprite, new Color(0.08f, 0.42f, 0.22f), Vector3.zero, new Vector3(0.72f, 0.72f, 1), 1);
            AddChild(root.transform, "Switch Light", _circleSprite, new Color(0.25f, 0.95f, 0.42f), new Vector3(0, 0, -0.05f), new Vector3(0.42f, 0.42f, 1), 2);
        }

        private static SpriteRenderer AddChild(Transform parent, string name, Sprite sprite, Color color, Vector3 position, Vector3 scale, int order)
        {
            var child = new GameObject(name, typeof(SpriteRenderer));
            child.transform.SetParent(parent, false);
            child.transform.localPosition = position;
            child.transform.localScale = scale;
            var renderer = child.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.color = color;
            renderer.sortingOrder = order;
            return renderer;
        }

        private GameObject CreateVisual(string name, GridPosition position, Color color, Vector3 scale, int order)
        {
            var item = new GameObject(name, typeof(SpriteRenderer));
            item.transform.SetParent(transform, false);
            item.transform.position = ToWorld(position);
            item.transform.localScale = scale;
            var renderer = item.GetComponent<SpriteRenderer>();
            renderer.sprite = _squareSprite;
            renderer.color = color;
            renderer.sortingOrder = order;
            _visuals.Add(item);
            return item;
        }

        private void HandleTap(Vector2 screenPosition)
        {
            if (_isPaused || _journeyTransitioning) return;
            var world = _camera.ScreenToWorldPoint(screenPosition);
            if (_isAnimating) return;
            var cart = _game.State.Carts.FirstOrDefault(candidate =>
            {
                if (candidate.HasExited || !_cartRenderers.TryGetValue(candidate.Id, out var candidateRenderer)) return false;
                var bounds = candidateRenderer.bounds;
                bounds.Expand(CellSize * 0.35f);
                return bounds.Contains(new Vector3(world.x, world.y, bounds.center.z));
            });
            var cargo = cart == null ? _game.State.Cargo.FirstOrDefault(candidate =>
            {
                if (candidate.IsDelivered || !_cargoRenderers.TryGetValue(candidate.Id, out var candidateRenderer)) return false;
                var bounds = candidateRenderer.bounds;
                bounds.Expand(CellSize * 0.35f);
                return bounds.Contains(new Vector3(world.x, world.y, bounds.center.z));
            }) : null;
            if (cart == null && cargo == null) return;
            var id = cart?.Id ?? cargo.Id;
            var selectedRenderer = cart != null ? _cartRenderers[id] : _cargoRenderers[id];
            ResetSelectedTint();
            _selectedObjectId = id;
            selectedRenderer.color = new Color(1f, 0.88f, 0.58f);
            _feedback.PlaySelection(selectedRenderer.transform);
            _message = GameplayStrings.Get("instruction.move_selected");
            RenderHud();
        }

        private void MoveSelectedObject()
        {
            if (_journeyTransitioning || _isAnimating || _isPaused || string.IsNullOrEmpty(_selectedObjectId)) return;
            var cart = _game.State.GetCart(_selectedObjectId);
            if (cart == null) { MoveSelectedCargo(); return; }
            if (cart.HasExited) return;
            var selectedRenderer = _cartRenderers[cart.Id];
            var result = _game.Move(cart.Id);
            _message = result == MoveResult.Success ? string.Empty : result switch
            {
                MoveResult.WrongExit => GameplayStrings.Get("status.wrong_exit"),
                MoveResult.Stored => GameplayStrings.Get("status.stored"),
                _ => GameplayStrings.Get("status.blocked")
            };
            if ((result == MoveResult.Success || result == MoveResult.WrongExit) && _cartRenderers.TryGetValue(cart.Id, out var renderer))
            {
                renderer.transform.rotation = Quaternion.Euler(0, 0, Rotation(cart.Direction));
                _feedback.PlayMove(renderer.transform.position);
                StartCoroutine(AnimateCart(renderer, ToWorld(cart.Position), cart.HasExited));
            }
            else if (result != MoveResult.Success) _feedback.PlayInvalid(selectedRenderer.transform);
            RefreshBarriers();
            foreach (var gate in _gateByPosition)
                gate.Value.gameObject.SetActive(_game.State.GetCell(gate.Key) == CellType.Exit);
            if (_game.State.IsComplete)
            {
                _message = GameplayStrings.Get("status.complete");
                _feedback.PlayCompletion(_gateRenderers);
            }
            if (cart.HasExited) _selectedObjectId = null;
            RenderHud();
        }

        private void MoveSelectedCargo()
        {
            var cargo = _game.State.GetCargo(_selectedObjectId);
            if (cargo == null || cargo.IsDelivered) return;
            var selectedRenderer = _cargoRenderers[cargo.Id];
            var result = _game.MoveCargo(cargo.Id);
            _message = result switch
            {
                CargoMoveResult.Success => string.Empty,
                CargoMoveResult.Delivered => GameplayStrings.Get("status.cargo_delivered"),
                CargoMoveResult.WrongDestination => GameplayStrings.Get("status.wrong_cargo_destination"),
                _ => GameplayStrings.Get("status.blocked")
            };
            if (result == CargoMoveResult.Success || result == CargoMoveResult.Delivered || result == CargoMoveResult.WrongDestination)
            {
                selectedRenderer.transform.rotation = Quaternion.Euler(0, 0, Rotation(cargo.Direction));
                _feedback.PlayMove(selectedRenderer.transform.position);
                StartCoroutine(AnimateCart(selectedRenderer, ToWorld(cargo.Position), cargo.IsDelivered));
            }
            else _feedback.PlayInvalid(selectedRenderer.transform);
            RefreshBarriers();
            if (_game.State.IsComplete)
            {
                _message = GameplayStrings.Get("status.complete");
                _feedback.PlayCompletion(_gateRenderers);
            }
            if (cargo.IsDelivered) _selectedObjectId = null;
            RenderHud();
        }

        private void ResetSelectedTint()
        {
            if (string.IsNullOrEmpty(_selectedObjectId)) return;
            if (_cartRenderers.TryGetValue(_selectedObjectId, out var cart)) cart.color = Color.white;
            if (_cargoRenderers.TryGetValue(_selectedObjectId, out var cargo))
            {
                var state = _game.State.GetCargo(_selectedObjectId);
                cargo.color = state == null ? Color.white : CargoColor(state.Type);
            }
        }

        private IEnumerator AnimateCart(SpriteRenderer renderer, Vector3 target, bool hideAtEnd)
        {
            _isAnimating = true;
            var start = renderer.transform.position;
            const float duration = 0.18f;
            for (var elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
            {
                var t = Mathf.Clamp01(elapsed / duration);
                renderer.transform.position = Vector3.Lerp(start, target, t * t * (3f - 2f * t));
                yield return null;
            }
            renderer.transform.position = target;
            renderer.gameObject.SetActive(!hideAtEnd);
            _isAnimating = false;
        }

        private IEnumerator BeginRepresentativeJourney()
        {
            _journeyTransitioning = true;
            yield return _journeyPresenter.PlayApproach();
            _journeyTransitioning = false;
        }

        private IEnumerator CompleteRepresentativeJourney()
        {
            _journeyTransitioning = true;
            yield return _journeyPresenter.PlayDeparture();
            _journeyPresenter.HideLandscape();
            _journeyTransitioning = false;
            LoadLevel(1);
        }

        private void OnApplicationPause(bool paused)
        {
            if (!paused || !_isPaused) return;
            _isPaused = false;
            Time.timeScale = 1;
            RenderHud();
        }

        private void RefreshCarts()
        {
            foreach (var cart in _game.State.Carts)
            {
                if (!_cartRenderers.TryGetValue(cart.Id, out var renderer)) continue;
                renderer.gameObject.SetActive(!cart.HasExited);
                renderer.transform.position = ToWorld(cart.Position);
                renderer.transform.rotation = Quaternion.Euler(0, 0, Rotation(cart.Direction));
            }
            foreach (var cargo in _game.State.Cargo)
            {
                if (!_cargoRenderers.TryGetValue(cargo.Id, out var renderer)) continue;
                renderer.gameObject.SetActive(!cargo.IsDelivered);
                renderer.transform.position = ToWorld(cargo.Position);
                renderer.transform.rotation = Quaternion.Euler(0, 0, Rotation(cargo.Direction));
                renderer.color = CargoColor(cargo.Type);
            }
            _selectedObjectId = null;
            RefreshBarriers();
        }

        private void RefreshBarriers()
        {
            foreach (var renderer in _rockRenderers) renderer.gameObject.SetActive(!_game.State.BarriersOpen);
            foreach (var gate in _gateByPosition)
                gate.Value.gameObject.SetActive(_game.State.GetCell(gate.Key) == CellType.Exit);
            foreach (var gate in _linkedGateByPosition)
                gate.Value.gameObject.SetActive(!_game.State.IsGateOpen(gate.Key));
            if (_game.State.BarriersOpen && !_switchActivatedVisual)
            {
                _switchActivatedVisual = true;
                foreach (var renderer in _switchRenderers) StartCoroutine(AnimateSwitchActivation(renderer.transform));
            }
        }

        private static IEnumerator AnimateSwitchActivation(Transform target)
        {
            if (target == null) yield break;
            var original = target.localScale;
            const float duration = 0.28f;
            for (var elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
            {
                var pulse = 1f + Mathf.Sin(elapsed / duration * Mathf.PI) * 0.18f;
                target.localScale = original * pulse;
                yield return null;
            }
            target.localScale = original;
        }

        private bool TryGetPointerDown(out Vector2 position)
        {
            if (Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true)
            {
                position = Touchscreen.current.primaryTouch.position.ReadValue();
                return true;
            }
            if (UnityEngine.Input.touchCount > 0)
            {
                var touch = UnityEngine.Input.GetTouch(0);
                if (touch.phase == UnityEngine.TouchPhase.Began)
                {
                    position = touch.position;
                    return true;
                }
            }
            if (Mouse.current?.leftButton.wasPressedThisFrame == true)
            {
                position = Mouse.current.position.ReadValue();
                return true;
            }
            if (UnityEngine.Input.GetMouseButtonDown(0))
            {
                position = UnityEngine.Input.mousePosition;
                return true;
            }
            position = default;
            return false;
        }

        private Vector3 ToWorld(GridPosition position)
        {
            var offsetX = -(_game.State.Width - 1) * CellSize * 0.5f;
            var offsetY = -(_game.State.Height - 1) * CellSize * 0.5f;
            return new Vector3(offsetX + position.X * CellSize, offsetY + position.Y * CellSize, 0);
        }

        private GridPosition ToGrid(Vector3 world)
        {
            var offsetX = -(_game.State.Width - 1) * CellSize * 0.5f;
            var offsetY = -(_game.State.Height - 1) * CellSize * 0.5f;
            return new GridPosition(Mathf.RoundToInt((world.x - offsetX) / CellSize), Mathf.RoundToInt((world.y - offsetY) / CellSize));
        }

        private static float Rotation(Direction direction) => direction switch
        {
            Direction.Up => 90,
            Direction.Left => 180,
            Direction.Down => 270,
            _ => 0
        };

        private static Sprite CreateSquareSprite()
        {
            var texture = new Texture2D(1, 1, TextureFormat.RGBA32, false) { name = "Runtime Square" };
            texture.SetPixel(0, 0, Color.white);
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        }

        private static Sprite CreateArrowSprite()
        {
            const int size = 32;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { name = "Runtime Arrow", filterMode = FilterMode.Bilinear };
            var clear = new Color(0, 0, 0, 0);
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
            {
                var shaft = x >= 4 && x <= 19 && y >= 12 && y <= 19;
                var head = x >= 16 && x <= 28 && Mathf.Abs(y - 15.5f) <= (28 - x) * 0.75f;
                texture.SetPixel(x, y, shaft || head ? Color.white : clear);
            }
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        private static Sprite CreateCircleSprite()
        {
            const int size = 32;
            var texture = new Texture2D(size, size, TextureFormat.RGBA32, false) { name = "Runtime Circle", filterMode = FilterMode.Bilinear };
            var center = (size - 1) * 0.5f;
            for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
                texture.SetPixel(x, y, Vector2.Distance(new Vector2(x, y), new Vector2(center, center)) <= center ? Color.white : new Color(0, 0, 0, 0));
            texture.Apply();
            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }
    }
}
