using HarmonyLib;
using TownOfUs.CrewmateRoles.AltruistMod;
using TownOfUs.Roles.Modifiers;
using TownOfUs.Roles;
using System.Linq;

namespace TownOfUs.ImpostorRoles.GodfatherMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class Die
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] DeathReason reason)
        {
            __instance.Data.IsDead = true;
            var flag3 = __instance.Is(RoleEnum.Godfather) && CustomGameOptions.MafiosoLifelink;
            if (!flag3) return true;
            var mafioso = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.Is(RoleEnum.Mafioso));
            if (mafioso == null) return true;
            if (mafioso.Data.IsDead) return true;

            if (reason == DeathReason.Exile)
            {
                KillButtonTarget.DontRevive = __instance.PlayerId;
                mafioso.Exiled();
            }
            else if (AmongUsClient.Instance.AmHost) Utils.RpcMurderPlayer(mafioso, mafioso);

            return true;
        }
    }
}