using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using TownOfUs.Extensions;

namespace TownOfUs.NeutralRoles.PirateMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Pirate);
            if (!flag) return true;
            var role = Role.GetRole<Pirate>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            if (!__instance.enabled) return false;
            if (role.DuelTimer() != 0) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                role.DueledPlayer = role.ClosestPlayer;
                Utils.Rpc(CustomRPC.Duel, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                role.LastDueled = DateTime.UtcNow;
            }
            if (interact[0] == true) role.LastDueled = DateTime.UtcNow;
            else if (interact[1] == true)
            {
                role.LastDueled = DateTime.UtcNow;
                role.LastDueled.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.DuelCooldown);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
