using HarmonyLib;
using TownOfUs.Roles;
using UnityEngine;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.TrapperMod;
using System.Collections.Generic;
using TownOfUs.CrewmateRoles.AurialMod;
using TownOfUs.Patches.ScreenEffects;

namespace TownOfUs.CrewmateRoles.ImitatorMod
{

    [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
    class StartMeetingPatch
    {
        public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)] GameData.PlayerInfo meetingTarget)
        {
            if (__instance == null)
            {
                return;
            }
            if (StartImitate.ImitatingPlayer != null && !StartImitate.ImitatingPlayer.Is(RoleEnum.Traitor))
            {
                List<RoleEnum> trappedPlayers = null;
                List<string> Messages = null;
                PlayerControl LastInspectedPlayer = null;
                PlayerControl confessingPlayer = null;
                byte firstPlayer = byte.MaxValue;
                byte secondPlayer = byte.MaxValue;
                byte visionPlayer = byte.MaxValue;
                List<byte> playersInteracted = null;
                List<byte> interactingPlayers = null;

                if (PlayerControl.LocalPlayer == StartImitate.ImitatingPlayer)
                {
                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) Footprint.DestroyAll(Role.GetRole<Investigator>(PlayerControl.LocalPlayer));

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
                    {
                        var engineerRole = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);
                        Object.Destroy(engineerRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
                    {
                        var trackerRole = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
                        trackerRole.TrackerArrows.Values.DestroyAll();
                        trackerRole.TrackerArrows.Clear();
                        Object.Destroy(trackerRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.VampireHunter))
                    {
                        var vhRole = Role.GetRole<VampireHunter>(PlayerControl.LocalPlayer);
                        Object.Destroy(vhRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                    {
                        var mysticRole = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);
                        mysticRole.BodyArrows.Values.DestroyAll();
                        mysticRole.BodyArrows.Clear();
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                    {
                        var transporterRole = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);
                        Object.Destroy(transporterRole.UsesText);
                        if (transporterRole.TransportList != null)
                        {
                            transporterRole.TransportList.Toggle();
                            transporterRole.TransportList.SetVisible(false);
                            transporterRole.TransportList = null;
                            transporterRole.PressedButton = false;
                            transporterRole.TransportPlayer1 = null;
                        }
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
                    {
                        var veteranRole = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);
                        Object.Destroy(veteranRole.UsesText);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
                    {
                        var trapperRole = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);
                        Object.Destroy(trapperRole.UsesText);
                        trapperRole.traps.ClearTraps();
                        trappedPlayers = trapperRole.trappedPlayers;
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
                    {
                        var oracleRole = Role.GetRole<Oracle>(PlayerControl.LocalPlayer);
                        confessingPlayer = oracleRole.Confessor;
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
                    {
                        var detecRole = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
                        detecRole.ClosestPlayer = null;
                        detecRole.ExamineButton.gameObject.SetActive(false);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial))
                    {
                        var aurialRole = Role.GetRole<Aurial>(PlayerControl.LocalPlayer);
                        aurialRole.NormalVision = true;
                        SeeAll.AllToNormal();
                        CameraEffect.singleton.materials.Clear();
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Spy))
                    {
                        var spyRole = Role.GetRole<Spy>(PlayerControl.LocalPlayer);
                        Messages = spyRole.Messages;
                        spyRole.BuggedPlayers = new List<byte>();
                        Utils.Rpc(CustomRPC.UnbugPlayers, PlayerControl.LocalPlayer.PlayerId);
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Inspector))
                    {
                        var inspectorRole = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);
                        LastInspectedPlayer = inspectorRole.LastInspectedPlayer;
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.TavernKeeper))
                    {
                        var tavernKeeperRole = Role.GetRole<TavernKeeper>(PlayerControl.LocalPlayer);
                        foreach (var player in tavernKeeperRole.DrunkPlayers)
                        {
                            Role.GetRole(player).Roleblocked = false;
                            Utils.Rpc(CustomRPC.UnroleblockPlayer, player.PlayerId, false);
                        }
                        tavernKeeperRole.DrunkPlayers = new List<PlayerControl>();
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Sage))
                    {
                        var sageRole = Role.GetRole<Sage>(PlayerControl.LocalPlayer);
                        firstPlayer = sageRole.FirstPlayer;
                        secondPlayer = sageRole.SecondPlayer;
                    }

                    if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
                    {
                        var mysticRole = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);
                        visionPlayer = mysticRole.VisionPlayer;
                        interactingPlayers = mysticRole.InteractingPlayers;
                        playersInteracted = mysticRole.PlayersInteracted;
                    }

                    if (!PlayerControl.LocalPlayer.Is(RoleEnum.Investigator) && !PlayerControl.LocalPlayer.Is(RoleEnum.Mystic)) DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
                }

                if (StartImitate.ImitatingPlayer.Is(RoleEnum.Medium))
                {
                    var medRole = Role.GetRole<Medium>(StartImitate.ImitatingPlayer);
                    medRole.MediatedPlayers.Values.DestroyAll();
                    medRole.MediatedPlayers.Clear();
                }

                var role = Role.GetRole(StartImitate.ImitatingPlayer);
                var killsList = (role.Kills, role.CorrectKills, role.IncorrectKills, role.CorrectAssassinKills, role.IncorrectAssassinKills);
                var bread = role.BreadLeft;
                var factionOverride = role.FactionOverride;
                Role.RoleDictionary.Remove(StartImitate.ImitatingPlayer.PlayerId);
                var imitator = new Imitator(StartImitate.ImitatingPlayer);
                imitator.trappedPlayers = trappedPlayers;
                imitator.confessingPlayer = confessingPlayer;
                imitator.LastInspectedPlayer = LastInspectedPlayer;
                imitator.Messages = Messages;
                imitator.SageFirst = firstPlayer;
                imitator.SageSecond = secondPlayer;
                imitator.VisionPlayer = visionPlayer;
                imitator.PlayersInteracted = playersInteracted;
                imitator.InteractingPlayers = interactingPlayers;
                var newRole = Role.GetRole(StartImitate.ImitatingPlayer);
                newRole.RemoveFromRoleHistory(newRole.RoleType);
                newRole.Kills = killsList.Kills;
                newRole.CorrectKills = killsList.CorrectKills;
                newRole.IncorrectKills = killsList.IncorrectKills;
                newRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
                newRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                newRole.BreadLeft = bread;
                newRole.FactionOverride = factionOverride;
                Role.GetRole<Imitator>(StartImitate.ImitatingPlayer).ImitatePlayer = null;
                StartImitate.ImitatingPlayer = null;
            }
            return;
        }
    }
}
