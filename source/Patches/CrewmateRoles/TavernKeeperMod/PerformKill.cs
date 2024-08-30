using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;

namespace TownOfUs.CrewmateRoles.TavernKeeperMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.TavernKeeper);
            if (!flag) return true;
            var role = Role.GetRole<TavernKeeper>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.DrinkTimer() == 0f && role.CanDrink;
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
                role.DrunkPlayers.Add(role.ClosestPlayer);
                Role.GetRole(role.ClosestPlayer).Roleblocked = true;
                Utils.Rpc(CustomRPC.Roleblock, role.ClosestPlayer.PlayerId, false);
            }
            if (interact[0] == true)
            {
                role.LastDrink = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastDrink = DateTime.UtcNow;
                role.LastDrink = role.LastDrink.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.DrinkCooldown);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
