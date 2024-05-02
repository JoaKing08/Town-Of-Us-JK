using System;
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
            var CKExists = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter))) > 0;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= (CustomGameOptions.OvertakeWin == OvertakeWin.Off || (CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK && CKExists) ? 1 : 2) &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling) || x.Is(Faction.NeutralApocalypse))) == 1 && Player.Is(FactionOverride.None))
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