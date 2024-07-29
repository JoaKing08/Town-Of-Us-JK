using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.BerserkerMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Berserker)) return;
            var role = Role.GetRole<Berserker>(PlayerControl.LocalPlayer);
            if (DestroyableSingleton<HudManager>.Instance && CustomGameOptions.KillsToWar > role.KilledPlayers)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{CustomGameOptions.KillsToWar - role.KilledPlayers}</b> more players to kill remaining." : $"Pozostalo <b>{CustomGameOptions.KillsToWar - role.KilledPlayers}</b> graczy do zabicia.");
                if (!Utils.UndercoverIsApocalypse()) Utils.Rpc(CustomRPC.SendChatInfo, (byte)RoleEnum.Berserker, role.Player.PlayerId, (byte)(CustomGameOptions.KillsToWar - role.KilledPlayers));
            }
        }
    }
}
