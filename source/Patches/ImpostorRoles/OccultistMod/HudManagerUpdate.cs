using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using System.Linq;
using TownOfUs.Extensions;
using AmongUs.GameOptions;

namespace TownOfUs.ImpostorRoles.OccultistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class HudManagerUpdate
    {
        public static Sprite Mark => TownOfUs.OccultistMarkSprite;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Occultist)) return;
            var role = Role.GetRole<Occultist>(PlayerControl.LocalPlayer);
            if (!PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && !x.IsMarked() &&
            !(PlayerControl.LocalPlayer.Is(FactionOverride.None) && (x.Is(ObjectiveEnum.ImpostorAgent) || (x.Data.IsImpostor() || (x.Is(RoleEnum.Undercover) && Utils.UndercoverIsImpostor()))) ||
            (PlayerControl.LocalPlayer.Is(FactionOverride.Recruit) && x.Is(FactionOverride.Recruit)) ||
            (PlayerControl.LocalPlayer.Is(FactionOverride.Undead) && x.Is(FactionOverride.Undead)))) && !role.Player.Data.IsDead)
            {
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (PlayerControl.LocalPlayer.Is(FactionOverride.None) && !player.Is(Faction.Impostors) && !player.Is(ObjectiveEnum.ImpostorAgent)) Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                    else if (PlayerControl.LocalPlayer.Is(FactionOverride.Recruit) && !player.Is(FactionOverride.Recruit)) Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                    else if (PlayerControl.LocalPlayer.Is(FactionOverride.Undead) && !player.Is(FactionOverride.Undead)) Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, player);
                }
            }
            if (role.MarkButton == null)
            {
                role.MarkButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.MarkButton.graphic.enabled = true;
                role.MarkButton.gameObject.SetActive(false);
            }

            role.MarkButton.graphic.sprite = Mark;
            role.MarkButton.buttonLabelText.gameObject.SetActive(true);
            role.MarkButton.buttonLabelText.text = "Mark";
            role.MarkButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            var notImpostor = (PlayerControl.LocalPlayer.Is(FactionOverride.Undead) ? PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(FactionOverride.Undead)).ToList() : PlayerControl.LocalPlayer.Is(FactionOverride.Recruit) ? PlayerControl.AllPlayerControls.ToArray().Where(x => !(x.Is(FactionOverride.Recruit) && !(x.Is(RoleEnum.Jackal) && !CustomGameOptions.RecruistSeeJackal))).ToList() : PlayerControl.AllPlayerControls.ToArray().Where(
                player => PlayerControl.LocalPlayer.Is(FactionOverride.None) && !player.Is(ObjectiveEnum.ImpostorAgent) && (!((player.Data.IsImpostor() || (player.Is(RoleEnum.Undercover) && Utils.UndercoverIsImpostor())) && !Utils.CheckImpostorFriendlyFire() && !player.Is((RoleEnum)254)))
            ).ToList()).Where(x => !role.MarkedPlayers.Contains(x.PlayerId)).ToList();
            role.MarkButton.SetCoolDown(role.MarkTimer(), CustomGameOptions.MarkCooldown + CustomGameOptions.MarkCooldownIncrease * role.MarkedPlayers.Count);
            Utils.SetTarget(ref role.ClosestPlayer, role.MarkButton, GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance], notImpostor);
        }
    }
}