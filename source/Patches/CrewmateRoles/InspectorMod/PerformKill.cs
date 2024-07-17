using System;
using HarmonyLib;
using TownOfUs.Roles;
using AmongUs.GameOptions;
using Reactor.Utilities;
using UnityEngine;
using TownOfUs.Patches;

namespace TownOfUs.CrewmateRoles.InspectorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Inspector);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);
            if (role.InspectTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4] == true)
            {
                role.LastInspectedPlayer = role.ClosestPlayer;
                role.LastInspected = DateTime.UtcNow;
                if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                if (role.BloodTimer(Role.GetRole(role.ClosestPlayer).LastBlood) == 0)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.green));
                    role.Notification(TranslationPatches.CurrentLanguage == 0 ? "Your Target Is Clean!" : "Twój Cel Jest Czysty!", 1000 * CustomGameOptions.NotificationDuration);
                }
                else
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.red));
                    role.Notification(TranslationPatches.CurrentLanguage == 0 ? "Your Target Is Bloody!" : "Twój Cel Jest Krwawy!", 1000 * CustomGameOptions.NotificationDuration);
                }
            }
            if (interact[0] == true)
            {
                role.LastInspected = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastInspected = DateTime.UtcNow;
                role.LastInspected = role.LastInspected.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.InspectCooldown);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}