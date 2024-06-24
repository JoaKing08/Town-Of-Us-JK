using HarmonyLib;
using TownOfUs.Roles;
using Reactor.Utilities.Extensions;

namespace TownOfUs.ImpostorRoles.DemagogueMod
{
    public class ShowHideButtonsDemagogue
    {
        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public static class Confirm
        {
            public static bool Prefix(MeetingHud __instance = null)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue)) return true;
                var demagogue = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
                foreach (var button in demagogue.MeetingKillButtons)
                {
                    button.Value.button.Destroy();
                    button.Value.text.Destroy();
                }
                demagogue.MeetingKillButtons.Clear();
                return true;
            }
        }
    }
}