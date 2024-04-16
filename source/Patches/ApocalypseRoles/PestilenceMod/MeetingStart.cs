using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.NeutralRoles.PestilenceMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            foreach (Pestilence role in Role.GetRoles(RoleEnum.Pestilence))
            {
                if (!role.Announced && CustomGameOptions.AnnouncePestilence)
                {
                    foreach (var player in PlayerControl.AllPlayerControls) if (DestroyableSingleton<HudManager>.Instance) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, $"A <color=#0000FFFF>Plague</color> has consumed the Crew, transforming the <color=#{Patches.Colors.Plaguebearer.ToHtmlStringRGBA()}>Plaguebearer</color> into <color=#{Patches.Colors.Pestilence.ToHtmlStringRGBA()}>Pestilence</color>, Horseman of the Apocalypse!");
                    role.Announced = true;
                }
            }
        }
    }
}
