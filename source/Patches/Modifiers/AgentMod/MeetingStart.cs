using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.ImpostorRoles.TraitorMod;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.Modifiers.AgentMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            CheckMeeting(true);
        }
        public static void CheckMeeting(bool showChatInfo)
        {

            var AnnounceImp = false;
            var AnnounceApoc = false;
            if (!CustomGameOptions.AgentHunt) return;
            if (Objective.AllObjectives.Any(x => x.ObjectiveType == ObjectiveEnum.ImpostorAgent && !x.Player.Data.IsDead && !x.Player.Data.Disconnected && x.Player.Is(FactionOverride.None)) && !Role.AllRoles.Any(x => x.Faction == Faction.Impostors && x.Player != null && !x.Player.Data.IsDead && !x.Player.Data.Disconnected) && !(SetTraitor.WillBeTraitor != null && PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) >= CustomGameOptions.LatestSpawn))
            {
                var agent = (ImpostorAgent)Objective.AllObjectives.FirstOrDefault(x => x.ObjectiveType == ObjectiveEnum.ImpostorAgent);
                if (agent != null && agent.Player != null)
                {
                    if (!PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.NeutralKilling) || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.Impostors))))
                    {
                        if (!agent.AgentHunt)
                        {
                            agent.AgentHunt = true;
                            agent.RoundsLeft = CustomGameOptions.AgentHuntRounds;
                            AnnounceImp = true;
                        }
                        else if (agent.RoundsLeft > CustomGameOptions.AgentHuntRounds)
                        {
                            agent.RoundsLeft = CustomGameOptions.AgentHuntRounds;
                        }
                    }
                    else if (!agent.AgentHunt && CustomGameOptions.AgentHuntRoundsKiller != 0)
                    {
                        agent.AgentHunt = true;
                        agent.RoundsLeft = CustomGameOptions.AgentHuntRoundsKiller;
                        AnnounceImp = true;
                    }
                }
            }
            foreach (ImpostorAgent agent in Objective.AllObjectives.Where(x => x.ObjectiveType == ObjectiveEnum.ImpostorAgent && ((ImpostorAgent)x).AgentHunt && x.Player != null && !x.Player.Data.IsDead && !x.Player.Data.Disconnected))
            {
                if (DestroyableSingleton<HudManager>.Instance) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{agent.RoundsLeft}</b> more meetings remaining to find the <color=#{Patches.Colors.ImpostorAgent.ToHtmlStringRGBA()}><b>Impostor Agent</b></color>!" : $"Pozostalo <b>{agent.RoundsLeft}</b> spotkan by znalezc <color=#{Patches.Colors.ImpostorAgent.ToHtmlStringRGBA()}><b>Agenta Impostorów</b></color>!");
            }
            if (Objective.AllObjectives.Any(x => x.ObjectiveType == ObjectiveEnum.ApocalypseAgent && !x.Player.Data.IsDead && !x.Player.Data.Disconnected && x.Player.Is(FactionOverride.None)) && !Role.AllRoles.Any(x => x.Faction == Faction.NeutralApocalypse && x.Player != null && !x.Player.Data.IsDead && !x.Player.Data.Disconnected))
            {
                var agent = (ApocalypseAgent)Objective.AllObjectives.FirstOrDefault(x => x.ObjectiveType == ObjectiveEnum.ApocalypseAgent);
                if (agent != null && agent.Player != null)
                {
                    if (!PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(Faction.NeutralKilling) || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.Impostors))))
                    {
                        if (!agent.AgentHunt)
                        {
                            agent.AgentHunt = true;
                            agent.RoundsLeft = CustomGameOptions.AgentHuntRounds;
                            AnnounceApoc = true;
                        }
                        else if (agent.RoundsLeft > CustomGameOptions.AgentHuntRounds)
                        {
                            agent.RoundsLeft = CustomGameOptions.AgentHuntRounds;
                        }
                    }
                    else if (!agent.AgentHunt && CustomGameOptions.AgentHuntRoundsKiller != 0)
                    {
                        agent.AgentHunt = true;
                        agent.RoundsLeft = CustomGameOptions.AgentHuntRoundsKiller;
                        AnnounceApoc = true;
                    }
                }
            }
            foreach (ApocalypseAgent agent in Objective.AllObjectives.Where(x => x.ObjectiveType == ObjectiveEnum.ApocalypseAgent && ((ApocalypseAgent)x).AgentHunt && x.Player != null && !x.Player.Data.IsDead && !x.Player.Data.Disconnected))
            {
                if (DestroyableSingleton<HudManager>.Instance && showChatInfo) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{agent.RoundsLeft}</b> more meetings remaining to find the <color=#{Patches.Colors.ApocalypseAgent.ToHtmlStringRGBA()}><b>Apocalypse Agent</b></color>!" : $"Pozostalo <b>{agent.RoundsLeft}</b> spotkan by znalezc <color=#{Patches.Colors.ApocalypseAgent.ToHtmlStringRGBA()}><b>Agenta Apocalypse</b></color>!");
            }
            if (AnnounceImp && AnnounceApoc)
                NotificationPatch.DelayNotification(2500, Patches.TranslationPatches.CurrentLanguage == 0 ? $"<color=#{Color.Lerp(Patches.Colors.ImpostorAgent, Patches.Colors.ApocalypseAgent, 0.5f).ToHtmlStringRGBA()}>DOUBLE AGENT HUNT BEGINS!</color>" : $"<color=#{Color.Lerp(Patches.Colors.ImpostorAgent, Patches.Colors.ApocalypseAgent, 0.5f).ToHtmlStringRGBA()}>PODWÓJNE POLOWANIE NA AGENTÓW SIE ROZPOCZELO!</color>", CustomGameOptions.NotificationDuration * 1000, Color.Lerp(Patches.Colors.ImpostorAgent, Patches.Colors.ApocalypseAgent, 0.5f));
            else if (AnnounceImp)
                NotificationPatch.DelayNotification(2500, Patches.TranslationPatches.CurrentLanguage == 0 ? $"<color=#{Patches.Colors.ImpostorAgent.ToHtmlStringRGBA()}>IMPOSTOR AGENT HUNT BEGINS!</color>" : $"<color=#{Patches.Colors.ImpostorAgent.ToHtmlStringRGBA()}>POLOWANIE NA AGENTA IMPOSTORÓW SIE ROZPOCZELO!</color>", CustomGameOptions.NotificationDuration * 1000, Patches.Colors.ImpostorAgent);
            else if (AnnounceApoc)
                NotificationPatch.DelayNotification(2500, Patches.TranslationPatches.CurrentLanguage == 0 ? $"<color=#{Patches.Colors.ApocalypseAgent.ToHtmlStringRGBA()}>APOCALYPSE AGENT HUNT BEGINS!</color>" : $"<color=#{Patches.Colors.ApocalypseAgent.ToHtmlStringRGBA()}>POLOWANIE NA AGENTA APOCALIPSE SIE ROZPOCZELO!</color>", CustomGameOptions.NotificationDuration * 1000, Patches.Colors.ApocalypseAgent);
        }
    }
}
