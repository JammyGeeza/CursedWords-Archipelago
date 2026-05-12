using HarmonyLib;

namespace Mod.Patches
{
    [HarmonyPatch(typeof(SaveFile))]
    internal class SaveFile_Patches : PatchBase
    {
        /// <summary>
        /// Prevent new save file data automatically including Rodman.
        /// </summary>
        /// <param name="__result"></param>
        [HarmonyPatch(nameof(SaveFile.SetNewSaveFileData))]
        [HarmonyPrefix]
        public static bool SetNewSaveFileData_Prefix()
        {
            Logger.LogInfo("SaveFile.SetNewSaveFileData Prefix!");
            return false;
        }
    }
}
