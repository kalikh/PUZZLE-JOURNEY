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

        private static void SetButtonText(Button button, string text, bool arabic)
        {
            var label = button.GetComponentInChildren<TMP_Text>();
            if (label != null) label.text = Shape(text, arabic);
        }

        private static string Shape(string text, bool arabic) => arabic ? ArabicText.Display(text) : text;
    }
}
