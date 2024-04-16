using System.Linq;
using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUs.Roles;

namespace TownOfUs.ImpostorRoles.PoltergeistMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.FixedUpdate))]
    public class UpdateArrows
    {
        public static void Postfix(PlayerControl __instance)
        {
            foreach (var role in Role.AllRoles.Where(x => x.RoleType == RoleEnum.Poltergeist))
            {
                var poltergeist = (Poltergeist)role;
                if (PlayerControl.LocalPlayer.Data.IsDead || poltergeist.Caught)
                {
                    poltergeist.PoltergeistArrows.DestroyAll();
                    poltergeist.PoltergeistArrows.Clear();
                    poltergeist.CrewArrows.DestroyAll();
                    poltergeist.CrewArrows.Clear();
                }

                foreach (var arrow in poltergeist.CrewArrows) arrow.target = poltergeist.Player.transform.position;

                foreach (var (arrow, target) in Utils.Zip(poltergeist.PoltergeistArrows, poltergeist.PoltergeistTargets))
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