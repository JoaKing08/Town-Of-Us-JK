using System;
using HarmonyLib;
using TownOfUs.Roles;
using AmongUs.GameOptions;
using Reactor.Utilities;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.DemagogueMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue);
            if (!flag) return true;
            var role = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
            if (__instance != role.ConvinceButton) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (role.ConvinceTimer() != 0) return false;
            if (role.Charges < CustomGameOptions.ChargesForConvince) return false;
            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.Convinced.Add(role.ClosestPlayer.PlayerId);
                role.Charges -= CustomGameOptions.ChargesForConvince;
                Utils.Rpc(CustomRPC.DemagogueCharges, role.Charges, role.Player.PlayerId);
                role.LastConvince = DateTime.UtcNow;
                Utils.Rpc(CustomRPC.DemagogueConvince, role.ClosestPlayer.PlayerId, (byte)role.Player.PlayerId);
                if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
            }
            if (interact[0] == true)
            {
                role.LastConvince = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastConvince = DateTime.UtcNow;
                role.LastConvince = role.LastConvince.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.ConvinceCooldown);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}