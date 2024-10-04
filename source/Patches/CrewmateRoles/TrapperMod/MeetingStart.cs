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
                message.Replace(Utils.DecryptString("gDoTEQovBOnS0E5ZqluIjA== 4475537506981217 3661701197368895"), Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("TvkSazKro4IyMYQknLDebw== 9604894916391860 7994660516358236") : Utils.DecryptString("nQyyQxDcO0U5STyg6iJv8g== 3236511562209920 9365460461510014"));
                message.Replace(Utils.DecryptString("0iGJxS2QFcgenHqg128Uhg== 2389311640881935 0029222437659448"), Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("TvkSazKro4IyMYQknLDebw== 9604894916391860 7994660516358236") : Utils.DecryptString("nQyyQxDcO0U5STyg6iJv8g== 3236511562209920 9365460461510014"));
                message.Replace(Utils.DecryptString("xsqe2t6rRBcxwOYmC1ypCg== 4163417005018998 3193203997118263"), Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("TvkSazKro4IyMYQknLDebw== 9604894916391860 7994660516358236") : Utils.DecryptString("nQyyQxDcO0U5STyg6iJv8g== 3236511562209920 9365460461510014"));
                message.Replace(Utils.DecryptString("Woz/RTT/+rpdlRn1TrzhnA== 1300172154123972 6877139374517782"), Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("TvkSazKro4IyMYQknLDebw== 9604894916391860 7994660516358236") : Utils.DecryptString("nQyyQxDcO0U5STyg6iJv8g== 3236511562209920 9365460461510014"));
                message.Replace(Utils.DecryptString("82z+k4qRCCDNgJxJqwILvw== 3536356761964177 0097990396288092"), Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("TvkSazKro4IyMYQknLDebw== 9604894916391860 7994660516358236") : Utils.DecryptString("nQyyQxDcO0U5STyg6iJv8g== 3236511562209920 9365460461510014"));
                message.Replace(Utils.DecryptString("z3j9lc0kKmIzVsgoCZ3AqQ== 4633810250565163 5178813482292058"), Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("TvkSazKro4IyMYQknLDebw== 9604894916391860 7994660516358236") : Utils.DecryptString("nQyyQxDcO0U5STyg6iJv8g== 3236511562209920 9365460461510014"));
                message.Replace(Utils.DecryptString("gqBubeh33CAFgVH1wDDhbw== 4524291940462625 9516809233515129"), Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("TvkSazKro4IyMYQknLDebw== 9604894916391860 7994660516358236") : Utils.DecryptString("nQyyQxDcO0U5STyg6iJv8g== 3236511562209920 9365460461510014"));
                if (DestroyableSingleton<HudManager>.Instance)
                    DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
            }
        }
    }
}
