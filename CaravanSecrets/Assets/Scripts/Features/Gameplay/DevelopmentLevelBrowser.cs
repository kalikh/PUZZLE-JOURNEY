#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaravanSecrets.Features.Gameplay
{
    /// <summary>
    /// Development-only level opener for Gate E device verification (Spec §50).
    /// Not a Stage 5 journey map.
    /// </summary>
    public sealed class DevelopmentLevelBrowser : MonoBehaviour
    {
        private GameObject _root;
        private Transform _content;
        private TMP_Text _title;
        private TMP_Text _languageLabel;
        private TMP_Text _compassLabel;
        private TMP_FontAsset _font;
        private Action<int> _onSelect;
        private Action _onLanguageToggle;
        private Action _onCompass;
        private int _builtCount = -1;

        public bool IsVisible => _root != null && _root.activeSelf;

        public void Show(int levelCount, int currentIndex, Action<int> onSelect, Action onLanguageToggle = null, Action onCompass = null)
        {
            _onSelect = onSelect;
            _onLanguageToggle = onLanguageToggle;
            _onCompass = onCompass;
            EnsureUi(levelCount);
            Highlight(currentIndex);
            RefreshLocalizedChrome();
            _root.SetActive(true);
        }

        public void Hide()
        {
            if (_root != null) _root.SetActive(false);
        }

        public void RefreshLocalizedChrome()
        {
            if (_title == null) return;
            var arabic = GameplayStrings.IsArabic;
            _title.text = Shape(GameplayStrings.Get("debug.levels"), arabic);
            if (_languageLabel != null)
            {
                var code = arabic ? "AR" : "EN";
                _languageLabel.text = Shape(GameplayStrings.Get("debug.language", code), arabic);
            }
            if (_compassLabel != null)
                _compassLabel.text = Shape(GameplayStrings.Get("debug.compass"), arabic);
        }

        private void EnsureUi(int levelCount)
        {
            if (_root != null && _builtCount == levelCount) return;
            if (_root != null) Destroy(_root);

            _builtCount = levelCount;
            _font = Resources.Load<TMP_FontAsset>("Fonts/ArialUnicode SDF");
            _root = new GameObject("DevelopmentLevelBrowser", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = _root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            var scaler = _root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 1f;

            var backdrop = CreateUi("Backdrop", _root.transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            var backdropImage = backdrop.gameObject.AddComponent<Image>();
            backdropImage.color = new Color(0.05f, 0.08f, 0.10f, 0.88f);
            var close = backdrop.gameObject.AddComponent<Button>();
            close.onClick.AddListener(Hide);

            var panel = CreateUi("Panel", _root.transform, new Vector2(0.08f, 0.14f), new Vector2(0.92f, 0.90f), Vector2.zero, Vector2.zero);
            panel.gameObject.AddComponent<Image>().color = new Color(0.18f, 0.12f, 0.08f, 0.96f);

            _title = CreateText(panel, "Title", GameplayStrings.Get("debug.levels"), 40, TextAlignmentOptions.Center,
                new Vector2(0.05f, 0.90f), new Vector2(0.95f, 0.98f));
            _title.color = new Color(0.95f, 0.88f, 0.70f);

            var langButton = CreateActionButton(panel, "LanguageButton", new Vector2(0.05f, 0.78f), new Vector2(0.48f, 0.88f), () =>
            {
                _onLanguageToggle?.Invoke();
            });
            _languageLabel = langButton.GetComponentInChildren<TMP_Text>();

            var compassButton = CreateActionButton(panel, "CompassButton", new Vector2(0.52f, 0.78f), new Vector2(0.95f, 0.88f), () =>
            {
                _onCompass?.Invoke();
            });
            _compassLabel = compassButton.GetComponentInChildren<TMP_Text>();

            var scrollGo = CreateUi("Scroll", panel, new Vector2(0.04f, 0.04f), new Vector2(0.96f, 0.76f), Vector2.zero, Vector2.zero);
            var scroll = scrollGo.gameObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;

            var viewport = CreateUi("Viewport", scrollGo, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            viewport.gameObject.AddComponent<Image>().color = new Color(1, 1, 1, 0.02f);
            viewport.gameObject.AddComponent<Mask>().showMaskGraphic = false;
            scroll.viewport = viewport;

            var content = CreateUi("Content", viewport, new Vector2(0, 1), new Vector2(1, 1), Vector2.zero, Vector2.zero);
            content.pivot = new Vector2(0.5f, 1f);
            var grid = content.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(140, 100);
            grid.spacing = new Vector2(12, 12);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;
            grid.childAlignment = TextAnchor.UpperCenter;
            grid.padding = new RectOffset(20, 20, 16, 16);
            var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            scroll.content = content;
            _content = content;

            for (var i = 0; i < levelCount; i++)
            {
                var index = i;
                var cell = new GameObject($"LevelButton_{i + 1}", typeof(RectTransform), typeof(Image), typeof(Button));
                cell.transform.SetParent(_content, false);
                var image = cell.GetComponent<Image>();
                image.color = new Color(0.35f, 0.24f, 0.14f, 1f);
                var button = cell.GetComponent<Button>();
                button.targetGraphic = image;
                button.onClick.AddListener(() =>
                {
                    _onSelect?.Invoke(index);
                    Hide();
                });
                var label = CreateText(cell.transform as RectTransform, "Label", (i + 1).ToString(), 34, TextAlignmentOptions.Center,
                    new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.92f));
                label.color = Color.white;
                label.overflowMode = TextOverflowModes.Overflow;
            }
        }

        private Button CreateActionButton(RectTransform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Action onClick)
        {
            var rect = CreateUi(name, parent, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
            var image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(0.28f, 0.42f, 0.40f, 1f);
            var button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(() => onClick());
            var label = CreateText(rect, "Label", string.Empty, 28, TextAlignmentOptions.Center,
                new Vector2(0.06f, 0.12f), new Vector2(0.94f, 0.88f));
            label.color = Color.white;
            return button;
        }

        private void Highlight(int currentIndex)
        {
            if (_content == null) return;
            for (var i = 0; i < _content.childCount; i++)
            {
                var image = _content.GetChild(i).GetComponent<Image>();
                if (image == null) continue;
                image.color = i == currentIndex
                    ? new Color(0.12f, 0.55f, 0.52f, 1f)
                    : new Color(0.35f, 0.24f, 0.14f, 1f);
            }
        }

        private static RectTransform CreateUi(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
            return rect;
        }

        private TMP_Text CreateText(RectTransform parent, string name, string value, float size, TextAlignmentOptions align, Vector2 anchorMin, Vector2 anchorMax)
        {
            var go = new GameObject(name, typeof(RectTransform));
            var rect = go.GetComponent<RectTransform>();
            rect.SetParent(parent, false);
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            var text = go.AddComponent<TextMeshProUGUI>();
            if (_font != null) text.font = _font;
            text.text = value;
            text.fontSize = size;
            text.alignment = align;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.enableAutoSizing = true;
            text.fontSizeMin = 18;
            text.fontSizeMax = size;
            return text;
        }

        private static string Shape(string text, bool arabic) => arabic ? ArabicText.Display(text) : text;
    }
}
#endif
