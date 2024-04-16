using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Extensions;
using UnityEngine;
using System;
using TownOfUs.Modifiers.UnderdogMod;

namespace TownOfUs.ImpostorRoles.SniperMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sniper)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Sniper>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (__instance == role.SnipeButton)
            {
                if (!__instance.isActiveAndEnabled || role.ClosestPlayer == null) return false;
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.AimedPlayer == null)
                {
                    if (role.AimTimer() != 0) return false;
                    var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
                    if (interact[4] == true)
                    {
                        if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                        role.AimedPlayer = role.ClosestPlayer;
                    }
                    if (interact[0] == true)
                    {
                        role.LastAim = DateTime.UtcNow;
                        return false;
                    }
                    else if (interact[1] == true)
                    {
                        role.LastAim = DateTime.UtcNow;
                        role.LastAim = role.LastAim.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.AimCooldown);
                        return false;
                    }
                    else if (interact[3] == true) return false;
                    return false;
                }
                else
                {
                    if (PlayerControl.LocalPlayer.killTimer != 0) return false;
                    if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)1);

                    if (!role.AimedPlayer.Is(RoleEnum.Pestilence) && !role.AimedPlayer.Is(RoleEnum.Famine) && !role.AimedPlayer.Is(RoleEnum.War) && !role.AimedPlayer.Is(RoleEnum.Death) && !role.AimedPlayer.IsShielded() && !role.AimedPlayer.IsVesting() && !role.AimedPlayer.IsOnAlert() && !role.AimedPlayer.IsProtected())
                    {
                        Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, role.AimedPlayer);
                    }
                    if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                    {
                        var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                        var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                        var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                        PlayerControl.LocalPlayer.SetKillTimer((Modifiers.UnderdogMod.PerformKill.LastImp() ? lowerKC : (Modifiers.UnderdogMod.PerformKill.IncreasedKC() ? normalKC : upperKC)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                    }
                    else PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                    role.LastAim = DateTime.UtcNow;
                    role.AimedPlayer = null;
                    Utils.Rpc(CustomRPC.Shoot, role.Player.PlayerId);
                    return false;
                }
            }
            return true;
        }
    }
}