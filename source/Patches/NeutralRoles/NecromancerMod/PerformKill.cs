using HarmonyLib;
using Reactor.Utilities.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Roles;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;
using System;
using AmongUs.GameOptions;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.NeutralRoles.NecromancerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformRevive
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.JKNecromancer);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Necromancer>(PlayerControl.LocalPlayer);
            if (__instance.isCoolingDown) return false;
            if (!__instance.isActiveAndEnabled) return false;
            if (role.ReviveTimer() != 0) return false;
            if (role.UsesLeft <= 0) return false;

            var flag2 = __instance.isCoolingDown;
            if (flag2) return false;
            if (!__instance.enabled) return false;
            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            if (role == null)
                return false;
            if (role.CurrentTarget == null)
                return false;
            if (Vector2.Distance(role.CurrentTarget.TruePosition,
                PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance) return false;
            var playerId = role.CurrentTarget.ParentId;

            role.ReviveCount += 1;
            role.LastRevived = DateTime.UtcNow;

            Utils.Rpc(CustomRPC.JKRevive, PlayerControl.LocalPlayer.PlayerId, playerId);

            Revive(role.CurrentTarget, role);
            return false;
        }

        public static void Revive(DeadBody target, Necromancer role)
        {
            var parentId = target.ParentId;
            var position = target.TruePosition;
            var player = Utils.PlayerById(parentId);

            var revived = new List<PlayerControl>();

            if (target != null)
            {
                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == target.ParentId) deadBody.gameObject.Destroy();
                }
            }

            player.Revive();
            Murder.KilledPlayers.Remove(
                Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == player.PlayerId));
            revived.Add(player);
            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));

            if (Patches.SubmergedCompatibility.isSubmerged() && PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                Patches.SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);
            }
            if (target != null) Object.Destroy(target.gameObject);

            if (player.IsLover() && CustomGameOptions.BothLoversDie)
            {
                var lover = Objective.GetObjective<Lover>(player).OtherLover.Player;

                lover.Revive();
                if (lover.Is(Faction.Impostors)) RoleManager.Instance.SetRole(lover, RoleTypes.Impostor);
                else RoleManager.Instance.SetRole(lover, RoleTypes.Crewmate);
                Murder.KilledPlayers.Remove(
                    Murder.KilledPlayers.FirstOrDefault(x => x.PlayerId == lover.PlayerId));
                revived.Add(lover);
                Role.GetRole(lover).FactionOverride = FactionOverride.Undead;
                Role.GetRole(lover).RegenTask();

                foreach (DeadBody deadBody in GameObject.FindObjectsOfType<DeadBody>())
                {
                    if (deadBody.ParentId == lover.PlayerId)
                    {
                        deadBody.gameObject.Destroy();
                    }
                }
            }

            if (revived.Any(x => x.AmOwner))
                try
                {
                    Minigame.Instance.Close();
                    Minigame.Instance.Close();
                }
                catch
                {
                }
            Role.GetRole(player).FactionOverride = FactionOverride.Undead;
            Role.GetRole(player).RegenTask();
            return;
        }
    }
}