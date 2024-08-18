using HarmonyLib;
using TownOfUs.Roles;
using System;
using System.Linq;
using TownOfUs.CrewmateRoles.OracleMod;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.DeathMod
{
    public class MeetingEnd
    {
        public static void Postfix()
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Death);
            if (!flag) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            var role = Role.GetRole<Death>(PlayerControl.LocalPlayer);

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (role.FactionOverride == FactionOverride.None)
                {
                    if (!player.Is(Faction.NeutralApocalypse) && !player.Is(ObjectiveEnum.ApocalypseAgent))
                    {
                        Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                        Utils.Rpc(CustomRPC.KillAbilityUsed, player.PlayerId);
                    }
                }
                else
                {
                    if (!player.Is(role.FactionOverride))
                    {
                        Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                        Utils.Rpc(CustomRPC.KillAbilityUsed, player.PlayerId);
                    }
                }
            }
        }
    }
}
