using HarmonyLib;
using TownOfUs.CrewmateRoles.AltruistMod;
using TownOfUs.Roles.Modifiers;
using TownOfUs.Roles;

namespace TownOfUs.Modifiers.CooperatorsMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class Die
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] DeathReason reason)
        {
            __instance.Data.IsDead = true;

            var flag3 = __instance.IsCooperator() && CustomGameOptions.BothCooperatorsDie;
            if (!flag3) return true;
            var otherCooperator = Objective.GetObjective<Cooperator>(__instance).OtherCooperator.Player;
            if (otherCooperator.Data.IsDead) return true;

            if (reason == DeathReason.Exile)
            {
                KillButtonTarget.DontRevive = __instance.PlayerId;
                if (!otherCooperator.Is(RoleEnum.Pestilence) && !otherCooperator.Is(RoleEnum.Famine) && !otherCooperator.Is(RoleEnum.War) && !otherCooperator.Is(RoleEnum.Death)) otherCooperator.Exiled();
            }
            else if (AmongUsClient.Instance.AmHost && !otherCooperator.Is(RoleEnum.Pestilence) && !otherCooperator.Is(RoleEnum.Famine) && !otherCooperator.Is(RoleEnum.War) && !otherCooperator.Is(RoleEnum.Death)) Utils.RpcMurderPlayer(otherCooperator, otherCooperator);
            if (otherCooperator.Is(RoleEnum.Sheriff))
            {
                var sheriff = Role.GetRole<Sheriff>(otherCooperator);
                sheriff.IncorrectKills -= 1;
            }

            return true;
        }
    }
}