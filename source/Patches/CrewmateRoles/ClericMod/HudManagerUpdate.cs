using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.ClericMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Cleric)) return;
            var role = Role.GetRole<Cleric>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (role.BarrieredPlayer != null)
                {
                    if (role.BarrieredPlayer.PlayerId == player.PlayerId)
                    {
                        if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                                player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                            player.nameText().color = Patches.Colors.Cleric;
                        else player.nameText().color = Color.clear;
                    }
                }
            }

            __instance.KillButton.SetCoolDown(role.BarrierTimer(), CustomGameOptions.BarrierCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
        }
    }
}