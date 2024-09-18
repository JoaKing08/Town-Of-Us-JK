using HarmonyLib;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.DemagogueMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))] // BBFDNCCEJHI
    public static class VotingComplete
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue))
            {
                var demagogue = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
                foreach (var button in demagogue.MeetingKillButtons)
                {
                    button.Value.button.Destroy();
                    button.Value.text.Destroy();
                }
                demagogue.MeetingKillButtons.Clear();
                if (demagogue.Revealed == 1 && demagogue.Player.AmOwner)
                {
                    if (CustomGameOptions.RevealDemagogue)
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                        NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Demagogue Has Been Revealed!" : "Demagogue Zostal Ujawniony!", 1000 * CustomGameOptions.NotificationDuration);
                    }
                    demagogue.Revealed = 2;
                }
            }
        }
    }
}