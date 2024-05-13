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
using TownOfUs.Roles.Horseman;
using Reactor.Utilities;

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
            if (__instance == role.KillButton && role.LastKiller)
            {
                if (role.KillTimer() == 0)
                {
                    if (role.ClosestPlayer == null) return false;
                    var distBetweenPlayers2 = Utils.GetDistBetweenPlayers(PlayerControl.LocalPlayer, role.ClosestPlayer);
                    var flag3 = distBetweenPlayers2 <
                                GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
                    if (!flag3) return false;

                    if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)1);
                    var interact2 = Utils.Interact(PlayerControl.LocalPlayer, role.ClosestPlayer, true);
                    if (interact2[4] == true) return false;
                    if (interact2[0] == true)
                    {
                        role.LastKill = DateTime.UtcNow;
                        return false;
                    }
                    else if (interact2[1] == true)
                    {
                        role.LastKill = DateTime.UtcNow;
                        role.LastKill.AddSeconds(CustomGameOptions.ProtectKCReset - (CustomGameOptions.RitualKillCooldown + CustomGameOptions.RitualKillCooldownIncrease * role.NecroKills));
                        return false;
                    }
                    else if (interact2[2] == true)
                    {
                        role.LastKill = DateTime.UtcNow;
                        role.LastKill.AddSeconds(CustomGameOptions.VestKCReset - (CustomGameOptions.RitualKillCooldown + CustomGameOptions.RitualKillCooldownIncrease * role.NecroKills));
                        return false;
                    }
                    else if (interact2[5] == true)
                    {
                        role.LastKill = DateTime.UtcNow;
                        role.LastKill = role.LastKill.AddSeconds(CustomGameOptions.BarrierCooldownReset - (CustomGameOptions.RitualKillCooldown + CustomGameOptions.RitualKillCooldownIncrease * role.NecroKills));
                        return false;
                    }
                    return false;
                }
                else return false;
            }
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
            var player = Utils.PlayerById(playerId);
            if ((player.IsInfected() || role.Player.IsInfected()) && !player.Is(RoleEnum.Plaguebearer))
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
            }
            if (player.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, playerId, (byte)role.RoleType, (byte)0);

            role.ReviveCount += 1;
            role.LastRevived = DateTime.UtcNow;

            Utils.Rpc(CustomRPC.JKRevive, PlayerControl.LocalPlayer.PlayerId, playerId);

            Revive(role.CurrentTarget, role);
            return false;
        }

        public static void Revive(DeadBody target, Necromancer role)
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic) && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.green));
                Role.GetRole(PlayerControl.LocalPlayer).Notification("Someone Has Been Revived!", 1000 * CustomGameOptions.NotificationDuration);
            }
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
            if (player.Is(Faction.Impostors)) RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
            else RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);

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
                if (lover.Is(Faction.Impostors)) RoleManager.Instance.SetRole(lover, RoleTypes.Impostor);
                else RoleManager.Instance.SetRole(lover, RoleTypes.Crewmate);

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