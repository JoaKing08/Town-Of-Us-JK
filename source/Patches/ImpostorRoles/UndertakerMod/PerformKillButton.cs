using System;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ImpostorRoles.UndertakerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);

            if (__instance == role.DragDropButton)
            {
                if (role.DragDropButton.graphic.sprite == TownOfUs.DragSprite)
                {
                    if (__instance.isCoolingDown) return false;
                    if (!__instance.enabled) return false;
                    var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                    if (Vector2.Distance(role.CurrentTarget.TruePosition,
                        PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
                    var playerId = role.CurrentTarget.ParentId;
                    var player = Utils.PlayerById(playerId);
                    if (PlayerControl.LocalPlayer.IsInVision() || player.IsInVision())
                    {
                        Utils.Rpc(CustomRPC.VisionInteract, PlayerControl.LocalPlayer.PlayerId, player.PlayerId);
                    }
                    if (player.IsInfected() || role.Player.IsInfected())
                    {
                        foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
                    }
                    if (player.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, playerId, (byte)role.RoleType, (byte)0);

                    Utils.Rpc(CustomRPC.Drag, PlayerControl.LocalPlayer.PlayerId, playerId);

                    role.CurrentlyDragging = role.CurrentTarget;

                    KillButtonTarget.SetTarget(__instance, null, role);
                    __instance.graphic.sprite = TownOfUs.DropSprite;
                    return false;
                }
                else
                {
                    if (!__instance.enabled) return false;
                    Vector3 position = PlayerControl.LocalPlayer.transform.position;

                    if (Patches.SubmergedCompatibility.isSubmerged())
                    {
                        if (position.y > -7f)
                        {
                            position.z = 0.0208f;
                        }
                        else
                        {
                            position.z = -0.0273f;
                        }
                    }

                    position.y -= 0.3636f;

                    Utils.Rpc(CustomRPC.Drop, PlayerControl.LocalPlayer.PlayerId, position, position.z);

                    var body = role.CurrentlyDragging;
                    foreach (var body2 in role.CurrentlyDragging.bodyRenderers) body2.material.SetFloat("_Outline", 0f);
                    role.CurrentlyDragging = null;
                    __instance.graphic.sprite = TownOfUs.DragSprite;
                    role.LastDragged = DateTime.UtcNow;

                    body.transform.position = position;

                    return false;
                }
            }

            return true;
        }
    }
}
