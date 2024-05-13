using System;
using HarmonyLib;
using TownOfUs.Roles;
using AmongUs.GameOptions;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.CrusaderMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Crusader);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Crusader>(PlayerControl.LocalPlayer);
            if (role.FortifyTimer() != 0) return false;
            if (!role.ButtonUsable) return false;

            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.FortifiedPlayers.Add(role.ClosestPlayer.PlayerId);
                role.LastFortified = DateTime.UtcNow;
                role.UsesLeft -= 1;
                Utils.Rpc(CustomRPC.Fortify, role.Player.PlayerId, role.ClosestPlayer.PlayerId);
                if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
            }
            if (interact[0] == true)
            {
                role.LastFortified = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastFortified = DateTime.UtcNow;
                role.LastFortified = role.LastFortified.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.FortifyCooldown);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}