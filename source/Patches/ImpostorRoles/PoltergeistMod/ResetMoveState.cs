using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.PoltergeistMod
{
    [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.ResetMoveState))]
    public class ResetMoveState
    {
        public static void Postfix(PlayerPhysics __instance)
        {
            if (!__instance.myPlayer.Is(RoleEnum.Poltergeist)) return;

            var role = Role.GetRole<Poltergeist>(__instance.myPlayer);
            __instance.myPlayer.Collider.enabled = !role.Caught;
        }
    }
}