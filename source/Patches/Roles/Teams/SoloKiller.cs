using System;
using System.Linq;

namespace TownOfUs.Roles.Teams
{
    public class SoloKiller : TeamMember
    {
        public SoloKiller(PlayerControl player) : base(player)
        {
            Name = "Killer";
            ImpostorText = () => "Kill To Win!";
            TaskText = () => "Kill to win!";
            Color = Patches.Colors.Impostor;
            RoleType = RoleEnum.SoloKiller;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
            LastKill = DateTime.UtcNow;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected) <= (CustomGameOptions.OvertakeWin == OvertakeWin.Off ? 1 : 2))
            {
                Utils.Rpc(CustomRPC.SoloKillerWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Wins()
        {
            TeamWins = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            var soloKillerTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            soloKillerTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = soloKillerTeam;
        }
    }
}