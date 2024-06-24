using HarmonyLib;
using TownOfUs.Roles;
using Reactor.Utilities.Extensions;

namespace TownOfUs.CrewmateRoles.DeputyMod
{
    public class ShowHideButtonsDeputy
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class Confirm
        {
            public static bool Prefix(MeetingHud __instance = null)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Deputy)) return true;
                var deputy = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                if (!deputy.Revealed)
                {
                    foreach (var button in deputy.ShootButtons)
                    {
                        button.Value.Destroy();
                    }
                    deputy.ShootButtons.Clear();
                }
                return true;
            }
        }
    }
}