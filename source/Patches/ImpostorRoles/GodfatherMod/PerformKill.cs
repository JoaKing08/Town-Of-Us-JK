using HarmonyLib;
using TownOfUs.Roles;
using TownOfUs.Extensions;
using UnityEngine;
using System;
using TownOfUs.Modifiers.UnderdogMod;
using TownOfUs.CrewmateRoles.MedicMod;
using AmongUs.GameOptions;
using TownOfUs.Roles.Modifiers;
using Reactor.Utilities;

namespace TownOfUs.ImpostorRoles.GodfatherMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Godfather)) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Godfather>(PlayerControl.LocalPlayer);
            var target = role.ClosestPlayer;
            if (__instance == role.RecruitButton)
            {
                if (!__instance.isActiveAndEnabled || role.ClosestPlayer == null) return false;
                if (__instance.isCoolingDown) return false;
                if (!__instance.isActiveAndEnabled) return false;
                if (role.RecruitTimer() != 0) return false;
                var interact = Utils.Interact(PlayerControl.LocalPlayer, target);
                if (interact[4] == true)
                {
                    if (role.ClosestPlayer.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, role.ClosestPlayer.PlayerId, (byte)role.RoleType, (byte)0);
                    Recruit(role, role.ClosestPlayer);
                    Utils.Rpc(CustomRPC.GodfatherRecruit, role.Player.PlayerId, role.ClosestPlayer.PlayerId);
                }
                if (interact[0] == true)
                {
                    role.LastRecruit = DateTime.UtcNow;
                    return false;
                }
                else if (interact[1] == true)
                {
                    role.LastRecruit = DateTime.UtcNow;
                    role.LastRecruit = role.LastRecruit.AddSeconds(-CustomGameOptions.ProtectKCReset);
                    return false;
                }
                else if (interact[3] == true) return false;
                return false;
            }
            return true;
        }
        public static void Recruit(Godfather godfather, PlayerControl target)
        {
            if (target == PlayerControl.LocalPlayer)
            {
                Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Impostor));
                Role.GetRole(target).Notification("You Were Recruited!", 1000 * CustomGameOptions.NotificationDuration);
            }
            godfather.Recruited = true;
            var targetRole = Role.GetRole(target);
            var bread = targetRole.BreadLeft;
            var factionOverride = targetRole.FactionOverride;
            var lastBlood = targetRole.LastBlood;
            var roleblocked = targetRole.Roleblocked;
            Role.RoleDictionary.Remove(target.PlayerId);
            var mafioso = new Mafioso(target);
            mafioso.BreadLeft = bread;
            mafioso.FactionOverride = factionOverride;
            mafioso.LastBlood = lastBlood;
            mafioso.Roleblocked = roleblocked;
            mafioso.RegenTask();
            if (target.Is(AbilityEnum.Assassin)) Ability.AbilityDictionary.Remove(target.PlayerId);
            if (CustomGameOptions.MafiosoAssassin) new Mafioso(target);
            target.Data.Role.TeamType = RoleTeamTypes.Impostor;
            RoleManager.Instance.SetRole(target, RoleTypes.Impostor);
            target.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor())
                {
                    player.nameText().color = Patches.Colors.Impostor;
                }
            }
            Role.RoleDictionary.Add(target.PlayerId, mafioso);
        }
    }
}