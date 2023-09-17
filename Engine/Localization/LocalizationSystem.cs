using LostHope.Engine.ContentLoading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace LostHope.Engine.Localization
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
        // TODO: Add new language dicts here
        // Example:
        // private static Dictionary<string, string> localizedFR; => For French
        // ----------------------------------------------------

        /// <summary>
        /// Initilizes the localization system at runtime.
        /// Should be called at the start of the game
        /// </summary>
        public static void Init()
        {
            // Initializing dictionaries
            localizedEN = new Dictionary<string, string>();

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
            if (!isInitialized) Init();

            string value = key;

            if (value == "") return "Enter a valid key";

            // Getting the localized string
            switch (currentLanguage)
            {
                case LocalizationStringData.Language.English:
                    if (!localizedEN.TryGetValue(key, out value)) return "Word not localized";
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
            if (!isInitialized) Init();

            currentLanguage = new_language;
            // UpdateDictionaries();

            onChangedLanguage?.Invoke();
        }

        private static void UpdateDictionaries()
        {
            // TODO: Load entries

            //foreach (var entry in entries)
            //{
            //    // Adding localized values to dictionaries
            //    localizedEN.Add(entry.id, entry.en);
            //}
        }
    }
}
