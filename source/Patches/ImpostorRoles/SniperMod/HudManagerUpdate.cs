using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using System.Linq;
using TownOfUs.Extensions;
using AmongUs.GameOptions;
using System;
using Reactor.Utilities.Extensions;

namespace TownOfUs.ImpostorRoles.SniperMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Aim => TownOfUs.AimSprite;
        public static Sprite Shoot => TownOfUs.ShootSprite;

        public static void Postfix(HudManager __instance)
        {
            if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && Role.GetRoles(RoleEnum.Sniper).Any())
            {
                var r = Role.GetRole(PlayerControl.LocalPlayer);
                if ((DateTime.UtcNow - r.SnipeTime).TotalSeconds > CustomGameOptions.SniperArrowDuration)
                {
                    r.DestroySnipeArrows();
                }
                if (PlayerControl.AllPlayerControls.Count <= 1) return;
                if (PlayerControl.LocalPlayer == null) return;
                if (PlayerControl.LocalPlayer.Data == null) return;
                if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sniper)) return;
                var role = (Sniper)r;
                if (role.SnipeButton == null)
                {
                    role.SnipeButton = GameObject.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                    role.SnipeButton.graphic.enabled = true;
                    role.SnipeButton.gameObject.SetActive(false);
                }

                role.SnipeButton.graphic.sprite = role.AimedPlayer == null ? Aim : Shoot;
                role.SnipeButton.buttonLabelText.gameObject.SetActive(true);
                role.SnipeButton.buttonLabelText.text = role.AimedPlayer == null ? "Aim" : "Shoot";
                role.SnipeButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                        && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                        && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

                var notImpostor = PlayerControl.LocalPlayer.Is(FactionOverride.Undead) ? PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(FactionOverride.Undead)).ToList() : PlayerControl.LocalPlayer.Is(FactionOverride.Recruit) ? PlayerControl.AllPlayerControls.ToArray().Where(x => !(x.Is(FactionOverride.Recruit) && !(x.Is(RoleEnum.Jackal) && !CustomGameOptions.RecruistSeeJackal))).ToList() : PlayerControl.AllPlayerControls.ToArray().Where(
                    player => !player.Is(ObjectiveEnum.ImpostorAgent) && !((player.Data.IsImpostor() || (player.Is(RoleEnum.Undercover) && Utils.UndercoverIsImpostor())) && !Utils.CheckImpostorFriendlyFire() && !player.Is((RoleEnum)254))
                ).ToList();

                if (role.AimedPlayer == null) Utils.SetTarget(ref role.ClosestPlayer, role.SnipeButton, GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance], notImpostor);

                if (role.AimedPlayer == null) role.SnipeButton.SetCoolDown(role.AimTimer(), CustomGameOptions.AimCooldown);
                else role.SnipeButton.SetCoolDown(PlayerControl.LocalPlayer.killTimer, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);

                if (role.AimedPlayer != null && !role.AimedPlayer.Data.IsDead && !role.AimedPlayer.Data.Disconnected)
                {
                    role.AimedPlayer.myRend().material.SetFloat("_Outline", 1f);
                    role.AimedPlayer.myRend().material.SetColor("_OutlineColor", new Color(0.3f, 0.0f, 0.0f));
                    if (role.AimedPlayer.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                        role.AimedPlayer.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                        role.AimedPlayer.nameText().color = new Color(0.3f, 0.0f, 0.0f);
                    else role.AimedPlayer.nameText().color = Color.clear;
                }
            }
        }
    }
}