using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CaravanSecrets.Data.Journey;
using CaravanSecrets.Features.Gameplay;
using CaravanSecrets.Features.Journey;
using CaravanSecrets.Game.Board;
using CaravanSecrets.Game.Journey;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace CaravanSecrets.PlayMode.Tests
{
    public sealed class JourneyChainPlayModeTests
    {
        [UnityTest]
        public IEnumerator JourneyChain_DepartureAdvancesIntoNextSegmentApproach()
        {
            yield return LocalizationSettings.InitializationOperation;
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale("en");
            SceneManager.LoadScene("Gameplay");
            yield return null;

            var presenter = Object.FindFirstObjectByType<RepresentativeJourneyPresenter>();
            Assert.That(presenter, Is.Not.Null, "A desert-chain level should always start with a journey presenter.");
            var startLevelId = presenter.LevelId;
            if (startLevelId == "desert_10")
                Assert.Ignore("Save sits at the final chain segment; nothing follows it.");

            yield return WaitFor(() => presenter.Session.Phase == JourneyPhase.AtPuzzle, 8f);

            var controller = Object.FindFirstObjectByType<GameplayController>();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var levelIndex = (int)typeof(GameplayController).GetField("_levelIndex", flags).GetValue(controller);
            var levels = (IReadOnlyList<LevelDefinition>)typeof(GameplayController).GetField("_levels", flags).GetValue(controller);
            var game = (BoardGame)typeof(GameplayController).GetField("_game", flags).GetValue(controller);
            Assert.That(levels[levelIndex].Id, Is.EqualTo(startLevelId));
            var solution = LevelSolver.Solve(levels[levelIndex]);
            Assert.That(solution.Status, Is.EqualTo(SolverStatus.Solved));
            foreach (var objectId in solution.Moves) Assert.That(game.MoveObject(objectId), Is.True, objectId);
            Assert.That(game.State.IsComplete, Is.True);

            var next = typeof(GameplayController).GetMethod("HintFromHud", flags);
            next.Invoke(controller, null);
            next.Invoke(controller, null);

            yield return WaitFor(() => presenter.Session.Phase == JourneyPhase.AtNextCheckpoint, 8f);
            var chain = Resources.Load<JourneyChainAsset>("Journey/DesertRoadJourney").ToChain();
            var expectedNextLevelId = chain[chain.IndexOfLevel(startLevelId) + 1].LevelId;
            yield return WaitFor(() => presenter.LevelId == expectedNextLevelId &&
                presenter.Session.Phase == JourneyPhase.AtPuzzle, 15f);
            var startCheckpoint = presenter.GetComponentsInChildren<Transform>(true)
                .FirstOrDefault(child => child.name == "Start Checkpoint");
            Assert.That(startCheckpoint, Is.Not.Null, "Rebound segment must rebuild its checkpoint landmarks.");
        }

        [UnityTest]
        public IEnumerator BoardReadability_RendersRoadCellsTracksAndInteractableHalos()
        {
            yield return LocalizationSettings.InitializationOperation;
            SceneManager.LoadScene("Gameplay");
            yield return null;

            Assert.That(GameObject.Find("Road Cell 0,0"), Is.Not.Null, "Board floor road cell is missing.");
            Assert.That(GameObject.Find("Natural Desert Track"), Is.Not.Null, "Route track is missing.");

            var controller = Object.FindFirstObjectByType<GameplayController>();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var game = (BoardGame)typeof(GameplayController).GetField("_game", flags).GetValue(controller);
            Assert.That(game.State.Carts.Count, Is.GreaterThan(0));
            var cart = GameObject.Find(game.State.Carts.First().Id);
            Assert.That(cart, Is.Not.Null);
            Assert.That(cart.transform.Find("Selection Halo"), Is.Not.Null,
                "Interactable carts must carry a halo to separate them from decoration.");
        }

        private static IEnumerator WaitFor(System.Func<bool> condition, float timeout)
        {
            var deadline = Time.realtimeSinceStartup + timeout;
            while (!condition() && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(condition(), Is.True, $"Condition was not met within {timeout} seconds.");
        }
    }
}
