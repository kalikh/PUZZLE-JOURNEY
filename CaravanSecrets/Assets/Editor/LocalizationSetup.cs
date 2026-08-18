using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace CaravanSecrets.Editor
{
    public static class LocalizationSetup
    {
        private const string Folder = "Assets/Localization";

        private static readonly IReadOnlyDictionary<string, string> English = new Dictionary<string, string>
        {
            ["hud.level"] = "Level {0}/{1}", ["hud.moves"] = "Moves {0}",
            ["button.pause"] = "Pause", ["button.resume"] = "Resume", ["button.undo"] = "Undo",
            ["button.move"] = "Move", ["button.restart"] = "Restart", ["button.hint"] = "Hint", ["button.next"] = "Next",
            ["debug.levels"] = "All Levels",
            ["debug.language"] = "Language: {0}",
            ["debug.compass"] = "Compass",
            ["objective.order"] = "Clear the routes in the correct order",
            ["objective.gate"] = "Guide the caravan cart to the gate",
            ["objective.exit"] = "Guide the caravan cart to the gate",
            ["objective.cargo"] = "Deliver cargo to matching destinations",
            ["objective.switches"] = "Activate all switches",
            ["objective.separator"] = " · ",
            ["status.paused"] = "Paused", ["status.wrong_exit"] = "This gate belongs to another cart",
            ["status.stored"] = "The cart is held until the route opens", ["status.blocked"] = "The route is blocked",
            ["status.complete"] = "Route cleared!", ["instruction.move_selected"] = "Press Move to drive the selected cart",
            ["status.journey_to_puzzle"] = "Travelling to the puzzle site", ["status.journey_to_checkpoint"] = "Caravan travelling to the next checkpoint", ["status.checkpoint_reached"] = "Next checkpoint reached",
            ["status.cargo_delivered"] = "Cargo delivered to its matching symbol!",
            ["status.wrong_cargo_destination"] = "This cargo belongs at a different symbol",
            ["status.compass_hint"] = "Compass: move {0} next",
            ["status.compass_unavailable"] = "Compass found no safe next move",
            ["hint.1"] = "Select the cart, then press Move until it reaches the gate",
            ["hint.2"] = "Keep moving the cart forward to its gate", ["hint.3"] = "Follow the arrow to the upper gate",
            ["hint.4"] = "Move the vertical cart out before the horizontal cart",
            ["hint.5"] = "Clear the upper cart, then the vertical cart, then the horizontal cart"
        };

        private static readonly IReadOnlyDictionary<string, string> Arabic = new Dictionary<string, string>
        {
            ["hud.level"] = "المرحلة {0}/{1}", ["hud.moves"] = "الحركات {0}",
            ["button.pause"] = "إيقاف", ["button.resume"] = "متابعة", ["button.undo"] = "تراجع",
            ["button.move"] = "تحريك", ["button.restart"] = "إعادة", ["button.hint"] = "تلميح", ["button.next"] = "التالي",
            ["debug.levels"] = "كل المراحل",
            ["debug.language"] = "اللغة: {0}",
            ["debug.compass"] = "بوصلة",
            ["objective.order"] = "أخرج العربات بالترتيب الصحيح",
            ["objective.gate"] = "أوصل العربة إلى بوابتها",
            ["objective.exit"] = "أوصل العربة إلى بوابتها",
            ["objective.cargo"] = "سلّم الحمولة إلى الرموز المطابقة",
            ["objective.switches"] = "فعّل كل المفاتيح",
            ["objective.separator"] = " · ",
            ["status.paused"] = "متوقف مؤقتاً", ["status.wrong_exit"] = "هذه البوابة لعربة أخرى",
            ["status.stored"] = "العربة في مساحة الانتظار حتى يفتح المسار", ["status.blocked"] = "الطريق مغلق",
            ["status.complete"] = "تم فتح الطريق!", ["instruction.move_selected"] = "اضغط تحريك لقيادة العربة المحددة",
            ["status.journey_to_puzzle"] = "التوجه إلى موقع اللغز", ["status.journey_to_checkpoint"] = "القافلة في طريقها إلى نقطة التوقف التالية", ["status.checkpoint_reached"] = "تم الوصول إلى نقطة التوقف التالية",
            ["status.cargo_delivered"] = "تم تسليم الحمولة إلى رمزها المطابق!",
            ["status.wrong_cargo_destination"] = "هذه الحمولة مخصصة لرمز آخر",
            ["status.compass_hint"] = "البوصلة: حرّك {0} التالي",
            ["status.compass_unavailable"] = "البوصلة لم تجد حركة آمنة تالية",
            ["hint.1"] = "حدد العربة ثم اضغط تحريك حتى تصل إلى البوابة",
            ["hint.2"] = "تابع تحريك العربة إلى الأمام نحو بوابتها", ["hint.3"] = "اتبع السهم حتى البوابة العلوية",
            ["hint.4"] = "أخرج العربة العمودية قبل العربة الأفقية",
            ["hint.5"] = "أخرج العربة العلوية ثم العمودية ثم الأفقية"
        };

        [MenuItem("Caravan Secrets/Localization/Create or Update Tables")]
        public static void CreateOrUpdate()
        {
            Directory.CreateDirectory(Folder);
            EnsureActiveSettings();
            var english = EnsureLocale("en", SystemLanguage.English);
            var arabic = EnsureLocale("ar", SystemLanguage.Arabic);
            var collection = LocalizationEditorSettings.GetStringTableCollection("Gameplay")
                ?? LocalizationEditorSettings.CreateStringTableCollection("Gameplay", Folder, new List<Locale> { english, arabic });
            UpdateTable(collection.GetTable(english.Identifier) as StringTable ?? collection.AddNewTable(english.Identifier) as StringTable, English);
            UpdateTable(collection.GetTable(arabic.Identifier) as StringTable ?? collection.AddNewTable(arabic.Identifier) as StringTable, Arabic);
            foreach (var table in collection.StringTables) LocalizationEditorSettings.SetPreloadTableFlag(table, true);
            AssetDatabase.SaveAssets();
            Debug.Log("CARAVAN_LOCALIZATION_TABLES_READY");
        }

        private static void EnsureActiveSettings()
        {
            if (LocalizationEditorSettings.ActiveLocalizationSettings != null) return;
            var settings = ScriptableObject.CreateInstance<LocalizationSettings>();
            settings.name = "Caravan Localization Settings";
            AssetDatabase.CreateAsset(settings, $"{Folder}/Localization Settings.asset");
            LocalizationEditorSettings.ActiveLocalizationSettings = settings;
            EditorUtility.SetDirty(settings);
        }

        private static Locale EnsureLocale(string code, SystemLanguage language)
        {
            var locale = LocalizationEditorSettings.GetLocale(code);
            if (locale != null) return locale;
            locale = Locale.CreateLocale(language);
            AssetDatabase.CreateAsset(locale, $"{Folder}/Locale-{code}.asset");
            LocalizationEditorSettings.AddLocale(locale);
            return locale;
        }

        private static void UpdateTable(StringTable table, IReadOnlyDictionary<string, string> values)
        {
            foreach (var pair in values)
            {
                var entry = table.GetEntry(pair.Key) ?? table.AddEntry(pair.Key, pair.Value);
                entry.Value = pair.Value;
            }
            EditorUtility.SetDirty(table);
            EditorUtility.SetDirty(table.SharedData);
        }
    }
}
