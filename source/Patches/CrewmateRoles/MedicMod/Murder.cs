using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs;

namespace TownOfUs.CrewmateRoles.MedicMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public class Murder
    {
        public static List<DeadPlayer> KilledPlayers = new List<DeadPlayer>();

        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            var deadBody = new DeadPlayer
            {
                PlayerId = target.PlayerId,
                KillerId = __instance.PlayerId,
                KillTime = DateTime.UtcNow,
                KillerVented = false,
                KillerEscapeAbility = (Role.GetRole<Swooper>(__instance).IsSwooped == true) || (Role.GetRole<Morphling>(__instance).Morphed == true) || (Role.GetRole<Venerer>(__instance).IsCamouflaged == true) || (Role.GetRole<Glitch>(__instance).IsUsingMimic == true),
                KillerRunTo = KillerDirection.None,
                KillerKillAbility = Role.GetRole(target).KilledByAbility,
                BodyPosition = target.transform.position,
                KillersRole = Role.GetRole(__instance).RoleType,
                KillersFaction = Role.GetRole(__instance).Faction

            };

            KilledPlayers.Add(deadBody);
        }
    }
    public enum KillerDirection
    {
        None,
        North,
        South,
        East,
        West
    }
}