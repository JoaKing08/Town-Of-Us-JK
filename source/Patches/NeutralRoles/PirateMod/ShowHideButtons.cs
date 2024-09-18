using HarmonyLib;
using TownOfUs.Roles;
using Reactor.Utilities.Extensions;

namespace TownOfUs.NeutralRoles.PirateMod
{
    public class ShowHideButtonsPirate
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class Confirm
        {
            public static bool Prefix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Pirate) && !PlayerControl.LocalPlayer.IsDueled()) return true;
                var role = Role.GetRole(PlayerControl.LocalPlayer);
                role.DefenseButton.Destroy();
                return true;
            }
        }
        public static void HideButtons()
        {
            var role = Role.GetRole(PlayerControl.LocalPlayer);
            if (role.DefenseButton != null) role.DefenseButton.Destroy();
        }
    }
}