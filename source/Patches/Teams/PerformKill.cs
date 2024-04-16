using System;
using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Roles.Teams;
using AmongUs.GameOptions;

namespace TownOfUs.Teams
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.RedMember) || PlayerControl.LocalPlayer.Is(RoleEnum.BlueMember) || PlayerControl.LocalPlayer.Is(RoleEnum.YellowMember) || PlayerControl.LocalPlayer.Is(RoleEnum.GreenMember) || PlayerControl.LocalPlayer.Is(RoleEnum.SoloKiller);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<TeamMember>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;
            if (role.KillTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
            if (interact[4] == true) return false;
            else if (interact[0] == true)
            {
                role.LastKill = DateTime.UtcNow;
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}