using System;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Jackal : Role
    {
        public Jackal(PlayerControl player) : base(player)
        {
            Name = "Jackal";
            ImpostorText = () => "Kill Everyone With Your Recruits";
            TaskText = () => RecruitsAlive ? "Survive and wait until recruits make a kingdom of yours\nFake Tasks:" : "Your recruits died, now take down the crew yourself\nFake Tasks:";
            Color = Patches.Colors.Jackal;
            LastKill = DateTime.UtcNow;
            RoleType = RoleEnum.Jackal;
            Faction = Faction.NeutralKilling;
            FactionOverride = FactionOverride.Recruit;
            AddToRoleHistory(RoleType);
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        public bool RecruitsAlive => PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(FactionOverride.Recruit) && !x.Is(RoleEnum.Jackal)) > 0;
        public bool JackalWin { get; set; }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = CustomGameOptions.JackalKCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            var Recruits = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(FactionOverride.Recruit));
            var AlivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(FactionOverride.Recruit));
            var KillingAlives = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(FactionOverride.Recruit) && ((x.Data.IsImpostor() || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.NeutralKilling)) || ((x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Hunter)) && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK)));

            if ((Recruits >= AlivePlayers && !(KillingAlives > 0 || CustomGameOptions.OvertakeWin == OvertakeWin.Off)) || (Recruits > 0 && AlivePlayers == 0))
            {
                Utils.Rpc(CustomRPC.JackalWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
            }
            return false;
        }

        public void Wins()
        {
            JackalWin = true;
        }
    }
}