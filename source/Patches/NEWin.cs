using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class NEWin
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (CustomGameOptions.NeutralEvilWinEndsGame) return;
            var neWin = false;
            var doomRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Doomsayer && ((Doomsayer)x).WonByGuessing && ((Doomsayer)x).Player == PlayerControl.LocalPlayer);
            if (doomRole != null) neWin = true;
            var exeRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Executioner && ((Executioner)x).TargetVotedOut && ((Executioner)x).Player == PlayerControl.LocalPlayer);
            if (exeRole != null) neWin = true;
            var jestRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Jester && ((Jester)x).VotedOut && ((Jester)x).Player == PlayerControl.LocalPlayer);
            if (jestRole != null) neWin = true;
            var pirateRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Pirate && ((Pirate)x).WonByDuel && ((Pirate)x).Player == PlayerControl.LocalPlayer);
            if (pirateRole != null) neWin = true;
            var inquisitorRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Inquisitor && ((Inquisitor)x).HereticsDead && ((Inquisitor)x).Player == PlayerControl.LocalPlayer);
            if (inquisitorRole != null) neWin = true;
            if (neWin)
            {
                __instance.WinText.text = "</color><color=#008DFFFF>Victory";
                var loveRole = Objective.AllObjectives.FirstOrDefault(x => x.ObjectiveType == ObjectiveEnum.Lover && ((Lover)x).LoveCoupleWins);
                if (loveRole != null) return;
                var survRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Survivor && Role.SurvOnlyWins);
                if (survRole != null) return;
                var vampRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Vampire && Role.VampireWins);
                if (vampRole != null) return;
                var arsoRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Arsonist && ((Arsonist)x).ArsonistWins);
                if (arsoRole != null) return;
                var glitchRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch && ((Glitch)x).GlitchWins);
                if (glitchRole != null) return;
                var juggRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Juggernaut && ((Juggernaut)x).JuggernautWins);
                if (juggRole != null) return;
                var skRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.SerialKiller && ((SerialKiller)x).SerialKillerWins);
                if (skRole != null) return;
                var apocRole = Role.AllRoles.FirstOrDefault(x => x.Faction == Faction.NeutralApocalypse && x.ApocalypseWins);
                if (apocRole != null) return;
                var wwRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Werewolf && ((Werewolf)x).WerewolfWins);
                if (wwRole != null) return;
                __instance.BackgroundBar.material.SetColor("_Color", Palette.CrewmateBlue);
            }
        }
    }
}