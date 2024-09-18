using HarmonyLib;
using Reactor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(HudManager))]
    public class NotificationPatch
    {
        public static TextMeshPro NotificationText;
        public static DateTime NotificationEnds = DateTime.MinValue;
        public static string NotificationString = "";
        public static List<(DateTime Key, (string notiftext, double notifmillis, Color coroutcolor, float coroutduration, float coroutalpha) Value)> FutureNotifications = new();

        public static void Notification(string text, double milliseconds)
        {
            NotificationString = text;
            NotificationEnds = DateTime.UtcNow;
            NotificationEnds = NotificationEnds.AddMilliseconds(milliseconds);
        }
        public static void DelayNotification(float delay, string notifText, double notifMillis, Color coroutColor, float coroutDuration = 1f, float coroutAlpha = 0.3f)
        {
            FutureNotifications.Add((DateTime.UtcNow.AddMilliseconds(delay), (notifText, notifMillis, coroutColor, coroutDuration, coroutAlpha)));
        }

        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (FutureNotifications.Any(x => x.Key <= DateTime.UtcNow))
            {
                List<(DateTime Key, (string notiftext, double notifmillis, Color coroutcolor, float coroutduration, float coroutalpha) Value)> toRemove = new();
                foreach (var notification in FutureNotifications.Where(x => x.Key <= DateTime.UtcNow))
                {
                    Notification(notification.Value.notiftext, notification.Value.notifmillis);
                    Coroutines.Start(Utils.FlashCoroutine(notification.Value.coroutcolor, notification.Value.coroutduration, notification.Value.coroutalpha));
                    toRemove.Add(notification);
                }
                FutureNotifications.RemoveAll(x => toRemove.Contains(x));
            }
            if (NotificationText == null)
            {
                NotificationText = GameObject.Instantiate(__instance.KillButton.cooldownTimerText, __instance.transform);
                NotificationText.transform.localPosition = new Vector3(NotificationText.transform.localPosition.x, NotificationText.transform.localPosition.y + 1.5f, NotificationText.transform.localPosition.z - 100f);
                NotificationText.transform.localScale = new Vector3(NotificationText.transform.localScale.x * 0.75f, NotificationText.transform.localScale.y * 0.75f, NotificationText.transform.localScale.z);
                NotificationText.enableWordWrapping = false;
                NotificationText.alignment = TMPro.TextAlignmentOptions.Center;
                NotificationText.fontStyle = TMPro.FontStyles.Normal;
            }
            if (NotificationText != null)
            {
                NotificationText.gameObject.SetActive(true);
                if (NotificationEnds > System.DateTime.UtcNow)
                {
                    NotificationText.text = NotificationString;
                }
                else
                {
                    NotificationText.text = "";
                }
            }
        }
    }
}