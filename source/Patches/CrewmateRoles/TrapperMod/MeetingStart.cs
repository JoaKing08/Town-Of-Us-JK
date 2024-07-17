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
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, Patches.TranslationPatches.CurrentLanguage == 0 ? "<b>No</b> players entered any of your traps." : "<b>Nikt</b> nie wszedl w zadne twoje pulapki.");
            }
            else if (trapperRole.trappedPlayers.Count < CustomGameOptions.MinAmountOfPlayersInTrap)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, Patches.TranslationPatches.CurrentLanguage == 0 ? "<b>Not enough</b> players triggered your traps." : "<b>Niewystarczajaco osób</b> weszlo w twoje pulapki.");
            }
            else
            {
                string message = Patches.TranslationPatches.CurrentLanguage == 0 ? "Roles caught in your trap:\n" : "Role zlapane w pulapce:\n";
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
