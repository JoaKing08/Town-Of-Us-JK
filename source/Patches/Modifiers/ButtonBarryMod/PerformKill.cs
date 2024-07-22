using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.Modifiers.ButtonBarryMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(ModifierEnum.ButtonBarry)) return true;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)) return true;

            var role = Modifier.GetModifier<ButtonBarry>(PlayerControl.LocalPlayer);
            if (__instance != role.ButtonButton) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (role.ButtonUsed) return false;
            if (role.StartTimer() > 0) return false;
            if (PlayerControl.LocalPlayer.RemainingEmergencies <= 0) return false;
            if (Role.GetRole(PlayerControl.LocalPlayer).Roleblocked)
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.white));
                NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                return false;
            }
            if (!__instance.enabled) return false;

            role.ButtonUsed = true;

            Utils.Rpc(CustomRPC.BarryButton, PlayerControl.LocalPlayer.PlayerId);

            if (AmongUsClient.Instance.AmHost)
            {
                MeetingRoomManager.Instance.reporter = PlayerControl.LocalPlayer;
                MeetingRoomManager.Instance.target = null;
                AmongUsClient.Instance.DisconnectHandlers.AddUnique(
                    MeetingRoomManager.Instance.Cast<IDisconnectHandler>());
                if (GameManager.Instance.CheckTaskCompletion()) return false;
                DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(PlayerControl.LocalPlayer);
                PlayerControl.LocalPlayer.RpcStartMeeting(null);
            }

            return false;
        }
    }
}