using BepInEx.Logging;
using FullSerializer;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using UnityEngine;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(StringSerializer))]
    internal class StringSerializer_Patches
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        //[HarmonyPatch(nameof(StringSerializer.Serialize))]
        //[HarmonyPostfix]
        private static void Serialize_Postfix(ref Type type, ref string __result)
        {
            // Ignore if not a save file
            if (type != typeof(SaveFile))
                return;

            // Parse save data
            fsData saveData = fsJsonParser.Parse(__result);

            // Add archipelago properties if they don't already exist
            if (!saveData.AsDictionary.TryGetValue("Archipelago", out fsData archipelagoData))
            {
                Debug.Log("Apparently the 'Archipelago' object didn't exist...");

                //saveData.AsDictionary["Archipelago"] = new fsData(
                //new Dictionary<string, fsData>
                //{
                //    ["Host"] = new fsData(""),
                //    ["Slot"] = new fsData(""),
                //    ["Password"] = new fsData("")
                //});

                __result = fsJsonPrinter.CompressedJson(saveData);
            }
        }
    }
}
