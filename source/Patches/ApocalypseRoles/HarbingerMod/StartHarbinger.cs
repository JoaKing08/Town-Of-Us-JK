using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ApocalypseRoles.HarbingerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Visible), MethodType.Setter)]
    public static class VisibleOverride
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] ref bool value)
        {
            if (!__instance.Is(RoleEnum.Harbinger)) return;
            if (Role.GetRole<Harbinger>(__instance).Caught) return;
            value = !__instance.inVent;
        }
    }
}