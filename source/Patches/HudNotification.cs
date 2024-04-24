using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudNotification
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            var role = Role.GetRole(PlayerControl.LocalPlayer);
            if (role == null) return;

            if (role.NotificationText == null)
            {
                role.NotificationText = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.transform);
                role.NotificationText.transform.localPosition = new Vector3(role.NotificationText.transform.localPosition.x, role.NotificationText.transform.localPosition.y + 2f, role.NotificationText.transform.localPosition.z);
                role.NotificationText.transform.localScale = new Vector3(role.NotificationText.transform.localScale.x * 0.75f, role.NotificationText.transform.localScale.y * 0.75f, role.NotificationText.transform.localScale.z);
                role.NotificationText.enableWordWrapping = false;
                role.NotificationText.alignment = TMPro.TextAlignmentOptions.Center;
                role.NotificationText.fontStyle = TMPro.FontStyles.Normal;
            }
            if (role.NotificationText != null)
            {
                role.NotificationText.gameObject.SetActive(true);
                if (role.NotificationEnds > System.DateTime.UtcNow)
                {
                    role.NotificationText.text = role.NotificationString;
                    PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Setting Notification");
                }
                else
                {
                    role.NotificationText.text = "";
                    PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Erasing");
                }
            }
        }
    }
}