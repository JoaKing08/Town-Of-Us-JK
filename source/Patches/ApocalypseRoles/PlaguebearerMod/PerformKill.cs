using System;
using HarmonyLib;
using Hazel;
using TownOfUs.Roles;
using AmongUs.GameOptions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.NeutralRoles.PlaguebearerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Plaguebearer>(PlayerControl.LocalPlayer);
            if (role.InfectTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            if (role.InfectedPlayers.Contains(role.ClosestPlayer.PlayerId)) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
            if (interact[4])
            {
                if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
            }
            else if (interact[0] == true)
            {
                role.LastInfected = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastInfected = DateTime.UtcNow;
                role.LastInfected.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.InfectCd);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}