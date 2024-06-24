using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.DemagogueMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))] // BBFDNCCEJHI
    public static class VotingComplete
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue))
            {
                var demagogue = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
                foreach (var button in demagogue.MeetingKillButtons)
                {
                    button.Value.button.Destroy();
                    button.Value.text.Destroy();
                }
                demagogue.MeetingKillButtons.Clear();
            }
        }
    }
}