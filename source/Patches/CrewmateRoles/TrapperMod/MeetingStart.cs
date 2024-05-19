using HarmonyLib;
using Reactor.Utilities.Extensions;
using System;
using System.Linq;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.TrapperMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Trapper)) return;
            var trapperRole = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);
            if (trapperRole.trappedPlayers.Count == 0)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "<b>No</b> players entered any of your traps");
            }
            else if (trapperRole.trappedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInTrap)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, "<b>Not enough</b> players triggered your traps");
            }
            else
            {
                string message = "Roles caught in your trap:\n";
                foreach (RoleEnum role in trapperRole.trappedPlayers.OrderBy(x => Guid.NewGuid()))
                {
                    message += $" <b><color=#{role.GetRoleColor().ToHtmlStringRGBA()}>{role.GetRoleName()}</color></b>,";
                }
                message.Remove(message.Length - 1, 1);
                if (DestroyableSingleton<HudManager>.Instance)
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
            }
        }
    }
}
