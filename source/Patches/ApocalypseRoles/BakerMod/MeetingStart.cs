using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.BakerMod
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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Baker)) return;
            var role = Role.GetRole<Baker>(PlayerControl.LocalPlayer);
            if (DestroyableSingleton<HudManager>.Instance && CustomGameOptions.BreadNeeded > role.BreadAlive)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{CustomGameOptions.BreadNeeded - role.BreadAlive - (Role.GetRoles(RoleEnum.Harbinger).Any(x => ((Harbinger)x).CompletedTasks && !((Harbinger)x).Caught && !x.Player.Data.Disconnected) ? CustomGameOptions.HarbingerBakerBonus : 0)}</b> more players to feed remaining." : $"Pozostalo <b>{CustomGameOptions.BreadNeeded - role.BreadAlive - (Role.GetRoles(RoleEnum.Harbinger).Any(x => ((Harbinger)x).CompletedTasks && !((Harbinger)x).Caught && !x.Player.Data.Disconnected) ? CustomGameOptions.HarbingerBakerBonus : 0)}</b> graczy do nakarmienia.");
                if (!Utils.UndercoverIsApocalypse()) Utils.Rpc(CustomRPC.SendChatInfo, (byte)RoleEnum.Baker, role.Player.PlayerId, (byte)(CustomGameOptions.BreadNeeded - role.BreadAlive - (Role.GetRoles(RoleEnum.Harbinger).Any(x => ((Harbinger)x).CompletedTasks && !((Harbinger)x).Caught && !x.Player.Data.Disconnected) ? CustomGameOptions.HarbingerBakerBonus : 0)), role.BreadPlayers.Where(x => Utils.PlayerById(x) != null && !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected).ToList());
            }
        }
    }
}
