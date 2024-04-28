using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.CrewmateRoles.LookoutMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class Watch
    {
        [HarmonyPriority(Priority.Last)]
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Lookout))
            {
                var lookout = (Lookout)role;
                if (lookout.Watching)
                    lookout.StartWatch();
                else if (lookout.Enabled) lookout.EndWatch();
            }
        }
    }
}