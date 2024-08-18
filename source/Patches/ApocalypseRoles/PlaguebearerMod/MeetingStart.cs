using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.NeutralRoles.PlaguebearerMod
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
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer)) return;
            var role = Role.GetRole<Plaguebearer>(PlayerControl.LocalPlayer);
            var players = PlayerControl.AllPlayerControls.ToArray().Count(x => x != null && !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.NeutralApocalypse) && !x.Is(ObjectiveEnum.ApocalypseAgent));
            if (DestroyableSingleton<HudManager>.Instance && players > role.InfectedAlive)
            {
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{players - role.InfectedAlive - (Role.GetRoles(RoleEnum.Harbinger).Any(x => ((Harbinger)x).CompletedTasks && !((Harbinger)x).Caught && !x.Player.Data.Disconnected) ? CustomGameOptions.HarbingerPlaguebearerBonus : 0)}</b> more players to infect remaining." : $"Pozostalo <b>{players - role.InfectedAlive - (Role.GetRoles(RoleEnum.Harbinger).Any(x => ((Harbinger)x).CompletedTasks && !((Harbinger)x).Caught && !x.Player.Data.Disconnected) ? CustomGameOptions.HarbingerPlaguebearerBonus : 0)}</b> niezainfekowanych graczy.");
                if (!Utils.UndercoverIsApocalypse()) Utils.Rpc(CustomRPC.SendChatInfo, (byte)RoleEnum.Plaguebearer, role.Player.PlayerId, (byte)(players - role.InfectedAlive - (Role.GetRoles(RoleEnum.Harbinger).Any(x => ((Harbinger)x).CompletedTasks && !((Harbinger)x).Caught && !x.Player.Data.Disconnected) ? CustomGameOptions.HarbingerPlaguebearerBonus : 0)), role.InfectedPlayers.Where(x => Utils.PlayerById(x) != null && !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected).ToList());
            }
        }
    }
}
