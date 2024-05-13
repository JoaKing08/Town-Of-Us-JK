using System;
using HarmonyLib;
using TownOfUs.Roles;
using AmongUs.GameOptions;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.BodyguardMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Bodyguard);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Bodyguard>(PlayerControl.LocalPlayer);
            if (role.GuardTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.GuardedPlayer = role.ClosestPlayer;
                role.LastGuard = DateTime.UtcNow;
                Utils.Rpc(CustomRPC.Guard, role.Player.PlayerId, role.ClosestPlayer.PlayerId);
                if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
            }
            if (interact[0] == true)
            {
                role.LastGuard = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastGuard = DateTime.UtcNow;
                role.LastGuard = role.LastGuard.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.GuardCooldown);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}