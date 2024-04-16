using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.PoltergeistMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.HandleAnimation))]
    public class HandleAnimation
    {
        public static void Prefix(PlayerPhysics __instance, [HarmonyArgument(0)] ref bool amDead)
        {
            if (__instance.myPlayer.Is(RoleEnum.Poltergeist)) amDead = Role.GetRole<Poltergeist>(__instance.myPlayer).Caught;
        }
    }
}