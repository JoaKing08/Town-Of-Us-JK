using System;
using System.Collections.Generic;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Linq;

namespace TownOfUs
{
    [HarmonyPatch(typeof(MeetingHud))]
    public class RegisterExtraVotes
    {
        public static Dictionary<byte, int> CalculateAllVotes(MeetingHud __instance)
        {
            var dictionary = new Dictionary<byte, int>();
            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];
                var player = Utils.PlayerById(playerVoteArea.TargetPlayerId);
                if (!player.Is(RoleEnum.Prosecutor)) continue;
                var pros = Role.GetRole<Prosecutor>(player);
                if (pros.Player.Data.IsDead || pros.Player.Data.Disconnected) continue;
                if (!playerVoteArea.DidVote
                    || playerVoteArea.AmDead
                    || playerVoteArea.VotedFor == PlayerVoteArea.MissedVote
                    || playerVoteArea.VotedFor == PlayerVoteArea.DeadVote)
                {
                    pros.ProsecuteThisMeeting = false;
                    continue;
                }
                else if (pros.ProsecuteThisMeeting)
                {
                    if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var num2))
                        dictionary[playerVoteArea.VotedFor] = num2 + 5;
                    else
                        dictionary[playerVoteArea.VotedFor] = 5;
                    return dictionary;
                }
            }
            byte blackmailed = byte.MaxValue;
            foreach (Blackmailer blackmailer in Role.GetRoles(RoleEnum.Blackmailer))
            {
                if (blackmailer.Blackmailed != null) blackmailed = blackmailer.Blackmailed.PlayerId;
            }
            List<byte> convinced = new();
            foreach (Demagogue demagogue in Role.GetRoles(RoleEnum.Demagogue))
            {
                if (demagogue.Convinced != null) convinced = demagogue.Convinced;
            }
            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];

                var player = Utils.PlayerById(playerVoteArea.TargetPlayerId);
                if (convinced.Any() && player != null && convinced.Contains(player.PlayerId))
                {
                    var demagogue = (Demagogue)Role.GetRoles(RoleEnum.Demagogue).FirstOrDefault();
                    if (demagogue != null && !demagogue.Player.Data.IsDead && !demagogue.Player.Data.Disconnected)
                    {
                        var demagogueVoteArea = __instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == demagogue.Player.PlayerId);
                        {
                            if (!demagogueVoteArea.DidVote) playerVoteArea.UnsetVote();
                            else playerVoteArea.SetVote(demagogueVoteArea.VotedFor);
                            playerVoteArea.VotedFor = demagogueVoteArea.VotedFor;
                        }
                    }
                }
                if (!playerVoteArea.DidVote
                    || playerVoteArea.AmDead
                    || playerVoteArea.VotedFor == PlayerVoteArea.MissedVote
                    || playerVoteArea.VotedFor == PlayerVoteArea.DeadVote) continue;
                if (player.PlayerId != blackmailed)
                {
                    if (player.Is(RoleEnum.Mayor))
                    {
                        var mayor = Role.GetRole<Mayor>(player);
                        if (mayor.Revealed)
                        {
                            if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var num2))
                                dictionary[playerVoteArea.VotedFor] = num2 + 2;
                            else
                                dictionary[playerVoteArea.VotedFor] = 2;
                        }
                    }
                    if (player.IsKnight())
                    {
                        var role = Role.GetRole(player);
                        if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var num2))
                            dictionary[playerVoteArea.VotedFor] = num2 + 1;
                        else
                            dictionary[playerVoteArea.VotedFor] = 1;
                    }

                    if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var num))
                        dictionary[playerVoteArea.VotedFor] = num + 1;
                    else
                        dictionary[playerVoteArea.VotedFor] = 1;
                }
                if (player.Is(RoleEnum.Demagogue))
                {
                    var demagogue = Role.GetRole<Demagogue>(player);
                    if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var num2))
                        dictionary[playerVoteArea.VotedFor] = num2 + demagogue.ExtraVotes;
                    else
                        dictionary[playerVoteArea.VotedFor] = demagogue.ExtraVotes;
                }
            }

            dictionary.MaxPair(out var tie);

            if (tie)
                foreach (var player in __instance.playerStates)
                {
                    if (!player.DidVote
                        || player.AmDead
                        || player.VotedFor == PlayerVoteArea.MissedVote
                        || player.VotedFor == PlayerVoteArea.DeadVote) continue;

                    var modifier = Modifier.GetModifier(player);
                    if (modifier == null) continue;
                    if (modifier.ModifierType == ModifierEnum.Tiebreaker)
                    {
                        if (dictionary.TryGetValue(player.VotedFor, out var num))
                            dictionary[player.VotedFor] = num + 1;
                        else
                            dictionary[player.VotedFor] = 1;
                    }
                }

            return dictionary;
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public static class VotingComplete
        {
            public static void Postfix(MeetingHud __instance,
                [HarmonyArgument(0)] Il2CppStructArray<MeetingHud.VoterState> states,
                [HarmonyArgument(1)] GameData.PlayerInfo exiled,
                [HarmonyArgument(2)] bool tie)
            {
                // __instance.exiledPlayer = __instance.wasTie ? null : __instance.exiledPlayer;
                var exiledString = exiled == null ? "null" : exiled.PlayerName;
                PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"Exiled PlayerName = {exiledString}");
                PluginSingleton<TownOfUs>.Instance.Log.LogMessage($"Was a tie = {tie}");
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.PopulateResults))]
        public static class PopulateResults
        {
            public static bool Prefix(MeetingHud __instance,
                [HarmonyArgument(0)] Il2CppStructArray<MeetingHud.VoterState> states)
            {
                DeadSeeVoteColorsPatch.DemagogueVote = false;
                var allNums = new Dictionary<int, int>();

                __instance.TitleText.text = Object.FindObjectOfType<TranslationController>()
                    .GetString(StringNames.MeetingVotingResults, Array.Empty<Il2CppSystem.Object>());
                var amountOfSkippedVoters = 0;

                var isProsecuting = false;
                foreach (var pros in Role.GetRoles(RoleEnum.Prosecutor))
                {
                    var prosRole = (Prosecutor)pros;
                    if (pros.Player.Data.IsDead || pros.Player.Data.Disconnected) continue;
                    if (prosRole.ProsecuteThisMeeting)
                    {
                        isProsecuting = true;
                    }
                }
                List<byte> convinced = new();
                foreach (Demagogue demagogue in Role.GetRoles(RoleEnum.Demagogue))
                {
                    if (demagogue.Convinced != null) convinced = demagogue.Convinced;
                }

                for (var i = 0; i < __instance.playerStates.Length; i++)
                {
                    var playerVoteArea = __instance.playerStates[i];
                    playerVoteArea.ClearForResults();
                    allNums.Add(i, 0);

                    for (var stateIdx = 0; stateIdx < states.Length; stateIdx++)
                    {
                        var voteState = states[stateIdx];

                        var playerInfo = GameData.Instance.GetPlayerById(voteState.VoterId);
                        foreach (var pros in Role.GetRoles(RoleEnum.Prosecutor))
                        {
                            var prosRole = (Prosecutor)pros;
                            if (pros.Player.Data.IsDead || pros.Player.Data.Disconnected) continue;
                            if (prosRole.ProsecuteThisMeeting)
                            {
                                if (voteState.VoterId == prosRole.Player.PlayerId)
                                {
                                    if (playerInfo == null)
                                    {
                                        Debug.LogError(string.Format("Couldn't find player info for voter: {0}",
                                            voteState.VoterId));
                                        prosRole.Revealed = true;
                                        prosRole.ProsecutionsLeft -= 1;
                                    }
                                    else if (i == 0 && voteState.SkippedVote)
                                    {
                                        __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                        __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                        __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                        __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                        __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                        amountOfSkippedVoters += 5;
                                        prosRole.Revealed = true;
                                        prosRole.ProsecutionsLeft -= 1;
                                    }
                                    else if (voteState.VotedForId == playerVoteArea.TargetPlayerId)
                                    {
                                        __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                        __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                        __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                        __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                        __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                        allNums[i] += 5;
                                        prosRole.Revealed = true;
                                        prosRole.ProsecutionsLeft -= 1;
                                    }
                                }
                            }
                        }

                        if (isProsecuting) continue;
                        byte blackmailed = byte.MaxValue;
                        foreach (Blackmailer blackmailer in Role.GetRoles(RoleEnum.Blackmailer))
                        {
                            if (blackmailer.Blackmailed != null) blackmailed = blackmailer.Blackmailed.PlayerId;
                        }
                        bool skipped = voteState.SkippedVote;
                        byte votedFor = voteState.VotedForId;
                        if (convinced.Contains(voteState.VoterId))
                        {
                            var demagogue = (Demagogue)Role.GetRoles(RoleEnum.Demagogue).FirstOrDefault();
                            if (demagogue != null && !demagogue.Player.Data.IsDead && !demagogue.Player.Data.Disconnected)
                            {
                                var demagogueVoteState = states.FirstOrDefault(x => x.VoterId == demagogue.Player.PlayerId);
                                skipped = demagogueVoteState.SkippedVote;
                                votedFor = voteState.VotedForId;
                            }
                        }
                        if (playerInfo == null)
                        {
                            Debug.LogError(string.Format("Couldn't find player info for voter: {0}",
                                voteState.VoterId));
                        }
                        else if (i == 0 && skipped && voteState.VoterId != blackmailed)
                        {
                            __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                            amountOfSkippedVoters++;
                        }
                        else if (votedFor == playerVoteArea.TargetPlayerId && voteState.VoterId != blackmailed)
                        {
                            __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                            allNums[i]++;
                        }
                        foreach (var mayor in Role.GetRoles(RoleEnum.Mayor))
                        {
                            var mayorRole = (Mayor)mayor;
                            if (mayorRole.Revealed)
                            {
                                if (voteState.VoterId == mayorRole.Player.PlayerId && voteState.VoterId != blackmailed)
                                {
                                    if (playerInfo == null)
                                    {
                                        Debug.LogError(string.Format("Couldn't find player info for voter: {0}",
                                            voteState.VoterId));
                                    }
                                    else if (i == 0 && skipped)
                                    {
                                        __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                        __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                        amountOfSkippedVoters++;
                                        amountOfSkippedVoters++;
                                    }
                                    else if (votedFor == playerVoteArea.TargetPlayerId)
                                    {
                                        __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                        __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                        allNums[i]++;
                                        allNums[i]++;
                                    }
                                }
                            }
                        }
                        foreach (var knight in PlayerControl.AllPlayerControls.ToArray().Where(x => x.IsKnight()).ToList())
                        {
                            if (voteState.VoterId == knight.PlayerId && voteState.VoterId != blackmailed)
                            {
                                if (playerInfo == null)
                                {
                                    Debug.LogError(string.Format("Couldn't find player info for voter: {0}",
                                        voteState.VoterId));
                                }
                                else if (i == 0 && skipped)
                                {
                                    __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                    amountOfSkippedVoters++;
                                }
                                else if (votedFor == playerVoteArea.TargetPlayerId)
                                {
                                    __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                    allNums[i]++;
                                }
                            }
                        }
                    }
                }
                foreach (Demagogue demagogue in Role.GetRoles(RoleEnum.Demagogue))
                {
                    if (!demagogue.Player.Data.IsDead && !demagogue.Player.Data.Disconnected && demagogue.ExtraVotes > 0)
                    {
                        var playerInfo = GameData.Instance.GetPlayerById(demagogue.Player.PlayerId);
                        DeadSeeVoteColorsPatch.DemagogueVote = true;
                        for (var i = 0; i < __instance.playerStates.Length; i++)
                        {
                            var playerVoteArea = __instance.playerStates[i];
                            foreach (var voteState in states.Where(x => x.VoterId == demagogue.Player.PlayerId))
                            {
                                if (i == 0 && voteState.SkippedVote)
                                {
                                    for (int j = 0; j < demagogue.ExtraVotes; j++) __instance.BloopAVoteIcon(playerInfo, amountOfSkippedVoters, __instance.SkippedVoting.transform);
                                    amountOfSkippedVoters += demagogue.ExtraVotes;
                                }
                                else if (voteState.VotedForId == playerVoteArea.TargetPlayerId)
                                {
                                    for (int j = 0; j < demagogue.ExtraVotes; j++) __instance.BloopAVoteIcon(playerInfo, allNums[i], playerVoteArea.transform);
                                    allNums[i] += demagogue.ExtraVotes;
                                }
                            }
                        }
                        DeadSeeVoteColorsPatch.DemagogueVote = false;
                    }
                    if (CustomGameOptions.VotesPerCharge > 0) demagogue.Charges += (byte)((float)amountOfSkippedVoters / (float)CustomGameOptions.VotesPerCharge);
                    if (PlayerControl.LocalPlayer.PlayerId == demagogue.Player.PlayerId) Utils.Rpc(CustomRPC.DemagogueCharges, demagogue.Charges, demagogue.Player.PlayerId);
                }
                return false;
            }
        }
    }
}