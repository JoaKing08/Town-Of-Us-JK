using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.FamineMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            foreach (Famine role in Role.GetRoles(RoleEnum.Famine))
            {
                if (!role.Announced && CustomGameOptions.AnnounceFamine)
                {
                    if (DestroyableSingleton<HudManager>.Instance) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, Patches.TranslationPatches.CurrentLanguage == 0 ? $"The <b><color=#{Patches.Colors.Baker.ToHtmlStringRGBA()}>Baker</color></b> has transformed into <b><color=#{Patches.Colors.Famine.ToHtmlStringRGBA()}>Famine</color></b>, <b>Horseman of the Apocalypse</b>! A <b>Famine</b> has begun!" : $"<b><color=#{Patches.Colors.Baker.ToHtmlStringRGBA()}>Baker</color></b> przemienil sie w <b><color=#{Patches.Colors.Famine.ToHtmlStringRGBA()}>Famine</color></b>, <b>Jezdzce Apokalipsy</b>! <b>Wielki Glód</b> sie rozpoczal!");
                    role.Announced = true;
                }
            }
        }
    }
}
