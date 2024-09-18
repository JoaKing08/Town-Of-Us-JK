using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities;
using TownOfUs.Extensions;
using UnityEngine;
using Reactor.Utilities.Extensions;
using TownOfUs.Patches;

namespace TownOfUs.Roles.Horseman
{
    public class Plaguebearer : Role
    {
        public PlayerControl ClosestPlayer;
        public List<byte> InfectedPlayers = new List<byte>();
        public DateTime LastInfected;

        public int InfectedAlive => InfectedPlayers.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null && !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected && !Utils.PlayerById(x).Is(Faction.NeutralApocalypse) && !Utils.PlayerById(x).Is(ObjectiveEnum.ApocalypseAgent));
        public bool CanTransform => PlayerControl.AllPlayerControls.ToArray().Count(x => x != null && !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.NeutralApocalypse) && !x.Is(ObjectiveEnum.ApocalypseAgent)) <= InfectedAlive + (Role.GetRoles(RoleEnum.Harbinger).Any(x => ((Harbinger)x).CompletedTasks && !((Harbinger)x).Caught && !x.Player.Data.Disconnected) ? CustomGameOptions.HarbingerPlaguebearerBonus : 0);

        public Plaguebearer(PlayerControl player) : base(player)
        {
            Name = "Plaguebearer";
            ImpostorText = () => "Infect Everyone To Become Pestilence";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Infect everyone to become Pestilence\nFake Tasks:" : "Zaraz wszystkich by stac sie Pestilence\nFake Tasks:";
            Color = Patches.Colors.Plaguebearer;
            RoleType = RoleEnum.Plaguebearer;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralApocalypse;
            InfectedPlayers.Add(player.PlayerId);
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var apoTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            apoTeam.Add(PlayerControl.LocalPlayer);
            foreach (var player in PlayerControl.AllPlayerControls) if ((player.Is(Faction.NeutralApocalypse) || player.Is(ObjectiveEnum.ApocalypseAgent) || (player.Is(RoleEnum.Undercover) && Role.GetRole<Undercover>(player).UndercoverApocalypse)) && player != PlayerControl.LocalPlayer) apoTeam.Add(player);
            __instance.teamToShow = apoTeam;
        }

        public float InfectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInfected;
            var num = CustomGameOptions.InfectCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void RpcSpreadInfection(PlayerControl source, PlayerControl target)
        {
            new WaitForSeconds(1f);
            SpreadInfection(source, target);
            Utils.Rpc(CustomRPC.Infect, Player.PlayerId, source.PlayerId, target.PlayerId);
        }

        public void SpreadInfection(PlayerControl source, PlayerControl target)
        {
            if (InfectedPlayers.Contains(source.PlayerId) && !InfectedPlayers.Contains(target.PlayerId)) InfectedPlayers.Add(target.PlayerId);
            else if (InfectedPlayers.Contains(target.PlayerId) && !InfectedPlayers.Contains(source.PlayerId)) InfectedPlayers.Add(source.PlayerId);
        }

        public void TurnPestilence()
        {
            var oldRole = GetRole(Player);
            var killsList = (oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
            RoleDictionary.Remove(Player.PlayerId);
            var role = new Pestilence(Player);
            role.CorrectAssassinKills = killsList.CorrectAssassinKills;
            role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            role.FactionOverride = oldRole.FactionOverride;
            if (CustomGameOptions.AnnouncePestilence)
            {
                NotificationPatch.DelayNotification(CustomGameOptions.AnnouncePestilenceDelay * 1000, TranslationPatches.CurrentLanguage == 0 ? $"<color=#{Patches.Colors.Pestilence.ToHtmlStringRGBA()}>PESTILENCE HAS TRANSFORMED!</color>" : $"<color=#{Patches.Colors.Pestilence.ToHtmlStringRGBA()}>PESTILENCE SIE PRZETRANSFORMOWAL!</color>", 1000 * CustomGameOptions.NotificationDuration, Patches.Colors.Pestilence);
            }
            else if (Player == PlayerControl.LocalPlayer)
            {
                NotificationPatch.DelayNotification(CustomGameOptions.AnnouncePestilenceDelay * 1000, TranslationPatches.CurrentLanguage == 0 ? $"<color=#{Patches.Colors.Pestilence.ToHtmlStringRGBA()}>PESTILENCE HAS TRANSFORMED!</color>" : $"<color=#{Patches.Colors.Pestilence.ToHtmlStringRGBA()}>PESTILENCE SIE PRZETRANSFORMOWAL!</color>", 1000 * CustomGameOptions.NotificationDuration, Patches.Colors.Pestilence);
            }
            if (Player == PlayerControl.LocalPlayer)
            {
                role.RegenTask();
            }
            role.Announced = false;
        }
    }
}