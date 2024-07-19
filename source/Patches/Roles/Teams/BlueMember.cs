using System;
using System.Linq;

namespace TownOfUs.Roles.Teams
{
    public class BlueMember : TeamMember
    {
        public BlueMember(PlayerControl player) : base(player)
        {
            Name = "Member";
            ImpostorText = () => "Kill To Help Your Team Win!";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Kill to help your team win!" : "Zabijaj by pomóc twojej druzynie wygrac!";
            Color = Patches.Colors.BlueTeam;
            RoleType = RoleEnum.BlueMember;
            AddToRoleHistory(RoleType);
            Faction = Faction.BlueTeam;
            LastKill = DateTime.UtcNow;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(RoleEnum.BlueMember)) >= 1 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Is(RoleEnum.RedMember) || x.Is(RoleEnum.YellowMember) || x.Is(RoleEnum.GreenMember))) == 0)
            {
                Utils.Rpc(CustomRPC.BlueWin, Player.PlayerId);
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

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var blueTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            blueTeam.Add(PlayerControl.LocalPlayer);
            foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.BlueTeam) && x != PlayerControl.LocalPlayer)) blueTeam.Add(player);
            __instance.teamToShow = blueTeam;
        }
    }
}