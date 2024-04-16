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
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Pirate)) return true;
                var pirate = Role.GetRole<Pirate>(PlayerControl.LocalPlayer);
                pirate.DefenseButton.Destroy();
                Role.GetRole(pirate.DueledPlayer).DefenseButton.Destroy();
                return true;
            }
        }
    }
}