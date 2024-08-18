using System.Linq;
using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles
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
            Role role = null;
            role = Role.AllRoles.FirstOrDefault(x =>
                x.Faction == Faction.NeutralApocalypse && Role.ApocalypseWins && !x.Player.Data.IsDead && !x.Player.Data.Disconnected && x.FactionOverride == FactionOverride.None);
            if (role == null)
            {
                role = Role.AllRoles.FirstOrDefault(x =>
                x.Player.Is(ObjectiveEnum.ApocalypseAgent) && Role.ApocalypseWins && !x.Player.Data.IsDead && !x.Player.Data.Disconnected && x.FactionOverride == FactionOverride.None);
            }
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