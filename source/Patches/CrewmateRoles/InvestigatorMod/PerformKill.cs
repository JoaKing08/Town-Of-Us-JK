using System;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using TownOfUs.Roles.Horseman;
using Reactor.Utilities;

namespace TownOfUs.CrewmateRoles.InvestigatorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            if (CustomGameOptions.GameMode == GameMode.Cultist) return false;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Investigator);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Investigator>(PlayerControl.LocalPlayer);
            if (role.InvestigateTimer() != 0) return false;
            if (PlayerControl.LocalPlayer.IsRoleblocked())
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.white));
                NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                return false;
            }

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
            if (role.Reports[role.CurrentTarget.ParentId].Count == 0) return false;
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
            role.LastInvestigate = DateTime.UtcNow;
            role.GenerateMessage(role.CurrentTarget);
            role.UsesLeft -= 1;
            return false;
        }
    }
}