using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.MysticMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)) return;
            if (!CustomGameOptions.AllowVision) return;
            var mysticRole = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);
            if (mysticRole.VisionPlayer == byte.MaxValue) return;
            if (!mysticRole.PlayersInteracted.Any() && !mysticRole.InteractingPlayers.Any()) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, Patches.TranslationPatches.CurrentLanguage == 0 ? ("Your vision was about darkness... <b>No one</b> interacted nor was interacted by <b>" + Utils.PlayerById(mysticRole.VisionPlayer).GetDefaultOutfit().PlayerName + "</b>.") : ("Twoja wizja dotyczyla ciemnosci... <b>Nikt</b> nie interaktowal ani nie byl interaktowany przez <b>" + Utils.PlayerById(mysticRole.VisionPlayer).GetDefaultOutfit().PlayerName + "</b>."));
            else
            {
                var message = "";
                if (!mysticRole.PlayersInteracted.Any())
                {
                    message += Patches.TranslationPatches.CurrentLanguage == 0 ? ("Your vision was about few ants in different colors picking sugar cube... <b>" + Utils.PlayerById(mysticRole.VisionPlayer).GetDefaultOutfit().PlayerName + "</b> <b>wasn't interacting</b> and <b>was interacted by</b>: \n") : ("Twoja wizja dotyczyla kilku mrówek w róznych kolorach zbierajacych kostki cukru... <b>" + Utils.PlayerById(mysticRole.VisionPlayer).GetDefaultOutfit().PlayerName + "</b> <b>nie interaktowal</b> i <b>byl interaktowany przez</b>: \n");
                    foreach (var player in mysticRole.InteractingPlayers.OrderBy(x => Guid.NewGuid())) message += $"<b>{Utils.PlayerById(player).Data.ColorName.RemoveAll(new char[] { '(', ')' })}</b>, ";
                    message = message.Remove(message.Length - 2);
                }
                else if (!mysticRole.InteractingPlayers.Any())
                {
                    message += Patches.TranslationPatches.CurrentLanguage == 0 ? ("Your vision was about flytrap catching few colored butterflies... <b>" + Utils.PlayerById(mysticRole.VisionPlayer).GetDefaultOutfit().PlayerName + "</b> <b>wasn't interacted</b> and <b>interacted with</b>: \n") : ("Twoja wizja dotyczyla mucholówki lapiacej kilka kolorowych motyli... <b>" + Utils.PlayerById(mysticRole.VisionPlayer).GetDefaultOutfit().PlayerName + "</b> <b>nie byl interaktowany</b> i <b>interaktowal z</b>: \n");
                    foreach (var player in mysticRole.PlayersInteracted.OrderBy(x => Guid.NewGuid())) message += $"<b>{Utils.PlayerById(player).Data.ColorName.RemoveAll(new char[] { '(', ')' })}</b>, ";
                    message = message.Remove(message.Length - 2);
                }
                else
                {
                    message += Patches.TranslationPatches.CurrentLanguage == 0 ? ("Your vision was about few colored rats fighting with cat... <b>" + Utils.PlayerById(mysticRole.VisionPlayer).GetDefaultOutfit().PlayerName + "</b> <b>was interacting with</b> or <b>was interacted by</b>: \n") : ("Twoja wizja dotyczyla kilku kolorowych szczurów walczacych z kotem... <b>" + Utils.PlayerById(mysticRole.VisionPlayer).GetDefaultOutfit().PlayerName + "</b> <b>interaktowal</b> lub <b>byl interaktowany przez</b>: \n");
                    var players = new List<byte>();
                    players.AddRange(mysticRole.PlayersInteracted);
                    players.AddRange(mysticRole.InteractingPlayers);
                    foreach (var player in players.OrderBy(x => Guid.NewGuid())) message += $"<b>{Utils.PlayerById(player).Data.ColorName.RemoveAll(new char[] { '(', ')' })}</b>, ";
                    message = message.Remove(message.Length - 2);
                }
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, message);
            }
            mysticRole.InteractingPlayers = new List<byte>();
            mysticRole.PlayersInteracted = new List<byte>();
            mysticRole.VisionPlayer = byte.MaxValue;
        }
    }
}
