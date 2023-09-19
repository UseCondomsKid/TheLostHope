using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LostHope.Engine.Localization
{
    public class LocalizationStringData
    {
        [System.Serializable]
        public struct Entry
        {
            public string id;
            public string en;
            public string fr;
            // TODO: Add other language ids here
            // Example:
            // public string fr; => For french
        }
        public enum Language
        {
            English,
            French,
            // TODO: Add other languages here
            // Example:
            // French => For french
        }
    }
}
