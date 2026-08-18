using System;
using UnityEngine.Localization.Settings;

namespace CaravanSecrets.Features.Gameplay
{
    internal static class GameplayStrings
    {
        private const string Table = "Gameplay";

        internal static bool IsArabic => (LocalizationSettings.SelectedLocale?.Identifier.Code ?? "ar")
            .StartsWith("ar", StringComparison.OrdinalIgnoreCase);

        internal static string Get(string key, params object[] arguments)
        {
            var value = LocalizationSettings.StringDatabase.GetLocalizedString(Table, key, arguments: arguments);
            return string.IsNullOrEmpty(value) ? $"[{key}]" : value;
        }
    }
}
