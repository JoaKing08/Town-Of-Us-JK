﻿using HarmonyLib;
using Hazel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.Patches;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;
using TownOfUs.Roles.Modifiers;
using Il2CppInterop.Runtime.InteropTypes;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using Object = UnityEngine.Object;
using PerformKill = TownOfUs.Modifiers.UnderdogMod.PerformKill;
using Random = UnityEngine.Random;
using AmongUs.GameOptions;
using TownOfUs.CrewmateRoles.TrapperMod;
using TownOfUs.ImpostorRoles.BomberMod;
using TownOfUs.CrewmateRoles.VampireHunterMod;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.CrewmateRoles.AurialMod;
using Reactor.Networking;
using Reactor.Networking.Extensions;
using TownOfUs.Roles.Teams;
using TownOfUs.Roles.Horseman;

namespace TownOfUs
{
    [HarmonyPatch]
    public static class Utils
    {
        internal static bool ShowDeadBodies = false;
        private static GameData.PlayerInfo voteTarget = null;

        public static bool CheckImpostorFriendlyFire()
        {
            bool undercoverEnabled = CustomGameOptions.UndercoverKillEachother;
            bool anyUndercoverImpostor = PlayerControl.AllPlayerControls.ToArray().Any(x => x.Is(RoleEnum.Undercover) && Utils.UndercoverIsImpostor());

            return undercoverEnabled && anyUndercoverImpostor && CustomGameOptions.GameMode != GameMode.Cultist;
        }
        public static bool CheckApocalypseFriendlyFire()
        {
            bool undercoverEnabled = CustomGameOptions.UndercoverKillEachother;
            bool anyUndercoverApocalypse = PlayerControl.AllPlayerControls.ToArray().Any(x => x.Is(RoleEnum.Undercover) && Utils.UndercoverIsApocalypse());

            return undercoverEnabled && anyUndercoverApocalypse;
        }
        public static void Morph(PlayerControl player, PlayerControl MorphedPlayer, bool resetAnim = false)
        {
            if (CamouflageUnCamouflage.IsCamoed) return;
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial) && !Role.GetRole<Aurial>(PlayerControl.LocalPlayer).NormalVision) return;
            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                player.SetOutfit(CustomPlayerOutfitType.Morph, MorphedPlayer.Data.DefaultOutfit);
        }

        public static void Unmorph(PlayerControl player)
        {
            if (!(PlayerControl.LocalPlayer.Is(RoleEnum.Aurial) && !Role.GetRole<Aurial>(PlayerControl.LocalPlayer).NormalVision)) player.SetOutfit(CustomPlayerOutfitType.Default);
        }

        public static void Camouflage()
        {
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial) && !Role.GetRole<Aurial>(PlayerControl.LocalPlayer).NormalVision) return;
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                    player.GetCustomOutfitType() != CustomPlayerOutfitType.Swooper &&
                    player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
                {
                    player.SetOutfit(CustomPlayerOutfitType.Camouflage, new GameData.PlayerOutfit()
                    {
                        ColorId = player.GetDefaultOutfit().ColorId,
                        HatId = "",
                        SkinId = "",
                        VisorId = "",
                        PlayerName = " ",
                        PetId = ""
                    });
                    PlayerMaterial.SetColors(Color.grey, player.myRend());
                    player.nameText().color = Color.clear;
                    player.cosmetics.colorBlindText.color = Color.clear;
                  
                }
            }
        }

        public static void UnCamouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls) Unmorph(player);
        }

        public static void AddUnique<T>(this Il2CppSystem.Collections.Generic.List<T> self, T item)
            where T : IDisconnectHandler
        {
            if (!self.Contains(item)) self.Add(item);
        }

        public static bool IsLover(this PlayerControl player)
        {
            return player.Is(ObjectiveEnum.Lover);
        }

        public static bool Is(this PlayerControl player, RoleEnum roleType)
        {
            return Role.GetRole(player)?.RoleType == roleType;
        }

        public static bool Is(this PlayerControl player, ModifierEnum modifierType)
        {
            return Modifier.GetModifier(player)?.ModifierType == modifierType;
        }

        public static bool Is(this PlayerControl player, AbilityEnum abilityType)
        {
            return Ability.GetAbility(player)?.AbilityType == abilityType;
        }

        public static bool Is(this PlayerControl player, Faction faction)
        {
            return Role.GetRole(player)?.Faction == faction;
        }

        public static bool Is(this PlayerControl player, FactionOverride factionOverride)
        {
            return Role.GetRole(player)?.FactionOverride == factionOverride;
        }

        public static bool Is(this PlayerControl player, ObjectiveEnum objectiveType)
        {
            return Objective.GetObjective(player)?.ObjectiveType == objectiveType;
        }
        public static bool LoverChat(this PlayerControl player)
        {
            return player.IsLover();
        }
        public static bool VampireChat(this PlayerControl player)
        {
            return player.Is(FactionOverride.Vampires);
        }
        public static bool RecruitChat(this PlayerControl player)
        {
            return player.Is(FactionOverride.Recruit);
        }
        public static bool UndeadChat(this PlayerControl player)
        {
            return player.Is(FactionOverride.Undead);
        }
        public static bool ImpostorChat(this PlayerControl player)
        {
            return (player.Data.IsImpostor() || player.Is(ObjectiveEnum.ImpostorAgent));
        }
        public static bool ApocalypseChat(this PlayerControl player)
        {
            return (player.Is(Faction.NeutralApocalypse) || player.Is(ObjectiveEnum.ApocalypseAgent));
        }
        public static bool Chat(this PlayerControl player)
        {
            return player.LoverChat() || player.VampireChat() || player.RecruitChat() || player.UndeadChat() || player.ImpostorChat() || player.ApocalypseChat();
        }
        public static bool RecieveChat(this PlayerControl player)
        {
            return PlayerControl.LocalPlayer.IsLover();
        }

        public static List<PlayerControl> GetCrewmates(List<PlayerControl> impostors)
        {
            return PlayerControl.AllPlayerControls.ToArray().Where(
                player => !impostors.Any(imp => imp.PlayerId == player.PlayerId)
            ).ToList();
        }

        public static List<PlayerControl> GetImpostors(
            List<GameData.PlayerInfo> infected)
        {
            var impostors = new List<PlayerControl>();
            foreach (var impData in infected)
                impostors.Add(impData.Object);

            return impostors;
        }

        public static RoleEnum GetRole(PlayerControl player)
        {
            if (player == null) return RoleEnum.None;
            if (player.Data == null) return RoleEnum.None;

            var role = Role.GetRole(player);
            if (role != null) return role.RoleType;

            return player.Data.IsImpostor() ? RoleEnum.Impostor : RoleEnum.Crewmate;
        }

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == id)
                    return player;

            return null;
        }

        public static bool IsExeTarget(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Executioner).Any(role =>
            {
                var exeTarget = ((Executioner)role).target;
                return exeTarget != null && player.PlayerId == exeTarget.PlayerId;
            });
        }

        public static bool IsDueled(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Pirate).Any(role =>
            {
                var dueledPlayer = ((Pirate)role).DueledPlayer;
                return dueledPlayer != null && player.PlayerId == dueledPlayer.PlayerId && !role.Player.Data.IsDead && !role.Player.Data.Disconnected;
            });
        }

        public static bool IsControled(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Witch).Any(role =>
            {
                var controledPlayer = ((Witch)role).ControledPlayer;
                return controledPlayer != null && player.PlayerId == controledPlayer.PlayerId && !role.Player.Data.IsDead && !role.Player.Data.Disconnected;
            });
        }

        public static bool UndercoverIsApocalypse()
        {
            return Role.GetRoles(RoleEnum.Undercover).Any(role =>
            {
                return ((Undercover)role).UndercoverApocalypse;
            });
        }

        public static bool UndercoverIsImpostor()
        {
            return Role.GetRoles(RoleEnum.Undercover).Any(role =>
            {
                return ((Undercover)role).UndercoverImpostor;
            });
        }

        public static bool IsKnight(this PlayerControl player)
        {
            var result = Role.GetRoles(RoleEnum.Monarch).Any(role =>
            {
                if (((Monarch)role).Knights == null) return false;
                return ((Monarch)role).Knights.Contains(player.PlayerId);
            });
            if (result == true) return true;
            else return false;
        }

        public static bool IsBugged(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Spy).Any(role =>
            {
                return ((Spy)role).BuggedPlayers.Contains(player.PlayerId);
            });
        }

        public static bool IsHeretic(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Inquisitor).Any(role =>
            {
                return ((Inquisitor)role).heretics.Contains(player.PlayerId) && !role.Player.Data.IsDead;
            });
        }

        public static bool IsShielded(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).Any(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            });
        }

        public static bool IsBarriered(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Cleric).Any(role =>
            {
                var barrieredPlayer = ((Cleric)role).BarrieredPlayer;
                return barrieredPlayer != null && player.PlayerId == barrieredPlayer.PlayerId;
            });
        }

        public static bool IsGuarded(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Bodyguard).Any(role =>
            {
                var guardedPlayer = ((Bodyguard)role).GuardedPlayer;
                return guardedPlayer != null && player.PlayerId == guardedPlayer.PlayerId;
            });
        }

        public static bool IsFortified(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Crusader).Any(role =>
            {
                return ((Crusader)role).FortifiedPlayers.Contains(player.PlayerId);
            });
        }

        public static bool PoltergeistTasks()
        {
            return Role.GetRoles(RoleEnum.Poltergeist).Any(role =>
            {
                return ((Poltergeist)role).CompletedTasks;
            });
        }

        public static Medic GetMedic(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).FirstOrDefault(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            }) as Medic;
        }

        public static Cleric GetCleric(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Cleric).FirstOrDefault(role =>
            {
                var barrieredPlayer = ((Cleric)role).BarrieredPlayer;
                return barrieredPlayer != null && player.PlayerId == barrieredPlayer.PlayerId;
            }) as Cleric;
        }

        public static Bodyguard GetBodyguard(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Bodyguard).FirstOrDefault(role =>
            {
                var guardedPlayer = ((Bodyguard)role).GuardedPlayer;
                return guardedPlayer != null && player.PlayerId == guardedPlayer.PlayerId;
            }) as Bodyguard;
        }

        public static Crusader GetCrusader(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Crusader).FirstOrDefault(role =>
            {
                return ((Crusader)role).FortifiedPlayers.Contains(player.PlayerId);
            }) as Crusader;
        }

        public static bool IsOnAlert(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Veteran).Any(role =>
            {
                var veteran = (Veteran)role;
                return veteran != null && veteran.OnAlert && player.PlayerId == veteran.Player.PlayerId;
            });
        }

        public static bool IsVesting(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Survivor).Any(role =>
            {
                var surv = (Survivor)role;
                return surv != null && surv.Vesting && player.PlayerId == surv.Player.PlayerId;
            });
        }

        public static bool IsProtected(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.GuardianAngel).Any(role =>
            {
                var gaTarget = ((GuardianAngel)role).target;
                var ga = (GuardianAngel)role;
                return gaTarget != null && ga.Protecting && player.PlayerId == gaTarget.PlayerId;
            });
        }

        public static bool IsInfected(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Plaguebearer).Any(role =>
            {
                var plaguebearer = (Plaguebearer)role;
                return plaguebearer != null && (plaguebearer.InfectedPlayers.Contains(player.PlayerId) || player.PlayerId == plaguebearer.Player.PlayerId);
            });
        }

        public static List<bool> Interact(PlayerControl player, PlayerControl target, bool toKill = false)
        {
            bool fullCooldownReset = false;
            bool gaReset = false;
            bool survReset = false;
            bool zeroSecReset = false;
            bool abilityUsed = false;
            bool barrierCdReset = false;
            if (Role.GetRole(player).Roleblocked)
            {
                if (player == PlayerControl.LocalPlayer)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
                    Role.GetRole(player).Notification("You Are Roleblocked!", 1000 * CustomGameOptions.NotificationDuration);
                }
                zeroSecReset = true;
            }
            else
            {
                if (target.IsInfected() || player.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(target, player);
                }
                if (target == ShowRoundOneShield.FirstRoundShielded && toKill)
                {
                    if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                    zeroSecReset = true;
                }
                else if (target.Is(RoleEnum.Pestilence))
                {
                    if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                    if (player.IsShielded())
                    {
                        var medic = player.GetMedic().Player.PlayerId;
                        Rpc(CustomRPC.AttemptSound, medic, player.PlayerId);

                        if (CustomGameOptions.ShieldBreaks) fullCooldownReset = true;
                        else zeroSecReset = true;

                        StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (player.IsFortified())
                    {
                        var crus = player.GetCrusader();
                        crus.FortifiedPlayers.Remove(player.PlayerId);
                        Rpc(CustomRPC.Unfortify, crus.Player.PlayerId, player.PlayerId);
                    }
                    else if (player.IsBarriered())
                    {
                        var cleric = player.GetCleric();
                        cleric.BarrieredPlayer = null;
                        Rpc(CustomRPC.Unbarrier, cleric.Player.PlayerId);
                    }
                    else if (player.IsProtected()) gaReset = true;
                    else
                    {
                        RpcMurderPlayer(target, player);
                    }
                }
                else if (target.IsOnAlert())
                {
                    if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                    if (player.Is(RoleEnum.Pestilence)) zeroSecReset = true;
                    else if (player.IsShielded())
                    {
                        var medic = player.GetMedic().Player.PlayerId;
                        Rpc(CustomRPC.AttemptSound, medic, player.PlayerId);

                        if (CustomGameOptions.ShieldBreaks) fullCooldownReset = true;
                        else zeroSecReset = true;

                        StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (player.IsFortified())
                    {
                        var crus = player.GetCrusader();
                        crus.FortifiedPlayers.Remove(player.PlayerId);
                        Rpc(CustomRPC.Unfortify, crus.Player.PlayerId, player.PlayerId);
                    }
                    else if (player.IsBarriered())
                    {
                        var cleric = player.GetCleric();
                        cleric.BarrieredPlayer = null;
                        Rpc(CustomRPC.Unbarrier, cleric.Player.PlayerId);
                    }
                    else if (player.IsProtected()) gaReset = true;
                    else if (!player.Is(RoleEnum.Famine) && !player.Is(RoleEnum.War) && !player.Is(RoleEnum.Death))
                    {
                        RpcMurderPlayer(target, player);
                    }
                    if (toKill && CustomGameOptions.KilledOnAlert)
                    {
                        if (target.IsShielded() && toKill)
                        {
                            if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                            Rpc(CustomRPC.AttemptSound, target.GetMedic().Player.PlayerId, target.PlayerId);

                            System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                            if (CustomGameOptions.ShieldBreaks) fullCooldownReset = true;
                            else zeroSecReset = true;
                            StopKill.BreakShield(target.GetMedic().Player.PlayerId, target.PlayerId, CustomGameOptions.ShieldBreaks);
                        }
                        else if (target.IsVesting() && toKill)
                        {
                            if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                            survReset = true;
                        }
                        else if (target.IsFortified())
                        {
                            if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                            if (!player.Is(RoleEnum.Pestilence) && !player.Is(RoleEnum.Famine) && !player.Is(RoleEnum.War) && !player.Is(RoleEnum.Death)) RpcMultiMurderPlayer(target.GetCrusader().Player, player);
                            var crus = target.GetCrusader();
                            crus.FortifiedPlayers.Remove(target.PlayerId);
                            Rpc(CustomRPC.Unfortify, crus.Player.PlayerId, target.PlayerId);
                            fullCooldownReset = true;
                        }
                        else if (target.IsBarriered() && toKill)
                        {
                            if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                            var cleric = target.GetCleric();
                            cleric.BarrieredPlayer = null;
                            Rpc(CustomRPC.Unbarrier, cleric.Player.PlayerId);
                            barrierCdReset = true;
                        }
                        else if (target.IsGuarded() && toKill)
                        {
                            var bg = target.GetBodyguard().Player;
                            if (bg.IsShielded())
                            {
                                if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                                Rpc(CustomRPC.AttemptSound, bg.GetMedic().Player.PlayerId, bg.PlayerId);

                                System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                                StopKill.BreakShield(bg.GetMedic().Player.PlayerId, bg.PlayerId, CustomGameOptions.ShieldBreaks);
                            }
                            else if (target.IsFortified())
                            {
                                if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                                fullCooldownReset = true;
                            }
                            else if (target.IsBarriered() && toKill)
                            {
                                if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                                barrierCdReset = true;
                            }
                            else if (bg.IsProtected())
                            {
                                if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                                gaReset = true;
                            }
                            else
                            {
                                if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills += 1;
                                if (player.Is(RoleEnum.Juggernaut)) Role.GetRole<Juggernaut>(player).JuggKills += 1;
                                if (player.Is(RoleEnum.Berserker)) Role.GetRole<Berserker>(player).KilledPlayers += 1;
                                RpcMultiMurderPlayer(player, bg);
                            }
                            if (!player.Is(RoleEnum.Pestilence) && !player.Is(RoleEnum.Famine) && !player.Is(RoleEnum.War) && !player.Is(RoleEnum.Death))
                            {
                                if (player.IsShielded())
                                {
                                    Rpc(CustomRPC.AttemptSound, bg.GetMedic().Player.PlayerId, bg.PlayerId);

                                    System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                                    StopKill.BreakShield(bg.GetMedic().Player.PlayerId, bg.PlayerId, CustomGameOptions.ShieldBreaks);
                                }
                                else if (player.IsFortified())
                                {
                                    var crus = player.GetCrusader();
                                    crus.FortifiedPlayers.Remove(player.PlayerId);
                                    Rpc(CustomRPC.Unfortify, crus.Player.PlayerId, player.PlayerId);
                                }
                                else if (player.IsBarriered())
                                {
                                    var cleric = player.GetCleric();
                                    cleric.BarrieredPlayer = null;
                                    Rpc(CustomRPC.Unbarrier, cleric.Player.PlayerId);
                                }
                                else if (!player.IsProtected())
                                {
                                    RpcMultiMurderPlayer(bg, player);
                                }
                            }
                            Role.GetRole<Bodyguard>(bg).GuardedPlayer = null;
                            Rpc(CustomRPC.Unguard, bg.PlayerId);
                            fullCooldownReset = true;
                        }
                        else if (target.IsProtected() && toKill)
                        {
                            if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                            gaReset = true;
                        }
                        else
                        {
                            if (player.Is(RoleEnum.Glitch))
                            {
                                var glitch = Role.GetRole<Glitch>(player);
                                glitch.LastKill = DateTime.UtcNow;
                            }
                            else if (player.Is(RoleEnum.Juggernaut))
                            {
                                var jugg = Role.GetRole<Juggernaut>(player);
                                jugg.JuggKills += 1;
                                jugg.LastKill = DateTime.UtcNow;
                            }
                            else if (player.Is(RoleEnum.Pestilence))
                            {
                                var pest = Role.GetRole<Pestilence>(player);
                                pest.LastKill = DateTime.UtcNow;
                            }
                            else if (player.Is(RoleEnum.Vampire))
                            {
                                var vamp = Role.GetRole<Vampire>(player);
                                vamp.LastBit = DateTime.UtcNow;
                            }
                            else if (player.Is(RoleEnum.VampireHunter))
                            {
                                var vh = Role.GetRole<VampireHunter>(player);
                                vh.LastStaked = DateTime.UtcNow;
                            }
                            else if (player.Is(RoleEnum.Werewolf))
                            {
                                var ww = Role.GetRole<Werewolf>(player);
                                ww.LastKilled = DateTime.UtcNow;
                            }
                            else if (player.Is(RoleEnum.Berserker))
                            {
                                var bers = Role.GetRole<Berserker>(player);
                                bers.KilledPlayers += 1;
                                bers.LastKill = DateTime.UtcNow;
                            }
                            else if (player.Is(RoleEnum.RedMember) || player.Is(RoleEnum.BlueMember) || player.Is(RoleEnum.YellowMember) || player.Is(RoleEnum.GreenMember) || player.Is(RoleEnum.SoloKiller))
                            {
                                var tm = Role.GetRole<TeamMember>(player);
                                tm.LastKill = DateTime.UtcNow;
                            }
                            else if (player.Is(RoleEnum.SerialKiller))
                            {
                                var sk = Role.GetRole<SerialKiller>(player);
                                sk.SKKills += 1;
                                sk.LastKill = DateTime.UtcNow;
                            }
                            else if (player.Is(RoleEnum.Jackal))
                            {
                                var jack = Role.GetRole<Jackal>(player);
                                jack.LastKill = DateTime.UtcNow;
                            }
                            else if (player.Is(RoleEnum.JKNecromancer))
                            {
                                var necro = Role.GetRole<Roles.Necromancer>(player);
                                necro.LastKill = DateTime.UtcNow;
                                necro.NecroKills += 1;
                            }
                            RpcMurderPlayer(player, target);
                            abilityUsed = true;
                            fullCooldownReset = true;
                            gaReset = false;
                            zeroSecReset = false;
                        }
                    }
                }
                else if (target.IsShielded() && toKill)
                {
                    if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                    Rpc(CustomRPC.AttemptSound, target.GetMedic().Player.PlayerId, target.PlayerId);

                    System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                    if (CustomGameOptions.ShieldBreaks) fullCooldownReset = true;
                    else zeroSecReset = true;
                    StopKill.BreakShield(target.GetMedic().Player.PlayerId, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (target.IsVesting() && toKill)
                {
                    if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                    survReset = true;
                }
                else if (target.IsFortified())
                {
                    if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                    if (!player.Is(RoleEnum.Pestilence) && !player.Is(RoleEnum.Famine) && !player.Is(RoleEnum.War) && !player.Is(RoleEnum.Death)) RpcMultiMurderPlayer(target.GetCrusader().Player, player);
                    var crus = target.GetCrusader();
                    crus.FortifiedPlayers.Remove(target.PlayerId);
                    Rpc(CustomRPC.Unfortify, crus.Player.PlayerId, target.PlayerId);
                    fullCooldownReset = true;
                }
                else if (target.IsBarriered() && toKill)
                {
                    if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                    var cleric = target.GetCleric();
                    cleric.BarrieredPlayer = null;
                    Rpc(CustomRPC.Unbarrier, cleric.Player.PlayerId);
                    barrierCdReset = true;
                }
                else if (target.IsGuarded() && toKill)
                {
                    var bg = target.GetBodyguard().Player;
                    if (bg.IsShielded())
                    {
                        if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                        Rpc(CustomRPC.AttemptSound, bg.GetMedic().Player.PlayerId, bg.PlayerId);

                        System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                        StopKill.BreakShield(bg.GetMedic().Player.PlayerId, bg.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (target.IsFortified())
                    {
                        if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                        fullCooldownReset = true;
                    }
                    else if (target.IsBarriered() && toKill)
                    {
                        if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                        barrierCdReset = true;
                    }
                    else if (bg.IsProtected())
                    {
                        if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                        gaReset = true;
                    }
                    else
                    {
                        if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills += 1;
                        if (player.Is(RoleEnum.Juggernaut)) Role.GetRole<Juggernaut>(player).JuggKills += 1;
                        if (player.Is(RoleEnum.Berserker)) Role.GetRole<Berserker>(player).KilledPlayers += 1;
                        RpcMultiMurderPlayer(player, bg);
                    }
                    if (!player.Is(RoleEnum.Pestilence) && !player.Is(RoleEnum.Famine) && !player.Is(RoleEnum.War) && !player.Is(RoleEnum.Death))
                    {
                        if (player.IsShielded())
                        {
                            Rpc(CustomRPC.AttemptSound, bg.GetMedic().Player.PlayerId, bg.PlayerId);

                            System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                            StopKill.BreakShield(bg.GetMedic().Player.PlayerId, bg.PlayerId, CustomGameOptions.ShieldBreaks);
                        }
                        else if (player.IsFortified())
                        {
                            var crus = player.GetCrusader();
                            crus.FortifiedPlayers.Remove(player.PlayerId);
                            Rpc(CustomRPC.Unfortify, crus.Player.PlayerId, player.PlayerId);
                        }
                        else if (player.IsBarriered())
                        {
                            var cleric = player.GetCleric();
                            cleric.BarrieredPlayer = null;
                            Rpc(CustomRPC.Unbarrier, cleric.Player.PlayerId);
                        }
                        else if (!player.IsProtected())
                        {
                            RpcMultiMurderPlayer(bg, player);
                        }
                    }
                    Role.GetRole<Bodyguard>(bg).GuardedPlayer = null;
                    Rpc(CustomRPC.Unguard, bg.PlayerId);
                    fullCooldownReset = true;
                }
                else if (target.IsProtected() && toKill)
                {
                    if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                    gaReset = true;
                }
                else if (toKill)
                {
                    if (player.Is(RoleEnum.Glitch))
                    {
                        var glitch = Role.GetRole<Glitch>(player);
                        glitch.LastKill = DateTime.UtcNow;
                    }
                    else if (player.Is(RoleEnum.Juggernaut))
                    {
                        var jugg = Role.GetRole<Juggernaut>(player);
                        if (!target.Is(RoleEnum.Famine) && !target.Is(RoleEnum.War) && !target.Is(RoleEnum.Death)) jugg.JuggKills += 1;
                        jugg.LastKill = DateTime.UtcNow;
                    }
                    else if (player.Is(RoleEnum.Pestilence))
                    {
                        var pest = Role.GetRole<Pestilence>(player);
                        pest.LastKill = DateTime.UtcNow;
                    }
                    else if (player.Is(RoleEnum.Vampire))
                    {
                        var vamp = Role.GetRole<Vampire>(player);
                        vamp.LastBit = DateTime.UtcNow;
                    }
                    else if (player.Is(RoleEnum.VampireHunter))
                    {
                        var vh = Role.GetRole<VampireHunter>(player);
                        vh.LastStaked = DateTime.UtcNow;
                    }
                    else if (player.Is(RoleEnum.Werewolf))
                    {
                        var ww = Role.GetRole<Werewolf>(player);
                        ww.LastKilled = DateTime.UtcNow;
                    }
                    else if (player.Is(RoleEnum.Berserker))
                    {
                        var bers = Role.GetRole<Berserker>(player);
                        if (!target.Is(RoleEnum.Famine) && !target.Is(RoleEnum.War) && !target.Is(RoleEnum.Death)) bers.KilledPlayers += 1;
                        bers.LastKill = DateTime.UtcNow;
                    }
                    else if (player.Is(RoleEnum.RedMember) || player.Is(RoleEnum.BlueMember) || player.Is(RoleEnum.YellowMember) || player.Is(RoleEnum.GreenMember) || player.Is(RoleEnum.SoloKiller))
                    {
                        var tm = Role.GetRole<TeamMember>(player);
                        tm.LastKill = DateTime.UtcNow;
                    }
                    else if (player.Is(RoleEnum.SerialKiller))
                    {
                        var sk = Role.GetRole<SerialKiller>(player);
                        if (!target.Is(RoleEnum.Famine) && !target.Is(RoleEnum.War) && !target.Is(RoleEnum.Death)) sk.SKKills += 1; else sk.SKKills = 0;
                        sk.LastKill = DateTime.UtcNow;
                    }
                    else if (player.Is(RoleEnum.Jackal))
                    {
                        var jack = Role.GetRole<Jackal>(player);
                        jack.LastKill = DateTime.UtcNow;
                    }
                    else if (player.Is(RoleEnum.JKNecromancer))
                    {
                        var necro = Role.GetRole<Roles.Necromancer>(player);
                        necro.LastKill = DateTime.UtcNow;
                        if (!target.Is(RoleEnum.Famine) && !target.Is(RoleEnum.War) && !target.Is(RoleEnum.Death)) necro.NecroKills += 1;
                    }
                    if (!target.Is(RoleEnum.Famine) && !target.Is(RoleEnum.War) && !target.Is(RoleEnum.Death))
                    {
                        RpcMurderPlayer(player, target);
                    }
                    abilityUsed = true;
                    fullCooldownReset = true;
                }
                else
                {
                    abilityUsed = true;
                    fullCooldownReset = true;
                }
            }

            if (abilityUsed)
            {
                foreach (Role role in Role.GetRoles(RoleEnum.Hunter))
                {
                    Hunter hunter = (Hunter)role;
                    hunter.CatchPlayer(player);
                }
            }
            var reset = new List<bool>();
            reset.Add(fullCooldownReset);
            reset.Add(gaReset);
            reset.Add(survReset);
            reset.Add(zeroSecReset);
            reset.Add(abilityUsed);
            reset.Add(barrierCdReset);
            return reset;
        }

        public static Il2CppSystem.Collections.Generic.List<PlayerControl> GetClosestPlayers(Vector2 truePosition, float radius, bool includeDead)
        {
            Il2CppSystem.Collections.Generic.List<PlayerControl> playerControlList = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            float lightRadius = radius * ShipStatus.Instance.MaxLightRadius;
            Il2CppSystem.Collections.Generic.List<GameData.PlayerInfo> allPlayers = GameData.Instance.AllPlayers;
            for (int index = 0; index < allPlayers.Count; ++index)
            {
                GameData.PlayerInfo playerInfo = allPlayers[index];
                if (!playerInfo.Disconnected && (!playerInfo.Object.Data.IsDead || includeDead))
                {
                    Vector2 vector2 = new Vector2(playerInfo.Object.GetTruePosition().x - truePosition.x, playerInfo.Object.GetTruePosition().y - truePosition.y);
                    float magnitude = ((Vector2)vector2).magnitude;
                    if (magnitude <= lightRadius)
                    {
                        PlayerControl playerControl = playerInfo.Object;
                        playerControlList.Add(playerControl);
                    }
                }
            }
            return playerControlList;
        }

        public static PlayerControl GetClosestPlayer(PlayerControl refPlayer, List<PlayerControl> AllPlayers)
        {
            var num = double.MaxValue;
            var refPosition = refPlayer.GetTruePosition();
            PlayerControl result = null;
            foreach (var player in AllPlayers)
            {
                if (player.Data.IsDead || player.PlayerId == refPlayer.PlayerId || !player.Collider.enabled || player.inVent) continue;
                var playerPosition = player.GetTruePosition();
                var distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
                var isClosest = distBetweenPlayers < num;
                if (!isClosest) continue;
                var vector = playerPosition - refPosition;
                if (PhysicsHelpers.AnyNonTriggersBetween(
                    refPosition, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask
                )) continue;
                num = distBetweenPlayers;
                result = player;
            }
            
            return result;
        }
        public static void SetTarget(
            ref PlayerControl closestPlayer,
            KillButton button,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (!button.isActiveAndEnabled) return;

            button.SetTarget(
                SetClosestPlayer(ref closestPlayer, maxDistance, targets)
            );
        }
        public static void SetTarget(
            ref PlayerControl closestPlayer,
            PlayerControl targetPlayer,
            KillButton button,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (!button.isActiveAndEnabled) return;

            button.SetTarget(
                SetClosestPlayer(ref closestPlayer, targetPlayer, maxDistance, targets)
            );
        }

        public static PlayerControl SetClosestPlayer(
            ref PlayerControl closestPlayer,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (float.IsNaN(maxDistance))
                maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var player = GetClosestPlayer(
                PlayerControl.LocalPlayer,
                targets ?? PlayerControl.AllPlayerControls.ToArray().ToList()
            );
            var closeEnough = player == null || (
                GetDistBetweenPlayers(PlayerControl.LocalPlayer, player) < maxDistance
            );
            return closestPlayer = closeEnough ? player : null;
        }

        public static PlayerControl SetClosestPlayer(
            ref PlayerControl closestPlayer,
            PlayerControl targetPlayer,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (float.IsNaN(maxDistance))
                maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];
            var player = GetClosestPlayer(
                targetPlayer,
                targets ?? PlayerControl.AllPlayerControls.ToArray().ToList()
            );
            var closeEnough = player == null || (
                GetDistBetweenPlayers(targetPlayer, player) < maxDistance
            );
            return closestPlayer = closeEnough ? player : null;
        }

        public static double GetDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            var truePosition = refplayer.GetTruePosition();
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target)
        {
            MurderPlayer(killer, target, true);
            Rpc(CustomRPC.BypassKill, killer.PlayerId, target.PlayerId);
        }

        public static void RpcMultiMurderPlayer(PlayerControl killer, PlayerControl target)
        {
            MurderPlayer(killer, target, false);
            Rpc(CustomRPC.BypassMultiKill, killer.PlayerId, target.PlayerId);
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool jumpToBody)
        {
            var data = target.Data;
            if (data != null && !data.IsDead)
            {
                if (ShowRoundOneShield.DiedFirst == "") ShowRoundOneShield.DiedFirst = target.GetDefaultOutfit().PlayerName;

                if (killer == PlayerControl.LocalPlayer)
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);

                if (!killer.Is(Faction.Crewmates) && killer != target
                    && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal) Role.GetRole(killer).Kills += 1;

                if (killer.Is(RoleEnum.Sheriff))
                {
                    var sheriff = Role.GetRole<Sheriff>(killer);
                    if (target.Is(Faction.Impostors) ||
                        target.Is(RoleEnum.Glitch) && CustomGameOptions.SheriffKillsGlitch ||
                        target.Is(RoleEnum.Arsonist) && CustomGameOptions.SheriffKillsArsonist ||
                        target.Is(Faction.NeutralApocalypse) && (CustomGameOptions.SheriffKillsPlaguebearer || CustomGameOptions.GameMode == GameMode.Horseman) ||
                        target.Is(RoleEnum.Pestilence) && CustomGameOptions.SheriffKillsPlaguebearer ||
                        target.Is(RoleEnum.Werewolf) && CustomGameOptions.SheriffKillsWerewolf ||
                        target.Is(RoleEnum.Juggernaut) && CustomGameOptions.SheriffKillsJuggernaut ||
                        target.Is(RoleEnum.Vampire) && CustomGameOptions.SheriffKillsVampire ||
                        target.Is(RoleEnum.Executioner) && CustomGameOptions.SheriffKillsExecutioner ||
                        target.Is(RoleEnum.Doomsayer) && CustomGameOptions.SheriffKillsDoomsayer ||
                        target.Is(RoleEnum.Jester) && CustomGameOptions.SheriffKillsJester ||
                        target.Is(RoleEnum.Pirate) && CustomGameOptions.SheriffKillsPirate||
                        target.Is(RoleEnum.SerialKiller) && CustomGameOptions.SheriffKillsSerialKiller ||
                        target.Is(RoleEnum.Inquisitor) && CustomGameOptions.SheriffKillsInquisitor ||
                        target.Is(RoleEnum.Witch) && CustomGameOptions.SheriffKillsWitch ||
                        target.Is(RoleEnum.JKNecromancer) && CustomGameOptions.SheriffKillsNecromancer ||
                        (target.Is(FactionOverride.Undead) && !target.Is(RoleEnum.JKNecromancer)) && CustomGameOptions.SheriffKillsUndead ||
                        target.Is(RoleEnum.Jackal) && CustomGameOptions.SheriffKillsJackal ||
                        (target.Is(FactionOverride.Recruit) && !target.Is(RoleEnum.Jackal)) && CustomGameOptions.SheriffKillsRecruits ||
                        (target.Is(ObjectiveEnum.ImpostorAgent) || target.Is(ObjectiveEnum.ApocalypseAgent)) && CustomGameOptions.SheriffKillsAgent) sheriff.CorrectKills += 1;
                    else if (killer == target) sheriff.IncorrectKills += 1;
                }

                if (killer.Is(RoleEnum.VampireHunter))
                {
                    var vh = Role.GetRole<VampireHunter>(killer);
                    if (killer != target) vh.CorrectKills += 1;
                }

                if (killer.Is(RoleEnum.Veteran))
                {
                    var veteran = Role.GetRole<Veteran>(killer);
                    if (target.Is(Faction.Impostors) || target.Is(Faction.NeutralKilling) || target.Is(Faction.NeutralEvil) || target.Is(Faction.NeutralApocalypse) || target.Is(Faction.NeutralChaos)) veteran.CorrectKills += 1;
                    else if (killer != target) veteran.IncorrectKills += 1;
                }
                if (killer.Is(RoleEnum.Hunter))
                {
                    var hunter = Role.GetRole<Hunter>(killer);
                    if (target.Is(RoleEnum.Doomsayer) || target.Is(Faction.Impostors) || target.Is(Faction.NeutralKilling))
                    {
                        hunter.CorrectKills += 1;
                    }
                    else
                    {
                        hunter.IncorrectKills += 1;
                    }
                }

                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                target.Visible = false;

                if (target.Is(ModifierEnum.Famous))
                {
                    Coroutines.Start(FlashCoroutine(Patches.Colors.Famous));
                    Role.GetRole(PlayerControl.LocalPlayer).Notification("Famous Has Died!", 1000 * CustomGameOptions.NotificationDuration);
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    Coroutines.Start(FlashCoroutine(Patches.Colors.Mystic));
                    Role.GetRole(PlayerControl.LocalPlayer).Notification("Someone Have Died!", 1000 * CustomGameOptions.NotificationDuration);
                }

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
                {
                    var detective = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
                    detective.LastKiller = killer;
                }

                if (!CustomGameOptions.GhostsDoTasks)
                {
                    if (AmongUsClient.Instance.AmHost)
                    {
                        var modded_criteria = Role.ShipStatus_KMPKPPGPNIH.Prefix((LogicGameFlowNormal)GameManager.Instance.LogicFlow);
                        if (modded_criteria) GameManager.Instance.LogicFlow.CheckEndCriteria();
                        if (GameManager.Instance.ShouldCheckForGameEnd && target.myTasks.ToArray().Count(x => !x.IsComplete) + GameData.Instance.CompletedTasks < GameData.Instance.TotalTasks)
                        {
                            // Host should only process tasks being removed if the game wouldn't have ended otherwise.
                            for (var i = 0; i < target.myTasks.Count; i++)
                            {
                                var playerTask = target.myTasks.ToArray()[i];
                                GameData.Instance.CompleteTask(target, playerTask.Id);
                            }
                        }
                    }
                    else
                    {
                        for (var i = 0; i < target.myTasks.Count; i++)
                        {
                            var playerTask = target.myTasks.ToArray()[i];
                            GameData.Instance.CompleteTask(target, playerTask.Id);
                        }
                    }
                }

                if (target.AmOwner)
                {
                    try
                    {
                        if (Minigame.Instance)
                        {
                            Minigame.Instance.Close();
                            Minigame.Instance.Close();
                        }

                        if (MapBehaviour.Instance)
                        {
                            MapBehaviour.Instance.Close();
                            MapBehaviour.Instance.Close();
                        }
                    }
                    catch
                    {
                    }

                    DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(killer.Data, data);
                    DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
                    target.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                    target.RpcSetScanner(false);
                    var importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                    importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                    if (!CustomGameOptions.GhostsDoTasks)
                    {
                        for (var i = 0; i < target.myTasks.Count; i++)
                        {
                            var playerTask = target.myTasks.ToArray()[i];
                            GameData.Instance.CompleteTask(target, playerTask.Id);
                            playerTask.Complete();
                            playerTask.OnRemove();
                            Object.Destroy(playerTask.gameObject);
                        }

                        target.myTasks.Clear();
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostIgnoreTasks,
                            new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }
                    else
                    {
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostDoTasks,
                            new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }

                    target.myTasks.Insert(0, importantTextTask);
                }

                if (jumpToBody)
                {
                    killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(killer, target));
                }
                else killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(target, target));

                if (target.Is(ModifierEnum.Frosty))
                {
                    var frosty = Modifier.GetModifier<Frosty>(target);
                    frosty.Chilled = killer;
                    frosty.LastChilled = DateTime.UtcNow;
                    frosty.IsChilled = true;
                }

                var deadBody = new DeadPlayer
                {
                    PlayerId = target.PlayerId,
                    KillerId = killer.PlayerId,
                    KillTime = DateTime.UtcNow
                };
                Role.GetRole(killer).LastBlood = DateTime.UtcNow;

                Murder.KilledPlayers.Add(deadBody);

                if (MeetingHud.Instance) target.Exiled();

                if (!killer.AmOwner) return;

                if (target.Is(ModifierEnum.Bait))
                {
                    BaitReport(killer, target);
                }

                if (target.Is(ModifierEnum.Aftermath))
                {
                    Aftermath.ForceAbility(killer, target);
                }

                if (!jumpToBody) return;

                if (killer.Data.IsImpostor() && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek)
                {
                    killer.SetKillTimer(GameOptionsManager.Instance.currentHideNSeekGameOptions.KillCooldown);
                    return;
                }

                if (killer == PlayerControl.LocalPlayer && killer.Is(RoleEnum.Warlock))
                {
                    var warlock = Role.GetRole<Warlock>(killer);
                    if (warlock.Charging)
                    {
                        warlock.UsingCharge = true;
                        warlock.ChargeUseDuration = warlock.ChargePercent * CustomGameOptions.ChargeUseDuration / 100f;
                        if (warlock.ChargeUseDuration == 0f) warlock.ChargeUseDuration += 0.01f;
                    }
                    killer.SetKillTimer(0.01f);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Werewolf))
                {
                    var werewolf = Role.GetRole<Werewolf>(killer);
                    werewolf.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.RampageKillCd);
                    werewolf.Player.SetKillTimer(CustomGameOptions.RampageKillCd * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Vampire))
                {
                    var vampire = Role.GetRole<Vampire>(killer);
                    vampire.LastBit = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.BiteCd);
                    vampire.Player.SetKillTimer(CustomGameOptions.BiteCd * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Glitch))
                {
                    var glitch = Role.GetRole<Glitch>(killer);
                    glitch.LastKill = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.GlitchKillCooldown);
                    glitch.Player.SetKillTimer(CustomGameOptions.GlitchKillCooldown * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Juggernaut))
                {
                    var juggernaut = Role.GetRole<Juggernaut>(killer);
                    juggernaut.LastKill = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * (CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * juggernaut.JuggKills));
                    juggernaut.Player.SetKillTimer((CustomGameOptions.JuggKCd - CustomGameOptions.ReducedKCdPerKill * juggernaut.JuggKills) * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.SerialKiller))
                {
                    var serialKiller = Role.GetRole<SerialKiller>(killer);
                    serialKiller.LastKill = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * (serialKiller.InBloodlust ? CustomGameOptions.BloodlustCooldown : CustomGameOptions.SerialKillerCooldown));
                    serialKiller.Player.SetKillTimer((serialKiller.InBloodlust ? CustomGameOptions.BloodlustCooldown : CustomGameOptions.SerialKillerCooldown) * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus) * CustomGameOptions.DiseasedMultiplier;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * CustomGameOptions.DiseasedMultiplier;
                    var upperKC = (GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus) * CustomGameOptions.DiseasedMultiplier;
                    killer.SetKillTimer((PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Data.IsImpostor())
                {
                    killer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown * CustomGameOptions.DiseasedMultiplier * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                    return;
                }

                if (killer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    killer.SetKillTimer((PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                    return;
                }

                if (killer.Data.IsImpostor())
                {
                    killer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                    return;
                }
            }
        }

        public static void BaitReport(PlayerControl killer, PlayerControl target)
        {
            Coroutines.Start(BaitReportDelay(killer, target));
        }

        public static IEnumerator BaitReportDelay(PlayerControl killer, PlayerControl target)
        {
            var extraDelay = Random.RandomRangeInt(0, (int) (100 * (CustomGameOptions.BaitMaxDelay - CustomGameOptions.BaitMinDelay) + 1));
            if (CustomGameOptions.BaitMaxDelay <= CustomGameOptions.BaitMinDelay)
                yield return new WaitForSeconds(CustomGameOptions.BaitMaxDelay + 0.01f);
            else
                yield return new WaitForSeconds(CustomGameOptions.BaitMinDelay + 0.01f + extraDelay/100f);
            var bodies = Object.FindObjectsOfType<DeadBody>();
            if (AmongUsClient.Instance.AmHost)
            {
                foreach (var body in bodies)
                {
                    try
                    {
                        if (body.ParentId == target.PlayerId) { killer.ReportDeadBody(target.Data); break; }
                    }
                    catch
                    {
                    }
                }
            }
            else
            {
                foreach (var body in bodies)
                {
                    try
                    {
                        if (body.ParentId == target.PlayerId)
                        {
                            Rpc(CustomRPC.BaitReport, killer.PlayerId, target.PlayerId);
                            break;
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

        public static void Convert(PlayerControl player)
        {
            if (PlayerControl.LocalPlayer == player)
            {
                Coroutines.Start(FlashCoroutine(Patches.Colors.Impostor));
                Role.GetRole(player).Notification("You Have Been Converted!", 1000 * CustomGameOptions.NotificationDuration);
            }
            else if (PlayerControl.LocalPlayer != player && PlayerControl.LocalPlayer.Is(RoleEnum.CultistMystic)
                && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Coroutines.Start(FlashCoroutine(Patches.Colors.Impostor));
                Role.GetRole(player).Notification("Someone Has Been Converted!", 1000 * CustomGameOptions.NotificationDuration);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter) && PlayerControl.LocalPlayer == player)
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

            if (player.Is(RoleEnum.Chameleon))
            {
                var chameleonRole = Role.GetRole<Chameleon>(player);
                if (chameleonRole.IsSwooped) chameleonRole.UnSwoop();
                Role.RoleDictionary.Remove(player.PlayerId);
                var swooper = new Swooper(player);
                swooper.LastSwooped = DateTime.UtcNow;
                swooper.RegenTask();
            }

            if (player.Is(RoleEnum.Engineer))
            {
                var engineer = Role.GetRole<Engineer>(player);
                engineer.Name = "Demolitionist";
                engineer.Color = Patches.Colors.Impostor;
                engineer.Faction = Faction.Impostors;
                engineer.RegenTask();
            }

            if (player.Is(RoleEnum.Investigator))
            {
                var investigator = Role.GetRole<Investigator>(player);
                investigator.Name = "Consigliere";
                investigator.Color = Patches.Colors.Impostor;
                investigator.Faction = Faction.Impostors;
                investigator.RegenTask();
            }

            if (player.Is(RoleEnum.CultistMystic))
            {
                var mystic = Role.GetRole<CultistMystic>(player);
                mystic.Name = "Clairvoyant";
                mystic.Color = Patches.Colors.Impostor;
                mystic.Faction = Faction.Impostors;
                mystic.RegenTask();
            }

            if (player.Is(RoleEnum.CultistSnitch))
            {
                var snitch = Role.GetRole<CultistSnitch>(player);
                snitch.Name = "Informant";
                snitch.TaskText = () => "Complete all your tasks to reveal a fake Impostor!";
                snitch.Color = Patches.Colors.Impostor;
                snitch.Faction = Faction.Impostors;
                snitch.RegenTask();
                if (PlayerControl.LocalPlayer == player && snitch.CompletedTasks)
                {
                    var crew = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(RoleEnum.Mayor)).ToList();
                    if (crew.Count != 0)
                    {
                        crew.Shuffle();
                        snitch.RevealedPlayer = crew[0];
                        Rpc(CustomRPC.SnitchCultistReveal, player.PlayerId, snitch.RevealedPlayer.PlayerId);
                    }
                }
            }

            if (player.Is(RoleEnum.Spy))
            {
                var spy = Role.GetRole<Spy>(player);
                spy.Name = "Rogue Agent";
                spy.Color = Patches.Colors.Impostor;
                spy.Faction = Faction.Impostors;
                spy.RegenTask();
            }

            if (player.Is(RoleEnum.Transporter))
            {
                Role.RoleDictionary.Remove(player.PlayerId);
                var escapist = new Escapist(player);
                escapist.LastEscape = DateTime.UtcNow;
                escapist.RegenTask();
            }

            if (player.Is(RoleEnum.Vigilante))
            {
                var vigi = Role.GetRole<Vigilante>(player);
                vigi.Name = "Assassin";
                vigi.TaskText = () => "Guess the roles of crewmates mid-meeting to kill them!";
                vigi.Color = Patches.Colors.Impostor;
                vigi.Faction = Faction.Impostors;
                vigi.RegenTask();
                var colorMapping = new Dictionary<string, Color>();
                if (CustomGameOptions.MayorCultistOn > 0) colorMapping.Add("Mayor", Colors.Mayor);
                if (CustomGameOptions.SeerCultistOn > 0) colorMapping.Add("Seer", Colors.Seer);
                if (CustomGameOptions.SheriffCultistOn > 0) colorMapping.Add("Sheriff", Colors.Sheriff);
                if (CustomGameOptions.SurvivorCultistOn > 0) colorMapping.Add("Survivor", Colors.Survivor);
                if (CustomGameOptions.MaxChameleons > 0) colorMapping.Add("Chameleon", Colors.Chameleon);
                if (CustomGameOptions.MaxEngineers > 0) colorMapping.Add("Engineer", Colors.Engineer);
                if (CustomGameOptions.MaxInvestigators > 0) colorMapping.Add("Investigator", Colors.Investigator);
                if (CustomGameOptions.MaxMystics > 0) colorMapping.Add("Mystic", Colors.Mystic);
                if (CustomGameOptions.MaxSnitches > 0) colorMapping.Add("Snitch", Colors.Snitch);
                if (CustomGameOptions.MaxSpies > 0) colorMapping.Add("Spy", Colors.Spy);
                if (CustomGameOptions.MaxTransporters > 0) colorMapping.Add("Transporter", Colors.Transporter);
                if (CustomGameOptions.MaxVigilantes > 1) colorMapping.Add("Vigilante", Colors.Vigilante);
                colorMapping.Add("Crewmate", Colors.Crewmate);
                vigi.SortedColorMapping = colorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            }

            if (player.Is(RoleEnum.Crewmate))
            {
                Role.RoleDictionary.Remove(player.PlayerId);
                new Impostor(player);
            }

            player.Data.Role.TeamType = RoleTeamTypes.Impostor;
            RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
            player.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);

            if (PlayerControl.LocalPlayer.Is(RoleEnum.CultistSnitch))
            {
                var snitch = Role.GetRole<CultistSnitch>(PlayerControl.LocalPlayer);
                if (snitch.RevealedPlayer == player)
                {
                    var crew = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(RoleEnum.Mayor)).ToList();
                    if (crew.Count != 0)
                    {
                        crew.Shuffle();
                        snitch.RevealedPlayer = crew[0];
                        Rpc(CustomRPC.SnitchCultistReveal, player.PlayerId, snitch.RevealedPlayer.PlayerId);
                    }
                }
            }

            foreach (var player2 in PlayerControl.AllPlayerControls)
            {
                if (player2.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor())
                {
                    player2.nameText().color = Patches.Colors.Impostor;
                }
            }
        }

        public static IEnumerator FlashCoroutine(Color color, float waitfor = 1f, float alpha = 0.3f)
        {
            color.a = alpha;
            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                fullscreen.enabled = true;
                fullscreen.gameObject.active = true;
                fullscreen.color = color;
            }

            yield return new WaitForSeconds(waitfor);

            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                if (fullscreen.color.Equals(color))
                {
                    fullscreen.color = new Color(1f, 0f, 0f, 0.37254903f);
                    fullscreen.enabled = false;
                }
            }
        }

        public static IEnumerable<(T1, T2)> Zip<T1, T2>(List<T1> first, List<T2> second)
        {
            return first.Zip(second, (x, y) => (x, y));
        }

        public static void RemoveTasks(PlayerControl player)
        {
            var totalTasks = GameOptionsManager.Instance.currentNormalGameOptions.NumCommonTasks + GameOptionsManager.Instance.currentNormalGameOptions.NumLongTasks +
                             GameOptionsManager.Instance.currentNormalGameOptions.NumShortTasks;


            foreach (var task in player.myTasks)
                if (task.TryCast<NormalPlayerTask>() != null)
                {
                    var normalPlayerTask = task.Cast<NormalPlayerTask>();

                    var updateArrow = normalPlayerTask.taskStep > 0;

                    normalPlayerTask.taskStep = 0;
                    normalPlayerTask.Initialize();
                    if (normalPlayerTask.TaskType == TaskTypes.PickUpTowels)
                        foreach (var console in Object.FindObjectsOfType<TowelTaskConsole>())
                            console.Image.color = Color.white;
                    normalPlayerTask.taskStep = 0;
                    if (normalPlayerTask.TaskType == TaskTypes.UploadData)
                        normalPlayerTask.taskStep = 1;
                    if ((normalPlayerTask.TaskType == TaskTypes.EmptyGarbage || normalPlayerTask.TaskType == TaskTypes.EmptyChute)
                        && (GameOptionsManager.Instance.currentNormalGameOptions.MapId == 0 ||
                        GameOptionsManager.Instance.currentNormalGameOptions.MapId == 3 ||
                        GameOptionsManager.Instance.currentNormalGameOptions.MapId == 4))
                        normalPlayerTask.taskStep = 1;
                    if (updateArrow)
                        normalPlayerTask.UpdateArrowAndLocation();

                    var taskInfo = player.Data.FindTaskById(task.Id);
                    taskInfo.Complete = false;
                }
        }

        public static void DestroyAll(this IEnumerable<Component> listie)
        {
            foreach (var item in listie)
            {
                if (item == null) continue;
                Object.Destroy(item);
                if (item.gameObject == null) return;
                Object.Destroy(item.gameObject);
            }
        }

        public static void EndGame(GameOverReason reason = GameOverReason.ImpostorByVote, bool showAds = false)
        {
            GameManager.Instance.RpcEndGame(reason, showAds);
        }


        public static void Rpc(params object[] data)
        {
            if (data[0] is not CustomRPC) throw new ArgumentException($"first parameter must be a {typeof(CustomRPC).FullName}");

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)(CustomRPC)data[0], SendOption.Reliable, -1);

            if (data.Length == 1)
            {
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                return;
            }

            foreach (var item in data[1..])
            {

                if (item is bool boolean)
                {
                    writer.Write(boolean);
                }
                else if (item is int integer)
                {
                    writer.Write(integer);
                }
                else if (item is uint uinteger)
                {
                    writer.Write(uinteger);
                }
                else if (item is float Float)
                {
                    writer.Write(Float);
                }
                else if (item is byte Byte)
                {
                    writer.Write(Byte);
                }
                else if (item is sbyte sByte)
                {
                    writer.Write(sByte);
                }
                else if (item is Vector2 vector)
                {
                    writer.Write(vector);
                }
                else if (item is Vector3 vector3)
                {
                    writer.Write(vector3);
                }
                else if (item is string str)
                {
                    writer.Write(str);
                }
                else if (item is byte[] array)
                {
                    writer.WriteBytesAndSize(array);
                }
                else
                {
                    Logger<TownOfUs>.Error($"unknown data type entered for rpc write: item - {nameof(item)}, {item.GetType().FullName}, rpc - {data[0]}");
                }
            }
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        [HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
        class MedScanMinigameFixedUpdatePatch
        {
            static void Prefix(MedScanMinigame __instance)
            {
                if (CustomGameOptions.ParallelMedScans)
                {
                    //Allows multiple medbay scans at once
                    __instance.medscan.CurrentUser = PlayerControl.LocalPlayer.PlayerId;
                    __instance.medscan.UsersList.Clear();
                }
            }
        }
      
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        class StartMeetingPatch {
            public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)]GameData.PlayerInfo meetingTarget) {
                voteTarget = meetingTarget;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        class MeetingHudUpdatePatch {
            static void Postfix(MeetingHud __instance) {
                // Deactivate skip Button if skipping on emergency meetings is disabled 
                if ((voteTarget == null && CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Emergency) || (CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Always)) {
                    __instance.SkipVoteButton.gameObject.SetActive(false);
                }
            }
        }

        //Submerged utils
        public static object TryCast(this Il2CppObjectBase self, Type type)
        {
            return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, Array.Empty<object>());
        }
        public static IList createList(Type myType)
        {
            Type genericListType = typeof(List<>).MakeGenericType(myType);
            return (IList)Activator.CreateInstance(genericListType);
        }

        public static void ResetCustomTimers()
        {
            #region CrewmateRoles
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                var medium = Role.GetRole<Medium>(PlayerControl.LocalPlayer);
                medium.LastMediated = DateTime.UtcNow;
            }
            foreach (var role in Role.GetRoles(RoleEnum.Medium))
            {
                var medium = (Medium)role;
                medium.MediatedPlayers.Values.DestroyAll();
                medium.MediatedPlayers.Clear();
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
            {
                var seer = Role.GetRole<Seer>(PlayerControl.LocalPlayer);
                seer.LastInvestigated = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
            {
                var oracle = Role.GetRole<Oracle>(PlayerControl.LocalPlayer);
                oracle.LastConfessed = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial))
            {
                var aurial = Role.GetRole<Aurial>(PlayerControl.LocalPlayer);
                aurial.LastRadiated = DateTime.UtcNow;
                aurial.CannotSeeDelay = DateTime.UtcNow;
                if (PlayerControl.LocalPlayer.Data.IsDead)
                {
                    aurial.NormalVision = true;
                    SeeAll.AllToNormal();
                    aurial.ClearEffect();
                }
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.CultistSeer))
            {
                var seer = Role.GetRole<CultistSeer>(PlayerControl.LocalPlayer);
                seer.LastInvestigated = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff))
            {
                var sheriff = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);
                sheriff.LastKilled = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
            {
                var tracker = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
                tracker.LastTracked = DateTime.UtcNow;
                tracker.UsesLeft = CustomGameOptions.MaxTracks;
                if (CustomGameOptions.ResetOnNewRound)
                {
                    tracker.TrackerArrows.Values.DestroyAll();
                    tracker.TrackerArrows.Clear();
                }
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Hunter))
            {
                var hunter = Role.GetRole<Hunter>(PlayerControl.LocalPlayer);
                hunter.LastKilled = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.VampireHunter))
            {
                var vh = Role.GetRole<VampireHunter>(PlayerControl.LocalPlayer);
                vh.LastStaked = DateTime.UtcNow;
            }
            foreach (var vh in Role.GetRoles(RoleEnum.VampireHunter))
            {
                var vhRole = (VampireHunter)vh;
                if (!vhRole.AddedStakes)
                {
                    vhRole.UsesLeft = CustomGameOptions.MaxFailedStakesPerGame;
                    vhRole.AddedStakes = true;
                }
                var vamps = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(RoleEnum.Vampire) || x.Is(RoleEnum.JKNecromancer)) && !x.Data.IsDead && !x.Data.Disconnected).ToList();
                if (vamps.Count == 0 && vh.Player != StartImitate.ImitatingPlayer && !vh.Player.Data.IsDead && !vh.Player.Data.Disconnected)
                {
                    var vhPlayer = vhRole.Player;

                    if (CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Sheriff)
                    {
                        Role.RoleDictionary.Remove(vhPlayer.PlayerId);
                        var kills = ((VampireHunter)vh).CorrectKills;
                        var sheriff = new Sheriff(vhPlayer);
                        sheriff.CorrectKills = kills;
                        sheriff.RegenTask();
                    }
                    else if (CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Veteran)
                    {
                        if (PlayerControl.LocalPlayer == vhPlayer) Object.Destroy(((VampireHunter)vh).UsesText);
                        Role.RoleDictionary.Remove(vhPlayer.PlayerId);
                        var kills = ((VampireHunter)vh).CorrectKills;
                        var vet = new Veteran(vhPlayer);
                        vet.CorrectKills = kills;
                        vet.RegenTask();
                        vet.LastAlerted = DateTime.UtcNow;
                    }
                    else if (CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Vigilante)
                    {
                        Role.RoleDictionary.Remove(vhPlayer.PlayerId);
                        var kills = ((VampireHunter)vh).CorrectKills;
                        var vigi = new Vigilante(vhPlayer);
                        vigi.CorrectKills = kills;
                        vigi.RegenTask();
                    }
                    else
                    {
                        Role.RoleDictionary.Remove(vhPlayer.PlayerId);
                        var kills = ((VampireHunter)vh).CorrectKills;
                        var crew = new Crewmate(vhPlayer);
                        crew.CorrectKills = kills;
                    }
                }
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
            {
                var transporter = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);
                transporter.LastTransported = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
            {
                var veteran = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);
                veteran.LastAlerted = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
            {
                var trapper = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);
                trapper.LastTrapped = DateTime.UtcNow;
                trapper.trappedPlayers.Clear();
                if (CustomGameOptions.TrapsRemoveOnNewRound) trapper.traps.ClearTraps();
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
            {
                var detective = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
                detective.LastExamined = DateTime.UtcNow;
                detective.ClosestPlayer = null;
                detective.CurrentTarget = null;
                detective.LastKiller = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Chameleon))
            {
                var chameleon = Role.GetRole<Chameleon>(PlayerControl.LocalPlayer);
                chameleon.LastSwooped = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Monarch))
            {
                var monarch = Role.GetRole<Monarch>(PlayerControl.LocalPlayer);
                monarch.LastKnighted = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Inspector))
            {
                var inspector = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);
                inspector.LastInspected = DateTime.UtcNow;
                inspector.LastInspectedPlayer = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.TavernKeeper))
            {
                var tavernKeeper = Role.GetRole<TavernKeeper>(PlayerControl.LocalPlayer);
                tavernKeeper.LastDrink = DateTime.UtcNow;
                tavernKeeper.DrunkPlayers = new List<PlayerControl>();
            }
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                Role.GetRole(player).Roleblocked = false;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Spy))
            {
                var spy = Role.GetRole<Spy>(PlayerControl.LocalPlayer);
                spy.LastBugged = DateTime.UtcNow;
            }
            foreach (Spy spy in Role.GetRoles(RoleEnum.Spy))
            {
                spy.BuggedPlayers = new List<byte>();
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator))
            {
                var investigator = Role.GetRole<Investigator>(PlayerControl.LocalPlayer);
                investigator.UsesLeft = CustomGameOptions.MaxInvestigates;
                investigator.LastInvestigate = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Lookout))
            {
                var lookout = Role.GetRole<Lookout>(PlayerControl.LocalPlayer);
                lookout.LastWatched = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Cleric))
            {
                var cleric = Role.GetRole<Cleric>(PlayerControl.LocalPlayer);
                cleric.LastBarrier = DateTime.UtcNow;
            }
            foreach (Cleric cleric in Role.GetRoles(RoleEnum.Cleric))
            {
                cleric.BarrieredPlayer = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Crusader))
            {
                var crusader = Role.GetRole<Crusader>(PlayerControl.LocalPlayer);
                crusader.LastFortified = DateTime.UtcNow;
            }
            foreach (Crusader crusader in Role.GetRoles(RoleEnum.Crusader))
            {
                crusader.FortifiedPlayers.Clear();
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Bodyguard))
            {
                var bodyguard = Role.GetRole<Bodyguard>(PlayerControl.LocalPlayer);
                bodyguard.LastGuard = DateTime.UtcNow;
            }
            foreach (Bodyguard bodyguard in Role.GetRoles(RoleEnum.Bodyguard))
            {
                bodyguard.GuardedPlayer = null;
            }
            #endregion
            #region NeutralRoles
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Survivor))
            {
                var surv = Role.GetRole<Survivor>(PlayerControl.LocalPlayer);
                surv.LastVested = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vampire))
            {
                var vamp = Role.GetRole<Vampire>(PlayerControl.LocalPlayer);
                vamp.LastBit = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
            {
                var ga = Role.GetRole<GuardianAngel>(PlayerControl.LocalPlayer);
                ga.LastProtected = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                var arsonist = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);
                arsonist.LastDoused = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
            {
                var glitch = Role.GetRole<Glitch>(PlayerControl.LocalPlayer);
                glitch.LastKill = DateTime.UtcNow;
                glitch.LastHack = DateTime.UtcNow;
                glitch.LastMimic = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut))
            {
                var juggernaut = Role.GetRole<Juggernaut>(PlayerControl.LocalPlayer);
                juggernaut.LastKill = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf))
            {
                var werewolf = Role.GetRole<Werewolf>(PlayerControl.LocalPlayer);
                werewolf.LastRampaged = DateTime.UtcNow;
                werewolf.LastKilled = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                var plaguebearer = Role.GetRole<Plaguebearer>(PlayerControl.LocalPlayer);
                plaguebearer.LastInfected = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Baker))
            {
                var baker = Role.GetRole<Baker>(PlayerControl.LocalPlayer);
                baker.LastBreaded = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Berserker))
            {
                var berserker = Role.GetRole<Berserker>(PlayerControl.LocalPlayer);
                berserker.LastKill = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector))
            {
                var soulCollector = Role.GetRole<SoulCollector>(PlayerControl.LocalPlayer);
                soulCollector.LastReaped = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Pestilence))
            {
                var pest = Role.GetRole<Pestilence>(PlayerControl.LocalPlayer);
                pest.LastKill = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Famine))
            {
                var famine = Role.GetRole<Famine>(PlayerControl.LocalPlayer);
                famine.LastStarved = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.War))
            {
                var war = Role.GetRole<War>(PlayerControl.LocalPlayer);
                war.LastKill = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Death))
            {
                var death = Role.GetRole<Death>(PlayerControl.LocalPlayer);
                death.LastApocalypse = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer))
            {
                var doom = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                doom.LastObserved = DateTime.UtcNow;
                doom.LastObservedPlayer = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller))
            {
                var serialKiller = Role.GetRole<SerialKiller>(PlayerControl.LocalPlayer);
                serialKiller.LastKill = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Inquisitor))
            {
                var inquisitor = Role.GetRole<Inquisitor>(PlayerControl.LocalPlayer);
                inquisitor.LastAbility = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Pirate))
            {
                var pirate = Role.GetRole<Pirate>(PlayerControl.LocalPlayer);
                pirate.LastDueled = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Witch))
            {
                var witch = Role.GetRole<Witch>(PlayerControl.LocalPlayer);
                witch.LastControl = DateTime.UtcNow;
                witch.ControledPlayer = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.CursedSoul))
            {
                var cursedSoul = Role.GetRole<CursedSoul>(PlayerControl.LocalPlayer);
                cursedSoul.LastSwapped = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.JKNecromancer))
            {
                var necromancer = Role.GetRole<Roles.Necromancer>(PlayerControl.LocalPlayer);
                necromancer.LastRevived = DateTime.UtcNow;
                necromancer.LastKill = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Jackal))
            {
                var jackal = Role.GetRole<Jackal>(PlayerControl.LocalPlayer);
                jackal.LastKill = DateTime.UtcNow;
            }
            #endregion
            #region ImposterRoles
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Escapist))
            {
                var escapist = Role.GetRole<Escapist>(PlayerControl.LocalPlayer);
                escapist.LastEscape = DateTime.UtcNow;
                escapist.EscapeButton.graphic.sprite = TownOfUs.MarkSprite;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Blackmailer))
            {
                var blackmailer = Role.GetRole<Blackmailer>(PlayerControl.LocalPlayer);
                blackmailer.LastBlackmailed = DateTime.UtcNow;
                if (blackmailer.Player.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                {
                    blackmailer.Blackmailed?.myRend().material.SetFloat("_Outline", 0f);
                }
            }
            foreach (var role in Role.GetRoles(RoleEnum.Blackmailer))
            {
                var blackmailer = (Blackmailer)role;
                blackmailer.Blackmailed = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Bomber))
            {
                var bomber = Role.GetRole<Bomber>(PlayerControl.LocalPlayer);
                bomber.PlantButton.graphic.sprite = TownOfUs.PlantSprite;
                bomber.Bomb.ClearBomb();
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Grenadier))
            {
                var grenadier = Role.GetRole<Grenadier>(PlayerControl.LocalPlayer);
                grenadier.LastFlashed = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Miner))
            {
                var miner = Role.GetRole<Miner>(PlayerControl.LocalPlayer);
                miner.LastMined = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Morphling))
            {
                var morphling = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);
                morphling.LastMorphed = DateTime.UtcNow;
                morphling.MorphButton.graphic.sprite = TownOfUs.SampleSprite;
                morphling.SampledPlayer = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Swooper))
            {
                var swooper = Role.GetRole<Swooper>(PlayerControl.LocalPlayer);
                swooper.LastSwooped = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Venerer))
            {
                var venerer = Role.GetRole<Venerer>(PlayerControl.LocalPlayer);
                venerer.LastCamouflaged = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker))
            {
                var undertaker = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);
                undertaker.LastDragged = DateTime.UtcNow;
                undertaker.DragDropButton.graphic.sprite = TownOfUs.DragSprite;
                undertaker.CurrentlyDragging = null;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Necromancer))
            {
                var necro = Role.GetRole<Roles.Cultist.Necromancer>(PlayerControl.LocalPlayer);
                necro.LastRevived = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Whisperer))
            {
                var whisperer = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);
                whisperer.LastWhispered = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Sniper))
            {
                var sniper = Role.GetRole<Sniper>(PlayerControl.LocalPlayer);
                sniper.LastAim = DateTime.UtcNow;
            }
            #endregion
            #region Modifiers
            foreach (var modifier in Modifier.GetModifiers(ModifierEnum.Drunk))
            {
                var drunk = (Drunk)modifier;
                drunk.RoundsLeft -= 1;
            }
            #endregion
        }

        public static string GetPossibleRoleCategory(PlayerControl player)
        {
            if (player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Witch)
                 || player.Is(RoleEnum.Spy) || player.Is(RoleEnum.Glitch) || player.Is(RoleEnum.Death))
                return $"{player.GetDefaultOutfit().PlayerName} has an altered perception of reality";
            else if (player.Is(RoleEnum.Detective) || player.Is(RoleEnum.Doomsayer) || player.Is(RoleEnum.Inspector)
                 || player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Snitch) || player.Is(RoleEnum.Lookout))
                return $"{player.GetDefaultOutfit().PlayerName} has an insight for private information";
            else if (player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor)
                 || player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.JKNecromancer) || player.Is(RoleEnum.SoulCollector))
                return $"{player.GetDefaultOutfit().PlayerName} has an unusual obsession with dead bodies";
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Tracker) || player.Is(RoleEnum.Hunter)
                 || player.Is(RoleEnum.Werewolf) || player.Is(RoleEnum.Berserker) || player.Is(RoleEnum.Inquisitor))
                return $"{player.GetDefaultOutfit().PlayerName} is well trained in hunting down prey";
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                 || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Transporter) || player.Is(RoleEnum.Pirate))
                return $"{player.GetDefaultOutfit().PlayerName} spreads fear amonst the group";
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Bodyguard) || player.Is(RoleEnum.Escapist)
                 || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Survivor) || player.Is(RoleEnum.Swooper))
                return $"{player.GetDefaultOutfit().PlayerName} hides to guard themself or others";
            else if (player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Pestilence) || player.Is(RoleEnum.Undercover)
                 || player.Is(RoleEnum.Traitor) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Famine))
                return $"{player.GetDefaultOutfit().PlayerName} has a trick up their sleeve";
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Crusader)
                 || player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.War))
                return $"{player.GetDefaultOutfit().PlayerName} is capable of performing relentless attacks";
            else if (player.Is(RoleEnum.Warlock) || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.Mystic)
                || player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.VampireHunter))
                return $"{player.GetDefaultOutfit().PlayerName} knows thing or two about magic";
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Prosecutor) || player.Is(RoleEnum.GuardianAngel)
                || player.Is(RoleEnum.Mayor) || player.Is(RoleEnum.Blackmailer) || player.Is(RoleEnum.Deputy))
                return $"{player.GetDefaultOutfit().PlayerName} knows perfectly how the law works";
            else if (player.Is(RoleEnum.TavernKeeper) || player.Is(RoleEnum.Poisoner) || player.Is(RoleEnum.SerialKiller)
                || player.Is(RoleEnum.Aurial) || player.Is(RoleEnum.Baker) || player.Is(RoleEnum.Grenadier))
                return $"{player.GetDefaultOutfit().PlayerName} loves parties";
            else if (player.Is(RoleEnum.Jackal) || player.Is(RoleEnum.Sniper) || player.Is(RoleEnum.Monarch)
                 || player.Is(RoleEnum.Trapper) || player.Is(RoleEnum.Vampire) || player.Is(RoleEnum.Cleric))
                return $"{player.GetDefaultOutfit().PlayerName} wants to keep his hands clean";
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return $"{player.GetDefaultOutfit().PlayerName} appears to be roleless";
            else
                return "Error";
        }

        public static string GetPossibleRoleList(PlayerControl player)
        {
            if (player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                 || player.Is(RoleEnum.Morphling)
                 || player.Is(RoleEnum.Spy) || player.Is(RoleEnum.Glitch) || player.Is(RoleEnum.Death) || player.Is(RoleEnum.Witch))
                return "(Imitator, Morphling, Spy, Glitch, Death or Witch)";
            else if (player.Is(RoleEnum.Detective) || player.Is(RoleEnum.Doomsayer) || player.Is(RoleEnum.Inspector)
                 || player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Snitch) || player.Is(RoleEnum.Lookout))
                return "(Detective, Doomsayer, Oracle, Snitch, Inspector or Lookout)";
            else if (player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Janitor)
                 || player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.JKNecromancer) || player.Is(RoleEnum.SoulCollector))
                return "(Altruist, Amnesiac, Janitor, Undertaker, Soul Collector or Necromancer)";
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Tracker) || player.Is(RoleEnum.Hunter)
                 || player.Is(RoleEnum.Inquisitor) || player.Is(RoleEnum.Werewolf) || player.Is(RoleEnum.Berserker))
                return "(Investigator, Tracker, Werewolf, Hunter, Berserker or Inquisitor)";
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                 || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Transporter) || player.Is(RoleEnum.Pirate))
                return "(Arsonist, Miner, Plaguebearer, Seer, Transporter or Pirate)";
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Bodyguard) || player.Is(RoleEnum.Escapist)
                 || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Survivor) || player.Is(RoleEnum.Swooper))
                return "(Engineer, Escapist, Medic, Survivor, Swooper or Bodyguard)";
            else if (player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Pestilence) || player.Is(RoleEnum.Undercover)
                 || player.Is(RoleEnum.Traitor) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Famine))
                return "(Jester, Pestilence, Traitor, Veteran, Famine or Undercover)";
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut) || player.Is(RoleEnum.Crusader)
                 || player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.War))
                return "(Bomber, Juggernaut, Sheriif, Vigilante, War or Crusader)";
            else if (player.Is(RoleEnum.Warlock) || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.Mystic)
                 || player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.VampireHunter))
                return "(Warlock, Venerer, Mystic, Swapper, Medium or Vampire Hunter)";
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Prosecutor) || player.Is(RoleEnum.GuardianAngel)
                 || player.Is(RoleEnum.Mayor) || player.Is(RoleEnum.Blackmailer) || player.Is(RoleEnum.Deputy))
                return "(Executioner, Prosecutor, Guardian Angel, Mayor, Blackmailer or Deputy)";
            else if (player.Is(RoleEnum.TavernKeeper) || player.Is(RoleEnum.Poisoner) || player.Is(RoleEnum.Grenadier)
                 || player.Is(RoleEnum.SerialKiller) || player.Is(RoleEnum.Aurial) || player.Is(RoleEnum.Baker))
                return "(Tavern Keeper, Poisoner, Serial Killer, Aurial, Baker or Grenadier)";
            else if (player.Is(RoleEnum.Jackal) || player.Is(RoleEnum.Sniper) || player.Is(RoleEnum.Monarch)
                 || player.Is(RoleEnum.Trapper) || player.Is(RoleEnum.Vampire) || player.Is(RoleEnum.Cleric))
                return "(Jackal, Sniper, Monarch, Trapper, Vampire or Cleric)";
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return "(Crewmate or Impostor)";
            else
                return "Error";
        }
        public static Color GetRoleColor(this RoleEnum? role)
        {
            return ((RoleEnum)role).GetRoleColor();
        }
        public static Color GetRoleColor(this RoleEnum role)
        {
            switch (role)
            {
                case RoleEnum.Impostor:
                case RoleEnum.Escapist:
                case RoleEnum.Grenadier:
                case RoleEnum.Morphling:
                case RoleEnum.Swooper:
                case RoleEnum.Venerer:
                case RoleEnum.Bomber:
                case RoleEnum.Traitor:
                case RoleEnum.Warlock:
                case RoleEnum.Poisoner:
                case RoleEnum.Sniper:
                case RoleEnum.Blackmailer:
                case RoleEnum.Janitor:
                case RoleEnum.Miner:
                case RoleEnum.Undertaker:
                case RoleEnum.Whisperer:
                case RoleEnum.Necromancer:
                case RoleEnum.SoloKiller:
                    return Colors.Impostor;

                case RoleEnum.Plaguebearer:
                    return Colors.Plaguebearer;
                case RoleEnum.Pestilence:
                    return Colors.Pestilence;
                case RoleEnum.Baker:
                    return Colors.Baker;
                case RoleEnum.Famine:
                    return Colors.Famine;
                case RoleEnum.Berserker:
                    return Colors.Berserker;
                case RoleEnum.War:
                    return Colors.War;
                case RoleEnum.SoulCollector:
                    return Colors.SoulCollector;
                case RoleEnum.Death:
                    return Colors.Death;
                case RoleEnum.Arsonist:
                    return Colors.Arsonist;
                case RoleEnum.Glitch:
                    return Colors.Glitch;
                case RoleEnum.Vampire:
                    return Colors.Vampire;
                case RoleEnum.Werewolf:
                    return Colors.Werewolf;
                case RoleEnum.SerialKiller:
                    return Colors.SerialKiller;
                case RoleEnum.Juggernaut:
                    return Colors.Juggernaut;
                case RoleEnum.Doomsayer:
                    return Colors.Doomsayer;
                case RoleEnum.Executioner:
                    return Colors.Executioner;
                case RoleEnum.Jester:
                    return Colors.Jester;
                case RoleEnum.Phantom:
                    return Colors.Phantom;
                case RoleEnum.Pirate:
                    return Colors.Pirate;
                case RoleEnum.Inquisitor:
                    return Colors.Inquisitor;
                case RoleEnum.Amnesiac:
                    return Colors.Amnesiac;
                case RoleEnum.GuardianAngel:
                    return Colors.GuardianAngel;
                case RoleEnum.Survivor:
                    return Colors.Survivor;
                case RoleEnum.Witch:
                    return Colors.Witch;
                case RoleEnum.CursedSoul:
                    return Colors.CursedSoul;
                case RoleEnum.JKNecromancer:
                    return Colors.Necromancer;
                case RoleEnum.Jackal:
                    return Colors.Jackal;

                case RoleEnum.Mayor:
                    return Colors.Mayor;
                case RoleEnum.Oracle:
                    return Colors.Oracle;
                case RoleEnum.Prosecutor:
                    return Colors.Prosecutor;
                case RoleEnum.Swapper:
                    return Colors.Swapper;
                case RoleEnum.Monarch:
                    return Colors.Monarch;
                case RoleEnum.Chameleon:
                    return Colors.Chameleon;
                case RoleEnum.Engineer:
                    return Colors.Engineer;
                case RoleEnum.Imitator:
                    return Colors.Imitator;
                case RoleEnum.Medium:
                    return Colors.Medium;
                case RoleEnum.Transporter:
                    return Colors.Transporter;
                case RoleEnum.TavernKeeper:
                    return Colors.TavernKeeper;
                case RoleEnum.Undercover:
                    return Colors.Undercover;
                case RoleEnum.Altruist:
                    return Colors.Altruist;
                case RoleEnum.Medic:
                    return Colors.Medic;
                case RoleEnum.Sheriff:
                    return Colors.Sheriff;
                case RoleEnum.VampireHunter:
                    return Colors.VampireHunter;
                case RoleEnum.Veteran:
                    return Colors.Veteran;
                case RoleEnum.Vigilante:
                    return Colors.Vigilante;
                case RoleEnum.Aurial:
                    return Colors.Aurial;
                case RoleEnum.Detective:
                    return Colors.Detective;
                case RoleEnum.Haunter:
                    return Colors.Haunter;
                case RoleEnum.Investigator:
                    return Colors.Investigator;
                case RoleEnum.Mystic:
                case RoleEnum.CultistMystic:
                    return Colors.Mystic;
                case RoleEnum.Seer:
                case RoleEnum.CultistSeer:
                    return Colors.Seer;
                case RoleEnum.Snitch:
                case RoleEnum.CultistSnitch:
                    return Colors.Snitch;
                case RoleEnum.Spy:
                    return Colors.Spy;
                case RoleEnum.Tracker:
                    return Colors.Trapper;
                case RoleEnum.Trapper:
                    return Colors.Trapper;
                case RoleEnum.Inspector:
                    return Colors.Inspector;
                case RoleEnum.Lookout:
                    return Colors.Lookout;
                case RoleEnum.Deputy:
                    return Colors.Deputy;
                case RoleEnum.Crusader:
                    return Colors.Crusader;
                case RoleEnum.Cleric:
                    return Colors.Cleric;
                case RoleEnum.Bodyguard:
                    return Colors.Bodyguard;

                case RoleEnum.RedMember:
                    return Colors.RedTeam;
                case RoleEnum.BlueMember:
                    return Colors.BlueTeam;
                case RoleEnum.YellowMember:
                    return Colors.YellowTeam;
                case RoleEnum.GreenMember:
                    return Colors.GreenTeam;

                default:
                    return Colors.Crewmate;
            }
        }
        public static string GetRoleName(this RoleEnum? role)
        {
            return ((RoleEnum)role).GetRoleName();
        }
        public static string GetRoleName(this RoleEnum role)
        {
            switch (role)
            {
                case RoleEnum.Impostor:
                    return "Impostor";
                case RoleEnum.Escapist:
                    return "Escapist";
                case RoleEnum.Grenadier:
                    return "Grenadier";
                case RoleEnum.Morphling:
                    return "Morphling";
                case RoleEnum.Swooper:
                    return "Swooper";
                case RoleEnum.Venerer:
                    return "Venerer";
                case RoleEnum.Bomber:
                    return "Bomber";
                case RoleEnum.Traitor:
                    return "Traitor";
                case RoleEnum.Warlock:
                    return "Warlock";
                case RoleEnum.Poisoner:
                    return "Poisoner";
                case RoleEnum.Sniper:
                    return "Sniper";
                case RoleEnum.Blackmailer:
                    return "Blackmailer";
                case RoleEnum.Janitor:
                    return "Janitor";
                case RoleEnum.Miner:
                    return "Miner";
                case RoleEnum.Undertaker:
                    return "Undertaker";
                case RoleEnum.Whisperer:
                    return "Whisperer";
                case RoleEnum.Necromancer:
                case RoleEnum.JKNecromancer:
                    return "Necromancer";
                case RoleEnum.SoloKiller:
                    return "Solo Killer";

                case RoleEnum.Plaguebearer:
                    return "Plaguebearer";
                case RoleEnum.Pestilence:
                    return "Pestilence";
                case RoleEnum.Baker:
                    return "Baker";
                case RoleEnum.Famine:
                    return "Famine";
                case RoleEnum.Berserker:
                    return "Berserker";
                case RoleEnum.War:
                    return "War";
                case RoleEnum.SoulCollector:
                    return "Soul Collector";
                case RoleEnum.Death:
                    return "Death";
                case RoleEnum.Arsonist:
                    return "Arsonist";
                case RoleEnum.Glitch:
                    return "The Glitch";
                case RoleEnum.Vampire:
                    return "Vampire";
                case RoleEnum.Werewolf:
                    return "Werewolf";
                case RoleEnum.SerialKiller:
                    return "Serial Killer";
                case RoleEnum.Juggernaut:
                    return "Juggernaut";
                case RoleEnum.Doomsayer:
                    return "Doomsayer";
                case RoleEnum.Executioner:
                    return "Executioner";
                case RoleEnum.Jester:
                    return "Jester";
                case RoleEnum.Phantom:
                    return "Phantom";
                case RoleEnum.Pirate:
                    return "Pirate";
                case RoleEnum.Inquisitor:
                    return "Inquisitor";
                case RoleEnum.Amnesiac:
                    return "Amnesiac";
                case RoleEnum.GuardianAngel:
                    return "Guardian Angel";
                case RoleEnum.Survivor:
                    return "Survivor";
                case RoleEnum.Witch:
                    return "Witch";
                case RoleEnum.CursedSoul:
                    return "Cursed Soul";
                case RoleEnum.Jackal:
                    return "Jackal";

                case RoleEnum.Mayor:
                    return "Mayor";
                case RoleEnum.Oracle:
                    return "Oracle";
                case RoleEnum.Prosecutor:
                    return "Prosecutor";
                case RoleEnum.Swapper:
                    return "Swapper";
                case RoleEnum.Monarch:
                    return "Monarch";
                case RoleEnum.Chameleon:
                    return "Chameleon";
                case RoleEnum.Engineer:
                    return "Engineer";
                case RoleEnum.Imitator:
                    return "Imitator";
                case RoleEnum.Medium:
                    return "Medium";
                case RoleEnum.Transporter:
                    return "Transporter";
                case RoleEnum.TavernKeeper:
                    return "Tavern Keeper";
                case RoleEnum.Undercover:
                    return "Undercover";
                case RoleEnum.Altruist:
                    return "Altruist";
                case RoleEnum.Medic:
                    return "Medic";
                case RoleEnum.Sheriff:
                    return "Sheriff";
                case RoleEnum.VampireHunter:
                    return "Vampire Hunter";
                case RoleEnum.Veteran:
                    return "Veteran";
                case RoleEnum.Vigilante:
                    return "Vigilante";
                case RoleEnum.Aurial:
                    return "Aurial";
                case RoleEnum.Detective:
                    return "Detective";
                case RoleEnum.Haunter:
                    return "Haunter";
                case RoleEnum.Investigator:
                    return "Investigator";
                case RoleEnum.Mystic:
                case RoleEnum.CultistMystic:
                    return "Mystic";
                case RoleEnum.Seer:
                case RoleEnum.CultistSeer:
                    return "Seer";
                case RoleEnum.Snitch:
                case RoleEnum.CultistSnitch:
                    return "Snitch";
                case RoleEnum.Spy:
                    return "Spy";
                case RoleEnum.Tracker:
                    return "Tracker";
                case RoleEnum.Trapper:
                    return "Trapper";
                case RoleEnum.Inspector:
                    return "Inspector";
                case RoleEnum.Lookout:
                    return "Lookout";
                case RoleEnum.Deputy:
                    return "Deputy";
                case RoleEnum.Crusader:
                    return "Crusader";
                case RoleEnum.Cleric:
                    return "Cleric";
                case RoleEnum.Bodyguard:
                    return "Bodyguard";

                case RoleEnum.RedMember:
                case RoleEnum.BlueMember:
                case RoleEnum.YellowMember:
                case RoleEnum.GreenMember:
                    return "Member";

                default:
                    return "Crewmate";
            }
        }
        public static string GetFactionOverrideDescription(this FactionOverride? factionOverride)
        {
            return ((FactionOverride)factionOverride).GetFactionOverrideDescription();
        }
        public static string GetFactionOverrideDescription(this FactionOverride factionOverride)
        {
            switch (factionOverride)
            {
                case FactionOverride.Undead: return $"<color=#{Patches.Colors.Necromancer.ToHtmlStringRGBA()}>Faction: Undead\nHelp the Necromancer get rid off the crew!</color>";
                case FactionOverride.Recruit: return $"<color=#{Patches.Colors.Jackal.ToHtmlStringRGBA()}>Faction: Jackal\nHelp the Jackal kill off the crew!</color>";
                default: return "";
            }
        }
    }
}
