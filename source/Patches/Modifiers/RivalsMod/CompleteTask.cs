using System.Linq;
using HarmonyLib;
using Reactor.Utilities;
using TownOfUs.Extensions;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.Modifiers.RivalsMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.CompleteTask))]
    public class CompleteTask
    {

        public static void Postfix(PlayerControl __instance)
        {
            if (!__instance.Is(ObjectiveEnum.Rival)) return;
            if (__instance.Data.IsDead) return;
            if (__instance.PlayerId != PlayerControl.LocalPlayer.PlayerId) return;
            var objective = Objective.GetObjective<Rival>(__instance);
            if (objective == null || objective.OtherRival == null || objective.OtherRival.Player == null || objective.OtherRival.Player.Data.IsDead || objective.OtherRival.Player.Data.Disconnected) return;
            var taskinfos = __instance.Data.Tasks.ToArray();

            var tasksLeft = taskinfos.Count(x => !x.Complete);
            if (tasksLeft == 0)
            {
                Utils.RpcMultiMurderPlayer(__instance, objective.OtherRival.Player);
            }
        }
    }
}