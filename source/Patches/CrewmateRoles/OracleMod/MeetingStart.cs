using HarmonyLib;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.OracleMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStartOracle
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Oracle)) return;
            var oracleRole = Role.GetRole<Oracle>(PlayerControl.LocalPlayer);
            if (oracleRole.Confessor != null)
            {
                var playerResults = PlayerReportFeedback(oracleRole.Confessor);

                if (!string.IsNullOrWhiteSpace(playerResults)) DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, playerResults);
            }
        }

        public static string PlayerReportFeedback(PlayerControl player)
        {
            if (player.Data.IsDead || player.Data.Disconnected) return Patches.TranslationPatches.CurrentLanguage == 0 ? "Your confessor <b>failed to survive</b> so you received no confession" : "Spowiadajacy <b>nie prztrwal</b> wiec nie dostajesz spowiedzi.";
            var allPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x != PlayerControl.LocalPlayer && x != player).ToList();
            if (allPlayers.Count < 2) return Patches.TranslationPatches.CurrentLanguage == 0 ? "Too <b>few people alive</b> to receive a confessional" : "Za <b>malo zyjacych ludzi</b> by uzyskac spowiedz.";
            var evilPlayers = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected &&
            (x.Is(Faction.Impostors) || (x.Is(Faction.NeutralKilling) && !x.Is(RoleEnum.JKNecromancer) && !x.Is(RoleEnum.Jackal) && !x.Is(RoleEnum.Vampire) &&
            CustomGameOptions.NeutralKillingShowsEvil) || (x.Is(Faction.NeutralEvil) && CustomGameOptions.NeutralEvilShowsEvil) || (x.Is(Faction.NeutralBenign) &&
            CustomGameOptions.NeutralBenignShowsEvil) || (x.Is(Faction.NeutralChaos) && CustomGameOptions.NeutralChaosShowsEvil) || (x.Is(Faction.NeutralApocalypse) &&
            CustomGameOptions.NeutralApocalypseShowsEvil) || ((x.Is(RoleEnum.JKNecromancer) || x.Is(RoleEnum.Jackal) || x.Is(RoleEnum.Vampire)) && CustomGameOptions.NeutralProselyteShowsEvil))).ToList();
            if (evilPlayers.Count == 0) return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> confesses to knowing that there are <b>no more evil players</b>!" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> spowiada sie ze <b>nie ma juz wiecej zlych graczy</b>!"; 
            allPlayers.Shuffle();
            evilPlayers.Shuffle();
            var secondPlayer = allPlayers[0];
            var firstTwoEvil = false;
            foreach (var evilPlayer in evilPlayers)
            {
                if (evilPlayer == player || evilPlayer == secondPlayer) firstTwoEvil = true;
            }
            PlayerControl thirdPlayer;
            if (firstTwoEvil)
            {
                thirdPlayer = allPlayers[1];
            }
            else
            {
                thirdPlayer = evilPlayers[0];
            }
            return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> confesses to knowing that <b>they</b>, <b>{secondPlayer.GetDefaultOutfit().PlayerName}</b> and/or <b>{thirdPlayer.GetDefaultOutfit().PlayerName}</b> is evil!" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> spowiada sie ze <b>on</b>, <b>{secondPlayer.GetDefaultOutfit().PlayerName}</b> i/lub <b>{thirdPlayer.GetDefaultOutfit().PlayerName}</b> sa zli!";
        }
    }
}