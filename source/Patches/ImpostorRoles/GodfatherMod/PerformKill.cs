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
using TownOfUs.CrewmateRoles.AurialMod;
using TownOfUs.Patches.ScreenEffects;
using Reactor.Utilities.Extensions;
using System.Linq;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.TrapperMod;

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
                if (role.Recruited) return false;
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

            if (target.Is(RoleEnum.Snitch))
            {
                var snitch = Role.GetRole<Snitch>(target);
                snitch.SnitchArrows.Values.DestroyAll();
                snitch.SnitchArrows.Clear();
                snitch.ImpArrows.DestroyAll();
                snitch.ImpArrows.Clear();
            }

            if (target == StartImitate.ImitatingPlayer) StartImitate.ImitatingPlayer = null;

            if (target.Is(RoleEnum.GuardianAngel))
            {
                var ga = Role.GetRole<GuardianAngel>(target);
                ga.UnProtect();
            }

            if (target.Is(RoleEnum.Medium))
            {
                var medRole = Role.GetRole<Medium>(target);
                medRole.MediatedPlayers.Values.DestroyAll();
                medRole.MediatedPlayers.Clear();
            }

            if (PlayerControl.LocalPlayer == target)
            {
                HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);
                if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator)) Footprint.DestroyAll(Role.GetRole<Investigator>(PlayerControl.LocalPlayer));

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff)) HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Engineer))
                {
                    var engineerRole = Role.GetRole<Engineer>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(engineerRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
                {
                    var trackerRole = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
                    trackerRole.TrackerArrows.Values.DestroyAll();
                    trackerRole.TrackerArrows.Clear();
                    UnityEngine.Object.Destroy(trackerRole.UsesText);
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
                    UnityEngine.Object.Destroy(transporterRole.UsesText);
                    if (transporterRole.TransportList != null)
                    {
                        transporterRole.TransportList.Toggle();
                        transporterRole.TransportList.SetVisible(false);
                        transporterRole.TransportList = null;
                        transporterRole.PressedButton = false;
                        transporterRole.TransportPlayer1 = null;
                    }
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
                {
                    var glitchRole = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);
                    glitchRole.Reset();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
                {
                    var veteranRole = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(veteranRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
                {
                    var trapperRole = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(trapperRole.UsesText);
                    trapperRole.traps.ClearTraps();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
                {
                    var detecRole = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
                    detecRole.ExamineButton.gameObject.SetActive(false);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial))
                {
                    var aurialRole = Role.GetRole<Aurial>(PlayerControl.LocalPlayer);
                    aurialRole.NormalVision = true;
                    SeeAll.AllToNormal();
                    CameraEffect.singleton.materials.Clear();
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Survivor))
                {
                    var survRole = Role.GetRole<Survivor>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(survRole.UsesText);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
                {
                    var gaRole = Role.GetRole<GuardianAngel>(PlayerControl.LocalPlayer);
                    UnityEngine.Object.Destroy(gaRole.UsesText);
                }
            }
            godfather.Recruited = true;
            var targetRole = Role.GetRole(target);
            var bread = targetRole.BreadLeft;
            var factionOverride = targetRole.FactionOverride;
            var lastBlood = targetRole.LastBlood;
            var roleblocked = targetRole.Roleblocked;
            var superRoleblocked = targetRole.SuperRoleblocked;
            Role.RoleDictionary.Remove(target.PlayerId);
            var mafioso = new Mafioso(target);
            mafioso.BreadLeft = bread;
            mafioso.FactionOverride = factionOverride;
            mafioso.LastBlood = lastBlood;
            mafioso.Roleblocked = roleblocked;
            mafioso.SuperRoleblocked = superRoleblocked;
            mafioso.RegenTask();
            if (target.Is(AbilityEnum.Assassin)) Ability.AbilityDictionary.Remove(target.PlayerId);
            if (CustomGameOptions.MafiosoAssassin) new Assassin(target);
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
        }
    }
}