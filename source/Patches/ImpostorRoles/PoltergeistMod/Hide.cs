using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ImpostorRoles.PoltergeistMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class Hide
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Poltergeist))
            {
                var poltergeist = (Poltergeist) role;
                if (role.Player.Data.Disconnected) return;
                var caught = poltergeist.Caught;
                if (!caught)
                {
                    poltergeist.Fade();
                }
                else if (poltergeist.Faded)
                {
                    Utils.Unmorph(poltergeist.Player);
                    poltergeist.Player.myRend().color = Color.white;
                    poltergeist.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    poltergeist.Faded = false;
                    poltergeist.Player.MyPhysics.ResetMoveState();
                }
            }
        }
    }
}