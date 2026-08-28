using System;
using System.Collections;
using System.Collections.Generic;
using CaravanSecrets.Game.Journey;
using UnityEngine;

namespace CaravanSecrets.Features.Journey
{
    /// <summary>
    /// Data-driven journey presenter. The class name is retained for backward
    /// compatibility with the accepted Phase 2/2.1 tests; behaviour is now driven
    /// entirely by <see cref="JourneyChainSegment"/> data instead of hard-coded ids.
    /// </summary>
    public sealed class RepresentativeJourneyPresenter : MonoBehaviour
    {
        private const float StartY = -14f;
        private const float PuzzleY = 0f;
        private const float NextY = 14f;
        private const float RoadWidth = 0.88f;
        private static readonly Color TraveledTint = new(0.74f, 0.62f, 0.44f);
        private static readonly Color UntraveledTint = new(1f, 0.98f, 0.92f);
        private Camera _camera;
        private Transform _landscape;
        private GameObject _travelCaravan;
        private GameObject _roadPrefab;
        private GameObject _backgroundPrefab;
        private GameObject _cartPrefab;
        private GameObject _gatePrefab;
        private GameObject _rockPrefab;
        private readonly List<SpriteRenderer> _approachRoad = new();
        private readonly List<SpriteRenderer> _departureRoad = new();
        private bool _suspended;
        private float _puzzleCameraSize;

        public JourneySession Session { get; private set; }
        public IJourneyProgress Progress => Session;
        public JourneyChainSegment Segment { get; private set; }
        public string LevelId => Segment?.LevelId;
        public bool IsTransitioning => Session != null &&
            (Session.Phase == JourneyPhase.TravellingToPuzzle || Session.Phase == JourneyPhase.TravellingToNextCheckpoint);

        public void Initialize(Camera camera, float puzzleCameraSize, JourneyChainSegment segment,
            string checkpointId = null, JourneyPhase restoredPhase = JourneyPhase.AtStartCheckpoint)
        {
            _camera = camera != null ? camera : throw new ArgumentNullException(nameof(camera));
            Segment = segment ?? throw new ArgumentNullException(nameof(segment));
            _puzzleCameraSize = puzzleCameraSize;
            _roadPrefab = Resources.Load<GameObject>("VerticalSlice/RoadStrip");
            _backgroundPrefab = Resources.Load<GameObject>("VerticalSlice/DesertBackground");
            _cartPrefab = Resources.Load<GameObject>("VerticalSlice/Cart");
            _gatePrefab = Resources.Load<GameObject>("VerticalSlice/Gate");
            _rockPrefab = Resources.Load<GameObject>("VerticalSlice/Rock");
            BuildLandscape(segment);
            RestoreSession(checkpointId, restoredPhase);
            if (Session.Phase == JourneyPhase.AtPuzzle) ShowPuzzlePosition();
            else if (Session.Phase == JourneyPhase.AtNextCheckpoint) ShowNextCheckpointPosition();
            else SetCamera(StartY);
        }

        /// <summary>
        /// Rebinds the presenter to another segment of the chain (for example after
        /// a checkpoint arrival, when the next level becomes the active puzzle).
        /// </summary>
        public void BindSegment(JourneyChainSegment segment, bool showAtStartCheckpoint)
        {
            Segment = segment ?? throw new ArgumentNullException(nameof(segment));
            Session = new JourneySession(segment.Definition);
            BuildLandscape(segment);
            if (showAtStartCheckpoint)
            {
                _landscape.gameObject.SetActive(true);
                _travelCaravan.SetActive(true);
                _travelCaravan.transform.position = new Vector3(0, StartY, 0);
                SetCamera(StartY);
            }
            else ShowPuzzlePosition();
        }

        private void RestoreSession(string checkpointId, JourneyPhase restoredPhase)
        {
            // Arriving at a checkpoint from the previous segment is the same physical
            // place as this segment's start checkpoint: the journey continues forward.
            var phase = restoredPhase;
            if (phase == JourneyPhase.AtNextCheckpoint &&
                string.Equals(checkpointId, Segment.StartCheckpointId, StringComparison.Ordinal))
                phase = JourneyPhase.AtStartCheckpoint;
            Session = JourneySession.RestoreStable(Segment.Definition, checkpointId, phase);
        }

        public void SetSuspended(bool suspended) => _suspended = suspended;

        public void ShowPuzzlePosition()
        {
            _camera.orthographicSize = _puzzleCameraSize;
            SetCamera(PuzzleY);
            if (_travelCaravan != null) _travelCaravan.SetActive(false);
            if (_landscape != null) _landscape.gameObject.SetActive(false);
        }

        public void ShowNextCheckpointPosition()
        {
            if (_landscape != null) _landscape.gameObject.SetActive(true);
            SetCamera(NextY);
            if (_travelCaravan != null)
            {
                _travelCaravan.SetActive(true);
                _travelCaravan.transform.position = new Vector3(0, NextY, 0);
            }
        }

        public IEnumerator PlayApproach()
        {
            Session.BeginApproach();
            yield return WaitUnpaused(0.9f);
            yield return AnimateTravel(StartY, PuzzleY - 3.8f, 2.1f, true);
            yield return AnimateCamera(PuzzleY - 3.8f, PuzzleY, 0.65f);
            MarkTraveled(_approachRoad);
            _travelCaravan.SetActive(false);
            _landscape.gameObject.SetActive(false);
            Session.ArriveAtPuzzle();
        }

        public IEnumerator PlayDeparture()
        {
            Session.BeginDeparture();
            _landscape.gameObject.SetActive(true);
            _travelCaravan.SetActive(true);
            _travelCaravan.transform.position = new Vector3(0, PuzzleY + 3.2f, 0);
            yield return AnimateTravel(PuzzleY + 3.2f, NextY, 2.35f, true);
            Session.ArriveAtNextCheckpoint();
            MarkTraveled(_departureRoad);
            yield return WaitUnpaused(1.1f);
        }

        public void HideLandscape()
        {
            if (_landscape != null) _landscape.gameObject.SetActive(false);
        }

        private static void MarkTraveled(List<SpriteRenderer> road)
        {
            foreach (var renderer in road)
                if (renderer != null) renderer.color = TraveledTint;
        }

        private void BuildLandscape(JourneyChainSegment segment)
        {
            if (_landscape != null) Destroy(_landscape.gameObject);
            _approachRoad.Clear();
            _departureRoad.Clear();
            _landscape = new GameObject($"Journey Segment {segment.SegmentId}").transform;
            _landscape.SetParent(transform, false);
            CreateBackground(StartY, _puzzleCameraSize);
            CreateBackground(PuzzleY, _puzzleCameraSize);
            CreateBackground(NextY, _puzzleCameraSize);
            var bend = Mathf.Clamp(segment.RoadBend, -1.8f, 1.8f);
            CreateRoadPath(StartY, PuzzleY - 4.2f, "Road To Puzzle", bend, _approachRoad);
            CreateRoadPath(PuzzleY + 4.2f, NextY, "Road To Next Checkpoint", -bend, _departureRoad);
            CreateCheckpoint(StartY, "Start Checkpoint");
            CreateCheckpoint(NextY, "Next Checkpoint");
            CreateLandmarks(segment);
            _travelCaravan = Instantiate(_cartPrefab, new Vector3(0, StartY, 0), Quaternion.identity, _landscape);
            _travelCaravan.name = "Journey Caravan";
            FitSprite(_travelCaravan, new Vector2(1.3f, 0.92f), 45);
        }

        private void CreateBackground(float y, float cameraSize)
        {
            if (_backgroundPrefab == null) return;
            var item = Instantiate(_backgroundPrefab, new Vector3(0, y, 8), Quaternion.identity, _landscape);
            item.name = $"Journey Background {y:0}";
            var renderer = item.GetComponent<SpriteRenderer>();
            if (renderer == null || renderer.sprite == null) return;
            var visibleHeight = cameraSize * 2.25f;
            var visibleWidth = visibleHeight * Mathf.Max(0.5f, _camera.aspect);
            var size = renderer.sprite.bounds.size;
            item.transform.localScale = Vector3.one * Mathf.Max(visibleWidth / size.x, visibleHeight / size.y);
            renderer.sortingOrder = -30;
        }

        private void CreateRoadPath(float fromY, float toY, string name, float bend, List<SpriteRenderer> collector)
        {
            if (_roadPrefab == null) return;
            var root = new GameObject(name).transform;
            root.SetParent(_landscape, false);
            const int count = 3;
            for (var index = 0; index < count; index++)
            {
                var fromT = index / (float)count;
                var toT = (index + 1f) / count;
                var start = new Vector2(Mathf.Sin(fromT * Mathf.PI) * bend, Mathf.Lerp(fromY, toY, fromT));
                var end = new Vector2(Mathf.Sin(toT * Mathf.PI) * bend, Mathf.Lerp(fromY, toY, toT));
                var direction = end - start;
                var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                var item = Instantiate(_roadPrefab, (Vector3)((start + end) * 0.5f), Quaternion.Euler(0, 0, angle), root);
                item.name = $"{name} Section {index + 1}";
                var renderer = item.GetComponent<SpriteRenderer>();
                if (renderer == null || renderer.sprite == null) continue;
                var size = renderer.sprite.bounds.size;
                item.transform.localScale = new Vector3(direction.magnitude * 1.12f / Mathf.Max(0.01f, size.x), RoadWidth / Mathf.Max(0.01f, size.y), 1);
                renderer.sortingOrder = -1;
                renderer.color = UntraveledTint;
                collector.Add(renderer);
            }
        }

        private void CreateCheckpoint(float y, string name)
        {
            if (_gatePrefab == null) return;
            var gate = Instantiate(_gatePrefab, new Vector3(0, y + 1.35f, 0), Quaternion.identity, _landscape);
            gate.name = name;
            FitSprite(gate, new Vector2(1.65f, 1.82f), 12);
        }

        private void CreateLandmarks(JourneyChainSegment segment)
        {
            foreach (var landmark in segment.Landmarks)
            {
                var prefab = landmark.PrefabKey == "gate" ? _gatePrefab : landmark.PrefabKey == "cart" ? _cartPrefab : _rockPrefab;
                if (prefab == null) continue;
                var item = Instantiate(prefab, new Vector3(landmark.X, landmark.Y, 0),
                    Quaternion.Euler(0, 0, landmark.RotationDegrees), _landscape);
                item.name = $"Journey Landmark {landmark.PrefabKey} {landmark.Y:0.0}";
                var size = landmark.PrefabKey == "gate"
                    ? new Vector2(1.1f, 1.2f) * landmark.Scale
                    : Vector2.one * landmark.Scale;
                FitSprite(item, size, 3);
            }
        }

        private static void FitSprite(GameObject item, Vector2 targetSize, int sortingOrder)
        {
            var renderer = item.GetComponent<SpriteRenderer>();
            if (renderer == null || renderer.sprite == null) return;
            var size = renderer.sprite.bounds.size;
            item.transform.localScale = new Vector3(targetSize.x / Mathf.Max(0.01f, size.x), targetSize.y / Mathf.Max(0.01f, size.y), 1);
            renderer.sortingOrder = sortingOrder;
        }

        private IEnumerator AnimateTravel(float fromY, float toY, float duration, bool followCamera)
        {
            var cameraStart = _camera.transform.position;
            var start = new Vector3(0, fromY, 0);
            var end = new Vector3(0, toY, 0);
            for (var elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
            {
                while (_suspended) yield return null;
                var t = Smooth(Mathf.Clamp01(elapsed / duration));
                var position = Vector3.Lerp(start, end, t);
                position.x = Mathf.Sin(t * Mathf.PI) * (toY > fromY ? 0.82f : -0.82f);
                _travelCaravan.transform.position = position;
                if (followCamera)
                    _camera.transform.position = new Vector3(position.x * 0.22f, position.y, cameraStart.z);
                yield return null;
            }
            _travelCaravan.transform.position = end;
            if (followCamera) SetCamera(toY);
        }

        private IEnumerator AnimateCamera(float fromY, float toY, float duration)
        {
            var z = _camera.transform.position.z;
            for (var elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
            {
                while (_suspended) yield return null;
                var t = Smooth(Mathf.Clamp01(elapsed / duration));
                _camera.transform.position = new Vector3(0, Mathf.Lerp(fromY, toY, t), z);
                yield return null;
            }
            SetCamera(toY);
        }

        private void SetCamera(float y) => _camera.transform.position = new Vector3(0, y, -10);
        private static float Smooth(float t) => t * t * (3f - 2f * t);

        private IEnumerator WaitUnpaused(float duration)
        {
            var elapsed = 0f;
            while (elapsed < duration)
            {
                if (!_suspended) elapsed += Time.unscaledDeltaTime;
                yield return null;
            }
        }
    }
}
