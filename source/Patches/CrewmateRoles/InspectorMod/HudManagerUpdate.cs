using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.InspectorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Inspector)) return;
            var role = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)role.InspectTimer(), (byte)CustomGameOptions.InspectCooldown);
            __instance.KillButton.SetCoolDown(role.InspectTimer(), CustomGameOptions.InspectCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);
        }
    }
}