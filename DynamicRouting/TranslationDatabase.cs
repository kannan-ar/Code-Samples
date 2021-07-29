using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DynamicRouting
{
    public class TranslationDatabase
    {
        private static Dictionary<string, Dictionary<string, string>> Translations = new Dictionary<string, Dictionary<string, string>>
        {
            {
                "en", new Dictionary<string, string>
                {
                    {"orders", "orders" },
                    {"list", "list" }
                }
            },
            {
                "de", new Dictionary<string, string>
                {
                    { "bestellungen", "orders" },
                    { "liste", "list" }
                }
            },
            {
                "pl", new Dictionary<string, string>
                {
                    { "zamowienia", "order" },
                    { "lista", "list" }
                }
            }
        };

        public  string Resolve(string lang, string value)
        {
            var normalizedLang = lang.ToLowerInvariant();
            var normalizedValue = value.ToLowerInvariant();

            if(Translations.ContainsKey(normalizedLang) && Translations[normalizedLang].ContainsKey(normalizedValue))
            {
                return Translations[normalizedLang][normalizedValue];
            }

            return null;
        }
    }
}
