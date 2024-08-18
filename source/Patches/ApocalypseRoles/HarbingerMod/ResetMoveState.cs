using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ApocalypseRoles.HarbingerMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ResetMoveState))]
    public class ResetMoveState
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (!__instance.myPlayer.Is(RoleEnum.Harbinger)) return;

            var role = Role.GetRole<Harbinger>(__instance.myPlayer);
            __instance.myPlayer.Collider.enabled = !role.Caught;
        }
    }
}