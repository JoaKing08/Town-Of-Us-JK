using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Modifiers;
using TownOfUs.CrewmateRoles.MedicMod;
using UnityEngine;

namespace TownOfUs.Modifiers.AgentMod
{
    public class MeetingEnd
    {
        public static void Postfix()
        {
            var AnnounceImp = false;
            var AnnounceApoc = false;
            foreach (ImpostorAgent agent in Objective.AllObjectives.Where(x => x.ObjectiveType == ObjectiveEnum.ImpostorAgent && ((ImpostorAgent)x).AgentHunt && x.Player != null))
            {
                if (agent.AgentHunt)
                {
                    if (agent.Player.Data.IsDead || agent.Player.Data.Disconnected)
                    {
                        agent.AgentHunt = false;
                        AnnounceImp = true;
                    }
                    else
                    {
                        agent.RoundsLeft--;
                        if (agent.RoundsLeft <= 0 || PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 1 || (!PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.NeutralKilling) || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.Impostors) || x.Is(RoleEnum.Sheriff) || (x.Is(RoleEnum.Hunter) && (Role.GetRole<Hunter>(x).UsesLeft > 0 || Role.GetRole<Hunter>(x).CaughtPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected) || Role.GetRole<Hunter>(x).StalkedPlayer != null)) || (x.Is(RoleEnum.Veteran) && Role.GetRole<Veteran>(x).UsesLeft > 0) || (x.Is(RoleEnum.Inquisitor) && Role.GetRole<Inquisitor>(x).CanVanquish))) && !PlayerControl.AllPlayerControls.ToArray().Any(x => x.RemainingEmergencies > 0 && !x.Data.IsDead && !x.Data.Disconnected))) Role.ImpostorAgentHuntOver = true;
                    }
                }
            }
            foreach (ApocalypseAgent agent in Objective.AllObjectives.Where(x => x.ObjectiveType == ObjectiveEnum.ApocalypseAgent && ((ApocalypseAgent)x).AgentHunt && x.Player != null))
            {
                if (agent.AgentHunt)
                {
                    if (agent.Player.Data.IsDead || agent.Player.Data.Disconnected)
                    {
                        agent.AgentHunt = false;
                        AnnounceApoc = true;
                    }
                    else
                    {
                        agent.RoundsLeft--;
                        if (agent.RoundsLeft <= 0 || PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= 1 || (!PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.NeutralKilling) || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.Impostors) || x.Is(RoleEnum.Sheriff) || (x.Is(RoleEnum.Hunter) && (Role.GetRole<Hunter>(x).UsesLeft > 0 || Role.GetRole<Hunter>(x).CaughtPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected) || Role.GetRole<Hunter>(x).StalkedPlayer != null)) || (x.Is(RoleEnum.Veteran) && Role.GetRole<Veteran>(x).UsesLeft > 0) || (x.Is(RoleEnum.Inquisitor) && Role.GetRole<Inquisitor>(x).CanVanquish))) && !PlayerControl.AllPlayerControls.ToArray().Any(x => x.RemainingEmergencies > 0 && !x.Data.IsDead && !x.Data.Disconnected))) Role.ApocalypseAgentHuntOver = true;
                    }
                }
            }
            if (AmongUsClient.Instance.AmHost)
            {
                if (Role.ImpostorAgentHuntOver && Role.ApocalypseAgentHuntOver)
                {
                    Utils.Rpc(CustomRPC.DoubleWin);
                    Role.DoubleWin();
                    Utils.EndGame();
                }
                else if (Role.ImpostorAgentHuntOver)
                {
                    Utils.EndGame();
                }
                else if (Role.ApocalypseAgentHuntOver)
                {
                    Utils.Rpc(CustomRPC.ApocalypseWin);
                    Role.ApocWin();
                    Utils.EndGame();
                }
            }
            if (AnnounceImp && AnnounceApoc)
                NotificationPatch.DelayNotification(500, Patches.TranslationPatches.CurrentLanguage == 0 ? "Both Agent Hunts Had Ended!" : "Obydwa polowania na Agentów sie zakonczyly!", CustomGameOptions.NotificationDuration * 1000, Color.Lerp(Patches.Colors.ImpostorAgent, Patches.Colors.ApocalypseAgent, 0.5f));
            else if (AnnounceImp)
                NotificationPatch.DelayNotification(500, Patches.TranslationPatches.CurrentLanguage == 0 ? "Impostor Agent Hunt Has Ended!" : "Polowanie na Agenta Impostorów sie zakonczylo!", CustomGameOptions.NotificationDuration * 1000, Patches.Colors.ImpostorAgent);
            else if (AnnounceApoc)
                NotificationPatch.DelayNotification(500, Patches.TranslationPatches.CurrentLanguage == 0 ? "Apocalypse Agent Hunt Has Ended!" : "Polowanie na Agenta Apocalypse sie zakonczylo!", CustomGameOptions.NotificationDuration * 1000, Patches.Colors.ApocalypseAgent);
            MeetingStart.CheckMeeting(false);
        }
        public static void CheckAgentWinCon()
        {
            foreach (ImpostorAgent agent in Objective.AllObjectives.Where(x => x.ObjectiveType == ObjectiveEnum.ImpostorAgent && ((ImpostorAgent)x).AgentHunt && x.Player != null))
            {
                if (agent.AgentHunt)
                {
                    if (!agent.Player.Data.IsDead && !agent.Player.Data.Disconnected)
                    {
                        if (agent.RoundsLeft <= 0 || (!PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.NeutralKilling) || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.Impostors) || x.Is(RoleEnum.Sheriff) || (x.Is(RoleEnum.Hunter) && (Role.GetRole<Hunter>(x).UsesLeft > 0 || Role.GetRole<Hunter>(x).CaughtPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected) || Role.GetRole<Hunter>(x).StalkedPlayer != null)) || (x.Is(RoleEnum.Veteran) && Role.GetRole<Veteran>(x).UsesLeft > 0) || (x.Is(RoleEnum.Crusader) && Role.GetRole<Crusader>(x).UsesLeft > 0) || (x.Is(RoleEnum.Inquisitor) && Role.GetRole<Inquisitor>(x).CanVanquish))) && !PlayerControl.AllPlayerControls.ToArray().Any(x => x.RemainingEmergencies > 0 && !x.Data.IsDead && !x.Data.Disconnected) && !GameObject.FindObjectsOfType<DeadBody>().Any())) Role.ImpostorAgentHuntOver = true;
                    }
                }
            }
            foreach (ApocalypseAgent agent in Objective.AllObjectives.Where(x => x.ObjectiveType == ObjectiveEnum.ApocalypseAgent && ((ApocalypseAgent)x).AgentHunt && x.Player != null))
            {
                if (agent.AgentHunt)
                {
                    if (!agent.Player.Data.IsDead && !agent.Player.Data.Disconnected)
                    {
                        if (agent.RoundsLeft <= 0 || (!PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.NeutralKilling) || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.Impostors) || x.Is(RoleEnum.Sheriff) || (x.Is(RoleEnum.Hunter) && (Role.GetRole<Hunter>(x).UsesLeft > 0 || Role.GetRole<Hunter>(x).CaughtPlayers.Any(x => !x.Data.IsDead && !x.Data.Disconnected) || Role.GetRole<Hunter>(x).StalkedPlayer != null)) || (x.Is(RoleEnum.Veteran) && Role.GetRole<Veteran>(x).UsesLeft > 0) || (x.Is(RoleEnum.Crusader) && Role.GetRole<Crusader>(x).UsesLeft > 0) || (x.Is(RoleEnum.Inquisitor) && Role.GetRole<Inquisitor>(x).CanVanquish))) && !PlayerControl.AllPlayerControls.ToArray().Any(x => x.RemainingEmergencies > 0 && !x.Data.IsDead && !x.Data.Disconnected) && !GameObject.FindObjectsOfType<DeadBody>().Any())) Role.ApocalypseAgentHuntOver = true;
                    }
                }
            }
            if (AmongUsClient.Instance.AmHost)
            {
                if (Role.ImpostorAgentHuntOver && Role.ApocalypseAgentHuntOver)
                {
                    Utils.Rpc(CustomRPC.DoubleWin);
                    Role.DoubleWin();
                    Utils.EndGame();
                }
                else if (Role.ImpostorAgentHuntOver)
                {
                    Utils.EndGame();
                }
                else if (Role.ApocalypseAgentHuntOver)
                {
                    Utils.Rpc(CustomRPC.ApocalypseWin);
                    Role.ApocWin();
                    Utils.EndGame();
                }
            }
        }
    }
}
