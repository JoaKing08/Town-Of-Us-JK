using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using AmongUs.GameOptions;
using System.Linq;

namespace TownOfUs.NeutralRoles.NecromancerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class ReviveHudManagerUpdate
    {
        public static Sprite ReviveSprite => TownOfUs.Revive2Sprite;
        public static Sprite KillSprite => TownOfUs.KillSprite;
        public static byte DontRevive = byte.MaxValue;

        public static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.JKNecromancer)) return;
            var role = Role.GetRole<Necromancer>(PlayerControl.LocalPlayer);

            if (role.KillButton == null)
            {
                role.KillButton = Object.Instantiate(__instance.KillButton, __instance.KillButton.transform.parent);
                role.KillButton.graphic.enabled = true;
                role.KillButton.gameObject.SetActive(false);
            }

            role.KillButton.graphic.sprite = KillSprite;
            role.KillButton.transform.localPosition = new Vector3(-1f, 1f, 0f);
            role.KillButton.buttonLabelText.gameObject.SetActive(true);
            role.KillButton.buttonLabelText.text = "Kill";

            if (role.UsesText == null && role.UsesLeft > 0)
            {
                role.UsesText = Object.Instantiate(__instance.KillButton.cooldownTimerText, __instance.KillButton.transform);
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

            __instance.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.KillButton.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            role.UsesText.gameObject.SetActive((__instance.UseButton.isActiveAndEnabled || __instance.PetButton.isActiveAndEnabled)
                    && !MeetingHud.Instance && !PlayerControl.LocalPlayer.Data.IsDead
                    && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started);
            if (!role.LastKiller) role.KillButton.SetCoolDown(0f, 1f);
            else role.KillButton.SetCoolDown(role.KillTimer(), CustomGameOptions.RitualKillCooldown + CustomGameOptions.RitualKillCooldownIncrease * role.NecroKills);

            Utils.SetTarget(ref role.ClosestPlayer, role.KillButton, float.NaN, PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Is(FactionOverride.Undead)).ToList());
            if (!role.LastKiller)
            {
                role.ClosestPlayer = null;
                role.KillButton.graphic.color = Palette.DisabledClear;
                role.KillButton.graphic.material.SetFloat("_Desat", 1f);
                role.KillButton.buttonLabelText.color = Palette.DisabledClear;
                role.KillButton.buttonLabelText.material.SetFloat("_Desat", 1f);
            }

            var data = PlayerControl.LocalPlayer.Data;
            var isDead = data.IsDead;
            var truePosition = PlayerControl.LocalPlayer.GetTruePosition();
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var flag = (GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !data.IsDead) &&
                       (!AmongUsClient.Instance || !AmongUsClient.Instance.IsGameOver) &&
                       PlayerControl.LocalPlayer.CanMove;
            var allocs = Physics2D.OverlapCircleAll(truePosition, maxDistance,
                LayerMask.GetMask(new[] { "Players", "Ghost" }));

            DeadBody closestBody = null;
            var closestDistance = float.MaxValue;

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

            if (role.UsesLeft > 0) __instance.KillButton.SetCoolDown(role.ReviveTimer(),
                CustomGameOptions.NecromancerReviveCooldown + CustomGameOptions.ReviveCooldownIncrease * role.ReviveCount);
            else __instance.KillButton.SetCoolDown(0f, 1f);

            if (role.CurrentTarget && role.CurrentTarget != closestBody)
            {
                foreach (var body in role.CurrentTarget.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            if (closestBody != null && closestBody.ParentId == DontRevive) closestBody = null;
            if (role.UsesLeft <= 0) closestBody = null;
            role.CurrentTarget = closestBody;
            if (role.CurrentTarget == null)
            {
                __instance.KillButton.graphic.color = Palette.DisabledClear;
                __instance.KillButton.graphic.material.SetFloat("_Desat", 1f);
                role.UsesText.color = Palette.DisabledClear;
                role.UsesText.material.SetFloat("_Desat", 1f);
                return;
            }
            var player = Utils.PlayerById(role.CurrentTarget.ParentId);
            if (role.CurrentTarget && __instance.KillButton.enabled)
            {
                SpriteRenderer component = null;
                foreach (var body in role.CurrentTarget.bodyRenderers) component = body;
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Patches.Colors.Necromancer);
                __instance.KillButton.graphic.color = Palette.EnabledColor;
                __instance.KillButton.graphic.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
                return;
            }

            __instance.KillButton.graphic.color = Palette.DisabledClear;
            __instance.KillButton.graphic.material.SetFloat("_Desat", 1f);
            role.UsesText.color = Palette.DisabledClear;
            role.UsesText.material.SetFloat("_Desat", 1f);
        }
    }
}