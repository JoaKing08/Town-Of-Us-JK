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
    public class SoulCollector : Role
    {
        public DeadBody CurrentTarget;
        public int ReapedSouls = 0;
        public DateTime LastReaped;
        public TextMeshPro UsesText;

        public bool CanTransform => ReapedSouls >= CustomGameOptions.SoulsNeeded;

        public SoulCollector(PlayerControl player) : base(player)
        {
            Name = "Soul Collector";
            ImpostorText = () => "Reap Souls To Become Death";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Reap souls from bodies to become Death\nFake Tasks:" : "Wyrywaj dusze z cial by stac sie Death\nFake Tasks:";
            Color = Patches.Colors.SoulCollector;
            RoleType = RoleEnum.SoulCollector;
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

        public float ReapTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastReaped;
            var num = CustomGameOptions.SoulCollectorCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public void TurnDeath()
        {
            var oldRole = GetRole(Player);
            var killsList = (oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
            RoleDictionary.Remove(Player.PlayerId);
            var role = new Death(Player);
            role.CorrectAssassinKills = killsList.CorrectAssassinKills;
            role.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
            role.FactionOverride = oldRole.FactionOverride;
            if (CustomGameOptions.AnnounceDeath)
            {
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Death));
                role.Notification(TranslationPatches.CurrentLanguage == 0 ? $"<color=#{Patches.Colors.Death.ToHtmlStringRGBA()}>DEATH HAS TRANSFORMED!</color>" : $"<color=#{Patches.Colors.Death.ToHtmlStringRGBA()}>DEATH SIE PRZETRANSFORMOWAL!</color>", 1000 * CustomGameOptions.NotificationDuration);
            }
            else if (Player == PlayerControl.LocalPlayer)
            {
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Death));
                role.Notification(TranslationPatches.CurrentLanguage == 0 ? $"<color=#{Patches.Colors.Death.ToHtmlStringRGBA()}>DEATH HAS TRANSFORMED!</color>" : $"<color=#{Patches.Colors.Death.ToHtmlStringRGBA()}>DEATH SIE PRZETRANSFORMOWAL!</color>", 1000 * CustomGameOptions.NotificationDuration);
            }
            if (Player == PlayerControl.LocalPlayer)
            {
                role.RegenTask();
            }
            role.Announced = false;
        }
    }
}