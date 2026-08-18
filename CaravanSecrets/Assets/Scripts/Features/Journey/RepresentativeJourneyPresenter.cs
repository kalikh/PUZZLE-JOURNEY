using System;
using System.Collections;
using CaravanSecrets.Game.Journey;
using UnityEngine;

namespace CaravanSecrets.Features.Journey
{
    public sealed class RepresentativeJourneyPresenter : MonoBehaviour
    {
        private const float StartY = -14f;
        private const float PuzzleY = 0f;
        private const float NextY = 14f;
        private const float RoadWidth = 1.15f;
        private Camera _camera;
        private Transform _landscape;
        private GameObject _travelCaravan;
        private GameObject _roadPrefab;
        private GameObject _backgroundPrefab;
        private GameObject _cartPrefab;
        private GameObject _gatePrefab;
        private GameObject _rockPrefab;

        public JourneySession Session { get; private set; }
        public IJourneyProgress Progress => Session;
        public bool IsTransitioning => Session != null &&
            (Session.Phase == JourneyPhase.TravellingToPuzzle || Session.Phase == JourneyPhase.TravellingToNextCheckpoint);

        public void Initialize(Camera camera, float puzzleCameraSize)
        {
            _camera = camera != null ? camera : throw new ArgumentNullException(nameof(camera));
            Session = new JourneySession(new JourneySegmentDefinition(
                "desert_representative_01", "desert_start", "desert_puzzle_01", "desert_checkpoint_02"));
            _roadPrefab = Resources.Load<GameObject>("VerticalSlice/RoadStrip");
            _backgroundPrefab = Resources.Load<GameObject>("VerticalSlice/DesertBackground");
            _cartPrefab = Resources.Load<GameObject>("VerticalSlice/Cart");
            _gatePrefab = Resources.Load<GameObject>("VerticalSlice/Gate");
            _rockPrefab = Resources.Load<GameObject>("VerticalSlice/Rock");
            BuildLandscape(puzzleCameraSize);
            SetCamera(StartY);
        }

        public IEnumerator PlayApproach()
        {
            Session.BeginApproach();
            yield return new WaitForSecondsRealtime(1.25f);
            yield return AnimateTravel(StartY, PuzzleY - 3.8f, 2.1f, true);
            yield return AnimateCamera(PuzzleY - 3.8f, PuzzleY, 0.65f);
            _travelCaravan.SetActive(false);
            Session.ArriveAtPuzzle();
        }

        public IEnumerator PlayDeparture()
        {
            Session.BeginDeparture();
            _travelCaravan.SetActive(true);
            _travelCaravan.transform.position = new Vector3(0, PuzzleY + 3.2f, 0);
            yield return AnimateTravel(PuzzleY + 3.2f, NextY, 2.35f, true);
            Session.ArriveAtNextCheckpoint();
            yield return new WaitForSecondsRealtime(1.25f);
        }

        public void HideLandscape()
        {
            if (_landscape != null) _landscape.gameObject.SetActive(false);
        }

        private void BuildLandscape(float cameraSize)
        {
            _landscape = new GameObject("Representative Journey Segment").transform;
            _landscape.SetParent(transform, false);
            CreateBackground(StartY, cameraSize);
            CreateBackground(PuzzleY, cameraSize);
            CreateBackground(NextY, cameraSize);
            CreateRoad(StartY, PuzzleY - 4.2f, "Road To Puzzle");
            CreateRoad(PuzzleY + 4.2f, NextY, "Road To Next Checkpoint");
            CreateCheckpoint(StartY, "Start Checkpoint");
            CreateCheckpoint(NextY, "Next Checkpoint");
            CreateLandmark(-2.15f, -9.5f, 0.75f, "West Dune Rock");
            CreateLandmark(1.9f, -6.8f, 0.58f, "Approach Rock");
            CreateLandmark(-1.8f, 7.1f, 0.66f, "Departure Rock");
            CreateLandmark(2.2f, 10.2f, 0.82f, "Horizon Rock");
            _travelCaravan = Instantiate(_cartPrefab, new Vector3(0, StartY, 0), Quaternion.Euler(0, 0, 90), _landscape);
            _travelCaravan.name = "Journey Caravan";
            FitSprite(_travelCaravan, new Vector2(1.15f, 0.9f), 45);
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

        private void CreateRoad(float fromY, float toY, string name)
        {
            if (_roadPrefab == null) return;
            var item = Instantiate(_roadPrefab, new Vector3(0, (fromY + toY) * 0.5f, 0), Quaternion.Euler(0, 0, 90), _landscape);
            item.name = name;
            var renderer = item.GetComponent<SpriteRenderer>();
            if (renderer == null || renderer.sprite == null) return;
            var size = renderer.sprite.bounds.size;
            item.transform.localScale = new Vector3(Mathf.Abs(toY - fromY) / Mathf.Max(0.01f, size.x), RoadWidth / Mathf.Max(0.01f, size.y), 1);
            renderer.sortingOrder = -1;
        }

        private void CreateCheckpoint(float y, string name)
        {
            if (_gatePrefab == null) return;
            var gate = Instantiate(_gatePrefab, new Vector3(0, y + 1.35f, 0), Quaternion.identity, _landscape);
            gate.name = name;
            FitSprite(gate, new Vector2(2.1f, 2.25f), 12);
        }

        private void CreateLandmark(float x, float y, float size, string name)
        {
            if (_rockPrefab == null) return;
            var rock = Instantiate(_rockPrefab, new Vector3(x, y, 0), Quaternion.Euler(0, 0, y * 7f), _landscape);
            rock.name = name;
            FitSprite(rock, Vector2.one * size, 3);
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
                var t = Smooth(Mathf.Clamp01(elapsed / duration));
                var position = Vector3.Lerp(start, end, t);
                position.x = Mathf.Sin(t * Mathf.PI * 2f) * 0.42f;
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
                var t = Smooth(Mathf.Clamp01(elapsed / duration));
                _camera.transform.position = new Vector3(0, Mathf.Lerp(fromY, toY, t), z);
                yield return null;
            }
            SetCamera(toY);
        }

        private void SetCamera(float y) => _camera.transform.position = new Vector3(0, y, -10);
        private static float Smooth(float t) => t * t * (3f - 2f * t);
    }
}
