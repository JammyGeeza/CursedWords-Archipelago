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

        //public static readonly List<Type> BlankStamps = Assembly.GetAssembly(typeof(Item)).GetTypes()
        //    .Where(t => IsStampWithAnyTags(t, ItemTag.BlankBuild, ItemTag.BlankGenerator))
        //    .ToList();

        //public static readonly List<Type> BlankStickers = Assembly.GetAssembly(typeof(Item)).GetTypes()
        //    .Where(t => IsStickerWithAnyTags(t, ItemTag.BlankBuild, ItemTag.BlankGenerator))
        //    .ToList();

        public static readonly List<Type> BlueStamps = Assembly.GetAssembly(typeof(Item)).GetTypes()
            .Where(t => IsStampWithAnyTags(t, ItemTag.BlueBuild, ItemTag.BlueGenerator))
            .ToList();

        public static readonly List<Type> BlueStickers = Assembly.GetAssembly(typeof(Item)).GetTypes()
            .Where(t => IsStickerWithAnyTags(t, ItemTag.BlueBuild, ItemTag.BlueGenerator))
            .ToList();

        //public static readonly List<Type> ChessStamps = Assembly.GetAssembly(typeof(Item)).GetTypes()
        //    .Where(t => IsStampWithAnyTags(t, ItemTag.ChessBuild, ItemTag.ChessGenerator))
        //    .ToList();

        //public static readonly List<Type> ChessStickers = Assembly.GetAssembly(typeof(Item)).GetTypes()
        //    .Where(t => IsStickerWithAnyTags(t, ItemTag.ChessBuild, ItemTag.ChessGenerator))
        //    .ToList();

        //public static readonly List<Type> CurrencyStamps = Assembly.GetAssembly(typeof(Item)).GetTypes()
        //    .Where(t => IsStampWithAnyTags(t, ItemTag.CurrencyGenerator))
        //    .ToList();

        //public static readonly List<Type> CurrencyStickers = Assembly.GetAssembly(typeof(Item)).GetTypes()
        //    .Where(t => IsStickerWithAnyTags(t, ItemTag.CurrencyGenerator))
        //    .ToList();

        //public static readonly List<Type> CursedStamps = Assembly.GetAssembly(typeof(Item)).GetTypes()
        //    .Where(t => IsStampWithAnyTags(t, ItemTag.CurseBuild, ItemTag.CurseGenerator))
        //    .ToList();

        //public static readonly List<Type> CursedStickers = Assembly.GetAssembly(typeof(Item)).GetTypes()
        //    .Where(t => IsStickerWithAnyTags(t, ItemTag.CurseBuild, ItemTag.CurseGenerator))
        //    .ToList();

        public static readonly List<Type> NumberStamps = Assembly.GetAssembly(typeof(Item)).GetTypes()
            .Where(t => IsStampWithAnyTags(t, ItemTag.NumbersBuild, ItemTag.NumbersGenerator))
            .ToList();

        public static readonly List<Type> NumberStickers = Assembly.GetAssembly(typeof(Item)).GetTypes()
            .Where(t => IsStickerWithAnyTags(t, ItemTag.NumbersBuild, ItemTag.NumbersGenerator))
            .ToList();

        public static readonly List<Type> RedStamps = Assembly.GetAssembly(typeof(Item)).GetTypes()
            .Where(t => IsStampWithAnyTags(t, ItemTag.RedBuild, ItemTag.RedGenerator))
            .ToList();

        public static readonly List<Type> RedStickers = Assembly.GetAssembly(typeof(Item)).GetTypes()
            .Where(t => IsStickerWithAnyTags(t, ItemTag.RedBuild, ItemTag.RedGenerator))
            .ToList();

        public static readonly List<Type> ShinyStamps = Assembly.GetAssembly(typeof(Item)).GetTypes()
            .Where(t => IsStampWithAnyTags(t, ItemTag.ShinyBuild, ItemTag.ShinyGenerator))
            .ToList();

        public static readonly List<Type> ShinyStickers = Assembly.GetAssembly(typeof(Item)).GetTypes()
            .Where(t => IsStickerWithAnyTags(t, ItemTag.ShinyBuild, ItemTag.ShinyGenerator))
            .ToList();

        public static readonly List<Type> VoidStamps = Assembly.GetAssembly(typeof(Item)).GetTypes()
            .Where(t => IsStampWithAnyTags(t, ItemTag.VoidBuild, ItemTag.VoidGenerator))
            .ToList();

        public static readonly List<Type> VoidStickers = Assembly.GetAssembly(typeof(Item)).GetTypes()
            .Where(t => IsStickerWithAnyTags(t, ItemTag.VoidBuild, ItemTag.VoidGenerator))
            .ToList();

        private static bool IsItemClass(Type t)
        {
            return t.IsClass && t.IsSubclassOf(typeof(Item));
        }

        private static bool IsStampWithAnyTags(Type type, params ItemTag[] itemTags)
        {
            if (!IsItemClass(type)) return false;

            Item item = Activator.CreateInstance(type) as Item;
            return item.IsStamp() && itemTags.Intersect(item.Tags).Any();
        }

        private static bool IsStickerWithAnyTags(Type type, params ItemTag[] itemTags)
        {
            if (!IsItemClass(type)) return false;

            Item item = Activator.CreateInstance(type) as Item;
            return item.IsSticker() && itemTags.Intersect(item.Tags).Any();
        }
    }
}
