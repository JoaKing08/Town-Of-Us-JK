using HarmonyLib;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.PoltergeistMod
{
    [HarmonyPatch(typeof(SpawnInMinigame), nameof(SpawnInMinigame.Begin))]
    public class NoSpawn
    {
        public static bool Prefix(SpawnInMinigame __instance)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Poltergeist))
            {
                var caught = Role.GetRole<Poltergeist>(PlayerControl.LocalPlayer).Caught;
                if (!caught)
                {
                    __instance.Close();
                    return false;
                }
            }

            return true;
        }
    }
}