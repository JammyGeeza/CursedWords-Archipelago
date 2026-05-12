using Mod.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mod.Mappings
{
    public static class Lookups
    {
        public static readonly Type[] ValidBulkUnlockTypes =
        {
            typeof(BlueBuildStampsUnlock),
            typeof(BlueBuildStickersUnlock),
            typeof(CardsBuildStampsUnlock),
            typeof(CardsBuildStickersUnlock),
            typeof(ChessBuildStampsUnlock),
            typeof(ChessBuildStickersUnlock),
            typeof(CurrencyBuildStampsUnlock),
            typeof(CurrencyBuildStickersUnlock),
            typeof(CurseBuildStampsUnlock),
            typeof(CurseBuildStickersUnlock),
            typeof(NumberBuildStampsUnlock),
            typeof(NumberBuildStickersUnlock),
            typeof(RainbowBuildStampsUnlock),
            typeof(RainbowBuildStickersUnlock),
            typeof(RedBuildStampsUnlock),
            typeof(RedBuildStickersUnlock),
            typeof(ScatteredBuildStickersUnlock),
            typeof(ScatteredBuildStampsUnlock),
            typeof(ShinyBuildStickersUnlock),
            typeof(ShinyBuildStampsUnlock),
            typeof(VoidBuildStampsUnlock),
            typeof(VoidBuildStickersUnlock),
        };


        /// <summary>
        /// Lookup character types by character name.
        /// </summary>
        public static readonly Dictionary<string, Type> CharacterNameToType = new Dictionary<string, Type>()
        {
            { "Rodman", typeof(WetDennis) },
            { "Nina Nix", typeof(NinaNix) },
            { "Hayley Bayles", typeof(HayleyBayles) },

            // TODO: Add the rest here
        };

        public static readonly List<Type> BlueBuildStamps = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStampWithTag(t, ItemTag.BlueBuild))
            .ToList();

        public static readonly List<Type> BlueBuildStickers = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStickerWithTag(t, ItemTag.BlueBuild))
            .ToList();

        public static readonly List<Type> CardsBuildStamps = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStampWithTag(t, ItemTag.CardsBuild))
            .ToList();

        public static readonly List<Type> CardsBuildStickers = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStickerWithTag(t, ItemTag.CardsBuild))
            .ToList();

        public static readonly List<Type> ChessBuildStamps = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStampWithTag(t, ItemTag.ChessBuild))
            .ToList();

        public static readonly List<Type> ChessBuildStickers = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStickerWithTag(t, ItemTag.ChessBuild))
            .ToList();

        public static readonly List<Type> CurrencyBuildStamps = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStampWithTag(t, ItemTag.CashBuild))
            .ToList();

        public static readonly List<Type> CurrencyBuildStickers = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStickerWithTag(t, ItemTag.CashBuild))
            .ToList();

        public static readonly List<Type> CurseBuildStamps = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStampWithTag(t, ItemTag.CurseBuild))
            .ToList();

        public static readonly List<Type> CurseBuildStickers = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStickerWithTag(t, ItemTag.CurseBuild))
            .ToList();

        public static readonly List<Type> NumberBuildStamps = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStampWithTag(t, ItemTag.NumbersBuild))
            .ToList();

        public static readonly List<Type> NumberBuildStickers = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStickerWithTag(t, ItemTag.NumbersBuild))
            .ToList();

        public static readonly List<Type> RainbowBuildStamps = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStampWithTag(t, ItemTag.RainbowBuild))
            .ToList();

        public static readonly List<Type> RainbowBuildStickers = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStickerWithTag(t, ItemTag.RainbowBuild))
            .ToList();

        public static readonly List<Type> RedBuildStamps = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStampWithTag(t, ItemTag.RedBuild))
            .ToList();

        public static readonly List<Type> RedBuildStickers = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStickerWithTag(t, ItemTag.RedBuild))
            .ToList();

        public static readonly List<Type> ScatteredBuildStamps = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStampWithTag(t, ItemTag.ScatteredItemsBuild))
            .ToList();

        public static readonly List<Type> ScatteredBuildStickers = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStickerWithTag(t, ItemTag.ScatteredItemsBuild))
            .ToList();

        public static readonly List<Type> ShinyBuildStamps = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStampWithTag(t, ItemTag.ShinyBuild))
            .ToList();

        public static readonly List<Type> ShinyBuildStickers = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStickerWithTag(t, ItemTag.ShinyBuild))
            .ToList();

        public static readonly List<Type> VoidBuildStamps = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStampWithTag(t, ItemTag.VoidBuild))
            .ToList();

        public static readonly List<Type> VoidBuildStickers = Assembly.GetAssembly(typeof(Item))
            .GetTypes()
            .Where(t => IsStickerWithTag(t, ItemTag.VoidBuild))
            .ToList();

        private static bool IsItemClass(Type t)
        {
            return t.IsClass && t.IsSubclassOf(typeof(Item));
        }

        private static bool IsStampWithTag(Type type, ItemTag itemTag)
        {
            if (!IsItemClass(type)) return false;

            Item item = Activator.CreateInstance(type) as Item;
            return item.IsStamp() && item.Tags.Contains(itemTag);
        }

        private static bool IsStickerWithTag(Type type, ItemTag itemTag)
        {
            if (!IsItemClass(type)) return false;

            Item item = Activator.CreateInstance(type) as Item;
            return item.IsSticker() && item.Tags.Contains(itemTag);
        }
    }
}
