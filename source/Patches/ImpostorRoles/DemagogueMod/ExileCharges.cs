using System;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUs.Patches;
using TownOfUs.Roles;
using TownOfUs.CrewmateRoles.AltruistMod;

namespace TownOfUs.ImpostorRoles.DemagogueMod
{
    [HarmonyPatch(typeof(AirshipExileController), nameof(AirshipExileController.WrapUpAndSpawn))]
    public static class AirshipExileController_WrapUpAndSpawn
    {
        public static void Postfix(AirshipExileController __instance) => ExilePros.ExileControllerPostfix(__instance);
    }

    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public class ExilePros
    {
        public static void ExileControllerPostfix(ExileController __instance)
        {
            foreach (Demagogue demagogue in Role.GetRoles(RoleEnum.Demagogue))
            {
                var exiled = __instance.exiled?.Object;
                if (exiled != null && exiled.Is(Faction.Crewmates) && exiled.Is(FactionOverride.None) && !exiled.Is(ObjectiveEnum.ImpostorAgent) && !exiled.Is(ObjectiveEnum.ApocalypseAgent) && !exiled.IsLover())
                {
                    demagogue.Charges += CustomGameOptions.ChargesPerWrongEjection;
                    Utils.Rpc(CustomRPC.DemagogueCharges, demagogue.Charges, demagogue.Player.PlayerId);
                }
            }
        }

        public static void Postfix(ExileController __instance) => ExileControllerPostfix(__instance);

        [HarmonyPatch(typeof(Object), nameof(Object.Destroy), new Type[] { typeof(GameObject) })]
        public static void Prefix(GameObject obj)
        {
            if (!SubmergedCompatibility.Loaded || GameOptionsManager.Instance?.currentNormalGameOptions?.MapId != 6) return;
            if (obj.name?.Contains("ExileCutscene") == true) ExileControllerPostfix(ExileControllerPatch.lastExiled);
        }
    }
}