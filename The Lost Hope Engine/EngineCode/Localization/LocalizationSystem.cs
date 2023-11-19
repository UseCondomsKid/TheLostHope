using TheLostHope.Engine.ContentManagement;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace TheLostHopeEngine.EngineCode.Localization
{
    public class LocalizationSystem
    {
        // Setting the current language to be the first language by default
        public static LocalizationStringData.Language currentLanguage = LocalizationStringData.Language.English;
        public static Action onChangedLanguage;

        private static bool isInitialized = false;


        // ----------------------------------------------------
        // Create language dictionaries
        private static Dictionary<string, string> localizedEN;
        private static Dictionary<string, string> localizedFR;
        // TODO: Add new language dicts here
        // Example:
        // private static Dictionary<string, string> localizedFR; => For French
        // ----------------------------------------------------

        /// <summary>
        /// Initilizes the localization system at runtime.
        /// Should be called at the start of the game
        /// </summary>
        public static void Initialize()
        {
            // Initializing dictionaries
            localizedEN = new Dictionary<string, string>();
            localizedFR = new Dictionary<string, string>();
            // TODO: Initialize other dictionaries

            UpdateDictionaries();

            isInitialized = true;
        }

        /// <summary>
        /// Returns the localized value of the provided key in the active language
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetLocalizedValue(string key)
        {
            if (!isInitialized) Initialize();

            string value = key;

            if (value == "") return "Enter a valid key";

            // Getting the localized string
            switch (currentLanguage)
            {
                // TODO: Add other cases for other languages
                case LocalizationStringData.Language.English:
                    if (!localizedEN.TryGetValue(key, out value)) return "English Word not localized";
                    break;
                case LocalizationStringData.Language.French:
                    if (!localizedFR.TryGetValue(key, out value)) return "French Word not localized";
                    break;
                default:
                    return "Language doesn't exist";
            }

            value = RuntimeReplaceKeysInValue(value);
            return value;
        }
        private static string RuntimeReplaceKeysInValue(string value)
        {
            if (!Regex.IsMatch(value, @"<(\w+)_(\w+)>")) return value;

            // TODO: Replace keys with correct values during runtime

            return value;
        }

        /// <summary>
        /// Changes the active language
        /// </summary>
        /// <param name="new_language"></param>
        public static void ChangeLanguage(LocalizationStringData.Language new_language)
        {
            if (!isInitialized) Initialize();

            currentLanguage = new_language;

            onChangedLanguage?.Invoke();
        }

        private static void UpdateDictionaries()
        {
            foreach (var entry in ContentLoader.LoadLocalizationFileAsEntries())
            {
                // Adding localized values to dictionaries
                localizedEN.Add(entry.id, entry.en);
                localizedFR.Add(entry.id, entry.fr);
                // TODO: Add other values to other languages' dictionaries
                // Example:
                // localizedFR.Add(entry.id, entry.fr); => For french
            }
        }
    }
}
