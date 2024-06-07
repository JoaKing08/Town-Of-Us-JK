using System;
using System.Linq;
using UnityEngine;
using TownOfUs.Extensions;
using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Werewolf : Role
    {
        private KillButton _rampageButton;
        public bool Enabled;
        public bool WerewolfWins;
        public PlayerControl ClosestPlayer;
        public DateTime LastRampaged;
        public DateTime LastKilled;
        public float TimeRemaining;


        public Werewolf(PlayerControl player) : base(player)
        {
            Name = "Werewolf";
            ImpostorText = () => "Rampage To Kill Everyone";
            TaskText = () => "Rampage to kill everyone\nFake Tasks:";
            Color = Patches.Colors.Werewolf;
            LastRampaged = DateTime.UtcNow;
            LastKilled = DateTime.UtcNow;
            RoleType = RoleEnum.Werewolf;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
        }

        public KillButton RampageButton
        {
            get => _rampageButton;
            set
            {
                _rampageButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;
            var AlivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.PlayerId != Player.PlayerId);
            var KillingAlives = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.PlayerId != Player.PlayerId && ((x.Data.IsImpostor() || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.NeutralKilling)) || ((x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Hunter)) && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK)));
            var ga = new Dictionary<byte, bool>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var i = false;
                if (player.Is(RoleEnum.GuardianAngel)) i = GetRole<GuardianAngel>(player).target.PlayerId == Player.PlayerId;
                ga.Add(player.PlayerId, i);
            }
            var onlyNonstopping = !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && !(x.Is(RoleEnum.GuardianAngel) && ga[x.PlayerId]) && !x.Is(RoleEnum.Survivor) && !x.Is(RoleEnum.Witch) && x.PlayerId != Player.PlayerId);

            if ((1 >= AlivePlayers && KillingAlives == 0 && CustomGameOptions.OvertakeWin != OvertakeWin.Off) || (1 > 0 && AlivePlayers == 0) || onlyNonstopping)
            {
                Utils.Rpc(CustomRPC.WerewolfWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Wins()
        {
            WerewolfWins = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var werewolfTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            werewolfTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = werewolfTeam;
        }
        public bool Rampaged => TimeRemaining > 0f;

        public float RampageTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastRampaged;
            var num = CustomGameOptions.RampageCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Rampage()
        {
            Enabled = true;
            TimeRemaining -= Time.deltaTime;
            if (Player.Data.IsDead)
            {
                TimeRemaining = 0f;
            }
        }

        public void Unrampage()
        {
            Enabled = false;
            LastRampaged = DateTime.UtcNow;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKilled;
            var num = CustomGameOptions.RampageKillCd * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
