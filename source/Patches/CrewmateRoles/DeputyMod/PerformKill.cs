using System;
using HarmonyLib;
using TownOfUs.Roles;
using AmongUs.GameOptions;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.DeputyMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Deputy);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
            if (role.AimTimer() != 0) return false;
            if (role.AliveTargets >= CustomGameOptions.MaxDeputyTargets) return false;
            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.Targets.Add(role.ClosestPlayer.PlayerId);
                role.LastAimed = DateTime.UtcNow;
                if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
            }
            if (interact[0] == true)
            {
                role.LastAimed = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastAimed = DateTime.UtcNow;
                role.LastAimed = role.LastAimed.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.DeputyAimCooldown);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}