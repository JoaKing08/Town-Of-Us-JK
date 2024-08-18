using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using Reactor.Utilities;
using TownOfUs.Extensions;
using UnityEngine;
using TMPro;
using Reactor.Utilities.Extensions;
using TownOfUs.Patches;

namespace TownOfUs.Roles.Horseman
{
    public class Baker : Role
    {
        public PlayerControl ClosestPlayer;
        public List<byte> BreadPlayers = new List<byte>();
        public DateTime LastBreaded;
        public TextMeshPro UsesText;

        public int BreadAlive => BreadPlayers.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null && !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected);
        public bool CanTransform => CustomGameOptions.BreadNeeded <= BreadAlive + (Role.GetRoles(RoleEnum.Harbinger).Any(x => ((Harbinger)x).CompletedTasks && !((Harbinger)x).Caught && !x.Player.Data.Disconnected) ? CustomGameOptions.HarbingerBakerBonus : 0);
        public bool CanWin => CustomGameOptions.BreadNeeded < PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.Disconnected && !x.Data.IsDead && !x.Is(Faction.NeutralApocalypse) && !x.Is(ObjectiveEnum.ApocalypseAgent));
        public Baker(PlayerControl player) : base(player)
        {
            Name = "Baker";
            ImpostorText = () => "Feed Players To Become Famine";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Feed players to become Famine\nFake Tasks:" : "Nakarm graczy by stac sie Famine\nFake Tasks:";
            Color = Patches.Colors.Baker;
            RoleType = RoleEnum.Baker;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralApocalypse;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var apoTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            apoTeam.Add(PlayerControl.LocalPlayer);
            foreach (var player in PlayerControl.AllPlayerControls) if ((player.Is(Faction.NeutralApocalypse) || player.Is(ObjectiveEnum.ApocalypseAgent) || (player.Is(RoleEnum.Undercover) && Utils.UndercoverIsApocalypse())) && player != PlayerControl.LocalPlayer) apoTeam.Add(player);
            __instance.teamToShow = apoTeam;
        }

        public float BreadTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBreaded;
            var num = CustomGameOptions.BakerCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void SpreadBread(PlayerControl target)
        {
            if (!BreadPlayers.Contains(target.PlayerId))
            {
                BreadPlayers.Add(target.PlayerId);
                Role.GetRole(target).BreadLeft = CustomGameOptions.BreadSize + 1;
            }
        }

        public void TurnFamine()
        {
            var oldRole = GetRole(Player);
            var killsList = (oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
            RoleDictionary.Remove(Player.PlayerId);
            var role = new Famine(Player);
            role.CorrectAssassinKills = killsList.CorrectAssassinKills;
            role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            role.FactionOverride = oldRole.FactionOverride;
            if (CustomGameOptions.AnnounceFamine)
            {
                NotificationPatch.DelayNotification(CustomGameOptions.AnnounceFamineDelay * 1000, TranslationPatches.CurrentLanguage == 0 ? $"<color=#{Patches.Colors.Famine.ToHtmlStringRGBA()}>FAMINE HAS TRANSFORMED!</color>" : $"<color=#{Patches.Colors.Famine.ToHtmlStringRGBA()}>FAMINE SIE PRZETRANSFORMOWAL!</color>", 1000 * CustomGameOptions.NotificationDuration, Patches.Colors.Famine);
            }
            else if (Player == PlayerControl.LocalPlayer)
            {
                NotificationPatch.DelayNotification(CustomGameOptions.AnnounceFamineDelay * 1000, TranslationPatches.CurrentLanguage == 0 ? $"<color=#{Patches.Colors.Famine.ToHtmlStringRGBA()}>FAMINE HAS TRANSFORMED!</color>" : $"<color=#{Patches.Colors.Famine.ToHtmlStringRGBA()}>FAMINE SIE PRZETRANSFORMOWAL!</color>", 1000 * CustomGameOptions.NotificationDuration, Patches.Colors.Famine);
            }
            if (Player == PlayerControl.LocalPlayer)
            {
                role.RegenTask();
            }
            role.Announced = false;
        }
    }
}