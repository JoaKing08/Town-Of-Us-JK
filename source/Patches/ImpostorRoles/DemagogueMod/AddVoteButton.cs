using HarmonyLib;
using Reactor.Utilities.Extensions;
using System.Linq;
using TMPro;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.DemagogueMod
{
    public class AddVoteButton
    {
        public static void UpdateButton(Demagogue role, MeetingHud __instance)
        {
            var skip = __instance.SkipVoteButton;
            role.ExtraVote.gameObject.SetActive(true);
            role.ExtraVote.voteComplete = skip.voteComplete;
            role.ExtraVote.GetComponent<SpriteRenderer>().enabled = skip.GetComponent<SpriteRenderer>().enabled;
            var value = role.ExtraVotes / (float)CustomGameOptions.MaxExtraVotes;
            if (value > 1f) value = 1f;
            var color = value < 0.5f ? Color.Lerp(Color.red, Color.yellow, value * 2).ToHtmlStringRGBA() : Color.Lerp(Color.yellow, Color.green, (value - 0.5f) * 2).ToHtmlStringRGBA();
            role.ExtraVote.GetComponentsInChildren<TextMeshPro>()[0].text = $"Extra Vote <color=#{color}>+{role.ExtraVotes}</color> <color=#{(role.Charges < CustomGameOptions.ChargesForExtraVote ? "FF00" : "00FF")}00FF>({CustomGameOptions.ChargesForExtraVote})</color> ";
        }


        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
        public class MeetingHudStart
        {
            public static void GenButton(Demagogue role, MeetingHud __instance)
            {
                var skip = __instance.SkipVoteButton;
                role.ExtraVote = Object.Instantiate(skip, skip.transform.parent);
                role.ExtraVote.Parent = __instance;
                role.ExtraVote.SetTargetPlayerId(251);
                role.ExtraVote.transform.localPosition = skip.transform.localPosition +
                                                       new Vector3(0f, -0.17f, 0f);
                skip.transform.localPosition += new Vector3(0f, 0.20f, 0f);
                UpdateButton(role, __instance);
            }

            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue)) return;
                var prosRole = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
                GenButton(prosRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.ClearVote))]
        public class MeetingHudClearVote
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue)) return;
                var prosRole = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
                UpdateButton(prosRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
        public class MeetingHudConfirm
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue)) return;
                var prosRole = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
                prosRole.ExtraVote.ClearButtons();
                UpdateButton(prosRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Select))]
        public class MeetingHudSelect
        {
            public static void Postfix(MeetingHud __instance, int __0)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue)) return;
                var prosRole = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
                if (__0 != 251) prosRole.ExtraVote.ClearButtons();

                UpdateButton(prosRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.VotingComplete))]
        public class MeetingHudVotingComplete
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue)) return;
                var prosRole = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
                UpdateButton(prosRole, __instance);
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        public class MeetingHudUpdate
        {
            public static void Postfix(MeetingHud __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue)) return;
                var demRole = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
                foreach (var text in demRole.MeetingKillButtons.Select(x => x.Value.text))
                {
                    text.color = demRole.Charges >= CustomGameOptions.ChargesForMeetingKill ? Color.green : Color.red;
                }
                switch (__instance.state)
                {
                    case MeetingHud.VoteStates.Discussion:
                        if (__instance.discussionTimer < GameOptionsManager.Instance.currentNormalGameOptions.DiscussionTime)
                        {
                            demRole.ExtraVote.SetDisabled();
                            break;
                        }


                        demRole.ExtraVote.SetEnabled();
                        break;
                }

                UpdateButton(demRole, __instance);
            }
        }
    }
}