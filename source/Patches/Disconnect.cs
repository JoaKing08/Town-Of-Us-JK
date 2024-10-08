using HarmonyLib;
using System.Linq;
using UnityEngine;
using TownOfUs.ImpostorRoles.TraitorMod;
using TownOfUs.Roles;
using TownOfUs.CrewmateRoles.AltruistMod;
using TownOfUs.NeutralRoles.PirateMod;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(GameData))]
    public class DisconnectHandler
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(GameData.HandleDisconnect), typeof(PlayerControl), typeof(DisconnectReasons))]
        public static void Prefix([HarmonyArgument(0)] PlayerControl player)
        {
            if (CustomGameOptions.GameMode == GameMode.Cultist)
            {
                if (player.Is(RoleEnum.Necromancer) || player.Is(RoleEnum.Whisperer))
                {
                    foreach (var player2 in PlayerControl.AllPlayerControls)
                    {
                        if (player2.Is(Faction.Impostors)) Utils.MurderPlayer(player2, player2, true);
                    }
                }
            }
            else
            {
                /*if (player.IsLover() && CustomGameOptions.BothLoversDie)
                {
                    var otherLover = Modifier.GetModifier<Lover>(player).OtherLover;
                    if (!otherLover.Is(RoleEnum.Pestilence) && !otherLover.Data.IsDead
                         && !otherLover.Data.Disconnected) MurderPlayer(otherLover, otherLover, true);
                    if (otherLover.Is(RoleEnum.Sheriff))
                    {
                        var sheriff = Role.GetRole<Sheriff>(otherLover);
                        sheriff.IncorrectKills -= 1;
                    }
                }*/
                if (player.Is(RoleEnum.JKNecromancer))
                {
                    foreach (var player2 in PlayerControl.AllPlayerControls)
                    {
                        if (player2.Is(FactionOverride.Undead)) Utils.MurderPlayer(player2, player2, true);
                    }
                }
                if (Role.GetRoles(RoleEnum.Inquisitor).Any(x => !x.Player.Data.IsDead && !x.Player.Data.Disconnected)) foreach (Inquisitor inq in Role.GetRoles(RoleEnum.Inquisitor).ToArray().Where(x => !x.Player.Data.IsDead && !x.Player.Data.Disconnected))
                    {
                        if (!inq.heretics.ToArray().Any(x => Utils.PlayerById(x) != null && !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected))
                        {
                            inq.Wins();
                            if (!CustomGameOptions.NeutralEvilWinEndsGame)
                            {
                                KillButtonTarget.DontRevive = inq.Player.PlayerId;
                                inq.Player.Exiled();
                            }
                        }
                    }
                if (AmongUsClient.Instance.AmHost)
                {
                    if (player == SetTraitor.WillBeTraitor)
                    {
                        var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(RoleEnum.Mayor) &&
                            !x.Is(ObjectiveEnum.Lover) && !x.Data.IsDead && !x.Data.Disconnected && !x.IsExeTarget() && x.Is(FactionOverride.None) &&
                            !x.Is(ObjectiveEnum.ApocalypseAgent) && !(x.Is(RoleEnum.Deputy) && CustomGameOptions.RevealDeputy) && !(x.Is(RoleEnum.Prosecutor) &&
                            CustomGameOptions.RevealProsecutor)).ToList();
                        if (toChooseFrom.Count == 0) return;
                        var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];

                        SetTraitor.WillBeTraitor = pc;

                        Utils.Rpc(CustomRPC.SetTraitor, pc.PlayerId);
                    }
                }

                if (player.Is(RoleEnum.Pirate) || player.IsDueled())
                {
                    ShowHideButtonsPirate.HideButtons();
                }
            }
        }
    }
}