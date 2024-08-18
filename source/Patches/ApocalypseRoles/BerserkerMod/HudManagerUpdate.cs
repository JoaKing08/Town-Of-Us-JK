using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Extensions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.BerserkerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Berserker)) return;
            var isDead = PlayerControl.LocalPlayer.Data.IsDead;
            var role = Role.GetRole<Berserker>(PlayerControl.LocalPlayer);

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            //if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)role.KillTimer(), (byte)(CustomGameOptions.BerserkerCooldown - CustomGameOptions.BerserkerCooldownBonus * role.KilledPlayers));
            __instance.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.BerserkerCooldown - CustomGameOptions.BerserkerCooldownBonus * role.KilledPlayers);

            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton, float.NaN, PlayerControl.LocalPlayer.Is(FactionOverride.Undead) ? PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(FactionOverride.Undead)).ToList() : PlayerControl.LocalPlayer.Is(FactionOverride.Recruit) ? PlayerControl.AllPlayerControls.ToArray().Where(x => !(x.Is(FactionOverride.Recruit) && !(x.Is(RoleEnum.Jackal) && !CustomGameOptions.RecruistSeeJackal))).ToList() : PlayerControl.AllPlayerControls.ToArray().Where(player => !player.Is(ObjectiveEnum.ApocalypseAgent) && !((player.Is(Faction.NeutralApocalypse) || (player.Is(RoleEnum.Undercover) && Utils.UndercoverIsApocalypse())) && !Utils.CheckApocalypseFriendlyFire())).ToList());

            if (role.CanTransform && (PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected).ToList().Count > 1) && !isDead)
            {
                var transform = false;
                var alives = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x != PlayerControl.LocalPlayer).ToList();
                if (alives.Count <= 1)
                {
                    foreach (var player in alives)
                    {
                        if (player.Data.IsImpostor() || player.Is(Faction.NeutralKilling) || player.Is(Faction.NeutralApocalypse) || ((player.Is(RoleEnum.Deputy) || player.Is(RoleEnum.Hunter) || player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.VampireHunter) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Vigilante)) && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK) || CustomGameOptions.OvertakeWin == OvertakeWin.Off)
                        {
                            transform = true;
                        }
                    }
                }
                else transform = true;
                if (transform)
                {
                    role.TurnWar();
                    Utils.Rpc(CustomRPC.TurnWar, PlayerControl.LocalPlayer.PlayerId);
                }
            }
        }
    }
}