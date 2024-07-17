using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.MedicMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class StopKill
    {
        public static void BreakShield(byte medicId, byte playerId, bool flag)
        {
            if (PlayerControl.LocalPlayer.PlayerId == playerId &&
                CustomGameOptions.NotificationShield == NotificationOptions.Shielded)
            {
                Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.5f, 0f, 1f)));
                Role.GetRole(PlayerControl.LocalPlayer).Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Were Attacked!" : "Zostales Zaatakowany!", 1000 * CustomGameOptions.NotificationDuration);
            }

            if (PlayerControl.LocalPlayer.PlayerId == medicId &&
                CustomGameOptions.NotificationShield == NotificationOptions.Medic)
            {
                Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.5f, 0f, 1f)));
                Role.GetRole(PlayerControl.LocalPlayer).Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Shielded Player Was Attacked!" : "Chroniony Gracz Zostal Zaatakowany!", 1000 * CustomGameOptions.NotificationDuration);
            }

            if (CustomGameOptions.NotificationShield == NotificationOptions.Everyone)
            {
                Coroutines.Start(Utils.FlashCoroutine(new Color(0f, 0.5f, 0f, 1f)));
                if (PlayerControl.LocalPlayer.PlayerId == playerId) Role.GetRole(PlayerControl.LocalPlayer).Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Were Attacked!" : "Zostales Zaatakowany!", 1000 * CustomGameOptions.NotificationDuration);
                else Role.GetRole(PlayerControl.LocalPlayer).Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Shielded Player Was Attacked!" : "Chroniony Gracz Zostal Zaatakowany!", 1000 * CustomGameOptions.NotificationDuration);
            }

            if (!flag)
                return;

            var player = Utils.PlayerById(playerId);
            foreach (var role in Role.GetRoles(RoleEnum.Medic))
                if (((Medic) role).ShieldedPlayer.PlayerId == playerId)
                {
                    ((Medic) role).ShieldedPlayer = null;
                    ((Medic) role).exShielded = player;
                    System.Console.WriteLine(player.name + " Is Ex-Shielded");
                }

            player.myRend().material.SetColor("_VisorColor", Palette.VisorColor);
            player.myRend().material.SetFloat("_Outline", 0f);
        }
    }
}