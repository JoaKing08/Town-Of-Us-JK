using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using Reactor.Utilities;
using TownOfUs.Extensions;
using UnityEngine;
using TMPro;
using Reactor.Utilities.Extensions;

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
            TaskText = () => "Reap souls from bodies to become Death\nFake Tasks:";
            Color = Patches.Colors.SoulCollector;
            RoleType = RoleEnum.SoulCollector;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralApocalypse;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;
            var Apocalypse = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.NeutralApocalypse));
            var AlivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.NeutralApocalypse));
            var KillingAlives = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !(x.Is(FactionOverride.None) && (x.Is(Faction.NeutralApocalypse) || x.Is(ObjectiveEnum.ApocalypseAgent))) && ((x.Data.IsImpostor() || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.NeutralKilling)) || ((x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Hunter)) && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK)));
            var ga = new Dictionary<byte, bool>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var i = false;
                if (player.Is(RoleEnum.GuardianAngel)) i = GetRole<GuardianAngel>(player).target.Is(Faction.NeutralApocalypse) || GetRole<GuardianAngel>(player).target.Is(ObjectiveEnum.ApocalypseAgent);
                ga.Add(player.PlayerId, i);
            }
            var onlyNonstopping = !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(ObjectiveEnum.ApocalypseAgent) && !(x.Is(RoleEnum.GuardianAngel) && ga[x.PlayerId]) && !x.Is(RoleEnum.Survivor) && !x.Is(RoleEnum.Witch) && !x.Is(FactionOverride.Undead));

            if ((Apocalypse >= AlivePlayers && KillingAlives == 0 && CustomGameOptions.OvertakeWin != OvertakeWin.Off) || (Apocalypse > 0 && AlivePlayers == 0) || onlyNonstopping)
            {
                Utils.Rpc(CustomRPC.ApocalypseWin6, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Wins()
        {
            ApocalypseWins = true;
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
                role.Notification($"<color=#{Patches.Colors.Death.ToHtmlStringRGBA()}>DEATH HAS TRANSFORMED!</color>", 1000 * CustomGameOptions.NotificationDuration);
            }
            else if (Player == PlayerControl.LocalPlayer)
            {
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Death));
                role.Notification($"<color=#{Patches.Colors.Death.ToHtmlStringRGBA()}>DEATH HAS TRANSFORMED!</color>", 1000 * CustomGameOptions.NotificationDuration);
            }
            if (Player == PlayerControl.LocalPlayer)
            {
                role.RegenTask();
            }
            role.Announced = false;
        }
    }
}