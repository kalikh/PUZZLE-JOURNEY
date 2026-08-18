using System.Collections;
using CaravanSecrets.Features.Gameplay;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using TMPro;
using System.Collections.Generic;
using System.Reflection;
using CaravanSecrets.Game.Board;

namespace CaravanSecrets.PlayMode.Tests
{
    public sealed class GameplaySceneTests
    {
        [UnityTest]
        public IEnumerator HudLocalization_ResolvesEnglishAndArabicInsteadOfKeys()
        {
            yield return LocalizationSettings.InitializationOperation;

            var english = LocalizationSettings.AvailableLocales.GetLocale("en");
            var arabic = LocalizationSettings.AvailableLocales.GetLocale("ar");
            Assert.That(english, Is.Not.Null);
            Assert.That(arabic, Is.Not.Null);

            LocalizationSettings.SelectedLocale = english;
            SceneManager.LoadScene("Gameplay");
            yield return null;
            var englishLevel = GameObject.Find("LevelLabel").GetComponent<TMP_Text>().text;
            Assert.That(englishLevel, Is.EqualTo("Level 1/30"));

            LocalizationSettings.SelectedLocale = arabic;
            SceneManager.LoadScene("Gameplay");
            yield return null;
            var arabicLevel = GameObject.Find("LevelLabel").GetComponent<TMP_Text>().text;
            Assert.That(arabicLevel, Does.Not.Contain("hud.level"));
            Assert.That(arabicLevel, Does.Not.Contain("["));
            Assert.That(arabicLevel, Is.Not.EqualTo(englishLevel));

            LocalizationSettings.SelectedLocale = english;
        }

        [UnityTest]
        public IEnumerator CompletedLevel_NextTraversesAllThirtyLevels()
        {
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
            SceneManager.LoadScene("Gameplay");
            yield return null;

            var controller = Object.FindFirstObjectByType<GameplayController>();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var loadLevel = typeof(GameplayController).GetMethod("LoadLevel", flags);
            var next = typeof(GameplayController).GetMethod("HintFromHud", flags);
            var levelsField = typeof(GameplayController).GetField("_levels", flags);
            var gameField = typeof(GameplayController).GetField("_game", flags);
            var levels = (IReadOnlyList<LevelDefinition>)levelsField.GetValue(controller);
            Assert.That(levels.Count, Is.EqualTo(30));

            CompleteAndAdvance(4, "Level 6/30");
            CompleteAndAdvance(29, "Level 1/30");

            void CompleteAndAdvance(int index, string expectedLabel)
            {
                loadLevel.Invoke(controller, new object[] { index });
                var game = (BoardGame)gameField.GetValue(controller);
                var solution = LevelSolver.Solve(levels[index]);
                Assert.That(solution.Status, Is.EqualTo(SolverStatus.Solved));
                foreach (var objectId in solution.Moves)
                    Assert.That(game.MoveObject(objectId), Is.True, objectId);
                Assert.That(game.State.IsComplete, Is.True);
                next.Invoke(controller, null);
                Assert.That(GameObject.Find("LevelLabel").GetComponent<TMP_Text>().text, Is.EqualTo(expectedLabel));
            }
        }

        [UnityTest]
        public IEnumerator GameplayScene_LoadsControllerAndCamera()
        {
            SceneManager.LoadScene("Gameplay");
            yield return null;
            Assert.That(Object.FindFirstObjectByType<GameplayController>(), Is.Not.Null);
            Assert.That(Camera.main, Is.Not.Null);
            Assert.That(Camera.main.orthographic, Is.True);
            Assert.That(Object.FindFirstObjectByType<GameplayHudView>(), Is.Not.Null);
            Assert.That(GameObject.Find("BottomBar"), Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator Hud_PauseResumeAndRestart_AreOperational()
        {
            SceneManager.LoadScene("Gameplay");
            yield return null;

            var pause = GameObject.Find("PauseButton").GetComponent<Button>();
            var restart = GameObject.Find("RestartButton").GetComponent<Button>();
            Assert.That(pause, Is.Not.Null);
            Assert.That(restart, Is.Not.Null);

            pause.onClick.Invoke();
            Assert.That(Time.timeScale, Is.Zero);
            pause.onClick.Invoke();
            Assert.That(Time.timeScale, Is.EqualTo(1));

            var controller = Object.FindFirstObjectByType<GameplayController>();
            restart.onClick.Invoke();
            yield return null;
            Assert.That(Object.FindFirstObjectByType<GameplayController>(), Is.SameAs(controller));
        }

        [UnityTest]
        public IEnumerator GameplayPlaytest_RendersCargoAndTypedDestination()
        {
            GameplayPlaytestOverride.PendingLevel = new LevelDefinition("cargo_playtest", 3, 2,
                new Dictionary<GridPosition, CellType> { [new GridPosition(2, 1)] = CellType.Exit },
                new[] { new CartDefinition("cart", new GridPosition(1, 1), Direction.Right) }, null,
                "market", 11, cargo: new[]
                {
                    new CargoDefinition("cargo_spices", new GridPosition(0, 0), Direction.Right, CargoType.Spices)
                }, cargoDestinations: new Dictionary<GridPosition, CargoType>
                {
                    [new GridPosition(2, 0)] = CargoType.Spices
                });
            SceneManager.LoadScene("Gameplay");
            yield return null;

            Assert.That(GameObject.Find("cargo_spices"), Is.Not.Null);
            Assert.That(GameObject.Find("Cargo Destination Spices"), Is.Not.Null);
            Assert.That(GameObject.Find("Cargo Symbol Spices 1"), Is.Not.Null);
        }

        [UnityTest]
        public IEnumerator GameplayPlaytest_RendersGateBMechanisms()
        {
            GameplayPlaytestOverride.PendingLevel = new LevelDefinition("gate_b_playtest", 5, 3,
                new Dictionary<GridPosition, CellType> { [new GridPosition(4, 2)] = CellType.Exit },
                new[] { new CartDefinition("cart", new GridPosition(0, 2), Direction.Right) }, null,
                "city", 21,
                gates: new[] { new GateDefinition("gate_01", new GridPosition(3, 2)) },
                switches: new[] { new SwitchDefinition("switch_01", new GridPosition(1, 0), new[] { "gate_01" }) },
                storageSlots: new[] { new StorageDefinition("storage_01", new GridPosition(2, 0), 2) },
                directionTiles: new[] { new DirectionTileDefinition("turn_01", new GridPosition(1, 2), Direction.Down) });
            SceneManager.LoadScene("Gameplay");
            yield return null;

            Assert.That(GameObject.Find("Linked Gate gate_01"), Is.Not.Null);
            Assert.That(GameObject.Find("Linked Switch switch_01"), Is.Not.Null);
            Assert.That(GameObject.Find("Storage Capacity 2"), Is.Not.Null);
            Assert.That(GameObject.Find("Direction Tile turn_01"), Is.Not.Null);
        }
    }
}
