using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Extensions;
using UnityEngine;
using System;
using TownOfUs.Modifiers.UnderdogMod;

namespace TownOfUs.ImpostorRoles.PoisonerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Poisoner>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (__instance == role.PoisonButton)
            {
                if (!__instance.isActiveAndEnabled || role.ClosestPlayer == null) return false;
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (PlayerControl.LocalPlayer.killTimer != 0) return false;

                var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
                if (interact[4] == true)
                {
                    if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                    role.PoisonTime = DateTime.UtcNow;
                    role.PoisonedPlayer = target;
                    Role.GetRole(role.ClosestPlayer).Roleblocked = true;
                    Role.GetRole(role.ClosestPlayer).SuperRoleblocked = true;
                    Utils.Rpc(CustomRPC.Roleblock, role.ClosestPlayer.PlayerId, true);
                    if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                    {
                        var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                        var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                        var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                        PlayerControl.LocalPlayer.SetKillTimer(((Modifiers.UnderdogMod.PerformKill.LastImp() ? lowerKC : (Modifiers.UnderdogMod.PerformKill.IncreasedKC() ? normalKC : upperKC)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f)) + CustomGameOptions.PoisonDelay);
                    }
                    else PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f) + CustomGameOptions.PoisonDelay);
                    Utils.Rpc(CustomRPC.Poison, target.PlayerId);
                }
                if (interact[0] == true)
                {
                    if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                    {
                        var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                        var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                        var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                        PlayerControl.LocalPlayer.SetKillTimer(((Modifiers.UnderdogMod.PerformKill.LastImp() ? lowerKC : (Modifiers.UnderdogMod.PerformKill.IncreasedKC() ? normalKC : upperKC)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f)) + CustomGameOptions.PoisonDelay);
                    }
                    else PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f) + CustomGameOptions.PoisonDelay);
                    return false;
                }
                else if (interact[1] == true)
                {
                    PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.ProtectKCReset);
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
            return true;
        }
    }
}