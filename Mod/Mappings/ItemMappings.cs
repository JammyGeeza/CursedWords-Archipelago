using BepInEx.Logging;
using Mod.Classes;
using Mod.Extensions;
using Mod.Helpers;
using Modd;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Mod.Mappings
{
    public static class ItemMappings
    {
        private static ManualLogSource Logger
        {
            get => CursedWordsArchipelago.Instance.LogSource;
        }

        public static Dictionary<string, Func<IEnumerator>> Map = new Dictionary<string, Func<IEnumerator>>()
        {
            // Characters
            { "Rodman", () => UnlockCharacter(typeof(WetDennis)) },
            { "Nina Nix", () => UnlockCharacter(typeof(NinaNix)) },
            { "Hayley Bayles", () => UnlockCharacter(typeof(HayleyBayles)) },

            // Re-rolls
            { "Progressive Re-roll", () => IncrementReroll() },

            // Slots
            { "Progressive Stamp Slot", () => FreeStampSlot() },
            { "Progressive Sticker Slot", () => FreeStickerSlot() },

            // Stamps
            { "Blue Stamps", () => UnlockBulkUnlock(new BlueBuildStampsUnlock()) },
            { "Rainbow Stamps", () => UnlockBulkUnlock(new RainbowBuildStampsUnlock()) },
            { "Red Stamps", () => UnlockBulkUnlock(new RedBuildStampsUnlock()) },
            { "Shiny Stamps", () => UnlockBulkUnlock(new ShinyBuildStampsUnlock()) },
            { "Void Stamps", () => UnlockBulkUnlock(new VoidBuildStampsUnlock()) },

            // Stickers
            { "Blue Stickers", () => UnlockBulkUnlock(new BlueBuildStickersUnlock()) },
            { "Rainbow Stickers", () => UnlockBulkUnlock(new RainbowBuildStickersUnlock()) },
            { "Red Stickers", () => UnlockBulkUnlock(new RedBuildStickersUnlock()) },
            { "Shiny Stickers", () => UnlockBulkUnlock(new ShinyBuildStickersUnlock()) },
            { "Void Stickers", () => UnlockBulkUnlock(new VoidBuildStickersUnlock()) },

            // Filler
            { "$1", () => IncrementMoney(1) },
            { "$2", () => IncrementMoney(2) },
            { "$3", () => IncrementMoney(3) },
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
            new LocationCriteria("Rodman - 4-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 4 && type == NodeType.EncounterFirst },
            new LocationCriteria("Rodman - 4-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 4 && type == NodeType.EncounterSecond },
            new LocationCriteria("Rodman - 4-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 4 && type == NodeType.Boss },
            new LocationCriteria("Rodman - 5-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 5 && type == NodeType.EncounterFirst },
            new LocationCriteria("Rodman - 5-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 5 && type == NodeType.EncounterSecond },
            new LocationCriteria("Rodman - 5-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is WetDennis && stage == 5 && type == NodeType.Boss },

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
            new LocationCriteria("Nina Nix - 5-1 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 5 && type == NodeType.EncounterFirst },
            new LocationCriteria("Nina Nix - 5-2 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 5 && type == NodeType.EncounterSecond },
            new LocationCriteria("Nina Nix - 5-3 Complete") { OnEncounterAction = (Character character, int stage, NodeType type) => character is NinaNix && stage == 5 && type == NodeType.Boss },

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

            new LocationCriteria("Sell a Tile") { OnGenericAction = (action) => action == "destroy_tile" },

            new LocationCriteria("Freeze a Stamp") { OnGenericAction = (action) => action == "freeze_stamp" },
            new LocationCriteria("Freeze a Sticker") { OnGenericAction = (action) => action == "freeze_sticker" },

            new LocationCriteria("Restock the Shop") { OnGenericAction = (action) => action == "restock_shop" },

            new LocationCriteria("Sell a Stamp") { OnGenericAction = (action) => action == "sell_stamp" },
            new LocationCriteria("Sell a Sticker") { OnGenericAction = (action) => action == "sell_sticker" },

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
            new LocationCriteria("Word Score > 75") { OnWordAction = (action, score) => action == "word_score" && score >= 75 },
            new LocationCriteria("Word Score > 100") { OnWordAction = (action, score) => action == "word_score" && score >= 100 },
            new LocationCriteria("Word Score > 250") { OnWordAction = (action, score) => action == "word_score" && score >= 250 },
            new LocationCriteria("Word Score > 500") { OnWordAction = (action, score) => action == "word_score" && score >= 500 },
            new LocationCriteria("Word Score > 750") { OnWordAction = (action, score) => action == "word_score" && score >= 750 },
            new LocationCriteria("Word Score > 1000") { OnWordAction = (action, score) => action == "word_score" && score >= 1000 },

            #endregion
        };

        static IEnumerator FreeStampSlot()
        {
            if (CharacterInfoPanel.SingletonInventoryVisualController != null)
            {
                Logger.LogInfo("Attempting to free a stamp slot...");

                Player player = GameStatics.GetPlayer();
                if (player.GetStamps().FirstOrDefault(itm => itm is APStampPadlock) is APStampPadlock stampPadlock && stampPadlock != null)
                {
                    Logger.LogInfo("Removing stamp padlock...");
                    player.RemoveItemFromInventory(stampPadlock);
                }

                CharacterInfoPanel.SingletonInventoryVisualController.PopulateStamps();
            }

            yield break;
        }

        static IEnumerator FreeStickerSlot()
        {
            if (CharacterInfoPanel.SingletonInventoryVisualController != null)
            {
                Logger.LogInfo("Attempting to free a sticker slot...");

                Player player = GameStatics.GetPlayer();
                if (player.GetStickers().FirstOrDefault(itm => itm is APStickerPadlock) is APStickerPadlock stickerPadlock && stickerPadlock != null)
                {
                    Logger.LogInfo("Removing sticker padlock...");
                    player.RemoveItemFromInventory(stickerPadlock);
                }

                CharacterInfoPanel.SingletonInventoryVisualController.PopulateStickers();
            }

            yield break;
        }

        static IEnumerator IncrementMoney(int amount)
        {
            Logger.LogInfo("Attempting to increment money...");

            if (CharacterInfoPanel.SingletonInventoryVisualController != null)
            {
                Logger.LogInfo($"Incrementing money by {amount}...");

                Player player = GameStatics.GetPlayer();
                player.ChangeMoney(amount);

                CharacterInfoPanel.SingletonInventoryVisualController.PopulateCash();
            }

            yield break;
        }

        static IEnumerator IncrementReroll()
        {
            Logger.LogInfo("Attempting to increment re-roll count...");
            
            if (UnityEngine.Object.FindFirstObjectByType<EncounterController>() is EncounterController encounterController && encounterController != null)
            {
                Logger.LogInfo("Attempting to increment re-roll count...");
                encounterController.IncrementEncounterRerollAmount(1);
            }

            yield break;
        }

        static IEnumerator UnlockBulkUnlock(BulkUnlock unlock)
        {
            Logger.LogInfo($"Unlocking bulk unlock '{unlock.Name}'...");
            SaveManager.UnlockBulkUnlock(unlock);
            yield break;
        }

        static IEnumerator UnlockCharacter(Type characterType)
        {
            Logger.LogInfo($"Unlocking character '{characterType.Name}");
            SaveManager.AddCharacterToUnlockedCharacters(characterType);
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
