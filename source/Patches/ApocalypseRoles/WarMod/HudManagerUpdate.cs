using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using Hazel;
using TownOfUs.Roles.Horseman;
using System;

namespace TownOfUs.ApocalypseRoles.WarMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.War)) return;
            var role = Role.GetRole<War>(PlayerControl.LocalPlayer);
            if (role.UsingCharge == true && role.UseTimer() == 0)
            {
                role.UsingCharge = false;
                role.LastKill = DateTime.UtcNow;
            }

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)role.KillTimer(), (byte)CustomGameOptions.WarCooldown);
            __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.WarCooldown);

            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(player => !player.Is(ObjectiveEnum.ApocalypseAgent) && !((player.Is(Faction.NeutralApocalypse) || player.Is(RoleEnum.Undercover)) && !Utils.CheckApocalypseFriendlyFire())).ToList());
        }
    }
}