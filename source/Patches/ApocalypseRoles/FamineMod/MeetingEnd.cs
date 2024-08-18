using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.FamineMod
{
    public class MeetingEnd
    {
        public static void Postfix()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Famine);
            if (!flag) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            var role = Role.GetRole<Famine>(PlayerControl.LocalPlayer);
            foreach (var player in PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected))
            {
                if (role.FactionOverride == FactionOverride.None)
                {
                    if (!player.Is(Faction.NeutralApocalypse) && !player.Is(ObjectiveEnum.ApocalypseAgent))
                    {
                        var playerRole = Role.GetRole(player);
                        playerRole.BreadLeft -= 1;
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
                        playerRole.BreadLeft -= 1;
                        if (playerRole.BreadLeft <= 0)
                        {
                            Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                            Utils.Rpc(CustomRPC.KillAbilityUsed, player.PlayerId);
                        }
                    }
                }
            }
        }
    }
}
