using System;
using HarmonyLib;
using TownOfUs.Roles;
using AmongUs.GameOptions;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SageMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Sage);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Sage>(PlayerControl.LocalPlayer);
            if (role.CompareTimer() != 0) return false;
            if (!role.ButtonUsable) return false;

            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                if (role.FirstPlayer == byte.MaxValue || Utils.PlayerById(role.FirstPlayer).Data.IsDead || Utils.PlayerById(role.FirstPlayer).Data.Disconnected) role.FirstPlayer = role.ClosestPlayer.PlayerId;
                else if (role.SecondPlayer == byte.MaxValue || Utils.PlayerById(role.SecondPlayer).Data.IsDead || Utils.PlayerById(role.SecondPlayer).Data.Disconnected) role.SecondPlayer = role.ClosestPlayer.PlayerId;
                if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                role.LastCompared = DateTime.UtcNow;
            }
            if (interact[0] == true)
            {
                role.LastCompared = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastCompared = DateTime.UtcNow;
                role.LastCompared = role.LastCompared.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.CompareCooldown);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}