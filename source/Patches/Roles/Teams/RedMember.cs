using System;
using System.Linq;

namespace TownOfUs.Roles.Teams
{
    public class RedMember : TeamMember
    {
        public RedMember(PlayerControl player) : base(player)
        {
            Name = "Member";
            ImpostorText = () => "Kill To Help Your Team Win!";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Kill to help your team win!" : "Zabijaj by pomóc twojej druzynie wygrac!";
            Color = Patches.Colors.RedTeam;
            RoleType = RoleEnum.RedMember;
            AddToRoleHistory(RoleType);
            Faction = Faction.RedTeam;
            LastKill = DateTime.UtcNow;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(RoleEnum.RedMember)) >= 1 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Is(RoleEnum.BlueMember) || x.Is(RoleEnum.YellowMember) || x.Is(RoleEnum.GreenMember))) == 0)
            {
                Utils.Rpc(CustomRPC.RedWin, Player.PlayerId);
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
            var redTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            redTeam.Add(PlayerControl.LocalPlayer);
            foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.RedTeam) && x != PlayerControl.LocalPlayer)) redTeam.Add(player);
            __instance.teamToShow = redTeam;
        }
    }
}