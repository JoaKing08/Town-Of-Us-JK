using System.Linq;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.ApocalypseRoles.HarbingerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows
    {
        public static void Postfix(PlayerControl __instance)
        {
            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Harbinger))
            {
                var harbinger = (Harbinger)role;
                if (PlayerControl.LocalPlayer.Data.IsDead || harbinger.Caught)
                {
                    harbinger.HarbingerArrows.DestroyAll();
                    harbinger.HarbingerArrows.Clear();
                    harbinger.CrewArrows.DestroyAll();
                    harbinger.CrewArrows.Clear();
                }

                foreach (var arrow in harbinger.CrewArrows) arrow.target = harbinger.Player.transform.position;

                foreach (var (arrow, target) in Utils.Zip(harbinger.HarbingerArrows, harbinger.HarbingerTargets))
                {
                    if (target.Data.IsDead)
                    {
                        arrow.Destroy();
                        if (arrow.gameObject != null) arrow.gameObject.Destroy();
                    }

                    arrow.target = target.transform.position;
                }
            }
        }
    }
}