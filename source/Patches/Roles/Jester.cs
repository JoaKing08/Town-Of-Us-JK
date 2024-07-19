using Il2CppSystem.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Jester : Role
    {
        public bool VotedOut;
        public bool SpawnedAs = true;
        public bool WasSwapped = false;


        public Jester(PlayerControl player) : base(player)
        {
            Name = "Jester";
            ImpostorText = () => "Get Voted Out";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? (WasSwapped ? "Your role was stolen. Now you get voted out!" : SpawnedAs ? "Get voted out!\nFake Tasks:" : "Your target was killed. Now you get voted out!\nFake Tasks:") : (WasSwapped ? "Twoja rola zostala podmieniona. Teraz zostan wyglosowany!" : SpawnedAs ? "Zostan wyglosowany!\nFake Tasks:" : "Twój cel zginal. Teraz zostan wyglosowany!\nFake Tasks:");
            Color = Patches.Colors.Jester;
            RoleType = RoleEnum.Jester;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralEvil;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var jestTeam = new List<PlayerControl>();
            jestTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = jestTeam;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (!VotedOut || !Player.Data.IsDead && !Player.Data.Disconnected) return true;
            if (!CustomGameOptions.NeutralEvilWinEndsGame) return true;
            if (!Player.Is(FactionOverride.None)) return true;
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            //System.Console.WriteLine("Reached Here - Jester edition");
            VotedOut = true;
        }
    }
}