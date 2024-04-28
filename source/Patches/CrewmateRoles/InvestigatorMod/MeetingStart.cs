using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.InvestigatorMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) return;
            var investigatorRole = Role.GetRole<Investigator>(PlayerControl.LocalPlayer);
            foreach (var message in investigatorRole.Messages)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
            }
            investigatorRole.Messages = new List<string>();
        }
    }
}
