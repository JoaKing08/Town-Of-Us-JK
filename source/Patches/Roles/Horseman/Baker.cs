using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using Reactor.Utilities;
using TownOfUs.Extensions;
using UnityEngine;
using TMPro;

namespace TownOfUs.Roles.Horseman
{
    public class Baker : Role
    {
        public PlayerControl ClosestPlayer;
        public List<byte> BreadPlayers = new List<byte>();
        public DateTime LastBreaded;
        public TextMeshPro UsesText;

        public int BreadAlive => BreadPlayers.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null && !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected);
        public bool CanTransform => CustomGameOptions.BreadNeeded <= BreadAlive;
        public bool CanWin => CustomGameOptions.BreadNeeded - BreadAlive <= PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.Disconnected && !x.Data.IsDead && !x.Is(Faction.NeutralApocalypse));
        public Baker(PlayerControl player) : base(player)
        {
            Name = "Baker";
            ImpostorText = () => "Feed Players To Become Famine";
            TaskText = () => "Feed players to become Famine\nFake Tasks:";
            Color = Patches.Colors.Baker;
            RoleType = RoleEnum.Baker;
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
                    (x.Is(RoleEnum.Plaguebearer) || x.Is(RoleEnum.Pestilence))) == 0)
            {
                Utils.Rpc(CustomRPC.ApocalypseWin2, Player.PlayerId);
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
            if (CustomGameOptions.AnnounceFamine) Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Famine));
            else if (Player == PlayerControl.LocalPlayer) Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Famine));
            if (Player == PlayerControl.LocalPlayer)
            {
                role.RegenTask();
            }
            role.Announced = false;
        }
    }
}