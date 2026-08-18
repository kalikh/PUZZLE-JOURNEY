using System.IO;
using System.Linq;
using CaravanSecrets.Core.Bootstrap;
using CaravanSecrets.Data.Levels;
using CaravanSecrets.Features.Gameplay;
using CaravanSecrets.Game.Board;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace CaravanSecrets.Editor
{
    public static class ProjectSetup
    {
        public static void ValidateFirstFiveLevels()
        {
            LevelTools.GenerateFirstFiveLevels();
            var levels = Resources.LoadAll<LevelAsset>("Levels").OrderBy(asset => asset.LevelId).Take(5).ToArray();
            if (levels.Length != 5) throw new BuildFailedException($"Expected 5 levels, found {levels.Length}.");
            foreach (var asset in levels)
            {
                var level = asset.ToDefinition();
                var issues = LevelValidator.Validate(level);
                if (issues.Count > 0) throw new BuildFailedException($"{level.Id}: {string.Join("; ", issues)}");
                var game = new BoardGame(level);
                for (var pass = 0; pass < 40 && !game.State.IsComplete; pass++)
                    foreach (var cart in level.Carts.Reverse()) game.Move(cart.Id);
                if (!game.State.IsComplete) throw new BuildFailedException($"{level.Id} did not solve.");
                Debug.Log($"CARAVAN_LEVEL_VALIDATED id={level.Id} moves={game.State.MoveCount}");
            }
            Debug.Log("CARAVAN_FIRST_FIVE_VALIDATION_COMPLETE");
        }

        [MenuItem("Caravan Secrets/Setup Project")]
        public static void Run()
        {
            LevelTools.GenerateAndValidate();
            PlayerSettings.companyName = "YSoft";
            PlayerSettings.productName = "Caravan Secrets: Puzzle Journey";
            PlayerSettings.SetApplicationIdentifier(NamedBuildTarget.Android, "com.ysoft.caravansecrets");
            PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64;
            PlayerSettings.SetScriptingBackend(NamedBuildTarget.Android, ScriptingImplementation.IL2CPP);
            PlayerSettings.Android.minSdkVersion = AndroidSdkVersions.AndroidApiLevel25;
            var projectSettings = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0]);
            var activeInputHandler = projectSettings.FindProperty("activeInputHandler");
            if (activeInputHandler != null)
            {
                activeInputHandler.intValue = 2;
                projectSettings.ApplyModifiedPropertiesWithoutUndo();
            }
            Directory.CreateDirectory("Assets/Scenes/Bootstrap");
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("GameBootstrap").AddComponent<GameBootstrap>();
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/Bootstrap/Bootstrap.unity");
            Directory.CreateDirectory("Assets/Scenes/Gameplay");
            var gameplay = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("GameplayController").AddComponent<GameplayController>();
            EditorSceneManager.SaveScene(gameplay, "Assets/Scenes/Gameplay/Gameplay.unity");
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/Scenes/Bootstrap/Bootstrap.unity", true),
                new EditorBuildSettingsScene("Assets/Scenes/Gameplay/Gameplay.unity", true)
            };
            AssetDatabase.SaveAssets();
            Debug.Log("CARAVAN_SETUP_COMPLETE");
        }

        public static void BuildAndroidDevelopment()
        {
            LevelTools.ValidateForBuild();
            Directory.CreateDirectory("Builds/Android");
            EditorUserBuildSettings.buildAppBundle = false;
            var options = new BuildPlayerOptions
            {
                scenes = new[] { "Assets/Scenes/Bootstrap/Bootstrap.unity", "Assets/Scenes/Gameplay/Gameplay.unity" },
                locationPathName = "Builds/Android/CaravanSecrets-development.apk",
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.Development
            };
            var report = BuildPipeline.BuildPlayer(options);
            if (report.summary.result != BuildResult.Succeeded)
                throw new BuildFailedException($"Android build failed: {report.summary.result}");
            Debug.Log($"CARAVAN_ANDROID_BUILD_COMPLETE bytes={report.summary.totalSize}");
        }
    }
}
