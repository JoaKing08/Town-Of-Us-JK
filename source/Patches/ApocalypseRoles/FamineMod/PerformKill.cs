using System;
using HarmonyLib;
using TownOfUs.Roles;
using AmongUs.GameOptions;
using TownOfUs.Roles.Horseman;
using System.Linq;

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
                Role.GetRole(role.ClosestPlayer).BreadLeft -= CustomGameOptions.StarveStrength;
                if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected))
                {
                    if (role.FactionOverride == FactionOverride.None)
                    {
                        if (!player.Is(Faction.NeutralApocalypse) && !player.Is(ObjectiveEnum.ApocalypseAgent))
                        {
                            var playerRole = Role.GetRole(player);
                            if (playerRole.BreadLeft <= 0)
                            {
                                Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                                Utils.Rpc(CustomRPC.KillAbilityUsed, player.PlayerId);
                            }
                        }
                    }
                    else
                    {
                        if (!player.Is(role.FactionOverride))
                        {
                            var playerRole = Role.GetRole(player);
                            if (playerRole.BreadLeft <= 0)
                            {
                                Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                                Utils.Rpc(CustomRPC.KillAbilityUsed, player.PlayerId);
                            }
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