using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.DeputyMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))] // BBFDNCCEJHI
    public static class VotingComplete
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Deputy))
            {
                var deputy = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                if (!deputy.Revealed)
                {
                    foreach (var button in deputy.ShootButtons)
                    {
                        button.Destroy();
                    }
                    deputy.ShootButtons.Clear();
                }
            }
        }
    }
}