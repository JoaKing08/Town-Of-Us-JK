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
    public class Berserker : Role
    {
        public PlayerControl ClosestPlayer;
        public int KilledPlayers { get; set; } = 0;
        public DateTime LastKill;
        public TextMeshPro UsesText;

        public bool CanTransform => CustomGameOptions.KillsToWar <= KilledPlayers + (Role.GetRoles(RoleEnum.Harbinger).Any(x => ((Harbinger)x).CompletedTasks && !((Harbinger)x).Caught && !x.Player.Data.Disconnected) ? CustomGameOptions.HarbingerBerserkerBonus : 0);

        public Berserker(PlayerControl player) : base(player)
        {
            Name = "Berserker";
            ImpostorText = () => "Kill Players To Become War";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Kill players to become War\nFake Tasks:" : "Zabijaj graczy by stac sie War\nFake Tasks:";
            Color = Patches.Colors.Berserker;
            RoleType = RoleEnum.Berserker;
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

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = (CustomGameOptions.BerserkerCooldown - (CustomGameOptions.BerserkerCooldownBonus * KilledPlayers)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void TurnWar()
        {
            var oldRole = GetRole(Player);
            var killsList = (oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills, oldRole.Kills);
            RoleDictionary.Remove(Player.PlayerId);
            var role = new War(Player);
            role.CorrectAssassinKills = killsList.CorrectAssassinKills;
            role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            role.Kills = killsList.Kills;
            role.FactionOverride = oldRole.FactionOverride;
            if (CustomGameOptions.AnnounceWar)
            {
                NotificationPatch.DelayNotification(CustomGameOptions.AnnounceWarDelay * 1000, TranslationPatches.CurrentLanguage == 0 ? $"<color=#{Patches.Colors.War.ToHtmlStringRGBA()}>WAR HAS TRANSFORMED!</color>" : $"<color=#{Patches.Colors.War.ToHtmlStringRGBA()}>WAR SIE PRZETRANSFORMOWAL!</color>", 1000 * CustomGameOptions.NotificationDuration, Patches.Colors.War);
            }
            else if (Player == PlayerControl.LocalPlayer)
            {
                NotificationPatch.DelayNotification(CustomGameOptions.AnnounceWarDelay * 1000, TranslationPatches.CurrentLanguage == 0 ? $"<color=#{Patches.Colors.War.ToHtmlStringRGBA()}>WAR HAS TRANSFORMED!</color>" : $"<color=#{Patches.Colors.War.ToHtmlStringRGBA()}>WAR SIE PRZETRANSFORMOWAL!</color>", 1000 * CustomGameOptions.NotificationDuration, Patches.Colors.War);
            }
            if (Player == PlayerControl.LocalPlayer)
            {
                role.RegenTask();
            }
            role.Announced = false;
        }
    }
}