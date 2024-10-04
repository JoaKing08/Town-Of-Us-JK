using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using System.Linq;
using TownOfUs.Extensions;
using AmongUs.GameOptions;
using TownOfUs.CrewmateRoles.MedicMod;

namespace TownOfUs.ImpostorRoles.PoisonerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Poison => TownOfUs.PoisonSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Poisoner)) return;
            var role = Role.GetRole<Poisoner>(PlayerControl.LocalPlayer);
            if (role.PoisonButton == null)
            {
                role.PoisonButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.PoisonButton.graphic.enabled = true;
                role.PoisonButton.gameObject.SetActive(false);
            }
            if (role.PoisonTimer() == 0 && role.PoisonedPlayer != null)
            {
                PoisonerKill.RpcMurderPlayer(role.PoisonedPlayer, role.Player);
                role.PoisonedPlayer = null;
            }

            role.PoisonButton.graphic.sprite = Poison;
            role.PoisonButton.buttonLabelText.gameObject.SetActive(true);
            role.PoisonButton.buttonLabelText.text = "Poison";
            role.PoisonButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            var notImpostor = PlayerControl.LocalPlayer.Is(FactionOverride.Undead) ? PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(FactionOverride.Undead)).ToList() : PlayerControl.LocalPlayer.Is(FactionOverride.Recruit) ? PlayerControl.AllPlayerControls.ToArray().Where(x => !(x.Is(FactionOverride.Recruit) && !(x.Is(RoleEnum.Jackal) && !CustomGameOptions.RecruistSeeJackal))).ToList() : PlayerControl.AllPlayerControls.ToArray().Where(
                player => !player.Is(ObjectiveEnum.ImpostorAgent) && !((player.Data.IsImpostor() || (player.Is(RoleEnum.Undercover) && Utils.UndercoverIsImpostor())) && !Utils.CheckImpostorFriendlyFire() && !player.Is((RoleEnum)254))
            ).ToList();

            Utils.SetTarget(ref role.ClosestPlayer, role.PoisonButton, GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance], notImpostor);

            if (role.PoisonedPlayer == null) role.PoisonButton.SetCoolDown(PlayerControl.LocalPlayer.killTimer, GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            else role.PoisonButton.SetCoolDown(role.PoisonTimer(), CustomGameOptions.PoisonDelay);

            if (role.PoisonedPlayer != null && !role.PoisonedPlayer.Data.IsDead && !role.PoisonedPlayer.Data.Disconnected)
            {
                role.PoisonedPlayer.myRend().material.SetFloat("_Outline", 1f);
                role.PoisonedPlayer.myRend().material.SetColor("_OutlineColor", new Color(0.0f, 1.0f, 0.0f));
                if (role.PoisonedPlayer.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                    role.PoisonedPlayer.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper)
                    role.PoisonedPlayer.nameText().color = new Color(0.0f, 1.0f, 0.0f);
                else role.PoisonedPlayer.nameText().color = Color.clear;
            }
        }
    }
}