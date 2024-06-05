using HarmonyLib;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.DeputyMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Deputy)) return;
            var role = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.AliveTargets < CustomGameOptions.MaxDeputyTargets)
            {
                role.UsesText = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                role.UsesText.gameObject.SetActive(false);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.UsesText != null)
            {
                role.UsesText.text = (CustomGameOptions.MaxDeputyTargets - role.AliveTargets) + "";
            }
            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (role.Targets.Any(x => x == player.PlayerId))
                {
                    if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                            player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                        player.nameText().color = Patches.Colors.Deputy;
                    else player.nameText().color = Color.clear;
                }
            }

            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Targets.Contains(x.PlayerId)).ToList());
            if (role.AliveTargets < CustomGameOptions.MaxDeputyTargets)
            {
                __instance.KillButton.SetCoolDown(role.AimTimer(), CustomGameOptions.DeputyAimCooldown);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                __instance.KillButton.SetCoolDown(0f, 1f);
                var renderer = __instance.KillButton.graphic;
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}