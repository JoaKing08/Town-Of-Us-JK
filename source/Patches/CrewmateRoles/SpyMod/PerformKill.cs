using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using Reactor.Utilities;

namespace TownOfUs.CrewmateRoles.SpyMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Spy);
            if (!flag) return true;
            var role = Role.GetRole<Spy>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.BugTimer() == 0f && role.ButtonUsable;
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
                role.BuggedPlayers.Add(role.ClosestPlayer.PlayerId);
                PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Added Bug, Current Bugs:");
                foreach (var bug in role.BuggedPlayers)
                {
                    PluginSingleton<TownOfUs>.Instance.Log.LogMessage(bug);
                }
                role.BugsLeft -= 1;
                Utils.Rpc(CustomRPC.BugPlayer, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
            }
            if (interact[0] == true)
            {
                role.LastBugged = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastBugged = DateTime.UtcNow;
                role.LastBugged = role.LastBugged.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.BugCooldown);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
