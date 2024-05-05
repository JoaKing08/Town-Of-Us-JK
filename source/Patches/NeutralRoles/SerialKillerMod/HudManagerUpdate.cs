using HarmonyLib;
using Reactor.Utilities;
using System;
using System.Linq;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.SerialKillerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller)) return;
            var role = Role.GetRole<SerialKiller>(PlayerControl.LocalPlayer);

            if (role.KillsText == null && role.KillsLeft > 0)
            {
                role.KillsText = UnityEngine.Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
                role.KillsText.gameObject.SetActive(false);
                role.KillsText.transform.localPosition = new Vector3(
                    role.KillsText.transform.localPosition.x + 0.26f,
                    role.KillsText.transform.localPosition.y + 0.29f,
                    role.KillsText.transform.localPosition.z);
                role.KillsText.transform.localScale = role.KillsText.transform.localScale * 0.65f;
                role.KillsText.alignment = TMPro.TextAlignmentOptions.Right;
                role.KillsText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.KillsText != null)
            {
                role.KillsText.text = role.KillsLeft + "";
            }
            if (role.BloodlustTimer() == 0 && role.InBloodlust)
            {
                role.InBloodlust = false;
                Coroutines.Start(Utils.FlashCoroutine(Color.white));
                role.Notification("Your Bloodlust Is Sated!", 1000 * CustomGameOptions.NotificationDuration);
            }
            if (role.SKKills >= CustomGameOptions.KillsToBloodlust)
            {
                role.BloodlustStart = DateTime.UtcNow;
                role.InBloodlust = true;
                role.SKKills -= CustomGameOptions.KillsToBloodlust;
                Coroutines.Start(Utils.FlashCoroutine(Color.red));
                role.Notification("<color=#FF0000FF>BLOODLUST!</color>", 1000 * CustomGameOptions.NotificationDuration);
            }

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.KillsText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            //if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)role.KillTimer(), (byte)(role.InBloodlust ? CustomGameOptions.BloodlustCooldown : CustomGameOptions.SerialKillerCooldown));
            __instance.KillButton.SetCoolDown(role.KillTimer(), role.InBloodlust ? CustomGameOptions.BloodlustCooldown : CustomGameOptions.SerialKillerCooldown);

            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !(role.FactionOverride == FactionOverride.Undead && x.Is(FactionOverride.Undead)) && !(role.FactionOverride == FactionOverride.Recruit && x.Is(FactionOverride.Recruit) && !(x.Is(RoleEnum.Jackal) && !CustomGameOptions.RecruistSeeJackal))).ToList());
        }
    }
}