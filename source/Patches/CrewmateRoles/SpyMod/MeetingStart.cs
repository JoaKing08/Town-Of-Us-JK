using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.SpyMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Spy)) return;
            var spyRole = Role.GetRole<Spy>(PlayerControl.LocalPlayer);
            if (spyRole.Messages.Count == 0) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "No players triggered any of your bugs");
            else
            {
                foreach (var message in spyRole.Messages)
                {
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
                }
            }
            spyRole.Messages = new List<string>();
        }
    }
}
