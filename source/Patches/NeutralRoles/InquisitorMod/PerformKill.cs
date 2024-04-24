using System;
using System.Linq;
using AmongUs.GameOptions;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Patches;
using TownOfUs.Roles;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TownOfUs.NeutralRoles.InquisitorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Inquisitor);
            if (!flag) return true;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            var role = Role.GetRole<Inquisitor>(PlayerControl.LocalPlayer);
            if (role.Player.inVent) return false;
            if (role.AbilityTimer() != 0) return false;

            if (role.ClosestPlayer == null) return false;
            var distBetweenPlayers = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
            var flag3 = distBetweenPlayers <
                        GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (!flag3) return false;
            if (__instance == role.VanquishButton && role.CanVanquish)
            {
                if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)1);
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
                if (interact[4] == true) if (!role.heretics.Contains(role.ClosestPlayer.PlayerId)) role.CanVanquish = false;
                    else if (interact[0] == true)
                    {
                        role.LastAbility = DateTime.UtcNow;
                        return false;
                    }
                    else if (interact[1] == true)
                    {
                        role.LastAbility = DateTime.UtcNow;
                        role.LastAbility = role.LastAbility.AddSeconds(-CustomGameOptions.InquisitorCooldown + CustomGameOptions.ProtectKCReset);
                        return false;
                    }
                    else if (interact[2] == true)
                    {
                        role.LastAbility = DateTime.UtcNow;
                        role.LastAbility = role.LastAbility.AddSeconds(-CustomGameOptions.InquisitorCooldown + CustomGameOptions.VestKCReset);
                        return false;
                    }
                    else if (interact[3] == true) return false;
                return false;
            }
            else
            {
                var interact = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer);
                if (interact[4] == true)
                {
                    if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                    if (role.heretics.Contains(role.ClosestPlayer.PlayerId))
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.red));
                        role.Notification("Your Target Is A Heretic!", 1000 * CustomGameOptions.NotificationDuration);
                    }
                    else
                    {
                        Coroutines.Start(Utils.FlashCoroutine(Color.green));
                        role.Notification("Your Target Isn't A Heretic!", 1000 * CustomGameOptions.NotificationDuration);
                    }
                }
                if (interact[0] == true)
                {
                    role.LastAbility = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastAbility = DateTime.UtcNow;
                    role.LastAbility.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.InquisitorCooldown);
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
        }
    }
}