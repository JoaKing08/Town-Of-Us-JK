using HarmonyLib;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using UnityEngine;

namespace TownOfUs.NeutralRoles.ExecutionerMod
{
    public enum OnTargetDead
    {
        Crew,
        Amnesiac,
        Survivor,
        Jester,
        CursedSoul
    }

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
    public class TargetColor
    {
        private static void UpdateMeeting(MeetingHud __instance, Executioner role)
        {
            foreach (var player in __instance.playerStates)
                if (player.TargetPlayerId == role.target.PlayerId)
                    player.NameText.color = Color.black;
        }

        private static void Postfix(HudManager __instance)
        {
            if (PlayerControl.AllPlayerControls.Count <= 1) return;
            if (PlayerControl.LocalPlayer == null) return;
            if (PlayerControl.LocalPlayer.Data == null) return;
            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Executioner)) return;
            if (PlayerControl.LocalPlayer.Data.IsDead) return;

            var role = Role.GetRole<Executioner>(PlayerControl.LocalPlayer);

            if (MeetingHud.Instance != null) UpdateMeeting(MeetingHud.Instance, role);

            if (role.target && role.target.nameText()) role.target.nameText().color = Color.black;

            if (!role.target.Data.IsDead && !role.target.Data.Disconnected && !role.target.Is(RoleEnum.Vampire)) return;
            if (role.TargetVotedOut) return;

            Utils.Rpc(CustomRPC.ExecutionerToJester, PlayerControl.LocalPlayer.PlayerId);

            ExeToJes(PlayerControl.LocalPlayer);
        }

        public static void ExeToJes(PlayerControl player)
        {
            var role = Role.GetRole(player);
            var oldBread = role.BreadLeft;
            var oldFaction = role.FactionOverride;
            player.myTasks.RemoveAt(0);
            Role.RoleDictionary.Remove(player.PlayerId);


            if (CustomGameOptions.OnTargetDead == OnTargetDead.Jester)
            {
                var jester = new Jester(player);
                jester.SpawnedAs = false;
                jester.BreadLeft = oldBread;
                jester.FactionOverride = oldFaction;
                jester.RegenTask();
            }
            else if (CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac)
            {
                var amnesiac = new Amnesiac(player);
                amnesiac.SpawnedAs = false;
                amnesiac.BreadLeft = oldBread;
                amnesiac.FactionOverride = oldFaction;
                amnesiac.RegenTask();
            }
            else if (CustomGameOptions.OnTargetDead == OnTargetDead.Survivor)
            {
                var surv = new Survivor(player);
                surv.SpawnedAs = false;
                surv.BreadLeft = oldBread;
                surv.FactionOverride = oldFaction;
                surv.RegenTask();
            }
            else if (CustomGameOptions.OnTargetDead == OnTargetDead.CursedSoul)
            {
                var cursedSoul = new CursedSoul(player);
                cursedSoul.SpawnedAs = false;
                cursedSoul.BreadLeft = oldBread;
                cursedSoul.FactionOverride = oldFaction;
                cursedSoul.RegenTask();
            }
            else
            {
                var crewmate = new Crewmate(player);
                crewmate.BreadLeft = oldBread;
                crewmate.FactionOverride = oldFaction;
            }
        }
    }
}