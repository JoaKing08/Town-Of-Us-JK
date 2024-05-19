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
                    foreach (var player in PlayerControl.AllPlayerControls) if (DestroyableSingleton<HudManager>.Instance) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, $"The <b><color=#{Patches.Colors.Baker.ToHtmlStringRGBA()}>Baker</color></b> has transformed into <b><color=#{Patches.Colors.Famine.ToHtmlStringRGBA()}>Famine</color></b>, <b>Horseman of the Apocalypse</b>! A <b>Famine</b> has begun!");
                    role.Announced = true;
                }
            }
        }
    }
}
