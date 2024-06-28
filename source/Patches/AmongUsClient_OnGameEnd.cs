using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using TownOfUs.Extensions;
using TownOfUs.Roles.Teams;
using TownOfUs.Roles.Horseman;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(EndGameManager), nameof(EndGameManager.Start))]
    public class EndGameManager_SetEverythingUp
    {
        public static System.Collections.Generic.List<bool> winner;
        public static void Prefix()
        {
            List<int> losers = new List<int>();
            foreach (var role in Role.GetRoles(RoleEnum.Amnesiac))
            {
                var amne = (Amnesiac)role;
                losers.Add(amne.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                losers.Add(ga.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Survivor))
            {
                var surv = (Survivor)role;
                losers.Add(surv.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Doomsayer))
            {
                var doom = (Doomsayer)role;
                losers.Add(doom.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;
                losers.Add(exe.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Jester))
            {
                var jest = (Jester)role;
                losers.Add(jest.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Phantom))
            {
                var phan = (Phantom)role;
                losers.Add(phan.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Pirate))
            {
                var pirate = (Pirate)role;
                losers.Add(pirate.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Arsonist))
            {
                var arso = (Arsonist)role;
                losers.Add(arso.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Juggernaut))
            {
                var jugg = (Juggernaut)role;
                losers.Add(jugg.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Pestilence))
            {
                var pest = (Pestilence)role;
                losers.Add(pest.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Plaguebearer))
            {
                var pb = (Plaguebearer)role;
                losers.Add(pb.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Famine))
            {
                var fam = (Famine)role;
                losers.Add(fam.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Baker))
            {
                var bak = (Baker)role;
                losers.Add(bak.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.War))
            {
                var war = (War)role;
                losers.Add(war.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Berserker))
            {
                var bers = (Berserker)role;
                losers.Add(bers.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Death))
            {
                var dth = (Death)role;
                losers.Add(dth.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.SoulCollector))
            {
                var sc = (SoulCollector)role;
                losers.Add(sc.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Glitch))
            {
                var glitch = (Glitch)role;
                losers.Add(glitch.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Vampire))
            {
                var vamp = (Vampire)role;
                losers.Add(vamp.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Werewolf))
            {
                var ww = (Werewolf)role;
                losers.Add(ww.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.RedMember))
            {
                var rm = (RedMember)role;
                losers.Add(rm.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.BlueMember))
            {
                var bm = (BlueMember)role;
                losers.Add(bm.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.YellowMember))
            {
                var ym = (YellowMember)role;
                losers.Add(ym.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.GreenMember))
            {
                var gm = (GreenMember)role;
                losers.Add(gm.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.SoloKiller))
            {
                var sk = (SoloKiller)role;
                losers.Add(sk.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.SerialKiller))
            {
                var sk = (SerialKiller)role;
                losers.Add(sk.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Inquisitor))
            {
                var inq = (Inquisitor)role;
                losers.Add(inq.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Witch))
            {
                var witch = (Witch)role;
                losers.Add(witch.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var objective in Objective.GetObjectives(ObjectiveEnum.ImpostorAgent))
            {
                var agent = (ImpostorAgent)objective;
                losers.Add(agent.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var objective in Objective.GetObjectives(ObjectiveEnum.ApocalypseAgent))
            {
                var agent = (ApocalypseAgent)objective;
                losers.Add(agent.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Jackal))
            {
                var jackal = (Jackal)role;
                losers.Add(jackal.Player.GetDefaultOutfit().ColorId);
            }
            foreach (var role in Role.GetRoles(RoleEnum.JKNecromancer))
            {
                var necromancer = (Necromancer)role;
                losers.Add(necromancer.Player.GetDefaultOutfit().ColorId);
            }

            var toRemoveWinners = TempData.winners.ToArray().Where(o => losers.Contains(o.ColorId)).ToArray();
            for (int i = 0; i < toRemoveWinners.Count(); i++) TempData.winners.Remove(toRemoveWinners[i]);
            var isImp = TempData.winners.Count != 0 && TempData.winners[0].IsImpostor;
            bool isApoc = false;
            bool isNeut = false;
            bool isCrew = !isImp;
            bool isOther = false;
            winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };

            if (Role.NobodyWins)
            {
                TempData.winners = new List<WinningPlayerData>();
                isImp = false;
                isApoc = false;
                isNeut = false;
                isCrew = false;
                isOther = true;
                winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                foreach (var role in Role.GetRoles(RoleEnum.Witch).ToArray().Where(x => x.FactionOverride == FactionOverride.None))
                {
                    var witch = (Witch)role;
                    if (!witch.Player.Data.IsDead && !witch.Player.Data.Disconnected)
                    {
                        var witchData = new WinningPlayerData(witch.Player.Data);
                        if (PlayerControl.LocalPlayer != witch.Player) witchData.IsYou = false;
                        TempData.winners.Add(witchData);
                    }
                }
                return;
            }
            if (Role.SurvOnlyWins)
            {
                TempData.winners = new List<WinningPlayerData>();
                isImp = false;
                isApoc = false;
                isNeut = false;
                isCrew = false;
                isOther = true;
                winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                foreach (var role in Role.GetRoles(RoleEnum.Survivor).ToArray().Where(x => x.FactionOverride == FactionOverride.None))
                {
                    var surv = (Survivor)role;
                    if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                    {
                        var survData = new WinningPlayerData(surv.Player.Data);
                        if (PlayerControl.LocalPlayer != surv.Player) survData.IsYou = false;
                        TempData.winners.Add(survData);
                    }
                }
                foreach (var role in Role.GetRoles(RoleEnum.Witch).ToArray().Where(x => x.FactionOverride == FactionOverride.None))
                {
                    var witch = (Witch)role;
                    if (!witch.Player.Data.IsDead && !witch.Player.Data.Disconnected)
                    {
                        var witchData = new WinningPlayerData(witch.Player.Data);
                        if (PlayerControl.LocalPlayer != witch.Player) witchData.IsYou = false;
                        TempData.winners.Add(witchData);
                    }
                }

                return;
            }

            if (CustomGameOptions.NeutralEvilWinEndsGame)
            {
                foreach (var role in Role.AllRoles.ToArray().Where(x => x.FactionOverride == FactionOverride.None || x.RoleType == RoleEnum.Phantom))
                {
                    var type = role.RoleType;

                    if (type == RoleEnum.Jester)
                    {
                        var jester = (Jester)role;
                        if (jester.VotedOut)
                        {
                            TempData.winners = new List<WinningPlayerData>();
                            isImp = false;
                            isApoc = false;
                            isNeut = true;
                            isCrew = false;
                            isOther = false;
                            winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                            var jestData = new WinningPlayerData(jester.Player.Data);
                            jestData.IsDead = false;
                            if (PlayerControl.LocalPlayer != jester.Player) jestData.IsYou = false;
                            TempData.winners.Add(jestData);
                            foreach (var r in Role.GetRoles(RoleEnum.Witch).ToArray().Where(x => x.FactionOverride == FactionOverride.None))
                            {
                                var witch = (Witch)r;
                                if (!witch.Player.Data.IsDead && !witch.Player.Data.Disconnected)
                                {
                                    var witchData = new WinningPlayerData(witch.Player.Data);
                                    if (PlayerControl.LocalPlayer != witch.Player) witchData.IsYou = false;
                                    TempData.winners.Add(witchData);
                                }
                            }
                            return;
                        }
                    }
                    else if (type == RoleEnum.Executioner)
                    {
                        var executioner = (Executioner)role;
                        if (executioner.TargetVotedOut)
                        {
                            TempData.winners = new List<WinningPlayerData>();
                            isImp = false;
                            isApoc = false;
                            isNeut = true;
                            isCrew = false;
                            isOther = false;
                            winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                            var exeData = new WinningPlayerData(executioner.Player.Data);
                            if (PlayerControl.LocalPlayer != executioner.Player) exeData.IsYou = false;
                            TempData.winners.Add(exeData);
                            foreach (var r in Role.GetRoles(RoleEnum.Witch).ToArray().Where(x => x.FactionOverride == FactionOverride.None))
                            {
                                var witch = (Witch)r;
                                if (!witch.Player.Data.IsDead && !witch.Player.Data.Disconnected)
                                {
                                    var witchData = new WinningPlayerData(witch.Player.Data);
                                    if (PlayerControl.LocalPlayer != witch.Player) witchData.IsYou = false;
                                    TempData.winners.Add(witchData);
                                }
                            }
                            return;
                        }
                    }
                    else if (type == RoleEnum.Doomsayer)
                    {
                        var doom = (Doomsayer)role;
                        if (doom.WonByGuessing)
                        {
                            TempData.winners = new List<WinningPlayerData>();
                            isImp = false;
                            isApoc = false;
                            isNeut = true;
                            isCrew = false;
                            isOther = false;
                            winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                            var doomData = new WinningPlayerData(doom.Player.Data);
                            if (PlayerControl.LocalPlayer != doom.Player) doomData.IsYou = false;
                            TempData.winners.Add(doomData);
                            foreach (var r in Role.GetRoles(RoleEnum.Witch).ToArray().Where(x => x.FactionOverride == FactionOverride.None))
                            {
                                var witch = (Witch)r;
                                if (!witch.Player.Data.IsDead && !witch.Player.Data.Disconnected)
                                {
                                    var witchData = new WinningPlayerData(witch.Player.Data);
                                    if (PlayerControl.LocalPlayer != witch.Player) witchData.IsYou = false;
                                    TempData.winners.Add(witchData);
                                }
                            }
                            return;
                        }
                    }
                    else if (type == RoleEnum.Phantom)
                    {
                        var phantom = (Phantom)role;
                        if (phantom.CompletedTasks)
                        {
                            TempData.winners = new List<WinningPlayerData>();
                            isImp = false;
                            isApoc = false;
                            isNeut = true;
                            isCrew = false;
                            isOther = false;
                            winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                            var phantomData = new WinningPlayerData(phantom.Player.Data);
                            if (PlayerControl.LocalPlayer != phantom.Player) phantomData.IsYou = false;
                            TempData.winners.Add(phantomData);
                            foreach (var r in Role.GetRoles(RoleEnum.Witch).ToArray().Where(x => x.FactionOverride == FactionOverride.None))
                            {
                                var witch = (Witch)r;
                                if (!witch.Player.Data.IsDead && !witch.Player.Data.Disconnected)
                                {
                                    var witchData = new WinningPlayerData(witch.Player.Data);
                                    if (PlayerControl.LocalPlayer != witch.Player) witchData.IsYou = false;
                                    TempData.winners.Add(witchData);
                                }
                            }
                            return;
                        }
                    }
                    else if (type == RoleEnum.Pirate)
                    {
                        var pirate = (Pirate)role;
                        if (pirate.WonByDuel)
                        {
                            TempData.winners = new List<WinningPlayerData>();
                            isImp = false;
                            isApoc = false;
                            isNeut = true;
                            isCrew = false;
                            isOther = false;
                            winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                            var pirateData = new WinningPlayerData(pirate.Player.Data);
                            if (PlayerControl.LocalPlayer != pirate.Player) pirateData.IsYou = false;
                            TempData.winners.Add(pirateData);
                            foreach (var r in Role.GetRoles(RoleEnum.Witch).ToArray().Where(x => x.FactionOverride == FactionOverride.None))
                            {
                                var witch = (Witch)r;
                                if (!witch.Player.Data.IsDead && !witch.Player.Data.Disconnected)
                                {
                                    var witchData = new WinningPlayerData(witch.Player.Data);
                                    if (PlayerControl.LocalPlayer != witch.Player) witchData.IsYou = false;
                                    TempData.winners.Add(witchData);
                                }
                            }
                            return;
                        }
                    }
                    else if (type == RoleEnum.Inquisitor)
                    {
                        var inquisitor = (Inquisitor)role;
                        if (inquisitor.HereticsDead && !inquisitor.Player.Data.IsDead && !inquisitor.Player.Data.Disconnected)
                        {
                            TempData.winners = new List<WinningPlayerData>();
                            isImp = false;
                            isApoc = false;
                            isNeut = true;
                            isCrew = false;
                            isOther = false;
                            winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                            var inquisitorData = new WinningPlayerData(inquisitor.Player.Data);
                            if (PlayerControl.LocalPlayer != inquisitor.Player) inquisitorData.IsYou = false;
                            TempData.winners.Add(inquisitorData);
                            foreach (var r in Role.GetRoles(RoleEnum.Witch).ToArray().Where(x => x.FactionOverride == FactionOverride.None))
                            {
                                var witch = (Witch)r;
                                if (!witch.Player.Data.IsDead && !witch.Player.Data.Disconnected)
                                {
                                    var witchData = new WinningPlayerData(witch.Player.Data);
                                    if (PlayerControl.LocalPlayer != witch.Player) witchData.IsYou = false;
                                    TempData.winners.Add(witchData);
                                }
                            }
                            return;
                        }
                    }
                }
            }

            foreach (var objective in Objective.AllObjectives)
            {
                var type = objective.ObjectiveType;
                var role = Role.GetRole(objective.Player);
                if (role == null)
                {
                    if (type == ObjectiveEnum.Lover)
                    {
                        var lover = (Lover)objective;
                        if (lover.LoveCoupleWins)
                        {
                            var otherLover = lover.OtherLover;
                            TempData.winners = new List<WinningPlayerData>();
                            isImp = false;
                            isApoc = false;
                            isNeut = false;
                            isCrew = false;
                            isOther = true;
                            winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                            var loverOneData = new WinningPlayerData(lover.Player.Data);
                            var loverTwoData = new WinningPlayerData(otherLover.Player.Data);
                            if (PlayerControl.LocalPlayer != lover.Player) loverOneData.IsYou = false;
                            if (PlayerControl.LocalPlayer != otherLover.Player) loverTwoData.IsYou = false;
                            TempData.winners.Add(loverOneData);
                            TempData.winners.Add(loverTwoData);
                            foreach (var r in Role.GetRoles(RoleEnum.Witch).ToArray().Where(x => x.FactionOverride == FactionOverride.None))
                            {
                                var witch = (Witch)r;
                                if (!witch.Player.Data.IsDead && !witch.Player.Data.Disconnected)
                                {
                                    var witchData = new WinningPlayerData(witch.Player.Data);
                                    if (PlayerControl.LocalPlayer != witch.Player) witchData.IsYou = false;
                                    TempData.winners.Add(witchData);
                                }
                            }
                            return;
                        }
                    }
                }
                else if (role.FactionOverride == FactionOverride.None)
                {
                    if (type == ObjectiveEnum.Lover)
                    {
                        var lover = (Lover)objective;
                        if (lover.LoveCoupleWins)
                        {
                            var otherLover = lover.OtherLover;
                            TempData.winners = new List<WinningPlayerData>();
                            isImp = false;
                            isApoc = false;
                            isNeut = false;
                            isCrew = false;
                            isOther = true;
                            winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                            var loverOneData = new WinningPlayerData(lover.Player.Data);
                            var loverTwoData = new WinningPlayerData(otherLover.Player.Data);
                            if (PlayerControl.LocalPlayer != lover.Player) loverOneData.IsYou = false;
                            if (PlayerControl.LocalPlayer != otherLover.Player) loverTwoData.IsYou = false;
                            TempData.winners.Add(loverOneData);
                            TempData.winners.Add(loverTwoData);
                            foreach (var r in Role.GetRoles(RoleEnum.Witch).ToArray().Where(x => x.FactionOverride == FactionOverride.None))
                            {
                                var witch = (Witch)r;
                                if (!witch.Player.Data.IsDead && !witch.Player.Data.Disconnected)
                                {
                                    var witchData = new WinningPlayerData(witch.Player.Data);
                                    if (PlayerControl.LocalPlayer != witch.Player) witchData.IsYou = false;
                                    TempData.winners.Add(witchData);
                                }
                            }
                            return;
                        }
                    }
                }

            }

            if (Role.VampireWins)
            {
                TempData.winners = new List<WinningPlayerData>();
                foreach (var role in Role.GetRoles(RoleEnum.Vampire).ToArray().Where(x => x.FactionOverride == FactionOverride.Vampires))
                {
                    var vamp = (Vampire)role;
                    var vampData = new WinningPlayerData(vamp.Player.Data);
                    isImp = false;
                    isApoc = false;
                    isNeut = true;
                    isCrew = false;
                    isOther = false;
                    winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                    if (PlayerControl.LocalPlayer != vamp.Player) vampData.IsYou = false;
                    TempData.winners.Add(vampData);
                }
            }

            foreach (var role in Role.AllRoles.ToArray().Where(x => x.FactionOverride == FactionOverride.None))
            {
                var type = role.RoleType;

                if (type == RoleEnum.Glitch)
                {
                    var glitch = (Glitch)role;
                    if (glitch.GlitchWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        isImp = false;
                        isApoc = false;
                        isNeut = true;
                        isCrew = false;
                        isOther = false;
                        winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                        var glitchData = new WinningPlayerData(glitch.Player.Data);
                        if (PlayerControl.LocalPlayer != glitch.Player) glitchData.IsYou = false;
                        TempData.winners.Add(glitchData);
                    }
                }
                else if (type == RoleEnum.Juggernaut)
                {
                    var juggernaut = (Juggernaut)role;
                    if (juggernaut.JuggernautWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        isImp = false;
                        isApoc = false;
                        isNeut = true;
                        isCrew = false;
                        isOther = false;
                        winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                        var juggData = new WinningPlayerData(juggernaut.Player.Data);
                        if (PlayerControl.LocalPlayer != juggernaut.Player) juggData.IsYou = false;
                        TempData.winners.Add(juggData);
                    }
                }
                else if (type == RoleEnum.Arsonist)
                {
                    var arsonist = (Arsonist)role;
                    if (arsonist.ArsonistWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        isImp = false;
                        isApoc = false;
                        isNeut = true;
                        isCrew = false;
                        isOther = false;
                        winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                        var arsonistData = new WinningPlayerData(arsonist.Player.Data);
                        if (PlayerControl.LocalPlayer != arsonist.Player) arsonistData.IsYou = false;
                        TempData.winners.Add(arsonistData);
                    }
                }
                else if (role.Faction == Faction.NeutralApocalypse)
                {
                    if (role.ApocalypseWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        isImp = false;
                        isApoc = true;
                        isNeut = false;
                        isCrew = false;
                        isOther = false;
                        winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                        foreach (var am in Role.AllRoles.ToArray().Where(x => x.Faction == Faction.NeutralApocalypse && x.FactionOverride == FactionOverride.None))
                        {
                            var apocalypseMemberData = new WinningPlayerData(am.Player.Data);
                            if (PlayerControl.LocalPlayer != am.Player) apocalypseMemberData.IsYou = false;
                            TempData.winners.Add(apocalypseMemberData);
                        }
                    }
                }
                else if (type == RoleEnum.Werewolf)
                {
                    var werewolf = (Werewolf)role;
                    if (werewolf.WerewolfWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        isImp = false;
                        isApoc = false;
                        isNeut = true;
                        isCrew = false;
                        isOther = false;
                        winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                        var werewolfData = new WinningPlayerData(werewolf.Player.Data);
                        if (PlayerControl.LocalPlayer != werewolf.Player) werewolfData.IsYou = false;
                        TempData.winners.Add(werewolfData);
                    }
                }
                else if (type == RoleEnum.SerialKiller)
                {
                    var serialKiller = (SerialKiller)role;
                    if (serialKiller.SerialKillerWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        isImp = false;
                        isApoc = false;
                        isNeut = true;
                        isCrew = false;
                        isOther = false;
                        winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                        var serialKillerData = new WinningPlayerData(serialKiller.Player.Data);
                        if (PlayerControl.LocalPlayer != serialKiller.Player) serialKillerData.IsYou = false;
                        TempData.winners.Add(serialKillerData);
                    }
                }
                else if (type == RoleEnum.RedMember)
                {
                    var redMember = (RedMember)role;
                    if (redMember.TeamWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var rm in Role.AllRoles.ToArray().Where(x => x.RoleType == RoleEnum.RedMember))
                        {
                            var redMemberData = new WinningPlayerData(rm.Player.Data);
                            if (PlayerControl.LocalPlayer != rm.Player) redMemberData.IsYou = false;
                            TempData.winners.Add(redMemberData);
                        }
                    }
                }
                else if (type == RoleEnum.BlueMember)
                {
                    var blueMember = (BlueMember)role;
                    if (blueMember.TeamWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var bm in Role.AllRoles.ToArray().Where(x => x.RoleType == RoleEnum.BlueMember))
                        {
                            var blueMemberData = new WinningPlayerData(bm.Player.Data);
                            if (PlayerControl.LocalPlayer != bm.Player) blueMemberData.IsYou = false;
                            TempData.winners.Add(blueMemberData);
                        }
                    }
                }
                else if (type == RoleEnum.YellowMember)
                {
                    var yellowMember = (YellowMember)role;
                    if (yellowMember.TeamWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var ym in Role.AllRoles.ToArray().Where(x => x.RoleType == RoleEnum.YellowMember))
                        {
                            var yellowMemberData = new WinningPlayerData(ym.Player.Data);
                            if (PlayerControl.LocalPlayer != ym.Player) yellowMemberData.IsYou = false;
                            TempData.winners.Add(yellowMemberData);
                        }
                    }
                }
                else if (type == RoleEnum.GreenMember)
                {
                    var greenMember = (GreenMember)role;
                    if (greenMember.TeamWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        foreach (var gm in Role.AllRoles.ToArray().Where(x => x.RoleType == RoleEnum.GreenMember))
                        {
                            var greenMemberData = new WinningPlayerData(gm.Player.Data);
                            if (PlayerControl.LocalPlayer != gm.Player) greenMemberData.IsYou = false;
                            TempData.winners.Add(greenMemberData);
                        }
                    }
                }
                else if (type == RoleEnum.SoloKiller)
                {
                    var soloKiller = (SoloKiller)role;
                    if (soloKiller.TeamWins)
                    {
                        TempData.winners = new List<WinningPlayerData>();
                        var soloKillerData = new WinningPlayerData(soloKiller.Player.Data);
                        if (PlayerControl.LocalPlayer != soloKiller.Player) soloKillerData.IsYou = false;
                        TempData.winners.Add(soloKillerData);
                    }
                }
            }

            foreach (var role in Role.GetRoles(RoleEnum.Survivor).ToArray().Where(x => x.FactionOverride == FactionOverride.None))
            {
                var surv = (Survivor)role;
                if (!surv.Player.Data.IsDead && !surv.Player.Data.Disconnected)
                {
                    var survWinData = new WinningPlayerData(surv.Player.Data);
                    if (isImp) survWinData.IsImpostor = true;
                    if (PlayerControl.LocalPlayer != surv.Player) survWinData.IsYou = false;
                    TempData.winners.Add(survWinData);
                }
            }
            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel).ToArray().Where(x => x.FactionOverride == FactionOverride.None))
            {
                var ga = (GuardianAngel)role;
                var gaTargetData = new WinningPlayerData(ga.target.Data);
                foreach (WinningPlayerData winner in TempData.winners.ToArray())
                {
                    if (gaTargetData.ColorId == winner.ColorId)
                    {
                        var gaWinData = new WinningPlayerData(ga.Player.Data);
                        if (isImp) gaWinData.IsImpostor = true;
                        if (PlayerControl.LocalPlayer != ga.Player) gaWinData.IsYou = false;
                        TempData.winners.Add(gaWinData);
                    }
                }
            }
            foreach (var modifier in Objective.GetObjectives(ObjectiveEnum.ImpostorAgent))
            {
                var agent = (ImpostorAgent)modifier;
                var role = Role.GetRole(modifier.Player);
                if (role == null)
                {
                    if (isImp)
                    {
                        var agentWinData = new WinningPlayerData(agent.Player.Data);
                        if (isImp) agentWinData.IsImpostor = true;
                        if (PlayerControl.LocalPlayer != agent.Player) agentWinData.IsYou = false;
                        TempData.winners.Add(agentWinData);
                    }
                }
                else if (role.FactionOverride == FactionOverride.None)
                {
                    if (isImp)
                    {
                        var agentWinData = new WinningPlayerData(agent.Player.Data);
                        if (isImp) agentWinData.IsImpostor = true;
                        if (PlayerControl.LocalPlayer != agent.Player) agentWinData.IsYou = false;
                        TempData.winners.Add(agentWinData);
                    }
                }
            }
            foreach (var modifier in Objective.GetObjectives(ObjectiveEnum.ApocalypseAgent).ToArray().Where(x => Role.GetRole(x.Player).FactionOverride == FactionOverride.None))
            {
                var agent = (ApocalypseAgent)modifier;
                var role = Role.GetRole(modifier.Player);
                if (role == null)
                {
                    if (isApoc)
                    {
                        var agentWinData = new WinningPlayerData(agent.Player.Data);
                        if (PlayerControl.LocalPlayer != agent.Player) agentWinData.IsYou = false;
                        TempData.winners.Add(agentWinData);
                    }
                }
                else if (role.FactionOverride == FactionOverride.None)
                {
                    if (isApoc)
                    {
                        var agentWinData = new WinningPlayerData(agent.Player.Data);
                        if (PlayerControl.LocalPlayer != agent.Player) agentWinData.IsYou = false;
                        TempData.winners.Add(agentWinData);
                    }
                }
            }
            foreach (var role in Role.GetRoles(RoleEnum.JKNecromancer))
            {
                var necromancer = (Necromancer)role;
                if (necromancer.NecromancerWin)
                {
                    TempData.winners = new List<WinningPlayerData>();
                    isImp = false;
                    isApoc = false;
                    isNeut = true;
                    isCrew = false;
                    isOther = false;
                    winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                    foreach (var undead in Role.AllRoles.ToArray().Where(x => x.FactionOverride == FactionOverride.Undead))
                    {
                        var undeadData = new WinningPlayerData(undead.Player.Data);
                        if (PlayerControl.LocalPlayer != undead.Player) undeadData.IsYou = false;
                        TempData.winners.Add(undeadData);
                    }
                }
            }
            foreach (var role in Role.GetRoles(RoleEnum.Jackal))
            {
                var jackal = (Jackal)role;
                if (jackal.JackalWin)
                {
                    TempData.winners = new List<WinningPlayerData>();
                    isImp = false;
                    isApoc = false;
                    isNeut = true;
                    isCrew = false;
                    isOther = false;
                    winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
                    foreach (var recruit in Role.AllRoles.ToArray().Where(x => x.FactionOverride == FactionOverride.Recruit))
                    {
                        var recruitData = new WinningPlayerData(recruit.Player.Data);
                        if (PlayerControl.LocalPlayer != recruit.Player) recruitData.IsYou = false;
                        TempData.winners.Add(recruitData);
                    }
                }
            }
            foreach (var role in Role.GetRoles(RoleEnum.Witch).ToArray().Where(x => x.FactionOverride == FactionOverride.None))
            {
                var witch = (Witch)role;
                var witchWinData = new WinningPlayerData(witch.Player.Data);
                if (!isCrew && !witchWinData.IsDead)
                    {
                        if (isImp) witchWinData.IsImpostor = true;
                        if (PlayerControl.LocalPlayer != witch.Player) witchWinData.IsYou = false;
                        TempData.winners.Add(witchWinData);
                }
            }
            winner = new System.Collections.Generic.List<bool>() { isImp, isApoc, isNeut, isCrew, isOther };
        }
        /*public static void Postfix(EndGameManager __instance)
        {
            if (winner[0])
            {
                PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();
                foreach (var player in array) player.NameText().text = Role.GetRole(player).ColorString + player.NameText().text + "</color>";
                __instance.BackgroundBar.material.color = Patches.Colors.Impostor;
                var text = Object.Instantiate(__instance.WinText);
                text.text = "Impostors Win!";
                text.color = Patches.Colors.Impostor;
                var pos = __instance.WinText.transform.localPosition;
                pos.y = 1.5f;
                text.transform.position = pos;
                text.text = $"<size=4>{text.text}</size>";
            }
            else if (winner[3])
            {
                PoolablePlayer[] array = Object.FindObjectsOfType<PoolablePlayer>();
                foreach (var player in array) player.NameText().text = Role.GetRole(player).ColorString + player.NameText().text + "</color>";
                __instance.BackgroundBar.material.color = Patches.Colors.Crewmate;
                var text = Object.Instantiate(__instance.WinText);
                text.text = "Crewmates Win!";
                text.color = Patches.Colors.Crewmate;
                var pos = __instance.WinText.transform.localPosition;
                pos.y = 1.5f;
                text.transform.position = pos;
                text.text = $"<size=4>{text.text}</size>";
            }
        }*/
    }
}
