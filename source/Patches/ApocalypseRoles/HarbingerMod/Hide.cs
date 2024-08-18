using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.ApocalypseRoles.HarbingerMod
{
    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    [HarmonyPriority(Priority.Last)]
    public class Hide
    {
        public static void Postfix(HudManager __instance)
        {
            foreach (var role in Role.GetRoles(RoleEnum.Harbinger))
            {
                var harbinger = (Harbinger) role;
                if (role.Player.Data.Disconnected) return;
                var caught = harbinger.Caught;
                if (!caught)
                {
                    harbinger.Fade();
                }
                else if (harbinger.Faded)
                {
                    Utils.Unmorph(harbinger.Player);
                    harbinger.Player.myRend().color = Color.white;
                    harbinger.Player.gameObject.layer = LayerMask.NameToLayer("Ghost");
                    harbinger.Faded = false;
                    harbinger.Player.MyPhysics.ResetMoveState();
                }
            }
        }
    }
}