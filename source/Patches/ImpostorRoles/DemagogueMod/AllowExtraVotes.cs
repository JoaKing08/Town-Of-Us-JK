using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.DemagogueMod
{
    [HarmonyPatch(typeof(PlayerVoteArea))]
    public class AllowExtraVotes
    {
        [HarmonyPatch(typeof(PlayerVoteArea), nameof(PlayerVoteArea.VoteForMe))]
        public static class VoteForMe
        {
            public static bool Prefix(PlayerVoteArea __instance)
            {
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue)) return true;
                var role = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
                if (__instance.Parent.state == MeetingHud.VoteStates.Proceeding ||
                    __instance.Parent.state == MeetingHud.VoteStates.Results)
                    return false;

                if (__instance != role.ExtraVote)
                {
                    return true;
                }
                else
                {
                    if (role.Charges >= CustomGameOptions.ChargesForExtraVote && role.ExtraVotes < CustomGameOptions.MaxExtraVotes)
                    {
                        if (Role.GetRole(PlayerControl.LocalPlayer).Roleblocked)
                        {
                            Coroutines.Start(Utils.FlashCoroutine(Color.white));
                            role.Notification("You Are Roleblocked!", 1000 * CustomGameOptions.NotificationDuration);
                            return false;
                        }
                        role.ExtraVotes++;
                        role.Charges -= CustomGameOptions.ChargesForExtraVote;
                        Utils.Rpc(CustomRPC.DemagogueCharges, role.Charges, role.Player.PlayerId);
                        AddVoteButton.UpdateButton(role, MeetingHud.Instance);
                        Utils.Rpc(CustomRPC.DemagogueVotes, (byte)role.ExtraVotes, role.Player.PlayerId);
                    }
                    return false;
                }
            }
        }
    }
}