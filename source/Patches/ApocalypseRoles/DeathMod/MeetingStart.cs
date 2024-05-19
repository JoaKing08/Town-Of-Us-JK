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
                    foreach (var player in PlayerControl.AllPlayerControls) if (DestroyableSingleton<HudManager>.Instance) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, $"Now <b><color=#{Patches.Colors.SoulCollector.ToHtmlStringRGBA()}>Soul Collector</color></b> has become <b><color=#{Patches.Colors.Death.ToHtmlStringRGBA()}>Death</color></b>, <b>Destroyer of Worlds</b> and <b>Horseman of the Apocalypse</b>!");
                    role.Announced = true;
                }
            }
        }
    }
}
