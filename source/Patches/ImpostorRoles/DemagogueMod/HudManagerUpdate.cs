using HarmonyLib;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.DemagogueMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static Sprite ConvinceSprite => TownOfUs.ConvinceSprite;
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue)) return;
            var role = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);

            role.RegenTask();

            if (role.ConvinceButton == null)
            {
                role.ConvinceButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.ConvinceButton.graphic.enabled = true;
                role.ConvinceButton.gameObject.SetActive(false);
            }

            role.ConvinceButton.graphic.sprite = ConvinceSprite;
            role.ConvinceButton.buttonLabelText.gameObject.SetActive(true);
            role.ConvinceButton.buttonLabelText.text = "Convince";

            if (role.CostText == null)
            {
                role.CostText = Object.Instantiate(role.ConvinceButton.cooldownTimerText, role.ConvinceButton.transform);
                role.CostText.gameObject.SetActive(false);
                role.CostText.transform.localPosition = new Vector3(
                    role.CostText.transform.localPosition.x + 0.26f,
                    role.CostText.transform.localPosition.y + 0.29f,
                    role.CostText.transform.localPosition.z);
                role.CostText.transform.localScale = role.CostText.transform.localScale * 0.65f;
                role.CostText.alignment = TMPro.TextAlignmentOptions.Right;
                role.CostText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.CostText != null)
            {
                role.CostText.text = CustomGameOptions.ChargesForConvince + "";
                role.CostText.color = role.Charges >= CustomGameOptions.ChargesForConvince ? Color.green : Color.red;
                role.CostText.material.SetFloat("_Desat", 0f);
            }
            role.ConvinceButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.CostText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (role.Charges >= CustomGameOptions.ChargesForConvince)
            {
                role.ConvinceButton.SetCoolDown(role.ConvinceTimer(), CustomGameOptions.ConvinceCooldown);
                Utils.SetTarget(ref role.ClosestPlayer, role.ConvinceButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !role.Convinced.Contains(x.PlayerId)).ToList());
            }
            else
            {
                role.ConvinceButton.SetCoolDown(0f, 1f);
                var renderer = role.ConvinceButton.graphic;
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}