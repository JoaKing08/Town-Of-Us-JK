using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.WitchMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Witch)) return;
            var role = Role.GetRole<Witch>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);


            __instance.KillButton.SetCoolDown(role.ControlTimer(), CustomGameOptions.ControlCooldown);
            if (role.ControledPlayer == null) Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
            else Utils.SetTarget(ref role.ClosestPlayer, role.ControledPlayer, __instance.KillButton);
        }
    }
}