using HarmonyLib;
using System.Linq;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;

namespace TownOfUs.CrewmateRoles.SageMod
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Start))]
    public class MeetingStart
    {
        public static void Postfix(MeetingHud __instance)
        {
            if (PlayerControl.LocalPlayer.Data.IsDead) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Sage)) return;
            var role = Role.GetRole<Sage>(PlayerControl.LocalPlayer);
            if (role.FirstPlayer != byte.MaxValue && !Utils.PlayerById(role.FirstPlayer).Data.IsDead && !Utils.PlayerById(role.FirstPlayer).Data.Disconnected && role.SecondPlayer != byte.MaxValue && !Utils.PlayerById(role.SecondPlayer).Data.IsDead && !Utils.PlayerById(role.SecondPlayer).Data.Disconnected)
            {
                var result = false;
                var firstPlayer = Utils.PlayerById(role.FirstPlayer);
                var secondPlayer = Utils.PlayerById(role.SecondPlayer);
                if ((firstPlayer.Is(RoleEnum.Witch) && !(secondPlayer.Is(Faction.Crewmates) && secondPlayer.Is(FactionOverride.None))) || (secondPlayer.Is(RoleEnum.Witch) && !(firstPlayer.Is(Faction.Crewmates) && firstPlayer.Is(FactionOverride.None))))
                {
                    result = true;
                }
                if (secondPlayer.Is(Utils.GetRole(firstPlayer)) && firstPlayer.Is(FactionOverride.None) && secondPlayer.Is(FactionOverride.None))
                {
                    result = true;
                }
                if ((firstPlayer.Is(RoleEnum.Survivor) && firstPlayer.Is(FactionOverride.None) && !(secondPlayer.Is(Faction.NeutralEvil) && !secondPlayer.Is(RoleEnum.Witch)) && !secondPlayer.Is(Faction.NeutralChaos)) || (secondPlayer.Is(RoleEnum.Survivor) && secondPlayer.Is(FactionOverride.None) && !(firstPlayer.Is(Faction.NeutralEvil) && !firstPlayer.Is(RoleEnum.Witch)) && !firstPlayer.Is(Faction.NeutralChaos)))
                {
                    result = true;
                }
                if (secondPlayer.Is(Role.GetRole(firstPlayer).Faction) && firstPlayer.Is(FactionOverride.None) && secondPlayer.Is(FactionOverride.None))
                {
                    result = true;
                }
                if (secondPlayer.Is(Role.GetRole(firstPlayer).FactionOverride) && !firstPlayer.Is(FactionOverride.None))
                {
                    result = true;
                }
                if ((secondPlayer.Data.IsImpostor() && secondPlayer.Is(FactionOverride.None) && firstPlayer.Is(ObjectiveEnum.ImpostorAgent) && firstPlayer.Is(FactionOverride.None)) || (firstPlayer.Data.IsImpostor() && firstPlayer.Is(FactionOverride.None) && secondPlayer.Is(ObjectiveEnum.ImpostorAgent) && secondPlayer.Is(FactionOverride.None)))
                {
                    result = true;
                }
                if ((secondPlayer.Is(Faction.NeutralApocalypse) && secondPlayer.Is(FactionOverride.None) && firstPlayer.Is(ObjectiveEnum.ApocalypseAgent) && firstPlayer.Is(FactionOverride.None)) || (firstPlayer.Is(Faction.NeutralApocalypse) && firstPlayer.Is(FactionOverride.None) && secondPlayer.Is(ObjectiveEnum.ApocalypseAgent) && secondPlayer.Is(FactionOverride.None)))
                {
                    result = true;
                }
                if (Objective.GetObjective(firstPlayer) != null && secondPlayer.Is(Objective.GetObjective(firstPlayer).ObjectiveType) && firstPlayer.Is(FactionOverride.None) && secondPlayer.Is(FactionOverride.None))
                {
                    result = true;
                }
                if (firstPlayer.Is(RoleEnum.GuardianAngel) && firstPlayer.Is(FactionOverride.None))
                {
                    var target = Role.GetRole<GuardianAngel>(firstPlayer).target;
                    if (target.PlayerId == secondPlayer.PlayerId)
                    {
                        result = true;
                    }
                    if ((target.Is(RoleEnum.Witch) && !(secondPlayer.Is(Faction.Crewmates) && secondPlayer.Is(FactionOverride.None))) || (secondPlayer.Is(RoleEnum.Witch) && !(target.Is(Faction.Crewmates) && target.Is(FactionOverride.None))))
                    {
                        result = true;
                    }
                    if (secondPlayer.Is(Utils.GetRole(target)) && target.Is(FactionOverride.None) && secondPlayer.Is(FactionOverride.None))
                    {
                        result = true;
                    }
                    if ((target.Is(RoleEnum.Survivor) && target.Is(FactionOverride.None) && !(secondPlayer.Is(Faction.NeutralEvil) && !secondPlayer.Is(RoleEnum.Witch)) && !secondPlayer.Is(Faction.NeutralChaos)) || (secondPlayer.Is(RoleEnum.Survivor) && secondPlayer.Is(FactionOverride.None) && !(target.Is(Faction.NeutralEvil) && !target.Is(RoleEnum.Witch)) && !target.Is(Faction.NeutralChaos)))
                    {
                        result = true;
                    }
                    if (secondPlayer.Is(Role.GetRole(target).Faction) && target.Is(FactionOverride.None) && secondPlayer.Is(FactionOverride.None))
                    {
                        result = true;
                    }
                    if (secondPlayer.Is(Role.GetRole(target).FactionOverride))
                    {
                        result = true;
                    }
                    if ((secondPlayer.Data.IsImpostor() && secondPlayer.Is(FactionOverride.None) && target.Is(ObjectiveEnum.ImpostorAgent) && target.Is(FactionOverride.None)) || (target.Data.IsImpostor() && target.Is(FactionOverride.None) && secondPlayer.Is(ObjectiveEnum.ImpostorAgent) && secondPlayer.Is(FactionOverride.None)))
                    {
                        result = true;
                    }
                    if ((secondPlayer.Is(Faction.NeutralApocalypse) && secondPlayer.Is(FactionOverride.None) && target.Is(ObjectiveEnum.ApocalypseAgent) && target.Is(FactionOverride.None)) || (target.Is(Faction.NeutralApocalypse) && target.Is(FactionOverride.None) && secondPlayer.Is(ObjectiveEnum.ApocalypseAgent) && secondPlayer.Is(FactionOverride.None)))
                    {
                        result = true;
                    }
                    if (Objective.GetObjective(target) != null && secondPlayer.Is(Objective.GetObjective(target).ObjectiveType) && target.Is(FactionOverride.None) && secondPlayer.Is(FactionOverride.None))
                    {
                        result = true;
                    }
                }
                if (secondPlayer.Is(RoleEnum.GuardianAngel) && secondPlayer.Is(FactionOverride.None))
                {
                    var target = Role.GetRole<GuardianAngel>(secondPlayer).target;
                    if (target.PlayerId == secondPlayer.PlayerId)
                    {
                        result = true;
                    }
                    if ((firstPlayer.Is(RoleEnum.Witch) && !(target.Is(Faction.Crewmates) && target.Is(FactionOverride.None))) || (target.Is(RoleEnum.Witch) && !(firstPlayer.Is(Faction.Crewmates) && firstPlayer.Is(FactionOverride.None))))
                    {
                        result = true;
                    }
                    if (target.Is(Utils.GetRole(firstPlayer)) && firstPlayer.Is(FactionOverride.None) && target.Is(FactionOverride.None))
                    {
                        result = true;
                    }
                    if ((firstPlayer.Is(RoleEnum.Survivor) && firstPlayer.Is(FactionOverride.None) && !(target.Is(Faction.NeutralEvil) && !target.Is(RoleEnum.Witch)) && !target.Is(Faction.NeutralChaos)) || (target.Is(RoleEnum.Survivor) && target.Is(FactionOverride.None) && !(firstPlayer.Is(Faction.NeutralEvil) && !firstPlayer.Is(RoleEnum.Witch)) && !firstPlayer.Is(Faction.NeutralChaos)))
                    {
                        result = true;
                    }
                    if (target.Is(Role.GetRole(firstPlayer).Faction) && firstPlayer.Is(FactionOverride.None) && target.Is(FactionOverride.None))
                    {
                        result = true;
                    }
                    if (target.Is(Role.GetRole(firstPlayer).FactionOverride))
                    {
                        result = true;
                    }
                    if ((target.Data.IsImpostor() && target.Is(FactionOverride.None) && firstPlayer.Is(ObjectiveEnum.ImpostorAgent) && firstPlayer.Is(FactionOverride.None)) || (firstPlayer.Data.IsImpostor() && firstPlayer.Is(FactionOverride.None) && target.Is(ObjectiveEnum.ImpostorAgent) && target.Is(FactionOverride.None)))
                    {
                        result = true;
                    }
                    if ((target.Is(Faction.NeutralApocalypse) && target.Is(FactionOverride.None) && firstPlayer.Is(ObjectiveEnum.ApocalypseAgent) && firstPlayer.Is(FactionOverride.None)) || (firstPlayer.Is(Faction.NeutralApocalypse) && firstPlayer.Is(FactionOverride.None) && target.Is(ObjectiveEnum.ApocalypseAgent) && target.Is(FactionOverride.None)))
                    {
                        result = true;
                    }
                    if (Objective.GetObjective(firstPlayer) != null && target.Is(Objective.GetObjective(firstPlayer).ObjectiveType) && firstPlayer.Is(FactionOverride.None) && target.Is(FactionOverride.None))
                    {
                        result = true;
                    }
                }
                if (UnityEngine.Random.RandomRangeInt(0, 100) >= CustomGameOptions.CompareAccuracy)
                {
                    result = !result;
                }
                DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, result ? $"You think that <b>{firstPlayer.GetDefaultOutfit().PlayerName}</b> and <b>{secondPlayer.GetDefaultOutfit().PlayerName}</b> seem like they are <b><color=#00FF00FF>friends</color></b>." : $"You think that <b>{firstPlayer.GetDefaultOutfit().PlayerName}</b> and <b>{secondPlayer.GetDefaultOutfit().PlayerName}</b> seem like they are <b><color=#FF0000FF>enemies</color></b>.");
            }
        }
    }
}