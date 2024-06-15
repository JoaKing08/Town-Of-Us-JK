using HarmonyLib;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SageMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sage)) return;
            var role = Role.GetRole<Sage>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (role.FirstPlayer == player.PlayerId || role.SecondPlayer == player.PlayerId)
                {
                    if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                            player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                        player.nameText().color = Patches.Colors.Sage;
                    else player.nameText().color = Color.clear;
                }
            }

            if (role.ButtonUsable) __instance.KillButton.SetCoolDown(role.CompareTimer(), CustomGameOptions.CompareCooldown);
            else __instance.KillButton.SetCoolDown(0f, 1f);
            var notCompared = PlayerControl.AllPlayerControls.ToArray().Where(x => role.FirstPlayer != x.PlayerId && role.SecondPlayer != x.PlayerId).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, notCompared);

            var renderer = __instance.KillButton.graphic;
            if (!__instance.KillButton.isCoolingDown && role.ButtonUsable && role.ClosestPlayer != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}