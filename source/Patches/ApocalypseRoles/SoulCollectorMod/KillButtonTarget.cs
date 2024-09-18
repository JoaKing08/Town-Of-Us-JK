using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.Roles.Horseman;

namespace TownOfUs.ApocalypseRoles.SoulCollectorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.SetTarget))]
    public class KillButtonTarget
    {
        public static byte DontReap = byte.MaxValue;

        public static bool Prefix(KillButton __instance)
        {
            return !PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector);
        }

        public static void SetTarget(KillButton __instance, DeadBody target, SoulCollector role)
        {
            if (role.CurrentTarget && role.CurrentTarget != target)
            {
                foreach (var body in role.CurrentTarget.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            if (target != null && target.ParentId == DontReap) target = null;
            if (Role.GetRole(Utils.PlayerById(target.ParentId)).Reaped) role.CurrentTarget = null;
            else role.CurrentTarget = target;
            if (role.CurrentTarget && __instance.enabled)
            {
                SpriteRenderer component = null;
                foreach (var body in role.CurrentTarget.bodyRenderers) component = body;
                component.material.SetFloat("_Outline", 1f);
                component.material.SetColor("_OutlineColor", Patches.Colors.SoulCollector);
                __instance.graphic.color = Palette.EnabledColor;
                __instance.graphic.material.SetFloat("_Desat", 0f);
                return;
            }

            __instance.graphic.color = Palette.DisabledClear;
            __instance.graphic.material.SetFloat("_Desat", 1f);
        }
    }
}