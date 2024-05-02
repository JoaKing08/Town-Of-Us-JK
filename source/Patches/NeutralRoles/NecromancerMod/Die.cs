using HarmonyLib;
using TownOfUs.CrewmateRoles.AltruistMod;
using TownOfUs.Roles.Modifiers;
using TownOfUs.Roles;
using System.Linq;

namespace TownOfUs.NeutralRoles.NecromancerMod
{
    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.Die))]
    public class Die
    {
        public static bool Prefix(PlayerControl __instance, [HarmonyArgument(0)] DeathReason reason)
        {
            __instance.Data.IsDead = true;

            var flag3 = __instance.Is(RoleEnum.JKNecromancer);
            if (!flag3) return true;
            var undead = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(FactionOverride.Undead) && !x.Is(RoleEnum.JKNecromancer) && !x.Data.IsDead && !x.Data.Disconnected).ToList();

            if (reason == DeathReason.Exile)
            {
                KillButtonTarget.DontRevive = __instance.PlayerId;
                foreach (var player in undead)
                {
                    player.Exiled();
                }
            }
            else if (AmongUsClient.Instance.AmHost) foreach (var player in undead) Utils.RpcMurderPlayer(player, player);
            foreach (var player in undead) if (player.Is(RoleEnum.Sheriff))
            {
                var sheriff = Role.GetRole<Sheriff>(player);
                sheriff.IncorrectKills -= 1;
            }

            return true;
        }
    }
}