using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Extensions;
using AmongUs.GameOptions;
using System;
using TownOfUs.CrewmateRoles.MedicMod;

namespace TownOfUs.CrewmateRoles.InvestigatorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static void Postfix(HudManager __instance)
        {
            if (CustomGameOptions.GameMode == GameMode.Cultist) return;
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) return;
            foreach (var kill in Murder.KilledPlayers)
            {
                var player = Utils.PlayerById(kill.KillerId);
                if ((DateTime.UtcNow - kill.KillTime).TotalSeconds < 10 && !kill.KillerKillAbility)
                {
                    if (player.inVent)
                    {
                        kill.KillerVented = true;
                    }
                    kill.KillerKillAbility = Role.GetRole(Utils.PlayerById(kill.PlayerId)).KilledByAbility;
                }
                if (((DateTime.UtcNow - kill.KillTime).TotalSeconds < 10 || kill.KillerRunTo == KillerDirection.None) && !kill.KillerKillAbility)
                {
                    var distanceX = player.transform.position.x - kill.BodyPosition.x;
                    var distanceY = player.transform.position.y - kill.BodyPosition.y;
                    var totalDistanceX = distanceX < 0 ? distanceX * -1 : distanceX;
                    var totalDistanceY = distanceY < 0 ? distanceY * -1 : distanceY;
                    if (!(Vector3.Distance(player.transform.position, kill.BodyPosition) < GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance] * 3))
                    {
                        if (totalDistanceX > totalDistanceY)
                        {
                            if (distanceX < 0)
                            {
                                kill.KillerRunTo = KillerDirection.West;
                            }
                            else
                            {
                                kill.KillerRunTo = KillerDirection.East;
                            }
                        }
                        else
                        {
                            if (distanceY < 0)
                            {
                                kill.KillerRunTo = KillerDirection.South;
                            }
                            else
                            {
                                kill.KillerRunTo = KillerDirection.North;
                            }
                        }
                    }
                }
            }

            var role = Role.GetRole<Investigator>(PlayerControl.LocalPlayer);

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] { "Players", "Ghost" }));

            var killButton = __instance.KillButton;

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = UnityEngine.Object.Instantiate(killButton.cooldownTimerText, killButton.transform);
                role.UsesText.gameObject.SetActive(false);
                role.UsesText.transform.localPosition = new Vector3(
                    role.UsesText.transform.localPosition.x + 0.26f,
                    role.UsesText.transform.localPosition.y + 0.29f,
                    role.UsesText.transform.localPosition.z);
                role.UsesText.transform.localScale = role.UsesText.transform.localScale * 0.65f;
                role.UsesText.alignment = TMPro.TextAlignmentOptions.Right;
                role.UsesText.fontStyle = TMPro.FontStyles.Bold;
            }
            if (role.UsesText != null)
            {
                role.UsesText.text = role.UsesLeft + "";
            }
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;
            //if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)role.ReapTimer(), (byte)CustomGameOptions.SoulCollectorCooldown);
            if (role.ButtonUsable) killButton.SetCoolDown(role.InvestigateTimer(), CustomGameOptions.InvestigateCooldown);
            else killButton.SetCoolDown(0f, 1f);

            foreach (var collider2D in allocs)
            {
                if (!flag || isDead || collider2D.tag != "DeadBody") continue;
                var component = collider2D.GetComponent<DeadBody>();


                if (!(Vector2.Distance(truePosition, component.TruePosition) <=
                      maxDistance)) continue;

                var distance = Vector2.Distance(truePosition, component.TruePosition);
                if (!(distance < closestDistance)) continue;
                closestBody = component;
                closestDistance = distance;
            }

            killButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && CustomGameOptions.MaxInvestigates > 0);

            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started && CustomGameOptions.MaxInvestigates > 0);

            KillButtonTarget.SetTarget(killButton, closestBody, role);
        }
    }
}