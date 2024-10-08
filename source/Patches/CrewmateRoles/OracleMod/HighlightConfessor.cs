using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.OracleMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HighlightConfessor
    {
        public static void UpdateMeeting(Oracle role, MeetingHud __instance)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    if (player == role.Confessor)
                    {
                        if (role.RevealedFaction == Faction.Crewmates) state.NameText.text = "<color=#00FFFFFF>(Crew) </color>" + state.NameText.text;
                        else if (role.RevealedFaction == Faction.Impostors || (role.RevealedFaction == Faction.NeutralApocalypse && CustomGameOptions.GameMode == GameMode.Horseman)) state.NameText.text = CustomGameOptions.GameMode == GameMode.Horseman ? "<color=#404040FF>(Apoc) </color>" : "<color=#FF0000FF>(Imp) </color>" + state.NameText.text;
                        else state.NameText.text = "<color=#808080FF>(Neut) </color>" + state.NameText.text;
                    }
                }
            }
        }
        public static void Postfix(HudManager __instance)
        {
            if (!MeetingHud.Instance || PlayerControl.LocalPlayer.Data.IsDead) return;
            foreach (var oracle in Role.GetRoles(RoleEnum.Oracle))
            {
                var role = Role.GetRole<Oracle>(oracle.Player);
                if (role == null || role.Player == null || !role.Player.Data.IsDead || role.Confessor == null) return;
                UpdateMeeting(role, MeetingHud.Instance);
            }
        }
    }
}