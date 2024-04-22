using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.WitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.IsControled())
            {
                Utils.Rpc(CustomRPC.ControlCooldown, __instance.KillButton.gameObject.active, (byte)PlayerControl.LocalPlayer.killTimer, __instance.KillButton.graphic.color = Palette.EnabledColor);
            }
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Witch)) return;
            var role = Role.GetRole<Witch>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);


            if (role.ControledPlayer == null || role.ControlTimer() != 0) __instance.KillButton.SetCoolDown(role.ControlTimer(), CustomGameOptions.ControlCooldown);
            else if (role.TargetIsEnabled) __instance.KillButton.SetCoolDown(role.TargetCooldown, role.TargetCooldown == 0 ? 1f : role.TargetCooldown);
            else __instance.KillButton.SetCoolDown(0f, 1f);
            if (role.ControledPlayer == null) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            else Utils.SetTarget(ref role.ClosestPlayer, role.ControledPlayer, __instance.KillButton);
            var renderer = __instance.KillButton.graphic;
            if (role.TargetIsActive || !role.TargetIsEnabled)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else if (!role.TargetIsActive)
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}