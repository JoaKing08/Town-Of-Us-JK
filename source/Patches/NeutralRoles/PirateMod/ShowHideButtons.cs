using HarmonyLib;
using TownOfUs.Roles;
using Reactor.Utilities.Extensions;

namespace TownOfUs.NeutralRoles.PirateMod
{
    public class ShowHideButtonsMayor
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
    }
}