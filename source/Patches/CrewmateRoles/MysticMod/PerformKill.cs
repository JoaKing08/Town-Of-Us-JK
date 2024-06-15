using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using Reactor.Utilities;

namespace TownOfUs.CrewmateRoles.MysticMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Mystic);
            if (!flag) return true;
            var role = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.VisionTimer() == 0f && !role.UsedAbility;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                role.VisionPlayer = role.ClosestPlayer.PlayerId;
                role.UsedAbility = true;
                Utils.Rpc(CustomRPC.VisionPlayer, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
            }
            if (interact[0] == true)
            {
                role.LastVision = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastVision = DateTime.UtcNow;
                role.LastVision = role.LastVision.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.VisionCooldown);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
