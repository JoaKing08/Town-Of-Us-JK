using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.DeathMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            foreach (Death role in Role.GetRoles(RoleEnum.Death))
            {
                if (!role.Announced && CustomGameOptions.AnnounceDeath)
                {
                    foreach (var player in PlayerControl.AllPlayerControls) if (DestroyableSingleton<HudManager>.Instance) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, $"Now <color=#{Patches.Colors.SoulCollector.ToHtmlStringRGBA()}>Soul Collector</color> has become <color=#{Patches.Colors.Death.ToHtmlStringRGBA()}>Death</color>, Destroyer of Worlds and Horseman of the Apocalypse!");
                    role.Announced = true;
                }
            }
        }
    }
}
