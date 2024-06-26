using System;
using HarmonyLib;
using TownOfUs.Roles;
using AmongUs.GameOptions;

namespace TownOfUs.NeutralRoles.JackalMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Jackal);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Jackal>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;
            if (role.KillTimer() != 0) return false;
            if (role.RecruitsAlive) return false;

            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
            if (interact[4] == true) return false;
            else if (interact[0] == true)
            {
                role.LastKill = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastKill = DateTime.UtcNow;
                role.LastKill = role.LastKill.AddSeconds(-CustomGameOptions.JackalKCd + CustomGameOptions.ProtectKCReset);
                return false;
            }
            else if (interact[2] == true)
            {
                role.LastKill = DateTime.UtcNow;
                role.LastKill = role.LastKill.AddSeconds(-CustomGameOptions.JackalKCd + CustomGameOptions.VestKCReset);
                return false;
            }
            else if (interact[5] == true)
            {
                role.LastKill = DateTime.UtcNow;
                role.LastKill = role.LastKill.AddSeconds(CustomGameOptions.BarrierCooldownReset - CustomGameOptions.JackalKCd);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}