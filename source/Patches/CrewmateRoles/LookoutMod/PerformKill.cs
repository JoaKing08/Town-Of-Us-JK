using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.CrewmateRoles.LookoutMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Lookout);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Lookout>(PlayerControl.LocalPlayer);
            var watchButton = DestroyableSingleton<HudManager>.Instance.KillButton;
            if (__instance == watchButton)
            {
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.WatchTimer() != 0) return false;
                if (Role.GetRole(PlayerControl.LocalPlayer).Roleblocked)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
                    role.Notification("You Are Roleblocked!", 1000 * CustomGameOptions.NotificationDuration);
                    return false;
                }
                role.TimeRemaining = CustomGameOptions.WatchDuration;
                role.StartWatch();
                return false;
            }

            return true;
        }
    }
}