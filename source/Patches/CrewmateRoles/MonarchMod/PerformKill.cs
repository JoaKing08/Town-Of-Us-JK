using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using System.Linq;

namespace TownOfUs.CrewmateRoles.MonarchMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static bool Prefix(MeetingHud __instance)
        {
            if (Role.GetRoles(RoleEnum.Monarch).Any()) foreach (Monarch monarch in Role.GetRoles(RoleEnum.Monarch))
                {
                    monarch.FirstRound = false;
                }
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Monarch);
            if (!flag) return true;
            var role = Role.GetRole<Monarch>(PlayerControl.LocalPlayer);
            if (role.toKnight.ToArray().Where(x => !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected).Any()) foreach (var vent in role.toKnight.ToArray().Where(x => !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected))
                {
                    role.Knights.Add(role.ClosestPlayer.PlayerId);
                    Utils.Rpc(CustomRPC.MonarchKnight, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                }
            role.toKnight.Clear();
            return true;
        }
    }
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Monarch);
            if (!flag) return true;
            var role = Role.GetRole<Monarch>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.KnightTimer() == 0f && role.CanKnight;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            if (!role.CanKnight) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;

            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                if (CustomGameOptions.InstantKnight)
                {
                    role.Knights.Add(role.ClosestPlayer.PlayerId);
                    Utils.Rpc(CustomRPC.MonarchKnight, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                }
                else
                {
                    role.toKnight.Add(role.ClosestPlayer.PlayerId);
                }
                role.LastKnighted = DateTime.UtcNow;
            }
            if (interact[0] == true)
            {
                role.LastKnighted = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastKnighted = DateTime.UtcNow;
                role.LastKnighted = role.LastKnighted.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.KnightCooldown);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}
