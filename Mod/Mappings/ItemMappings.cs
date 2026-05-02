using Mod.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Mod.Mappings
{
    public static class ItemMappings
    {
        public static Dictionary<string, IEnumerator> Map = new Dictionary<string, IEnumerator>()
        {
            { "Rodman", UnlockRodman() },
            { "Nina Nix", UnlockNinaNix() },
            { "Hayley Bayles", UnlockHayleyBayles() },

            // Filler
            { "$", AddToPiggyBank(1)  },
            { "$$", AddToPiggyBank(2) },
            { "$$$", AddToPiggyBank(3) },
        };

        public static List<LocationCriteria> Locations = new List<LocationCriteria>()
        {
            // Word lengths
            new LocationCriteria("Word Length 1") { Check = (string action, long amount) => action == "word_length" && amount == 1 },
            new LocationCriteria("Word Length 2") { Check = (string action, long amount) => action == "word_length" && amount == 2 },
            new LocationCriteria("Word Length 3") { Check = (string action, long amount) => action == "word_length" && amount == 3 },
            new LocationCriteria("Word Length 4") { Check = (string action, long amount) => action == "word_length" && amount == 4 },
            new LocationCriteria("Word Length 5") { Check = (string action, long amount) => action == "word_length" && amount == 5 },
            new LocationCriteria("Word Length 6") { Check = (string action, long amount) => action == "word_length" && amount == 6 },
            new LocationCriteria("Word Length 7") { Check = (string action, long amount) => action == "word_length" && amount == 7 },
            new LocationCriteria("Word Length 8") { Check = (string action, long amount) => action == "word_length" && amount == 8 },
            new LocationCriteria("Word Length 9") { Check = (string action, long amount) => action == "word_length" && amount == 9 },
            new LocationCriteria("Word Length 10") { Check = (string action, long amount) => action == "word_length" && amount == 10 },

            // Word scores
            new LocationCriteria("Word Score > 5") { Check = (string action, long amount) => action == "word_score" && amount >= 5 },
            new LocationCriteria("Word Score > 10") { Check = (string action, long amount) => action == "word_score" && amount >= 10 },
            new LocationCriteria("Word Score > 25") { Check = (string action, long amount) => action == "word_score" && amount >= 25 },
            new LocationCriteria("Word Score > 50") { Check = (string action, long amount) => action == "word_score" && amount >= 50 },
            new LocationCriteria("Word Score > 100") { Check = (string action, long amount) => action == "word_score" && amount >= 100 },
            new LocationCriteria("Word Score > 250") { Check = (string action, long amount) => action == "word_score" && amount >= 250 },
            new LocationCriteria("Word Score > 500") { Check = (string action, long amount) => action == "word_score" && amount >= 500 },
            new LocationCriteria("Word Score > 1000") { Check = (string action, long amount) => action == "word_score" && amount >= 1000 },
        };

        static IEnumerator UnlockRodman()
        {
            SaveManager.AddCharacterToUnlockedCharacters(typeof(WetDennis));
            yield break;
        }

        static IEnumerator UnlockNinaNix()
        {
            SaveManager.AddCharacterToUnlockedCharacters(typeof(NinaNix));
            SaveManager.SetSeenNinaIntroDialogue();
            yield break;
        }

        static IEnumerator UnlockHayleyBayles()
        {
            SaveManager.AddCharacterToUnlockedCharacters(typeof(HayleyBayles));
            yield break;
        }

        static IEnumerator AddToPiggyBank(int amount)
        {
            SaveManager.SaveMoneyInPiggyBank(amount);
            yield break;
        }
    }

    public class LocationCriteria
    {
        public Action Action { get; set; }
        public Func<string, long, bool> Check { get; set; }
        public string LocationName { get; set; }

        public LocationCriteria(string locationName)
        {
            LocationName = locationName;
        }
    }
}
