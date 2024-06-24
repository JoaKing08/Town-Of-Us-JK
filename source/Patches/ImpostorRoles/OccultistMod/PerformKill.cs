using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Extensions;
using UnityEngine;
using System;
using TownOfUs.Modifiers.UnderdogMod;
using TownOfUs.CrewmateRoles.MedicMod;
using AmongUs.GameOptions;
using TownOfUs.Roles.Modifiers;
using Reactor.Utilities;

namespace TownOfUs.ImpostorRoles.OccultistMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Occultist)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Occultist>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (__instance == role.MarkButton)
            {
                if (!__instance.isActiveAndEnabled || role.ClosestPlayer == null) return false;
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.MarkTimer() != 0) return false;
                var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
                if (interact[4] == true)
                {
                    if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                    role.MarkedPlayers.Add(target.PlayerId);
                }
                if (interact[0] == true)
                {
                    role.LastMark = DateTime.UtcNow;
                    if (CustomGameOptions.OccultistCdLinked)
                    {
                        if (PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog))
                        {
                            var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                            var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                            var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                            PlayerControl.LocalPlayer.SetKillTimer((Modifiers.UnderdogMod.PerformKill.LastImp() ? lowerKC : (Modifiers.UnderdogMod.PerformKill.IncreasedKC() ? normalKC : upperKC)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                        }
                        else PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                    }
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastMark = DateTime.UtcNow;
                    role.LastMark = role.LastMark.AddSeconds(CustomGameOptions.ProtectKCReset - CustomGameOptions.MarkCooldown - CustomGameOptions.MarkCooldownIncrease * role.MarkedPlayers.Count);
                    if (CustomGameOptions.OccultistCdLinked) PlayerControl.LocalPlayer.SetKillTimer(CustomGameOptions.ProtectKCReset + 0.01f);
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
            return true;
        }
    }
}