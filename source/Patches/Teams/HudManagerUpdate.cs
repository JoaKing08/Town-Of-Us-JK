using HarmonyLib;
using System.Linq;
using TownOfUs.Roles;
using TownOfUs.Roles.Teams;

namespace TownOfUs.Teams
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!(PlayerControl.LocalPlayer.Is(RoleEnum.RedMember) || PlayerControl.LocalPlayer.Is(RoleEnum.BlueMember) || PlayerControl.LocalPlayer.Is(RoleEnum.YellowMember) || PlayerControl.LocalPlayer.Is(RoleEnum.GreenMember) || PlayerControl.LocalPlayer.Is(RoleEnum.SoloKiller))) return;
            var role = Role.GetRole<TeamMember>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            __instance.KillButton.SetCoolDown(role.KillTimer(), (CustomGameOptions.GameMode == GameMode.Teams ? CustomGameOptions.TeamsKCd : CustomGameOptions.SoloKillerKCd) == 0 ? (CustomGameOptions.GameMode == GameMode.Teams ? CustomGameOptions.TeamsKCd : CustomGameOptions.SoloKillerKCd) + 0.001f : (CustomGameOptions.GameMode == GameMode.Teams ? CustomGameOptions.TeamsKCd : CustomGameOptions.SoloKillerKCd));
            var notTeam = PlayerControl.AllPlayerControls.ToArray().ToList();
            if (PlayerControl.LocalPlayer.Is(RoleEnum.RedMember)) notTeam = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(RoleEnum.RedMember)).ToList();
            if (PlayerControl.LocalPlayer.Is(RoleEnum.BlueMember)) notTeam = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(RoleEnum.BlueMember)).ToList();
            if (PlayerControl.LocalPlayer.Is(RoleEnum.YellowMember)) notTeam = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(RoleEnum.YellowMember)).ToList();
            if (PlayerControl.LocalPlayer.Is(RoleEnum.GreenMember)) notTeam = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(RoleEnum.GreenMember)).ToList();
            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, notTeam);
        }
    }
}