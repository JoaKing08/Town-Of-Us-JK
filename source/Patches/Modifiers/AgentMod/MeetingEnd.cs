using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.Modifiers.AgentMod
{
    public class MeetingEnd
    {
        public static void Postfix()
        {
            foreach (ImpostorAgent agent in Objective.AllObjectives.Where(x => x.ObjectiveType == ObjectiveEnum.ImpostorAgent && ((ImpostorAgent)x).AgentHunt && x.Player != null))
            {
                if (agent.AgentHunt)
                {
                    if (agent.Player.Data.IsDead || agent.Player.Data.Disconnected)
                    {
                        agent.AgentHunt = false;
                        NotificationPatch.DelayNotification(500, Patches.TranslationPatches.CurrentLanguage == 0 ? "Impostor Agent Hunt Has Ended!" : "Polowanie na Agenta Impostorów sie zakonczylo!", CustomGameOptions.NotificationDuration * 1000, Patches.Colors.ImpostorAgent);
                    }
                    else
                    {
                        agent.RoundsLeft--;
                        if (agent.RoundsLeft <= 0) Role.ImpostorAgentHuntOver = true;
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
                        NotificationPatch.DelayNotification(1000, Patches.TranslationPatches.CurrentLanguage == 0 ? "Apocalypse Agent Hunt Has Ended!" : "Polowanie na Agenta Apocalypse sie zakonczylo!", CustomGameOptions.NotificationDuration * 1000, Patches.Colors.ApocalypseAgent);
                    }
                    else
                    {
                        agent.RoundsLeft--;
                        if (agent.RoundsLeft <= 0) Role.ApocalypseAgentHuntOver = true;
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
