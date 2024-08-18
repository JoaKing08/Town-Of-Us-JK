using HarmonyLib;
using Object = UnityEngine.Object;
using TownOfUs.Modifiers.AgentMod;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(Object), nameof(Object.Destroy), typeof(Object))]
    public static class HUDClose
    {
        public static void Postfix(Object obj)
        {
            if (ExileController.Instance == null || obj != ExileController.Instance.gameObject) return;
            Utils.ResetCustomTimers();
            ApocalypseRoles.DeathMod.MeetingEnd.Postfix();
            ApocalypseRoles.FamineMod.MeetingEnd.Postfix();
            MeetingEnd.Postfix();
        }
    }
}