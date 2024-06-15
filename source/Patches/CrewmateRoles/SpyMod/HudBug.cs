using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.SpyMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudBug
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateBugButton(__instance);
        }

        public static void UpdateBugButton(HudManager __instance)
        {
            if (CustomGameOptions.GameMode == GameMode.Cultist) return;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Spy)) return;
            var bugButton = __instance.KillButton;

            var role = Role.GetRole<Spy>(PlayerControl.LocalPlayer);

            if (role.UsesText == null && role.BugsLeft > 0)
            {
                role.UsesText = Object.Instantiate(bugButton.cooldownTimerText, bugButton.transform);
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
                role.UsesText.text = role.BugsLeft + "";
            }
            bugButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && CustomGameOptions.BugsPerGame > 0);
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && CustomGameOptions.BugsPerGame > 0);
            //if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)(role.ButtonUsable ? role.BugTimer() : 0), (byte)CustomGameOptions.BugCooldown);
            if (role.ButtonUsable) bugButton.SetCoolDown(role.BugTimer(), CustomGameOptions.BugCooldown);
            else bugButton.SetCoolDown(0f, CustomGameOptions.BugCooldown);

            var notBugged = PlayerControl.AllPlayerControls
                .ToArray()
                .Where(x => !role.BuggedPlayers.Contains(x.PlayerId))
                .ToList();

            Utils.SetTarget(ref role.ClosestPlayer, bugButton, float.NaN, notBugged);

            var renderer = bugButton.graphic;

            if (role.ClosestPlayer != null && role.ButtonUsable)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}
