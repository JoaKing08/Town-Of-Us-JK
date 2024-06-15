using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.MysticMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudVision
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            UpdateVisionButton(__instance);
        }

        public static void UpdateVisionButton(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)) return;
            var visionButton = __instance.KillButton;

            var role = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);

            visionButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && CustomGameOptions.AllowVision);
            //if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)(role.ButtonUsable ? role.BugTimer() : 0), (byte)CustomGameOptions.BugCooldown);
            if (!role.UsedAbility) visionButton.SetCoolDown(role.VisionTimer(), CustomGameOptions.VisionCooldown);
            else visionButton.SetCoolDown(0f, CustomGameOptions.BugCooldown);

            Utils.SetTarget(ref role.ClosestPlayer, visionButton);

            var renderer = visionButton.graphic;

            if (role.ClosestPlayer != null && !role.UsedAbility)
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
