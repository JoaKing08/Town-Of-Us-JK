using AmongUs.GameOptions;
using HarmonyLib;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs
{
    [HarmonyPatch(typeof(ShipStatus), nameof(ShipStatus.CalculateLightRadius))]
    public static class LowLights
    {
        public static bool Prefix(ShipStatus __instance, [HarmonyArgument(0)] GameData.PlayerInfo player,
            ref float __result)
        {
            if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
            {
                if (GameOptionsManager.Instance.currentHideNSeekGameOptions.useFlashlight)
                {
                    if (player.IsImpostor()) __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.ImpostorFlashlightSize;
                    else __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.CrewmateFlashlightSize;
                }
                else
                {
                    if (player.IsImpostor()) __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.ImpostorLightMod;
                    else __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentHideNSeekGameOptions.CrewLightMod;
                }
                return false;
            }

            if (player == null || player.IsDead)
            {
                __result = __instance.MaxLightRadius;
                return false;
            }
            if (Role.GetRoles((RoleEnum)255).ToArray().Any(x => ((RoleA)x).AbilityCActive) && !player._object.Is((RoleEnum)255))
            {
                __result = __instance.MaxLightRadius / 6f / (player._object.IsRoleD() ? 3f : 1f);
                return false;
            }

            var switchSystem = GameOptionsManager.Instance.currentNormalGameOptions.MapId == 5 ? null : __instance.Systems[SystemTypes.Electrical]?.TryCast<SwitchSystem>();
            if (player.IsImpostor() || player._object.Is(RoleEnum.Glitch) ||
                player._object.Is(RoleEnum.Juggernaut) || player._object.Is(Faction.NeutralApocalypse) ||
                (player._object.Is(RoleEnum.Jester) && CustomGameOptions.JesterImpVision) ||
                (player._object.Is(RoleEnum.Arsonist) && CustomGameOptions.ArsoImpVision) ||
                (player._object.Is(RoleEnum.Vampire) && CustomGameOptions.VampImpVision) ||
                player._object.Is(RoleEnum.SerialKiller) || player._object.Is(RoleEnum.JKNecromancer) ||
                player._object.Is(RoleEnum.Jackal) || player._object.Is((RoleEnum)255))
            {
                __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod / (player._object.IsRoleD() ? 3f : 1f);
                return false;
            }
            else if (player._object.Is(RoleEnum.Werewolf))
            {
                var role = Role.GetRole<Werewolf>(player._object);
                if (role.Rampaged)
                {
                    __result = __instance.MaxLightRadius * GameOptionsManager.Instance.currentNormalGameOptions.ImpostorLightMod / (player._object.IsRoleD() ? 3f : 1f);
                    return false;
                }
            }

            if (Patches.SubmergedCompatibility.isSubmerged())
            {
                if (player._object.Is(ModifierEnum.Torch)) __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, 1) * GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod / (player._object.IsRoleD() ? 3f : 1f);
                return false;
            }

            var t = switchSystem != null ? switchSystem.Value / 255f : 1;

            if (player._object.Is(ModifierEnum.Torch)) t = 1;

            if (player._object.Is(RoleEnum.Mayor))
            {
                var role = Role.GetRole<Mayor>(player._object);
                if (role.Revealed)
                {
                    __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius/2, t) *
                       GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod / (player._object.IsRoleD() ? 3f : 1f);
                    return false;
                }
            }

            __result = Mathf.Lerp(__instance.MinLightRadius, __instance.MaxLightRadius, t) *
                       GameOptionsManager.Instance.currentNormalGameOptions.CrewLightMod / (player._object.IsRoleD() ? 3f : 1f);
            return false;
        }
    }
}