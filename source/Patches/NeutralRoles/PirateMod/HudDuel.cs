using System;
using System.Linq;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.PirateMod
{
    [HarmonyPatch(typeof(HudManager))]
    public class HudConfess
    {
        [HarmonyPatch(nameof(HudManager.Update))]
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.LocalPlayer.IsDueled()) if (PlayerControl.LocalPlayer.GetPirate().notify && PlayerControl.LocalPlayer.GetPirate().NotificationTimer() == 0f)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Pirate));
                    Role.GetRole(PlayerControl.LocalPlayer).Notification("Ya Ar Duel'g!", 1000 * CustomGameOptions.NotificationDuration);
                    PlayerControl.LocalPlayer.GetPirate().notify = false;
                }
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Pirate)) return;
            var duelButton = __instance.KillButton;

            var role = Role.GetRole<Pirate>(PlayerControl.LocalPlayer);
            if (role.notify && role.NotificationTimer() == 0f)
            {
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Pirate));
                role.Notification("Ya Ar Duel'g!", 1000 * CustomGameOptions.NotificationDuration);
                role.notify = false;
            }

            duelButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (role.DueledPlayer != null)
                {
                    if (role.DueledPlayer.PlayerId == player.PlayerId)
                    {
                        if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                                player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                            player.nameText().color = Patches.Colors.Pirate;
                        else player.nameText().color = Color.clear;
                        if (MeetingHud.Instance != null)
                        {
                            foreach (var state in MeetingHud.Instance.playerStates)
                            {
                                if (player.PlayerId != state.TargetPlayerId) continue;
                                state.NameText.color = Patches.Colors.Pirate;
                            }
                        }
                    }
                }
            }
            //if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)role.DuelTimer(), (byte)CustomGameOptions.DuelCooldown);
            duelButton.SetCoolDown(role.DuelTimer(), CustomGameOptions.DuelCooldown);
            var notDueled = PlayerControl.AllPlayerControls.ToArray().Where(x => x != role.DueledPlayer).ToList();

            Utils.SetTarget(ref role.ClosestPlayer, duelButton, float.NaN, notDueled);

            var renderer = duelButton.graphic;

            if (role.ClosestPlayer != null)
            {
                renderer.color = Palette.EnabledColor;
                renderer.material.SetFloat("_Desat", 0f);
            }
            else
            {
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}
