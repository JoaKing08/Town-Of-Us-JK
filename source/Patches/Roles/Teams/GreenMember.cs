using System;
using System.Linq;

namespace TownOfUs.Roles.Teams
{
    public class GreenMember : TeamMember
    {
        public GreenMember(PlayerControl player) : base(player)
        {
            Name = "Member";
            ImpostorText = () => "Kill To Help Your Team Win!";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Kill to help your team win!" : "Zabijaj by pomóc twojej druzynie wygrac!";
            Color = Patches.Colors.GreenTeam;
            RoleType = RoleEnum.GreenMember;
            AddToRoleHistory(RoleType);
            Faction = Faction.GreenTeam;
            LastKill = DateTime.UtcNow;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;

            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(RoleEnum.GreenMember)) >= 1 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Is(RoleEnum.BlueMember) || x.Is(RoleEnum.YellowMember) || x.Is(RoleEnum.RedMember))) == 0)
            {
                Utils.Rpc(CustomRPC.GreenWin, Player.PlayerId);
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
            var greenTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            greenTeam.Add(PlayerControl.LocalPlayer);
            foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.GreenTeam) && x != PlayerControl.LocalPlayer)) greenTeam.Add(player);
            __instance.teamToShow = greenTeam;
        }
    }
}