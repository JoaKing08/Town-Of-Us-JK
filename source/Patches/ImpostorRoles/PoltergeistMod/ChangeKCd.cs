using HarmonyLib;
using System.Linq;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.PoltergeistMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.ClearTasks))]
    public class StopClearTasks
    {
        public static bool Prefix()
        {
            return false;
        }
    }
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.MurderPlayer))]
    public class ChangeKCd
    {
        public static void Postfix(PlayerControl __instance, [HarmonyArgument(0)] PlayerControl target)
        {
            if (Utils.PoltergeistTasks())
                ((Poltergeist)Role.GetRoles(RoleEnum.Poltergeist).FirstOrDefault()).SetKillTimer(__instance.Is(ModifierEnum.Underdog), __instance);
        }
    }
    [HarmonyPatch(typeof(ExileController), nameof(ExileController.WrapUp))]
    public static class HUDClose
    {
        public static void Postfix()
        {
            if (Utils.PoltergeistTasks())
                ((Poltergeist)Role.GetRoles(RoleEnum.Poltergeist).FirstOrDefault()).SetKillTimer(PlayerControl.LocalPlayer.Is(ModifierEnum.Underdog), PlayerControl.LocalPlayer);
        }
    }
}