using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using Hazel;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.DeathMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Death)) return;
            var role = Role.GetRole<Death>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)role.ApocalypseTimer(), (byte)CustomGameOptions.DeathCooldown);
            __instance.KillButton.SetCoolDown(role.ApocalypseTimer(), CustomGameOptions.DeathCooldown);
            if (role.ApocalypseTimer() <= 0 && __instance.KillButton.enabled)
            {
                __instance.KillButton.graphic.color = Palette.EnabledColor;
                __instance.KillButton.graphic.material.SetFloat("_Desat", 0f);
                __instance.KillButton.buttonLabelText.color = Palette.EnabledColor;
                __instance.KillButton.buttonLabelText.material.SetFloat("_Desat", 0f);
                return;
            }
        }
    }
}