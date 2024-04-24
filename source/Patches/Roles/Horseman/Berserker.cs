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
    public class Berserker : Role
    {
        public PlayerControl ClosestPlayer;
        public int KilledPlayers { get; set; } = 0;
        public DateTime LastKill;
        public TextMeshPro UsesText;

        public bool CanTransform => CustomGameOptions.KillsToWar <= KilledPlayers;

        public Berserker(PlayerControl player) : base(player)
        {
            Name = "Berserker";
            ImpostorText = () => "Kill Players To Become War";
            TaskText = () => "Kill players to become War\nFake Tasks:";
            Color = Patches.Colors.Berserker;
            RoleType = RoleEnum.Berserker;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralApocalypse;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            var CKExists = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) && !x.Is(ObjectiveEnum.ApocalypseAgent))) > 0;
            var AliveCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.NeutralApocalypse) && !x.Is(ObjectiveEnum.ApocalypseAgent) && !x.Is(RoleEnum.Witch) && !(x.Is(RoleEnum.Undercover) && Utils.UndercoverIsApocalypse() && CustomGameOptions.OvertakeWin == OvertakeWin.Off)).ToList();
            var AliveApocs = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.NeutralApocalypse)).ToList();
            if (AliveCrew.Count <= (((!CKExists && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK) || CustomGameOptions.OvertakeWin == OvertakeWin.On) ? AliveApocs.Count : 0) &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling))) == 0 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Is(RoleEnum.Plaguebearer) || x.Is(RoleEnum.Pestilence) || x.Is(RoleEnum.Baker) || x.Is(RoleEnum.Famine))) == 0)
            {
                Utils.Rpc(CustomRPC.ApocalypseWin4, Player.PlayerId);
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
            if (CustomGameOptions.AnnounceWar)
            {
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.War));
                role.Notification($"<color=#{Patches.Colors.War.ToHtmlStringRGBA()}>WAR HAS TRANSFORMED!</color>", 1000 * CustomGameOptions.NotificationDuration);
            }
            else if (Player == PlayerControl.LocalPlayer)
            {
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.War));
                role.Notification($"<color=#{Patches.Colors.War.ToHtmlStringRGBA()}>WAR HAS TRANSFORMED!</color>", 1000 * CustomGameOptions.NotificationDuration);
            }
            if (Player == PlayerControl.LocalPlayer)
            {
                role.RegenTask();
            }
            role.Announced = false;
        }
    }
}