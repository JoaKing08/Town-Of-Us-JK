using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Modifiers;
using TownOfUs.Extensions;
using System.Collections.Generic;

namespace TownOfUs.CrewmateRoles.ImitatorMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Imitator)) return;
            var imitatorRole = Role.GetRole<Imitator>(PlayerControl.LocalPlayer);
            if (imitatorRole.trappedPlayers != null)
            {
                if (imitatorRole.trappedPlayers.Count == 0)
                {
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "<b>No</b> players entered any of your traps");
                }
                else if (imitatorRole.trappedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInTrap)
                {
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "<b>Not enough</b> players triggered your traps");
                }
                else
                {
                    string message = "Roles caught in your trap:\n";
                    foreach (RoleEnum role in imitatorRole.trappedPlayers.OrderBy(x => Guid.NewGuid()))
                    {
                        message += $" <b><color=#{role.GetRoleColor().ToHtmlStringRGBA()}>{role.GetRoleName()}</color></b>,";
                    }
                    message.Remove(message.Length - 1, 1);
                    if (DestroyableSingleton<HudManager>.Instance)
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
                }
                imitatorRole.trappedPlayers.Clear();
            }
            else if (imitatorRole.confessingPlayer != null)
            {
                var playerResults = MeetingStartOracle.PlayerReportFeedback(imitatorRole.confessingPlayer);

                if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
            }
            else if (imitatorRole.LastInspectedPlayer != null)
            {
                var playerResults = "You found out that " + Utils.GetPossibleRoleCategory(imitatorRole.LastInspectedPlayer);
                var roleResults = Utils.GetPossibleRoleList(imitatorRole.LastInspectedPlayer);

                if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
                if (!string.IsNullOrWhiteSpace(roleResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, roleResults);
            }
            else if (imitatorRole.Messages != null)
            {
                if (imitatorRole.Messages.Count == 0) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "<b>No</b> players triggered any of your bugs");
                else
                {
                    foreach (var message in imitatorRole.Messages)
                    {
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
                    }
                }
                imitatorRole.Messages = null;
            }
            else if (imitatorRole.SageFirst != byte.MaxValue && !Utils.PlayerById(imitatorRole.SageFirst).Data.IsDead && !Utils.PlayerById(imitatorRole.SageFirst).Data.Disconnected && imitatorRole.SageSecond != byte.MaxValue && !Utils.PlayerById(imitatorRole.SageSecond).Data.IsDead && !Utils.PlayerById(imitatorRole.SageSecond).Data.Disconnected)
            {
                var result = false;
                var firstPlayer = Utils.PlayerById(imitatorRole.SageFirst);
                var secondPlayer = Utils.PlayerById(imitatorRole.SageSecond);
                if ((firstPlayer.Is(RoleEnum.Witch) && !(secondPlayer.Is(Faction.Crewmates) && secondPlayer.Is(FactionOverride.None))) || (secondPlayer.Is(RoleEnum.Witch) && !(firstPlayer.Is(Faction.Crewmates) && firstPlayer.Is(FactionOverride.None))))
                {
                    result = true;
                }
                if (secondPlayer.Is(Utils.GetRole(firstPlayer)) && firstPlayer.Is(FactionOverride.None) && secondPlayer.Is(FactionOverride.None))
                {
                    result = true;
                }
                if ((firstPlayer.Is(RoleEnum.Survivor) && firstPlayer.Is(FactionOverride.None) && !(secondPlayer.Is(Faction.NeutralEvil) && !secondPlayer.Is(RoleEnum.Witch)) && !secondPlayer.Is(Faction.NeutralChaos)) || (secondPlayer.Is(RoleEnum.Survivor) && secondPlayer.Is(FactionOverride.None) && !(firstPlayer.Is(Faction.NeutralEvil) && !firstPlayer.Is(RoleEnum.Witch)) && !firstPlayer.Is(Faction.NeutralChaos)))
                {
                    result = true;
                }
                if (secondPlayer.Is(Role.GetRole(firstPlayer).Faction) && firstPlayer.Is(FactionOverride.None) && secondPlayer.Is(FactionOverride.None))
                {
                    result = true;
                }
                if (secondPlayer.Is(Role.GetRole(firstPlayer).FactionOverride) && !firstPlayer.Is(FactionOverride.None))
                {
                    result = true;
                }
                if ((secondPlayer.Data.IsImpostor() && secondPlayer.Is(FactionOverride.None) && firstPlayer.Is(ObjectiveEnum.ImpostorAgent) && firstPlayer.Is(FactionOverride.None)) || (firstPlayer.Data.IsImpostor() && firstPlayer.Is(FactionOverride.None) && secondPlayer.Is(ObjectiveEnum.ImpostorAgent) && secondPlayer.Is(FactionOverride.None)))
                {
                    result = true;
                }
                if ((secondPlayer.Is(Faction.NeutralApocalypse) && secondPlayer.Is(FactionOverride.None) && firstPlayer.Is(ObjectiveEnum.ApocalypseAgent) && firstPlayer.Is(FactionOverride.None)) || (firstPlayer.Is(Faction.NeutralApocalypse) && firstPlayer.Is(FactionOverride.None) && secondPlayer.Is(ObjectiveEnum.ApocalypseAgent) && secondPlayer.Is(FactionOverride.None)))
                {
                    result = true;
                }
                if (Objective.GetObjective(firstPlayer) != null && secondPlayer.Is(Objective.GetObjective(firstPlayer).ObjectiveType) && firstPlayer.Is(FactionOverride.None) && secondPlayer.Is(FactionOverride.None))
                {
                    result = true;
                }
                if (firstPlayer.Is(RoleEnum.GuardianAngel) && firstPlayer.Is(FactionOverride.None))
                {
                    var target = Role.GetRole<GuardianAngel>(firstPlayer).target;
                    if (target.PlayerId == secondPlayer.PlayerId)
                    {
                        result = true;
                    }
                    if ((target.Is(RoleEnum.Witch) && !(secondPlayer.Is(Faction.Crewmates) && secondPlayer.Is(FactionOverride.None))) || (secondPlayer.Is(RoleEnum.Witch) && !(target.Is(Faction.Crewmates) && target.Is(FactionOverride.None))))
                    {
                        result = true;
                    }
                    if (secondPlayer.Is(Utils.GetRole(target)) && target.Is(FactionOverride.None) && secondPlayer.Is(FactionOverride.None))
                    {
                        result = true;
                    }
                    if ((target.Is(RoleEnum.Survivor) && target.Is(FactionOverride.None) && !(secondPlayer.Is(Faction.NeutralEvil) && !secondPlayer.Is(RoleEnum.Witch)) && !secondPlayer.Is(Faction.NeutralChaos)) || (secondPlayer.Is(RoleEnum.Survivor) && secondPlayer.Is(FactionOverride.None) && !(target.Is(Faction.NeutralEvil) && !target.Is(RoleEnum.Witch)) && !target.Is(Faction.NeutralChaos)))
                    {
                        result = true;
                    }
                    if (secondPlayer.Is(Role.GetRole(target).Faction) && target.Is(FactionOverride.None) && secondPlayer.Is(FactionOverride.None))
                    {
                        result = true;
                    }
                    if (secondPlayer.Is(Role.GetRole(target).FactionOverride))
                    {
                        result = true;
                    }
                    if ((secondPlayer.Data.IsImpostor() && secondPlayer.Is(FactionOverride.None) && target.Is(ObjectiveEnum.ImpostorAgent) && target.Is(FactionOverride.None)) || (target.Data.IsImpostor() && target.Is(FactionOverride.None) && secondPlayer.Is(ObjectiveEnum.ImpostorAgent) && secondPlayer.Is(FactionOverride.None)))
                    {
                        result = true;
                    }
                    if ((secondPlayer.Is(Faction.NeutralApocalypse) && secondPlayer.Is(FactionOverride.None) && target.Is(ObjectiveEnum.ApocalypseAgent) && target.Is(FactionOverride.None)) || (target.Is(Faction.NeutralApocalypse) && target.Is(FactionOverride.None) && secondPlayer.Is(ObjectiveEnum.ApocalypseAgent) && secondPlayer.Is(FactionOverride.None)))
                    {
                        result = true;
                    }
                    if (Objective.GetObjective(target) != null && secondPlayer.Is(Objective.GetObjective(target).ObjectiveType) && target.Is(FactionOverride.None) && secondPlayer.Is(FactionOverride.None))
                    {
                        result = true;
                    }
                }
                if (secondPlayer.Is(RoleEnum.GuardianAngel) && secondPlayer.Is(FactionOverride.None))
                {
                    var target = Role.GetRole<GuardianAngel>(secondPlayer).target;
                    if (target.PlayerId == secondPlayer.PlayerId)
                    {
                        result = true;
                    }
                    if ((firstPlayer.Is(RoleEnum.Witch) && !(target.Is(Faction.Crewmates) && target.Is(FactionOverride.None))) || (target.Is(RoleEnum.Witch) && !(firstPlayer.Is(Faction.Crewmates) && firstPlayer.Is(FactionOverride.None))))
                    {
                        result = true;
                    }
                    if (target.Is(Utils.GetRole(firstPlayer)) && firstPlayer.Is(FactionOverride.None) && target.Is(FactionOverride.None))
                    {
                        result = true;
                    }
                    if ((firstPlayer.Is(RoleEnum.Survivor) && firstPlayer.Is(FactionOverride.None) && !(target.Is(Faction.NeutralEvil) && !target.Is(RoleEnum.Witch)) && !target.Is(Faction.NeutralChaos)) || (target.Is(RoleEnum.Survivor) && target.Is(FactionOverride.None) && !(firstPlayer.Is(Faction.NeutralEvil) && !firstPlayer.Is(RoleEnum.Witch)) && !firstPlayer.Is(Faction.NeutralChaos)))
                    {
                        result = true;
                    }
                    if (target.Is(Role.GetRole(firstPlayer).Faction) && firstPlayer.Is(FactionOverride.None) && target.Is(FactionOverride.None))
                    {
                        result = true;
                    }
                    if (target.Is(Role.GetRole(firstPlayer).FactionOverride))
                    {
                        result = true;
                    }
                    if ((target.Data.IsImpostor() && target.Is(FactionOverride.None) && firstPlayer.Is(ObjectiveEnum.ImpostorAgent) && firstPlayer.Is(FactionOverride.None)) || (firstPlayer.Data.IsImpostor() && firstPlayer.Is(FactionOverride.None) && target.Is(ObjectiveEnum.ImpostorAgent) && target.Is(FactionOverride.None)))
                    {
                        result = true;
                    }
                    if ((target.Is(Faction.NeutralApocalypse) && target.Is(FactionOverride.None) && firstPlayer.Is(ObjectiveEnum.ApocalypseAgent) && firstPlayer.Is(FactionOverride.None)) || (firstPlayer.Is(Faction.NeutralApocalypse) && firstPlayer.Is(FactionOverride.None) && target.Is(ObjectiveEnum.ApocalypseAgent) && target.Is(FactionOverride.None)))
                    {
                        result = true;
                    }
                    if (Objective.GetObjective(firstPlayer) != null && target.Is(Objective.GetObjective(firstPlayer).ObjectiveType) && firstPlayer.Is(FactionOverride.None) && target.Is(FactionOverride.None))
                    {
                        result = true;
                    }
                }
                if (UnityEngine.Random.RandomRangeInt(0, 100) >= CustomGameOptions.CompareAccuracy)
                {
                    result = !result;
                }
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, result ? $"You think that <b>{firstPlayer.GetDefaultOutfit().PlayerName}</b> and <b>{secondPlayer.GetDefaultOutfit().PlayerName}</b> seem like they are <b><color=#00FF00FF>friends</color></b>." : $"You think that <b>{firstPlayer.GetDefaultOutfit().PlayerName}</b> and <b>{secondPlayer.GetDefaultOutfit().PlayerName}</b> seem like they are <b><color=#FF0000FF>enemies</color></b>.");
            }
            else if (imitatorRole.VisionPlayer != byte.MaxValue)
            {
                if (!imitatorRole.PlayersInteracted.Any() && !imitatorRole.InteractingPlayers.Any()) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "Your vision was about darkness... <b>No one</b> interacted nor was interacted by <b>" + Utils.PlayerById(imitatorRole.VisionPlayer).GetDefaultOutfit().PlayerName + "</b>.");
                else
                {
                    var message = "";
                    if (!imitatorRole.PlayersInteracted.Any())
                    {
                        message += "Your vision was about few ants in diffrent colors picking sugar cube... <b>" + Utils.PlayerById(imitatorRole.VisionPlayer).GetDefaultOutfit().PlayerName + "</b> <b>wasn't interacting</b> and <b>was interacted by</b>: \n";
                        foreach (var player in imitatorRole.InteractingPlayers.OrderBy(x => Guid.NewGuid())) message += $"<b>{Utils.PlayerById(player).Data.ColorName}</b>, ";
                        message = message.Remove(message.Length - 2);
                    }
                    else if (!imitatorRole.InteractingPlayers.Any())
                    {
                        message += "Your vision was about flytrap catching few colored butterflies... <b>" + Utils.PlayerById(imitatorRole.VisionPlayer).GetDefaultOutfit().PlayerName + "</b> <b>wasn't interacted</b> and <b>interacted with</b>: \n";
                        foreach (var player in imitatorRole.PlayersInteracted.OrderBy(x => Guid.NewGuid())) message += $"<b>{Utils.PlayerById(player).Data.ColorName}</b>, ";
                        message = message.Remove(message.Length - 2);
                    }
                    else
                    {
                        message += "Your vision was about few colored rats fighting with cat... <b>" + Utils.PlayerById(imitatorRole.VisionPlayer).GetDefaultOutfit().PlayerName + "</b> <b>was interacting with</b> or <b>was interacted by</b>: \n";
                        var players = new List<byte>();
                        players.AddRange(imitatorRole.PlayersInteracted);
                        players.AddRange(imitatorRole.InteractingPlayers);
                        foreach (var player in players.OrderBy(x => Guid.NewGuid())) message += $"<b>{Utils.PlayerById(player).Data.ColorName}</b>, ";
                        message = message.Remove(message.Length - 2);
                    }
                }
            }
        }
    }
}
