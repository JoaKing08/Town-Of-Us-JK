using HarmonyLib;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.InspectorMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Inspector)) return;
            var inspectorRole = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);
            if (inspectorRole.LastInspectedPlayer != null)
            {
                var playerResults = (Patches.TranslationPatches.CurrentLanguage == 0 ? "You found out that " : "Dowiedziales sie ze ") + Utils.GetPossibleRoleCategory(inspectorRole.LastInspectedPlayer);
                var roleResults = Utils.GetPossibleRoleList(inspectorRole.LastInspectedPlayer);

                if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
                if (!string.IsNullOrWhiteSpace(roleResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, roleResults);
            }
        }
    }
}