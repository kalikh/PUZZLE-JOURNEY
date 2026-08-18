using System;
using System.Linq;
using UnityEngine.Localization.Settings;

namespace CaravanSecrets.Data.Localization
{
    public interface ILocalizationService
    {
        string LanguageCode { get; }
        bool IsRightToLeft { get; }
        event Action LanguageChanged;
        void SetLanguage(string languageCode);
        string Get(string key);
    }

    public sealed class RuntimeLocalizationService : ILocalizationService
    {
        public string LanguageCode => LocalizationSettings.SelectedLocale?.Identifier.Code ?? "ar";
        public bool IsRightToLeft => LanguageCode.StartsWith("ar", StringComparison.OrdinalIgnoreCase);
        public event Action LanguageChanged;

        public void SetLanguage(string languageCode)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales.FirstOrDefault(candidate =>
                string.Equals(candidate.Identifier.Code, languageCode, StringComparison.OrdinalIgnoreCase));
            if (locale == null) throw new ArgumentException("Unsupported language.", nameof(languageCode));
            if (LocalizationSettings.SelectedLocale == locale) return;
            LocalizationSettings.SelectedLocale = locale;
            LanguageChanged?.Invoke();
        }

        public string Get(string key)
        {
            var value = LocalizationSettings.StringDatabase.GetLocalizedString("Gameplay", key);
            return string.IsNullOrEmpty(value) ? $"[{key}]" : value;
        }
    }
}
