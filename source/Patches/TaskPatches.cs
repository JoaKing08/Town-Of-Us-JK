using HarmonyLib;
using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs
{
    internal static class TaskPatches
    {
        [HarmonyPatch(typeof(GameData), nameof(GameData.RecomputeTaskCounts))]
        private class GameData_RecomputeTaskCounts
        {
            private static bool Prefix(GameData __instance)
            {
                __instance.TotalTasks = 0;
                __instance.CompletedTasks = 0;
                for (var i = 0; i < __instance.AllPlayers.Count; i++)
                {
                    var playerInfo = __instance.AllPlayers.ToArray()[i];
                    if (!playerInfo.Disconnected && playerInfo.Tasks != null && playerInfo.Object &&
                        ((GameOptionsManager.Instance.currentNormalGameOptions.GhostsDoTasks || !playerInfo.IsDead) && !playerInfo.IsImpostor() &&
                        !(
                            playerInfo._object.Is(RoleEnum.Jester) || playerInfo._object.Is(RoleEnum.Amnesiac) ||
                            playerInfo._object.Is(RoleEnum.Survivor) || playerInfo._object.Is(RoleEnum.GuardianAngel) ||
                            playerInfo._object.Is(RoleEnum.Glitch) || playerInfo._object.Is(RoleEnum.Executioner) ||
                            playerInfo._object.Is(RoleEnum.Arsonist) || playerInfo._object.Is(RoleEnum.Juggernaut) ||
                            playerInfo._object.Is(RoleEnum.Plaguebearer) || playerInfo._object.Is(RoleEnum.Pestilence) ||
                            playerInfo._object.Is(RoleEnum.Baker) || playerInfo._object.Is(RoleEnum.Famine) ||
                            playerInfo._object.Is(RoleEnum.Berserker) || playerInfo._object.Is(RoleEnum.War) ||
                            playerInfo._object.Is(RoleEnum.SoulCollector) || playerInfo._object.Is(RoleEnum.Death) ||
                            playerInfo._object.Is(RoleEnum.Werewolf) || playerInfo._object.Is(RoleEnum.Doomsayer) ||
                            playerInfo._object.Is(RoleEnum.Vampire) || playerInfo._object.Is(RoleEnum.Phantom) ||
                            playerInfo._object.Is(RoleEnum.Haunter) || playerInfo._object.Is(RoleEnum.SoloKiller) ||
                            playerInfo._object.Is(ObjectiveEnum.ImpostorAgent) || playerInfo._object.Is(ObjectiveEnum.ApocalypseAgent) ||
                            playerInfo._object.Is(RoleEnum.Pirate) || playerInfo._object.Is(RoleEnum.SerialKiller) ||
                            playerInfo._object.Is(RoleEnum.Inquisitor) || playerInfo._object.Is(RoleEnum.Witch) ||
                            playerInfo._object.Is(RoleEnum.CursedSoul) || playerInfo._object.Is(FactionOverride.Undead) ||
                            playerInfo._object.Is(FactionOverride.Recruit) || playerInfo._object.Is(RoleEnum.Jackal) ||
                            playerInfo._object.Is(RoleEnum.JKNecromancer)
                        )) || !PlayerControl.AllPlayerControls.ToArray().Any(x => x.Is(Faction.Crewmates) && x.Is(FactionOverride.None) && !x.Is(ObjectiveEnum.ImpostorAgent) && !x.Is(ObjectiveEnum.ApocalypseAgent) && !x.Data.IsDead && !x.Data.Disconnected))
                        for (var j = 0; j < playerInfo.Tasks.Count; j++)
                        {
                            __instance.TotalTasks++;
                            if (playerInfo.Tasks.ToArray()[j].Complete) __instance.CompletedTasks++;
                        }
                }

                return false;
            }
        }

        [HarmonyPatch(typeof(Console), nameof(Console.CanUse))]
        private class Console_CanUse
        {
            private static bool Prefix(Console __instance, [HarmonyArgument(0)] GameData.PlayerInfo playerInfo, ref float __result)
            {
                var playerControl = playerInfo.Object;

                var flag = playerControl.Is(RoleEnum.Glitch)
                           || playerControl.Is(RoleEnum.Jester)
                           || playerControl.Is(RoleEnum.Executioner)
                           || playerControl.Is(RoleEnum.Juggernaut)
                           || playerControl.Is(RoleEnum.Arsonist)
                           || playerControl.Is(RoleEnum.Plaguebearer)
                           || playerControl.Is(RoleEnum.Pestilence)
                           || playerControl.Is(RoleEnum.Werewolf)
                           || playerControl.Is(RoleEnum.Doomsayer)
                           || playerControl.Is(RoleEnum.Vampire)
                           || CustomGameOptions.GameMode == GameMode.Teams
                           || playerControl.Is(RoleEnum.SoloKiller)
                           || playerControl.Is(RoleEnum.Baker) 
                           || playerControl.Is(RoleEnum.Famine)
                           || playerControl.Is(RoleEnum.Berserker)
                           || playerControl.Is(RoleEnum.War)
                           || playerControl.Is(RoleEnum.SoulCollector)
                           || playerControl.Is(RoleEnum.Death)
                           || playerControl.Is(RoleEnum.Pirate)
                           || playerControl.Is(RoleEnum.SerialKiller)
                           || playerControl.Is(RoleEnum.Inquisitor)
                           || playerControl.Is(RoleEnum.Witch)
                           || playerControl.Is(RoleEnum.Jackal)
                           || playerControl.Is(RoleEnum.JKNecromancer);

                // If the console is not a sabotage repair console
                if (flag && !__instance.AllowImpostor)
                {
                    __result = float.MaxValue;
                    return false;
                }

                return true;
            }
        }
    }
}