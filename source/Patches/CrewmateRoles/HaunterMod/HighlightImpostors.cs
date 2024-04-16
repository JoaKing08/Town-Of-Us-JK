using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.HaunterMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightImpostors
    {
        public static void UpdateMeeting(MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var role = Role.GetRole(player);
                    if (player.Is(Faction.Impostors))
                        state.NameText.color = Palette.ImpostorRed;
                    if ((player.Is(Faction.NeutralKilling) || (player.Is(Faction.NeutralApocalypse) && CustomGameOptions.GameMode != GameMode.Horseman)) && CustomGameOptions.HaunterRevealsNeutrals)
                        state.NameText.color = role.Color;
                    if (player.Is(Faction.NeutralApocalypse) && CustomGameOptions.GameMode == GameMode.Horseman)
                        state.NameText.color = Color.gray;
                }
            }
        }
        public static void Postfix(HudManager __instance)
        {
            foreach (var haunter in Role.GetRoles(RoleEnum.Haunter))
            {
                var role = (Haunter)haunter;
                if (!role.CompletedTasks || role.Caught) return;
                if (MeetingHud.Instance) UpdateMeeting(MeetingHud.Instance);
            }
        }
    }
}