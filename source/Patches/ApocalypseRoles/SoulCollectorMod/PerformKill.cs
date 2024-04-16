using System;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using TownOfUs.Roles.Horseman;
using Reactor.Utilities;

namespace TownOfUs.ApocalypseRoles.SoulCollectorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<SoulCollector>(PlayerControl.LocalPlayer);
            if (role.ReapTimer() != 0) return false;
            if (Role.GetRole(PlayerControl.LocalPlayer).Roleblocked)
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.white));
                return false;
            }
            if (Role.GetRole(Utils.PlayerById(role.CurrentTarget.ParentId)).Reaped) return false;

            var flag2 = __instance.isCoolingDown;
            if (flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (role == null)
                return false;
            if (role.CurrentTarget == null)
                return false;
            if (Vector2.Distance(role.CurrentTarget.TruePosition,
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            var playerId = role.CurrentTarget.ParentId;
            var player = Utils.PlayerById(playerId);
            if (player.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, playerId, (byte)role.RoleType, (byte)0);
            role.ReapedSouls += 1;
            Role.GetRole(Utils.PlayerById(role.CurrentTarget.ParentId)).Reaped = true;
            role.LastReaped = DateTime.UtcNow;
            return false;
        }
    }
}