using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.BakerMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
    public static class MeetingHudUpdate
    {
        public static void Postfix(MeetingHud __instance)
        {
            var localPlayer = PlayerControl.LocalPlayer;
            var _role = Role.GetRole(localPlayer);
            if (_role?.RoleType != RoleEnum.Baker) return;
            if (localPlayer.Data.IsDead) return;
            var role = (Baker)_role;
            foreach (var state in __instance.playerStates)
            {
                var targetId = state.TargetPlayerId;
                var playerData = Utils.PlayerById(targetId)?.Data;
                if (playerData == null || playerData.Disconnected)
                {
                    role.BreadPlayers.Remove(targetId);
                    continue;
                }
                if (role.BreadPlayers.Contains(targetId) && role.Player.PlayerId != targetId) state.NameText.color = Color.yellow;
            }
        }
    }
}