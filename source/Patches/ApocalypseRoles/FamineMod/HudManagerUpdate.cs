using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using Hazel;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.FamineMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Famine)) return;
            var role = Role.GetRole<Famine>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)role.StarveTimer(), (byte)CustomGameOptions.FamineCooldown);
            __instance.KillButton.SetCoolDown(role.StarveTimer(), CustomGameOptions.FamineCooldown);

            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(player => !player.Is(ObjectiveEnum.ApocalypseAgent) && !((player.Is(Faction.NeutralApocalypse) || player.Is(RoleEnum.Undercover)) && !Utils.CheckApocalypseFriendlyFire())).ToList());
        }
    }
}