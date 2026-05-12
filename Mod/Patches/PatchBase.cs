using BepInEx.Logging;
using Modd;

namespace Mod.Patches
{
    public abstract class PatchBase
    {
        protected static ManualLogSource Logger => CursedWordsArchipelago.Instance.LogSource;
    }
}
