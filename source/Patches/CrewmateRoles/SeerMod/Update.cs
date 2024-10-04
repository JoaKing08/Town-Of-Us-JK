using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SeerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class Update
    {
        private static void UpdateMeeting(MeetingHud __instance, Seer seer)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!seer.Investigated.Contains(player.PlayerId)) continue;
                foreach (var state in __instance.playerStates)
                {
                    if (player.PlayerId != state.TargetPlayerId) continue;
                    var roleType = Utils.GetRole(player);
                    switch (roleType)
                    {
                        default:
                            if (((player.Is(Faction.Crewmates) || player.Is((RoleEnum)254)) && player.Is(FactionOverride.None) && !(player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Deputy) || player.Is(ObjectiveEnum.ImpostorAgent) || player.Is(ObjectiveEnum.ApocalypseAgent))) ||
                            ((player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Deputy)) && !CustomGameOptions.CrewKillingRed) ||
                            (player.Is(Faction.NeutralBenign) && !CustomGameOptions.NeutBenignRed) ||
                            (player.Is(Faction.NeutralEvil) && !CustomGameOptions.NeutEvilRed) ||
                            (player.Is(Faction.NeutralChaos) && !CustomGameOptions.NeutChaosRed) ||
                            (player.Is(Faction.NeutralKilling) && !player.Is(RoleEnum.JKNecromancer) && !player.Is(RoleEnum.Vampire) && !player.Is(RoleEnum.Jackal) && !CustomGameOptions.NeutKillingRed) ||
                            ((player.Is(RoleEnum.JKNecromancer) || player.Is(RoleEnum.Vampire) || player.Is(RoleEnum.Jackal)) && !CustomGameOptions.NeutProselyteRed) ||
                            (player.Is(Faction.NeutralApocalypse) && !CustomGameOptions.NeutApocalypseRed) ||
                            ((player.Is(ObjectiveEnum.ImpostorAgent) || player.Is(ObjectiveEnum.ApocalypseAgent)) && !CustomGameOptions.AgentRed) ||
                            (player.Is(FactionOverride.Undead) && !CustomGameOptions.UndeadRed) ||
                            (player.Is(FactionOverride.Recruit) && !CustomGameOptions.RecruitRed))
                            {
                                state.NameText.color = Color.green;
                            }
                            else if (player.Is(RoleEnum.Traitor) && CustomGameOptions.TraitorColourSwap)
                            {
                                foreach (var role in Role.GetRoles(RoleEnum.Traitor))
                                {
                                    var traitor = (Traitor)role;
                                    if ((traitor.formerRole == RoleEnum.Sheriff || traitor.formerRole == RoleEnum.Vigilante ||
                                        traitor.formerRole == RoleEnum.Veteran || traitor.formerRole == RoleEnum.VampireHunter || traitor.formerRole == RoleEnum.Hunter)
                                    && CustomGameOptions.CrewKillingRed) state.NameText.color = Color.red;
                                    else state.NameText.color = Color.green;
                                }
                            }
                            else
                            {
                                state.NameText.color = Color.red;
                            }
                            break;
                    }
                }
            }
        }

        [HarmonyPriority(Priority.Last)]
        private static void Postfix(HudManager __instance)

        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Seer)) return;
            var seer = Role.GetRole<Seer>(PlayerControl.LocalPlayer);
            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance, seer);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!seer.Investigated.Contains(player.PlayerId)) continue;
                var roleType = Utils.GetRole(player);
                switch (roleType)
                {
                    default:
                        if (((player.Is(Faction.Crewmates) || player.Is((RoleEnum)254)) && player.Is(FactionOverride.None) && !(player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Deputy) || player.Is(ObjectiveEnum.ImpostorAgent) || player.Is(ObjectiveEnum.ApocalypseAgent))) ||
                        ((player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Deputy)) && !CustomGameOptions.CrewKillingRed) ||
                        (player.Is(Faction.NeutralBenign) && !CustomGameOptions.NeutBenignRed) ||
                        (player.Is(Faction.NeutralEvil) && !CustomGameOptions.NeutEvilRed) ||
                        (player.Is(Faction.NeutralChaos) && !CustomGameOptions.NeutChaosRed) ||
                        (player.Is(Faction.NeutralKilling) && !player.Is(RoleEnum.JKNecromancer) && !player.Is(RoleEnum.Vampire) && !player.Is(RoleEnum.Jackal) && !CustomGameOptions.NeutKillingRed) ||
                        ((player.Is(RoleEnum.JKNecromancer) || player.Is(RoleEnum.Vampire) || player.Is(RoleEnum.Jackal)) && !CustomGameOptions.NeutProselyteRed) ||
                        (player.Is(Faction.NeutralApocalypse) && !CustomGameOptions.NeutApocalypseRed) ||
                        ((player.Is(ObjectiveEnum.ImpostorAgent) || player.Is(ObjectiveEnum.ApocalypseAgent)) && !CustomGameOptions.AgentRed) ||
                        (player.Is(FactionOverride.Undead) && !CustomGameOptions.UndeadRed) ||
                        (player.Is(FactionOverride.Recruit) && !CustomGameOptions.RecruitRed))
                        {
                            player.nameText().color = Color.green;
                        }
                        else if (player.Is(RoleEnum.Traitor) && CustomGameOptions.TraitorColourSwap)
                        {
                            foreach (var role in Role.GetRoles(RoleEnum.Traitor))
                            {
                                var traitor = (Traitor)role;
                                if ((traitor.formerRole == RoleEnum.Sheriff || traitor.formerRole == RoleEnum.Vigilante ||
                                    traitor.formerRole == RoleEnum.Veteran || traitor.formerRole == RoleEnum.VampireHunter || traitor.formerRole == RoleEnum.Hunter)
                                    && CustomGameOptions.CrewKillingRed) player.nameText().color = Color.red;
                                else player.nameText().color = Color.green;
                            }
                        }
                        else
                        {
                            player.nameText().color = Color.red;
                        }
                        break;
                }
            }
        }
    }
}