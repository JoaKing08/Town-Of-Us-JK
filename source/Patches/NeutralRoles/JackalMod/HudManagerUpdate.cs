using HarmonyLib;
using System.Linq;
using TownOfUs.Roles;

namespace TownOfUs.NeutralRoles.JackalMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Jackal)) return;
            var role = Role.GetRole<Jackal>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            //if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)role.KillTimer(), (byte)(CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * role.JuggKills));
            if (role.RecruitsAlive) __instance.KillButton.SetCoolDown(0f, 1f);
            else __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.JackalKCd);
            
            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(FactionOverride.Recruit)).ToList());
            if (role.RecruitsAlive)
            {
                role.ClosestPlayer = null;
                __instance.KillButton.graphic.color = Palette.DisabledClear;
                __instance.KillButton.graphic.material.SetFloat("_Desat", 1f);
                __instance.KillButton.buttonLabelText.color = Palette.DisabledClear;
                __instance.KillButton.buttonLabelText.material.SetFloat("_Desat", 1f);
            }
        }
    }
}