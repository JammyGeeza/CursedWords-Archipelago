using Mod.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Mod.Mappings
{
    public static class ItemMappings
    {
        public static Dictionary<string, IEnumerator> Map = new Dictionary<string, IEnumerator>()
        {
            // Characters
            { "Rodman", UnlockRodman() },
            { "Nina Nix", UnlockNinaNix() },
            { "Hayley Bayles", UnlockHayleyBayles() },

            // Filler
            { "$1", AddToPiggyBank(1)  },
            { "$2", AddToPiggyBank(2) },
            { "$3", AddToPiggyBank(3) },
        };

        public static List<LocationCriteria> Locations = new List<LocationCriteria>()
        {
            #region Encounters

            // Rodman
            new LocationCriteria("Rodman - 1-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 1 && type == NodeType.EncounterFirst },
            new LocationCriteria("Rodman - 1-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 1 && type == NodeType.EncounterSecond },
            new LocationCriteria("Rodman - 1-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 1 && type == NodeType.Boss },
            new LocationCriteria("Rodman - 2-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 2 && type == NodeType.EncounterFirst },
            new LocationCriteria("Rodman - 2-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 2 && type == NodeType.EncounterSecond },
            new LocationCriteria("Rodman - 2-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 2 && type == NodeType.Boss },
            new LocationCriteria("Rodman - 3-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 3 && type == NodeType.EncounterFirst },
            new LocationCriteria("Rodman - 3-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 3 && type == NodeType.EncounterSecond },
            new LocationCriteria("Rodman - 3-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 3 && type == NodeType.Boss },

            // Nina Nix
            new LocationCriteria("Nina Nix - 1-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 1 && type == NodeType.EncounterFirst },
            new LocationCriteria("Nina Nix - 1-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 1 && type == NodeType.EncounterSecond },
            new LocationCriteria("Nina Nix - 1-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 1 && type == NodeType.Boss },
            new LocationCriteria("Nina Nix - 2-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 2 && type == NodeType.EncounterFirst },
            new LocationCriteria("Nina Nix - 2-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 2 && type == NodeType.EncounterSecond },
            new LocationCriteria("Nina Nix - 2-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 2 && type == NodeType.Boss },
            new LocationCriteria("Nina Nix - 3-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 3 && type == NodeType.EncounterFirst },
            new LocationCriteria("Nina Nix - 3-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 3 && type == NodeType.EncounterSecond },
            new LocationCriteria("Nina Nix - 3-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 3 && type == NodeType.Boss },
            new LocationCriteria("Nina Nix - 4-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 4 && type == NodeType.EncounterFirst },
            new LocationCriteria("Nina Nix - 4-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 4 && type == NodeType.EncounterSecond },
            new LocationCriteria("Nina Nix - 4-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 4 && type == NodeType.Boss },

            // Hayley Bayles
            new LocationCriteria("Hayley Bayles - 1-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 1 && type == NodeType.EncounterFirst },
            new LocationCriteria("Hayley Bayles - 1-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 1 && type == NodeType.EncounterSecond },
            new LocationCriteria("Hayley Bayles - 1-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 1 && type == NodeType.Boss },
            new LocationCriteria("Hayley Bayles - 2-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 2 && type == NodeType.EncounterFirst },
            new LocationCriteria("Hayley Bayles - 2-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 2 && type == NodeType.EncounterSecond },
            new LocationCriteria("Hayley Bayles - 2-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 2 && type == NodeType.Boss },
            new LocationCriteria("Hayley Bayles - 3-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 3 && type == NodeType.EncounterFirst },
            new LocationCriteria("Hayley Bayles - 3-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 3 && type == NodeType.EncounterSecond },
            new LocationCriteria("Hayley Bayles - 3-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 3 && type == NodeType.Boss },
            new LocationCriteria("Hayley Bayles - 4-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 4 && type == NodeType.EncounterFirst },
            new LocationCriteria("Hayley Bayles - 4-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 4 && type == NodeType.EncounterSecond },
            new LocationCriteria("Hayley Bayles - 4-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 4 && type == NodeType.Boss },
            new LocationCriteria("Hayley Bayles - 5-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 5 && type == NodeType.EncounterFirst },
            new LocationCriteria("Hayley Bayles - 5-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 5 && type == NodeType.EncounterSecond },
            new LocationCriteria("Hayley Bayles - 5-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is HayleyBayles && stage == 5 && type == NodeType.Boss },

            #endregion

            #region Shop

            new LocationCriteria("Buy a Stamp") { OnGenericAction = (action) => action == "buy_stamp" },
            new LocationCriteria("Buy a Sticker") { OnGenericAction = (action) => action == "buy_sticker" },
            new LocationCriteria("Buy a Tile") { OnGenericAction = (action) => action == "buy_tile" },

            new LocationCriteria("Freeze a Stamp") { OnGenericAction = (action) => action == "freeze_stamp" },
            new LocationCriteria("Freeze a Sticker") { OnGenericAction = (action) => action == "freeze_sticker" },

            new LocationCriteria("Restock the Shop") { OnGenericAction = (action) => action == "restock_shop" },

            new LocationCriteria("Sell a Stamp") { OnGenericAction = (action) => action == "sell_stamp" },
            new LocationCriteria("Sell a Sticker") { OnGenericAction = (action) => action == "sell_sticker" },
            new LocationCriteria("Sell a Tile") { OnGenericAction = (action) => action == "sell_tile" },

            #endregion

            #region Words

            // Word lengths
            new LocationCriteria("Word Length 1") { OnWordAction = (action, length) => action == "word_length" && length == 1 },
            new LocationCriteria("Word Length 2") { OnWordAction = (action, length) => action == "word_length" && length == 2 },
            new LocationCriteria("Word Length 3") { OnWordAction = (action, length) => action == "word_length" && length == 3 },
            new LocationCriteria("Word Length 4") { OnWordAction = (action, length) => action == "word_length" && length == 4 },
            new LocationCriteria("Word Length 5") { OnWordAction = (action, length) => action == "word_length" && length == 5 },
            new LocationCriteria("Word Length 6") { OnWordAction = (action, length) => action == "word_length" && length == 6 },
            new LocationCriteria("Word Length 7") { OnWordAction = (action, length) => action == "word_length" && length == 7 },
            new LocationCriteria("Word Length 8") { OnWordAction = (action, length) => action == "word_length" && length == 8 },
            new LocationCriteria("Word Length 9") { OnWordAction = (action, length) => action == "word_length" && length == 9 },
            new LocationCriteria("Word Length 10") { OnWordAction = (action, length) => action == "word_length" && length == 10 },

            // Word scores
            new LocationCriteria("Word Score > 5") { OnWordAction = (action, score) => action == "word_score" && score >= 5 },
            new LocationCriteria("Word Score > 10") { OnWordAction = (action, score) => action == "word_score" && score >= 10 },
            new LocationCriteria("Word Score > 25") { OnWordAction = (action, score) => action == "word_score" && score >= 25 },
            new LocationCriteria("Word Score > 50") { OnWordAction = (action, score) => action == "word_score" && score >= 50 },
            new LocationCriteria("Word Score > 100") { OnWordAction = (action, score) => action == "word_score" && score >= 100 },
            new LocationCriteria("Word Score > 250") { OnWordAction = (action, score) => action == "word_score" && score >= 250 },
            new LocationCriteria("Word Score > 500") { OnWordAction = (action, score) => action == "word_score" && score >= 500 },
            new LocationCriteria("Word Score > 1000") { OnWordAction = (action, score) => action == "word_score" && score >= 1000 },

            #endregion
        };

        static IEnumerator UnlockRodman()
        {
            Debug.Log("Unlocking Rodman...");
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
        public Func<string, bool> OnGenericAction { get; set; }
        public Func<Character, int, NodeType, bool> OnEncounterAction { get; set; }
        public Func<string, long, bool> OnWordAction { get; set; }
        public string LocationName { get; set; }

        public LocationCriteria(string locationName)
        {
            LocationName = locationName;
        }
    }
}
