using HarmonyLib;

namespace TownOfUs.RainbowMod
{
    [HarmonyPatch(typeof(AmongUs.Data.Player.PlayerData), nameof(AmongUs.Data.Player.PlayerData.FileName), MethodType.Getter)]
    public class SaveManagerPatch
    {
        public static void Postfix(ref string __result)
        {
            __result += "_TOUJK";
        }
    }
    [HarmonyPatch(typeof(AmongUs.Data.Legacy.LegacySaveManager), nameof(AmongUs.Data.Legacy.LegacySaveManager.GetPrefsName))]
    public class LegacySaveManagerPatch
    {
        public static void Postfix(ref string __result)
        {
            __result += "_TOUJK";
        }
    }
}
