using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using System.Linq;
using TownOfUs.Extensions;
using AmongUs.GameOptions;

namespace TownOfUs.ImpostorRoles.GodfatherMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Recruit => TownOfUs.RecruitSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Godfather)) return;
            var role = Role.GetRole<Godfather>(PlayerControl.LocalPlayer);
            if (role.RecruitButton == null)
            {
                role.RecruitButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.RecruitButton.graphic.enabled = true;
                role.RecruitButton.gameObject.SetActive(false);
            }

            role.RecruitButton.graphic.sprite = Recruit;
            role.RecruitButton.buttonLabelText.gameObject.SetActive(true);
            role.RecruitButton.buttonLabelText.text = "Recruit";
            role.RecruitButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            var notImpostor = PlayerControl.LocalPlayer.Is(FactionOverride.Undead) ? PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(FactionOverride.Undead)).ToList() : PlayerControl.LocalPlayer.Is(FactionOverride.Recruit) ? PlayerControl.AllPlayerControls.ToArray().Where(x => !(x.Is(FactionOverride.Recruit) && !(x.Is(RoleEnum.Jackal) && !CustomGameOptions.RecruistSeeJackal))).ToList() : PlayerControl.AllPlayerControls.ToArray().Where(
                player => !player.Is(ObjectiveEnum.ImpostorAgent) && (!((player.Data.IsImpostor() || (player.Is(RoleEnum.Undercover) && Utils.UndercoverIsImpostor())) && !Utils.CheckImpostorFriendlyFire()))
            ).ToList();

            if (!role.Recruited)
            {
                role.RecruitButton.SetCoolDown(role.RecruitTimer(), CustomGameOptions.InitialCooldowns);
                Utils.SetTarget(ref role.ClosestPlayer, role.RecruitButton, GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance], notImpostor);
            }
            else
            {
                role.RecruitButton.SetCoolDown(0f, 1f);
                var renderer = role.RecruitButton.graphic;
                renderer.color = Palette.DisabledClear;
                renderer.material.SetFloat("_Desat", 1f);
            }
        }
    }
}