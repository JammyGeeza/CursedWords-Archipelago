using HarmonyLib;
using Mod.Classes;
using Mod.Helpers;
using Mod.Mappings;
using nickeltin.SDF.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(ItemSpriteData))]
    internal class ItemSpriteData_Patches : PatchBase
    {
        [HarmonyPatch(nameof(ItemSpriteData.GetSDFSprite))]
        [HarmonyPrefix]
        static bool Prefix(ItemSpriteData __instance, ref SDFSpriteMetadataAsset __result)
        {
            if (__instance.AssetPath == "Archipelago")
            {
                __instance.ItemSprite = ArchipelagoShopitem.GetSprite();
                __result = default(SDFSpriteMetadataAsset);
                return false;
            }
            return true;
        }
    }
}
