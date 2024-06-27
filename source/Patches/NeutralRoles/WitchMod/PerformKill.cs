using System;
using HarmonyLib;
using TownOfUs.Roles;
using AmongUs.GameOptions;

namespace TownOfUs.NeutralRoles.WitchMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Witch);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Witch>(PlayerControl.LocalPlayer);
            if (role.ControlTimer() != 0) return false;
            if (role.ControledPlayer == null)
            {
                if (role.ClosestPlayer == null) return false;
                var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
                var flag3 = distBetweenPlayers <
                            GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                if (!flag3) return false;
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[4] == true)
                {
                    role.LastControl = DateTime.UtcNow;
                    role.ControledPlayer = role.ClosestPlayer;
                    role.RevealedPlayers.Add(role.ClosestPlayer.PlayerId);
                    Utils.Rpc(CustomRPC.ControlSet, PlayerControl.LocalPlayer.PlayerId, role.ClosestPlayer.PlayerId);
                    if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                }
                if (interact[0] == true)
                {
                    role.LastControl = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastControl = DateTime.UtcNow;
                    role.LastControl = role.LastControl.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.ControlCooldown);
                    return false;
                }
                else if (interact[3] == true) return false;
            }
            else
            {
                role.LastControl = DateTime.UtcNow;
                Utils.Rpc(CustomRPC.ControlPerform, PlayerControl.LocalPlayer.PlayerId, role.ControledPlayer.PlayerId, role.ClosestPlayer == null ? byte.MaxValue : role.ClosestPlayer.PlayerId);
                if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)1);
            }
            return false;
        }
    }
}