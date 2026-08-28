using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CaravanSecrets.Features.Gameplay
{
    public sealed class GameplayHudView : MonoBehaviour
    {
        [SerializeField] private TMP_Text levelLabel;
        [SerializeField] private TMP_Text objectiveLabel;
        [SerializeField] private TMP_Text movesLabel;
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button undoButton;
        [SerializeField] private Button driveButton;
        [SerializeField] private Button restartButton;
        [SerializeField] private Button hintButton;

        private void Awake()
        {
            ApplySafeArea();
            NormalizeButtonStyles();
        }

        public void Bind(Action pause, Action undo, Action drive, Action restart, Action hint)
        {
            pauseButton.onClick.AddListener(() => pause());
            undoButton.onClick.AddListener(() => undo());
            driveButton.GetComponent<HoldMoveButton>().Bind(drive);
            restartButton.onClick.AddListener(() => restart());
            hintButton.onClick.AddListener(() => hint());
        }

        public void Render(int level, int totalLevels, int moves, string objective, bool paused, bool completed)
        {
            var arabic = GameplayStrings.IsArabic;
            levelLabel.text = Shape(GameplayStrings.Get("hud.level", level, totalLevels), arabic);
            movesLabel.text = Shape(GameplayStrings.Get("hud.moves", moves), arabic);
            objectiveLabel.text = Shape(objective, arabic);
            SetButtonText(pauseButton, GameplayStrings.Get(paused ? "button.resume" : "button.pause"), arabic);
            SetButtonText(undoButton, GameplayStrings.Get("button.undo"), arabic);
            SetButtonText(driveButton, GameplayStrings.Get("button.move"), arabic);
            SetButtonText(restartButton, GameplayStrings.Get("button.restart"), arabic);
            SetButtonText(hintButton, GameplayStrings.Get(completed ? "button.next" : "button.hint"), arabic);
        }

        /// <summary>
        /// Insets the top panel and bottom bar by the device safe area so notches and
        /// gesture bars never cover HUD controls. Portrait layout is preserved.
        /// </summary>
        private void ApplySafeArea()
        {
            var canvas = GetComponentInParent<Canvas>();
            if (canvas == null) canvas = GetComponent<Canvas>();
            if (canvas == null) return;
            var root = canvas.rootCanvas.transform as RectTransform;
            if (root == null || Screen.height <= 0 || Screen.width <= 0) return;
            var factor = root.rect.height / Screen.height;
            var safe = Screen.safeArea;
            var topInset = (Screen.height - safe.yMax) * factor;
            var bottomInset = safe.yMin * factor;
            var sideInset = Mathf.Max(safe.xMin, Screen.width - safe.xMax) * factor;
            var top = transform.Find("TopPanel") as RectTransform;
            var bottom = transform.Find("BottomBar") as RectTransform;
            if (top != null)
            {
                top.anchoredPosition = new Vector2(top.anchoredPosition.x, top.anchoredPosition.y - topInset);
                top.sizeDelta = new Vector2(top.sizeDelta.x - sideInset * 2f, top.sizeDelta.y);
            }
            if (bottom != null)
            {
                bottom.anchoredPosition = new Vector2(bottom.anchoredPosition.x, bottom.anchoredPosition.y + bottomInset);
                bottom.sizeDelta = new Vector2(bottom.sizeDelta.x - sideInset * 2f, bottom.sizeDelta.y);
            }
        }

        /// <summary>
        /// Gives all HUD buttons one consistent interaction style and typography while
        /// keeping the prefab artwork and localized text untouched.
        /// </summary>
        private void NormalizeButtonStyles()
        {
            foreach (var button in new[] { pauseButton, undoButton, driveButton, restartButton, hintButton })
            {
                if (button == null) continue;
                button.transition = Selectable.Transition.ColorTint;
                var colors = button.colors;
                colors.normalColor = Color.white;
                colors.highlightedColor = new Color(1.1f, 1.06f, 0.98f);
                colors.pressedColor = new Color(0.8f, 0.72f, 0.62f);
                colors.selectedColor = colors.highlightedColor;
                colors.fadeDuration = 0.08f;
                button.colors = colors;
                var label = button.GetComponentInChildren<TMP_Text>();
                if (label == null) continue;
                label.enableAutoSizing = true;
                label.fontSizeMin = 28;
                label.fontSizeMax = 46;
                label.alignment = TextAlignmentOptions.Center;
                label.color = Color.white;
                label.raycastTarget = false;
            }
            if (objectiveLabel != null)
            {
                objectiveLabel.enableAutoSizing = true;
                objectiveLabel.fontSizeMin = 24;
                objectiveLabel.fontSizeMax = 40;
            }
        }

        private static void SetButtonText(Button button, string text, bool arabic)
        {
            var label = button.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = Shape(text, arabic);
        }

        private static string Shape(string text, bool arabic) => arabic ? ArabicText.Display(text) : text;
    }
}
