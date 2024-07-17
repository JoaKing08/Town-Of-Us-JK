using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.Modifiers.DisperserMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.Disperser)) return true;

            var role = Modifier.GetModifier<Disperser>(PlayerControl.LocalPlayer);
            if (__instance != role.DisperseButton) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (role.ButtonUsed) return false;
            if (role.StartTimer() > 0) return false;
            if (Role.GetRole(PlayerControl.LocalPlayer).Roleblocked)
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.white));
                Role.GetRole(PlayerControl.LocalPlayer).Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                return false;
            }
            if (!__instance.enabled) return false;

            role.ButtonUsed = true;

            role.Disperse();

            return false;
        }
    }
}