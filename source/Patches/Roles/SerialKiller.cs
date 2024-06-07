using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class SerialKiller : Role
    {
        public SerialKiller(PlayerControl owner) : base(owner)
        {
            Name = "Serial Killer";
            Color = Patches.Colors.SerialKiller;
            LastKill = DateTime.UtcNow;
            RoleType = RoleEnum.SerialKiller;
            AddToRoleHistory(RoleType);
            ImpostorText = () => "Kill To Fill Your Bloodlust";
            TaskText = () => "Fill your Bloodlust to kill faster\nFake Tasks:";
            Faction = Faction.NeutralKilling;
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        public DateTime BloodlustStart { get; set; }
        public bool SerialKillerWins { get; set; }
        public int SKKills { get; set; } = 0;
        public bool InBloodlust = false;
        public int KillsLeft => CustomGameOptions.KillsToBloodlust - SKKills;
        public TextMeshPro KillsText;

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
                Utils.Rpc(CustomRPC.SerialKillerWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Wins()
        {
            SerialKillerWins = true;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = (InBloodlust ? CustomGameOptions.BloodlustCooldown : CustomGameOptions.SerialKillerCooldown) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float BloodlustTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - BloodlustStart;
            var num = CustomGameOptions.BloodlustDuration * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var skTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            skTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = skTeam;
        }
    }
}