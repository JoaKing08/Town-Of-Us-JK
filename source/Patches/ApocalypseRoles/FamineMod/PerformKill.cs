using System;
using HarmonyLib;
using TownOfUs.Roles;
using AmongUs.GameOptions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.FamineMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Famine);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Famine>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;
            if (role.StarveTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, false);
            if (interact[4] == true)
            {
                Role.GetRole(role.ClosestPlayer).BreadLeft -= 1;
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (!player.Is(Faction.NeutralApocalypse) && !player.Is(ModifierEnum.ApocalypseAgent))
                    {
                        var playerRole = Role.GetRole(player);
                        playerRole.BreadLeft -= 1;
                        if (playerRole.BreadLeft <= 0)
                        {
                            Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                        }
                        if (player == role.ClosestPlayer)
                        {
                            if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                        }
                        else
                        {
                            if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)1);
                        }
                    }
                }
                role.LastStarved = DateTime.UtcNow;
            }
            else if (interact[0] == true)
            {
                role.LastStarved = DateTime.UtcNow;
                return false;
            }
            else if (interact[1] == true)
            {
                role.LastStarved = DateTime.UtcNow;
                role.LastStarved.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.FamineCooldown);
                return false;
            }
            else if (interact[3] == true) return false;
            return false;
        }
    }
}