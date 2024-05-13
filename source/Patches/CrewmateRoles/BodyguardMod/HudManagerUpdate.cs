using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.BodyguardMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Bodyguard)) return;
            var role = Role.GetRole<Bodyguard>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (role.GuardedPlayer != null)
                {
                    if (role.GuardedPlayer.PlayerId == player.PlayerId)
                    {
                        if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                                player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                            player.nameText().color = Patches.Colors.Bodyguard;
                        else player.nameText().color = Color.clear;
                    }
                }
            }

            __instance.KillButton.SetCoolDown(role.GuardTimer(), CustomGameOptions.GuardCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
        }
    }
}