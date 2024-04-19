using HarmonyLib;
using System.Linq;
using TownOfUs.CrewmateRoles.HaunterMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.ImpostorRoles.PoltergeistMod;

namespace TownOfUs
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.OnClick))]
    public class ClickGhostRole
    {
        public static void Prefix(PlayerControl __instance)
        {
            if (MeetingHud.Instance) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null || PlayerControl.LocalPlayer.Data.Tasks == null) return;
            var taskinfos = __instance.Data.Tasks.ToArray();
            var tasksLeft = taskinfos.Count(x => !x.Complete);
            if (__instance.Is(RoleEnum.Phantom))
            {
                if (tasksLeft <= CustomGameOptions.PhantomTasksRemaining)
                {
                    var role = Role.GetRole<Phantom>(__instance);
                    role.Caught = true;
                    role.Player.Exiled();
                    Utils.Rpc(CustomRPC.CatchPhantom, role.Player.PlayerId);
                }
            }
            else if (__instance.Is(RoleEnum.Haunter))
            {
                if (CustomGameOptions.HaunterCanBeClickedBy == HaunterCanBeClickedBy.ImpsOnly && !PlayerControl.LocalPlayer.Data.IsImpostor()) return;
                if (CustomGameOptions.HaunterCanBeClickedBy == HaunterCanBeClickedBy.NonCrew && !(PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(Faction.NeutralKilling))) return;
                if (tasksLeft <= CustomGameOptions.HaunterTasksRemainingClicked)
                {
                    var role = Role.GetRole<Haunter>(__instance);
                    role.Caught = true;
                    role.Player.Exiled();
                    Utils.Rpc(CustomRPC.CatchHaunter, role.Player.PlayerId);
                }
            }
            else if (__instance.Is(RoleEnum.Poltergeist))
            {
                if (CustomGameOptions.PoltergeistCanBeClickedBy == PoltergeistCanBeClickedBy.CrewOnly && !PlayerControl.LocalPlayer.Is(Faction.Crewmates)) return;
                if (CustomGameOptions.PoltergeistCanBeClickedBy == PoltergeistCanBeClickedBy.NonImps && PlayerControl.LocalPlayer.Data.IsImpostor()) return;
                if (tasksLeft <= CustomGameOptions.PoltergeistTasksRemainingClicked)
                {
                    var role = Role.GetRole<Poltergeist>(__instance);
                    role.Caught = true;
                    if (PlayerControl.LocalPlayer.Data.IsImpostor())
                    {
                        PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.killTimer * 2);
                    }
                    role.Player.Exiled();
                    Utils.Rpc(CustomRPC.CatchPoltergeist, role.Player.PlayerId);
                }
            }
            return;
        }
    }
}