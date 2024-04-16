using System.Linq;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.DeathMod
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public static class Outro
    {
        public static void Postfix(EndGameManager __instance)
        {
            if (CustomGameOptions.NeutralEvilWinEndsGame)
            {
                if (Role.GetRoles(RoleEnum.Jester).Any(x => ((Jester)x).VotedOut)) return;
                if (Role.GetRoles(RoleEnum.Executioner).Any(x => ((Executioner)x).TargetVotedOut)) return;
                if (Role.GetRoles(RoleEnum.Doomsayer).Any(x => ((Doomsayer)x).WonByGuessing)) return;
                if (Role.GetRoles(RoleEnum.Pirate).Any(x => ((Pirate)x).WonByDuel)) return;
                if (Role.GetRoles(RoleEnum.Inquisitor).Any(x => ((Inquisitor)x).HereticsDead)) return;
            }
            if (PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
            (x.Is(RoleEnum.Plaguebearer) || x.Is(RoleEnum.Pestilence) || x.Is(RoleEnum.Baker) || x.Is(RoleEnum.Famine) ||
            x.Is(RoleEnum.Berserker) || x.Is(RoleEnum.War))) > 0) return;
            var role = Role.AllRoles.FirstOrDefault(x =>
            x.RoleType == RoleEnum.Death && ((Death)x).ApocalypseWins && !x.Player.Data.IsDead && !x.Player.Data.Disconnected);
            if (role == null) return;
            PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();
            foreach (var player in array) player.NameText().text = "<color=#" + Color.gray.ToHtmlStringRGBA() + ">" + player.NameText().text + "</color>";
            __instance.BackgroundBar.material.color = Color.gray;
            var text = Object.Instantiate(__instance.WinText);
            text.text = "The Apocalypse Is Nigh!";
            text.color = Color.gray;
            var pos = __instance.WinText.transform.localPosition;
            pos.y = 1.5f;
            text.transform.position = pos;
            text.text = $"<size=4>{text.text}</size>";
        }
    }
}