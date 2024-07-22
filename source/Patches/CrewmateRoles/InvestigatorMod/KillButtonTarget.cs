using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.CrewmateRoles.InvestigatorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class KillButtonTarget
    {
        public static byte DontInvestigate = byte.MaxValue;

        public static bool Prefix(KillButton __instance)
        {
            return !PlayerControl.LocalPlayer.Is(RoleEnum.Investigator);
        }

        public static void SetTarget(KillButton __instance, DeadBody target, Investigator role)
        {
            if (role.CurrentTarget && role.CurrentTarget != target)
            {
                foreach (var body in role.CurrentTarget.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            if (target != null && target.ParentId == DontInvestigate) target = null;
            else if (role.Reports == null) role.CurrentTarget = null;
            else if (target == null || role.Reports[target.ParentId] == null) role.CurrentTarget = null;
            else if (role.Reports[target.ParentId].Count == 0) role.CurrentTarget = null;
            else role.CurrentTarget = target;
            if (role.CurrentTarget && __instance.enabled && role.ButtonUsable)
            {
                SpriteRenderer component = null;
                foreach (var body in role.CurrentTarget.bodyRenderers) component = body;
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Patches.Colors.Investigator);
                __instance.graphic.color = Palette.EnabledColor;
                __instance.graphic.material.SetFloat("_Desat", 0f);
                role.UsesText.color = Palette.EnabledColor;
                role.UsesText.material.SetFloat("_Desat", 0f);
                __instance.buttonLabelText.color = Palette.EnabledColor;
                __instance.buttonLabelText.material.SetFloat("_Desat", 0f);
                return;
            }

            __instance.graphic.color = Palette.DisabledClear;
            __instance.graphic.material.SetFloat("_Desat", 1f);
            role.UsesText.color = Palette.DisabledClear;
            role.UsesText.material.SetFloat("_Desat", 1f);
            __instance.buttonLabelText.color = Palette.DisabledClear;
            __instance.buttonLabelText.material.SetFloat("_Desat", 1f);
        }
    }
}