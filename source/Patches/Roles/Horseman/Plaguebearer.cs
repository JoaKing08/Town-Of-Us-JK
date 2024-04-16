using System;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities;
using TownOfUs.Extensions;
using UnityEngine;

namespace TownOfUs.Roles.Horseman
{
    public class Plaguebearer : Role
    {
        public PlayerControl ClosestPlayer;
        public List<byte> InfectedPlayers = new List<byte>();
        public DateTime LastInfected;

        public int InfectedAlive => InfectedPlayers.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null && !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected && !Utils.PlayerById(x).Is(Faction.NeutralApocalypse));
        public bool CanTransform => PlayerControl.AllPlayerControls.ToArray().Count(x => x != null && !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.NeutralApocalypse)) <= InfectedAlive;

        public Plaguebearer(PlayerControl player) : base(player)
        {
            Name = "Plaguebearer";
            ImpostorText = () => "Infect Everyone To Become Pestilence";
            TaskText = () => "Infect everyone to become Pestilence\nFake Tasks:";
            Color = Patches.Colors.Plaguebearer;
            RoleType = RoleEnum.Plaguebearer;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralApocalypse;
            InfectedPlayers.Add(player.PlayerId);
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;
            var CKExists = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) && !x.Is(ModifierEnum.ApocalypseAgent))) > 0;
            var AliveCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.NeutralApocalypse) && !x.Is(ModifierEnum.ApocalypseAgent) && !x.Is(RoleEnum.Witch) && !(x.Is(RoleEnum.Undercover) && Utils.UndercoverIsApocalypse() && CustomGameOptions.OvertakeWin == OvertakeWin.Off)).ToList();
            var AliveApocs = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.NeutralApocalypse)).ToList();
            if (AliveCrew.Count <= (((!CKExists && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK) || CustomGameOptions.OvertakeWin == OvertakeWin.On) ? AliveApocs.Count : 0) &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling))) == 0)
            {
                Utils.Rpc(CustomRPC.ApocalypseWin0, Player.PlayerId);
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

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            var apoTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            apoTeam.Add(PlayerControl.LocalPlayer);
            foreach (var player in PlayerControl.AllPlayerControls) if ((player.Is(Faction.NeutralApocalypse) || player.Is(ModifierEnum.ApocalypseAgent) || (player.Is(RoleEnum.Undercover) && Role.GetRole<Undercover>(player).UndercoverApocalypse)) && player != PlayerControl.LocalPlayer) apoTeam.Add(player);
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
            if (CustomGameOptions.AnnouncePestilence) Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Pestilence));
            else if (Player == PlayerControl.LocalPlayer) Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Pestilence));
            if (Player == PlayerControl.LocalPlayer)
            {
                role.RegenTask();
            }
            role.Announced = false;
        }
    }
}