using System;
using HarmonyLib;
using TownOfUs.Roles;
using AmongUs.GameOptions;
using TownOfUs.Roles.Horseman;
using UnityEngine;
using Reactor.Utilities;

namespace TownOfUs.ApocalypseRoles.DeathMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Death);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Death>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;
            if (role.ApocalypseTimer() != 0) return false;
            if (Role.GetRole(PlayerControl.LocalPlayer).Roleblocked)
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.white));
                return false;
            }

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (!player.Is(Faction.NeutralApocalypse) && !player.Is(ModifierEnum.ApocalypseAgent))
                {
                    Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                }
            }
            return false;
        }
    }
}