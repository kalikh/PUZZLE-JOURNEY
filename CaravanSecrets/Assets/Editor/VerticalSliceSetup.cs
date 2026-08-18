using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
using TMPro;
using CaravanSecrets.Features.Gameplay;

namespace CaravanSecrets.Editor
{
    public static class VerticalSliceSetup
    {
        private const string ArtRoot = "Assets/Art/Region1/VerticalSlice";
        private const string ResourceRoot = "Assets/Resources/VerticalSlice";

        [MenuItem("Caravan Secrets/Vertical Slice/Generate Art Prefabs")]
        public static void Generate()
        {
            Directory.CreateDirectory(ResourceRoot);
            ConfigureSprite($"{ArtRoot}/cart.png", 512, true);
            ConfigureSprite($"{ArtRoot}/rock.png", 512, true);
            ConfigureSprite($"{ArtRoot}/gate.png", 512, true);
            ConfigureSprite($"{ArtRoot}/road-tile.png", 512, true);
            ConfigureSprite($"{ArtRoot}/road-strip.png", 512, true);
            ConfigureSprite($"{ArtRoot}/switch.png", 512, true);
            ConfigureSprite($"{ArtRoot}/desert-background.png", 512, false);

            CreatePrefab("Cart", LoadSprite("cart.png"), 30);
            CreatePrefab("Rock", LoadSprite("rock.png"), 15);
            CreatePrefab("Gate", LoadSprite("gate.png"), 20);
            CreatePrefab("RoadTile", LoadSprite("road-tile.png"), 0);
            CreatePrefab("RoadStrip", LoadSprite("road-strip.png"), 0);
            CreatePrefab("DesertSwitch", LoadSprite("switch.png"), 18);
            CreatePrefab("DesertBackground", LoadSprite("desert-background.png"), -20);
            CreateHudPrefab();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("CARAVAN_VERTICAL_SLICE_PREFABS_READY count=8");
        }

        private static void ConfigureSprite(string path, float pixelsPerUnit, bool alpha)
        {
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = pixelsPerUnit;
            importer.alphaIsTransparency = alpha;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.CompressedHQ;
            importer.SaveAndReimport();
        }

        private static Sprite LoadSprite(string name) => AssetDatabase.LoadAssetAtPath<Sprite>($"{ArtRoot}/{name}");

        private static void CreatePrefab(string name, Sprite sprite, int order)
        {
            var root = new GameObject(name, typeof(SpriteRenderer));
            var renderer = root.GetComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            renderer.sortingOrder = order;
            PrefabUtility.SaveAsPrefabAsset(root, $"{ResourceRoot}/{name}.prefab");
            Object.DestroyImmediate(root);
        }

        private static void CreateHudPrefab()
        {
            const string fontPath = "Assets/Resources/Fonts/ArialUnicode.ttf";
            const string fontAssetPath = "Assets/Resources/Fonts/ArialUnicode SDF.asset";
            var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(fontAssetPath);
            if (fontAsset == null)
            {
                var font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);
                fontAsset = TMP_FontAsset.CreateFontAsset(font, 64, 7, GlyphRenderMode.SDFAA, 1024, 1024, AtlasPopulationMode.Dynamic, true);
                AssetDatabase.CreateAsset(fontAsset, fontAssetPath);
                foreach (var atlas in fontAsset.atlasTextures) AssetDatabase.AddObjectToAsset(atlas, fontAsset);
                AssetDatabase.AddObjectToAsset(fontAsset.material, fontAsset);
                EditorUtility.SetDirty(fontAsset);
                AssetDatabase.SaveAssets();
            }

            var root = new GameObject("GameplayHUD", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster), typeof(EventSystem), typeof(StandaloneInputModule), typeof(GameplayHudView));
            root.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 2160);
            scaler.matchWidthOrHeight = 0.5f;

            var topPanel = Panel(root.transform, "TopPanel", new Color(0.055f, 0.10f, 0.14f, 0.92f), new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, 190), new Vector2(0, -105));
            var level = Text(topPanel.transform, "LevelLabel", fontAsset, 40, TextAlignmentOptions.Left, new Vector2(0, 0.55f), new Vector2(0.5f, 1), new Vector2(35, -18), new Vector2(-10, -5));
            var moves = Text(topPanel.transform, "MovesLabel", fontAsset, 40, TextAlignmentOptions.Right, new Vector2(0.5f, 0.55f), new Vector2(1, 1), new Vector2(10, -18), new Vector2(-35, -5));
            var objective = Text(topPanel.transform, "ObjectiveLabel", fontAsset, 31, TextAlignmentOptions.Center, new Vector2(0.04f, 0.06f), new Vector2(0.96f, 0.56f), Vector2.zero, Vector2.zero);

            var bottom = Panel(root.transform, "BottomBar", new Color(0.055f, 0.10f, 0.14f, 0.94f), new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 190), new Vector2(0, 115));
            var pause = Button(bottom.transform, "PauseButton", fontAsset, 29, 0.01f, 0.17f);
            var undo = Button(bottom.transform, "UndoButton", fontAsset, 29, 0.19f, 0.35f);
            var drive = Button(bottom.transform, "DriveButton", fontAsset, 36, 0.37f, 0.63f);
            drive.gameObject.AddComponent<HoldMoveButton>();
            var restart = Button(bottom.transform, "RestartButton", fontAsset, 29, 0.65f, 0.81f);
            var hint = Button(bottom.transform, "HintButton", fontAsset, 29, 0.83f, 0.99f);

            var serialized = new SerializedObject(root.GetComponent<GameplayHudView>());
            serialized.FindProperty("levelLabel").objectReferenceValue = level;
            serialized.FindProperty("objectiveLabel").objectReferenceValue = objective;
            serialized.FindProperty("movesLabel").objectReferenceValue = moves;
            serialized.FindProperty("pauseButton").objectReferenceValue = pause;
            serialized.FindProperty("undoButton").objectReferenceValue = undo;
            serialized.FindProperty("driveButton").objectReferenceValue = drive;
            serialized.FindProperty("restartButton").objectReferenceValue = restart;
            serialized.FindProperty("hintButton").objectReferenceValue = hint;
            serialized.ApplyModifiedPropertiesWithoutUndo();

            PrefabUtility.SaveAsPrefabAsset(root, $"{ResourceRoot}/GameplayHUD.prefab");
            Object.DestroyImmediate(root);
        }

        private static Image Panel(Transform parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 sizeDelta, Vector2 position)
        {
            var item = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            item.transform.SetParent(parent, false);
            var rect = (RectTransform)item.transform;
            rect.anchorMin = anchorMin; rect.anchorMax = anchorMax; rect.sizeDelta = sizeDelta; rect.anchoredPosition = position;
            var image = item.GetComponent<Image>(); image.color = color;
            return image;
        }

        private static TextMeshProUGUI Text(Transform parent, string name, TMP_FontAsset font, float size, TextAlignmentOptions alignment, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var item = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(TextMeshProUGUI));
            item.transform.SetParent(parent, false);
            var rect = (RectTransform)item.transform;
            rect.anchorMin = anchorMin; rect.anchorMax = anchorMax; rect.offsetMin = offsetMin; rect.offsetMax = offsetMax;
            var text = item.GetComponent<TextMeshProUGUI>();
            text.font = font; text.fontSize = size; text.alignment = alignment; text.color = Color.white; text.enableWordWrapping = true;
            return text;
        }

        public static void ImportTextMeshProResources()
        {
            var packages = Directory.GetFiles("Library/PackageCache", "TMP Essential Resources.unitypackage", SearchOption.AllDirectories);
            if (packages.Length == 0) throw new FileNotFoundException("TMP Essential Resources.unitypackage was not found.");
            AssetDatabase.ImportPackage(packages[0], false);
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            Debug.Log("CARAVAN_TMP_RESOURCES_READY");
        }

        private static Button Button(Transform parent, string name, TMP_FontAsset font, float size, float minX, float maxX)
        {
            var image = Panel(parent, name, new Color(0.18f, 0.12f, 0.07f, 0.98f), new Vector2(minX, 0.16f), new Vector2(maxX, 0.84f), Vector2.zero, Vector2.zero);
            var button = image.gameObject.AddComponent<Button>();
            var label = Text(image.transform, "Label", font, size, TextAlignmentOptions.Center, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            label.raycastTarget = false;
            return button;
        }
    }
}
