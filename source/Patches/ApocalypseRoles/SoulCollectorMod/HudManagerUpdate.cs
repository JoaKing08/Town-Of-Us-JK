using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Extensions;
using AmongUs.GameOptions;
using TownOfUs.Roles.Horseman;
using System;
using TownOfUs.CrewmateRoles.MedicMod;

namespace TownOfUs.ApocalypseRoles.SoulCollectorMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public static class HudManagerUpdate
    {
        public static Sprite Arrow => TownOfUs.Arrow;
        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector)) return;

            var role = Role.GetRole<SoulCollector>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.SCKillNotif)
            {
                var validBodies = GameObject.FindObjectsOfType<DeadBody>().Where(x =>
                    Murder.KilledPlayers.Any(y => y.PlayerId == x.ParentId && y.KillTime.AddSeconds(CustomGameOptions.SCKillArrowDuration) > System.DateTime.UtcNow));

                foreach (var bodyArrow in role.BodyArrows.Keys)
                {
                    if (!validBodies.Any(x => x.ParentId == bodyArrow))
                    {
                        role.DestroyArrow(bodyArrow);
                    }
                }

                foreach (var body in validBodies)
                {
                    if (!role.BodyArrows.ContainsKey(body.ParentId))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Arrow;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        role.BodyArrows.Add(body.ParentId, arrow);
                    }
                    role.BodyArrows.GetValueSafe(body.ParentId).target = body.TruePosition;
                }
            }
            else
            {
                if (role.BodyArrows.Count != 0)
                {
                    role.BodyArrows.Values.DestroyAll();
                    role.BodyArrows.Clear();
                }
            }

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;

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
                    role.TurnDeath();
                    Utils.Rpc(CustomRPC.TurnDeath, PlayerControl.LocalPlayer.PlayerId);
                }
            }
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] { "Players", "Ghost" }));

            var killButton = __instance.KillButton;
            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;
            //if (PlayerControl.LocalPlayer.IsControled()) Utils.Rpc(CustomRPC.ControlCooldown, (byte)role.ReapTimer(), (byte)CustomGameOptions.SoulCollectorCooldown);
            killButton.SetCoolDown(role.ReapTimer(), CustomGameOptions.SoulCollectorCooldown);

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
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);

            KillButtonTarget.SetTarget(killButton, closestBody, role);
        }
    }
}