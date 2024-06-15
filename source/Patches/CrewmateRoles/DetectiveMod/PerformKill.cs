using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.CrewmateRoles.MedicMod;
using Reactor.Utilities;
using AmongUs.GameOptions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.CrewmateRoles.DetectiveMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Detective)) return true;
            var role = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];

            if (__instance == role.ExamineButton)
            {
                var flag2 = role.ExamineTimer() == 0f;
                if (!flag2) return false;
                if (role.ClosestPlayer == null) return false;
                if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                if (role.ClosestPlayer == null) return false;
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[4] == true)
                {
                    if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)1);
                    if (role.DetectedKillers.Contains(role.ClosestPlayer.PlayerId) || (CustomGameOptions.CanDetectLastKiller && role.LastKiller == role.ClosestPlayer))
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                        role.Notification("Your Target Is A Killer!", 1000 * CustomGameOptions.NotificationDuration);
                    }
                    else
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                        role.Notification("Your Target Isn't A Killer!", 1000 * CustomGameOptions.NotificationDuration);
                    }
                }
                if (interact[0] == true)
                {
                    role.LastExamined = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastExamined = DateTime.UtcNow;
                    role.LastExamined = role.LastExamined.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.ExamineCd);
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
            else
            {
                if (role.CurrentTarget == null)
                    return false;
                if (Vector2.Distance(role.CurrentTarget.TruePosition,
                    PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                if (Role.GetRole(PlayerControl.LocalPlayer).Roleblocked)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
                    role.Notification("You Are Roleblocked!", 1000 * CustomGameOptions.NotificationDuration);
                    return false;
                }
                var playerId = role.CurrentTarget.ParentId;
                var player = Utils.PlayerById(playerId);
                if (PlayerControl.LocalPlayer.IsInVision() || player.IsInVision())
                {
                    Utils.Rpc(CustomRPC.VisionInteract, PlayerControl.LocalPlayer.PlayerId, player.PlayerId);
                }
                if (player.IsInfected() || role.Player.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
                }
                if (player.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, playerId, (byte)role.RoleType, (byte)0);
                foreach (var deadPlayer in Murder.KilledPlayers)
                {
                    if (deadPlayer.PlayerId == playerId) role.DetectedKillers.Add(deadPlayer.KillerId);
                }
                return false;
            }
        }
    }
}
