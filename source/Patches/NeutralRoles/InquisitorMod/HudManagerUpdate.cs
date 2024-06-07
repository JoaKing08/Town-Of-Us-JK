using System.Linq;
using HarmonyLib;
using TownOfUs.CrewmateRoles.AltruistMod;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.InquisitorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite VanquishSprite => TownOfUs.VanquishSprite;

        public static void Postfix(HudManager __instance)
        {
            if (Role.GetRoles(RoleEnum.Inquisitor).Any(x => !x.Player.Data.IsDead && !x.Player.Data.Disconnected)) foreach (Inquisitor inq in Role.GetRoles(RoleEnum.Inquisitor).ToArray().Where(x => !x.Player.Data.IsDead && !x.Player.Data.Disconnected))
            {
                if (inq.heretics.ToArray().Any(x => !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected))
                {
                    inq.Wins();
                    if (!CustomGameOptions.NeutralEvilWinEndsGame)
                    {
                        KillButtonTarget.DontRevive = inq.Player.PlayerId;
                        inq.Player.Exiled();
                    }
                }
            }
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Inquisitor)) return;
            var role = Role.GetRole<Inquisitor>(PlayerControl.LocalPlayer);

            if (role.VanquishButton == null)
            {
                role.VanquishButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.VanquishButton.graphic.enabled = true;
                role.VanquishButton.gameObject.SetActive(false);
            }

            role.VanquishButton.graphic.sprite = VanquishSprite;
            role.VanquishButton.transform.localPosition = new Vector3(-2f, 0f, 0f);
            role.VanquishButton.buttonLabelText.gameObject.SetActive(true);
            role.VanquishButton.buttonLabelText.text = "Vanquish";

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.VanquishButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (role.CanVanquish) role.VanquishButton.SetCoolDown(role.AbilityTimer(), CustomGameOptions.InquisitorCooldown);
            else role.VanquishButton.SetCoolDown(0f, CustomGameOptions.InquisitorCooldown);
            //if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)role.AbilityTimer(), (byte)CustomGameOptions.InquisitorCooldown);
            __instance.KillButton.SetCoolDown(role.AbilityTimer(), CustomGameOptions.InquisitorCooldown);
            Utils.SetTarget(ref role.ClosestPlayer, __instance.KillButton);

            var inquireRenderer = __instance.KillButton.graphic;
            var vanquishRenderer = role.VanquishButton.graphic;

            if (role.ClosestPlayer != null)
            {
                inquireRenderer.color = Palette.EnabledColor;
                inquireRenderer.material.SetFloat("_Desat", 0f);

                if (role.CanVanquish)
                {
                    vanquishRenderer.color = Palette.EnabledColor;
                    vanquishRenderer.material.SetFloat("_Desat", 0f);
                }
                else
                {
                    vanquishRenderer.color = Palette.DisabledClear;
                    vanquishRenderer.material.SetFloat("_Desat", 1f);
                }
            }
            else
            {
                inquireRenderer.color = Palette.DisabledClear;
                inquireRenderer.material.SetFloat("_Desat", 1f);
                vanquishRenderer.color = Palette.DisabledClear;
                vanquishRenderer.material.SetFloat("_Desat", 1f);
            }
            role.RegenTask();
        }
    }

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class PlayerControl_MurderPlayer
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (Role.GetRoles(RoleEnum.Inquisitor).Any(x => !x.Player.Data.IsDead && !x.Player.Data.Disconnected)) foreach (Inquisitor inq in Role.GetRoles(RoleEnum.Inquisitor).ToArray().Where(x => !x.Player.Data.IsDead && !x.Player.Data.Disconnected))
                {
                    if (inq.heretics.ToArray().Any(x => !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected))
                    {
                        inq.Wins();
                        if (!CustomGameOptions.NeutralEvilWinEndsGame)
                        {
                            KillButtonTarget.DontRevive = inq.Player.PlayerId;
                            inq.Player.Exiled();
                        }
                    }
                }
        }
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.Begin))]
    internal class MeetingExiledEnd
    {
        private static void Postfix(ExileController __instance)
        {
            if (Role.GetRoles(RoleEnum.Inquisitor).Any(x => !x.Player.Data.IsDead && !x.Player.Data.Disconnected)) foreach (Inquisitor inq in Role.GetRoles(RoleEnum.Inquisitor).ToArray().Where(x => !x.Player.Data.IsDead && !x.Player.Data.Disconnected))
                {
                    if (inq.heretics.ToArray().Any(x => !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected))
                    {
                        inq.Wins();
                        if (!CustomGameOptions.NeutralEvilWinEndsGame)
                        {
                            KillButtonTarget.DontRevive = inq.Player.PlayerId;
                            inq.Player.Exiled();
                        }
                    }
                }
        }
    }
}