using System;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;
using UnityEngine;
using AmongUs.GameOptions;
using TownOfUs.Patches;

namespace TownOfUs.CultistRoles.SeerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.CultistSeer);
            if (!flag) return true;
            var role = Role.GetRole<CultistSeer>(PlayerControl.LocalPlayer);
            if (!PlayerControl.LocalPlayer.CanMove || role.ClosestPlayer == null) return false;
            var flag2 = role.SeerTimer() == 0f;
            if (!flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(),
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            if (role.ClosestPlayer == null) return false;
            if (role.ClosestPlayer.Is(Faction.Impostors) && !role.ClosestPlayer.Is(RoleEnum.Necromancer)
                && !role.ClosestPlayer.Is(RoleEnum.Whisperer))
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.red));
                NotificationPatch.Notification(TranslationPatches.CurrentLanguage == 0 ? "Your Target Was Converted!" : "Twój Cel Zostal Przekonwrtowany!", 1000 * CustomGameOptions.NotificationDuration);
            }
            else
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.green));
                NotificationPatch.Notification(TranslationPatches.CurrentLanguage == 0 ? "Your Target Wasn't Converted!" : "Twój Cel Nie Zostal Przekonwertowany!", 1000 * CustomGameOptions.NotificationDuration);
            }
            role.LastInvestigated = DateTime.UtcNow;
            role.UsesLeft--;
            return false;
        }
    }
}
