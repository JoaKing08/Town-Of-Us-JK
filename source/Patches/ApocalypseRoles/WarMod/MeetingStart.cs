using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;
using Reactor.Utilities.Extensions;
using UnityEngine;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.WarMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            foreach (War role in Role.GetRoles(RoleEnum.War))
            {
                if (!role.Announced && CustomGameOptions.AnnounceWar)
                {
                    foreach (var player in PlayerControl.AllPlayerControls) if (DestroyableSingleton<HudManager>.Instance) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(player, $"The <b><color=#{Patches.Colors.Berserker.ToHtmlStringRGBA()}>Berserker</color></b> has transformed into <b><color=#{Patches.Colors.War.ToHtmlStringRGBA()}>War</color></b>, <b>Horseman of the Apocalypse</b>! Cry <b>'Havoc!'</b>, and let slip the dogs of war.");
                    role.Announced = true;
                }
            }
        }
    }
}
