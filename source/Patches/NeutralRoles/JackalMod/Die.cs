using HarmonyLib;
using TownOfUs.CrewmateRoles.AltruistMod;
using TownOfUs.Roles.Modifiers;
using TownOfUs.Roles;
using System.Linq;

namespace TownOfUs.NeutralRoles.JackalMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class Die
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] DeathReason reason)
        {
            __instance.Data.IsDead = true;
            var flag3 = __instance.Is(FactionOverride.Recruit) && !__instance.Is(RoleEnum.Jackal) && CustomGameOptions.RecruistLifelink;
            if (!flag3) return true;
            var otherRecruit = PlayerControl.AllPlayerControls.ToArray().First(x => x.PlayerId != __instance.PlayerId && x.Is(FactionOverride.Recruit) && !x.Is(RoleEnum.Jackal));
            if (otherRecruit.Data.IsDead) return true;

            if (reason == DeathReason.Exile)
            {
                KillButtonTarget.DontRevive = __instance.PlayerId;
                if (!otherRecruit.Is(RoleEnum.Pestilence) && !otherRecruit.Is(RoleEnum.Famine) && !otherRecruit.Is(RoleEnum.War) && !otherRecruit.Is(RoleEnum.Death)) otherRecruit.Exiled();
            }
            else if (AmongUsClient.Instance.AmHost && !otherRecruit.Is(RoleEnum.Pestilence) && !otherRecruit.Is(RoleEnum.Famine) && !otherRecruit.Is(RoleEnum.War) && !otherRecruit.Is(RoleEnum.Death)) Utils.RpcMurderPlayer(otherRecruit, otherRecruit);
            if (otherRecruit.Is(RoleEnum.Sheriff))
            {
                var sheriff = Role.GetRole<Sheriff>(otherRecruit);
                sheriff.IncorrectKills -= 1;
            }

            return true;
        }
    }
}