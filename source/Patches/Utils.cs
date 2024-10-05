using HarmonyLib;
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
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace TownOfUs
{
    [HarmonyPatch]
    public static class Utils
    {
        internal static bool ShowDeadBodies = false;
        private static GameData.PlayerInfo voteTarget = null;
        public static bool IsMeeting = true;
        public static GameObject ChatButton;
        public static bool VariableA = true;
        public static List<byte> synchronizedPlayers = new();
        public static void TeleportRpc(this PlayerControl player, Vector2 position)
        {
            Utils.Rpc(CustomRPC.Escape, player.PlayerId, position);
            player.Teleport(position);
        }
        public static void Teleport(this PlayerControl player, Vector2 position)
        {
            player.MyPhysics.ResetMoveState();
            player.NetTransform.SnapTo(new Vector2(position.x, position.y));

            if (SubmergedCompatibility.isSubmerged())
            {
                if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
                {
                    SubmergedCompatibility.ChangeFloor(player.GetTruePosition().y > -7);
                    SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
                }
            }

            if (PlayerControl.LocalPlayer.PlayerId == player.PlayerId)
            {
                if (Minigame.Instance) Minigame.Instance.Close();
            }

            player.moveable = true;
            player.Collider.enabled = true;
            player.NetTransform.enabled = true;
        }

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

        public static bool IsCooperator(this PlayerControl player)
        {
            return player.Is(ObjectiveEnum.Cooperator);
        }

        public static bool IsRival(this PlayerControl player)
        {
            return player.Is(ObjectiveEnum.Rival);
        }
        public static bool RivalCanWin(this PlayerControl player)
        {
            if (player?.IsRival() != true) return true;

            var playerData = player.Data;
            if (playerData.Disconnected || playerData.IsDead) return false;

            var objective = Objective.GetObjective<Rival>(player);
            var otherRival = objective?.OtherRival?.Player;

            return otherRival == null || otherRival.Data.Disconnected || otherRival.Data.IsDead;
        }

        public static bool Is(this PlayerControl player, RoleEnum roleType)
        {
            return Role.GetRole(player)?.RoleType == roleType;
        }

        public static bool IsRoleblocked(this PlayerControl player)
        {
            return (Role.GetRole(player) != null && Role.GetRole(player).Roleblocked) || player.IsRoleD();
        }

        public static bool IsSuperRoleblocked(this PlayerControl player)
        {
            return (Role.GetRole(player) != null && Role.GetRole(player).SuperRoleblocked) || player.IsRoleD();
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
            return player.IsLover() && ((IsMeeting && (CustomGameOptions.LoversChat == AllowChat.Meeting || CustomGameOptions.LoversChat == AllowChat.Both)) || (!IsMeeting && (CustomGameOptions.LoversChat == AllowChat.Rounds || CustomGameOptions.LoversChat == AllowChat.Both)));
        }
        public static bool CooperatorChat(this PlayerControl player)
        {
            return player.IsCooperator() && ((IsMeeting && (CustomGameOptions.CooperatorsChat == AllowChat.Meeting || CustomGameOptions.CooperatorsChat == AllowChat.Both)) || (!IsMeeting && (CustomGameOptions.CooperatorsChat == AllowChat.Rounds || CustomGameOptions.CooperatorsChat == AllowChat.Both)));
        }
        public static bool VampireChat(this PlayerControl player)
        {
            return player.Is(RoleEnum.Vampire) && ((IsMeeting && (CustomGameOptions.VampiresChat == AllowChat.Meeting || CustomGameOptions.VampiresChat == AllowChat.Both)) || (!IsMeeting && (CustomGameOptions.VampiresChat == AllowChat.Rounds || CustomGameOptions.VampiresChat == AllowChat.Both)));
        }
        public static bool RecruitChat(this PlayerControl player)
        {
            return player.Is(FactionOverride.Recruit) && !(player.Is(RoleEnum.Jackal) && !CustomGameOptions.RecruistSeeJackal) && ((IsMeeting && (CustomGameOptions.RecruitsChat == AllowChat.Meeting || CustomGameOptions.RecruitsChat == AllowChat.Both)) || (!IsMeeting && (CustomGameOptions.RecruitsChat == AllowChat.Rounds || CustomGameOptions.RecruitsChat == AllowChat.Both)));
        }
        public static bool UndeadChat(this PlayerControl player)
        {
            return player.Is(FactionOverride.Undead) && ((IsMeeting && (CustomGameOptions.UndeadChat == AllowChat.Meeting || CustomGameOptions.UndeadChat == AllowChat.Both)) || (!IsMeeting && (CustomGameOptions.UndeadChat == AllowChat.Rounds || CustomGameOptions.UndeadChat == AllowChat.Both)));
        }
        public static bool ImpostorChat(this PlayerControl player)
        {
            return ((player.Data.IsImpostor() && !player.Is((RoleEnum)254)) || (player.Is(RoleEnum.Undercover) && UndercoverIsImpostor()) || player.Is(ObjectiveEnum.ImpostorAgent)) && ((IsMeeting && (CustomGameOptions.ImpostorsChat == AllowChat.Meeting || CustomGameOptions.ImpostorsChat == AllowChat.Both)) || (!IsMeeting && (CustomGameOptions.ImpostorsChat == AllowChat.Rounds || CustomGameOptions.ImpostorsChat == AllowChat.Both)));
        }
        public static bool ApocalypseChat(this PlayerControl player)
        {
            return (player.Is(Faction.NeutralApocalypse) || (player.Is(RoleEnum.Undercover) && UndercoverIsApocalypse()) || player.Is(ObjectiveEnum.ApocalypseAgent)) && ((IsMeeting && (CustomGameOptions.ApocalypseChat == AllowChat.Meeting || CustomGameOptions.ApocalypseChat == AllowChat.Both)) || (!IsMeeting && (CustomGameOptions.ApocalypseChat == AllowChat.Rounds || CustomGameOptions.ApocalypseChat == AllowChat.Both)));
        }
        public static bool Chat(this PlayerControl player)
        {
            return player.LoverChat() || player.CooperatorChat() || player.VampireChat() || player.RecruitChat() || player.UndeadChat() || player.ImpostorChat() || player.ApocalypseChat();
        }

        public static List<PlayerControl> GetCrewmates(List<PlayerControl> impostors)
        {
            return PlayerControl.AllPlayerControls.ToArray().Where(
                player => !impostors.Any(imp => imp.PlayerId == player.PlayerId) && !player.IsSpectator()
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
            return Role.GetRoles(RoleEnum.Monarch).Any(role =>
            {
                var monarch = (Monarch)role;
                return monarch != null && monarch.Knights.Contains(player.PlayerId);
            });
        }

        public static bool ToKnight(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Monarch).Any(role =>
            {
                var monarch = (Monarch)role;
                return monarch != null && monarch.toKnight.Contains(player.PlayerId);
            });
        }

        public static bool IsBugged(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Spy).Any(role =>
            {
                var spy = (Spy)role;
                return spy != null && spy.BuggedPlayers.Contains(player.PlayerId);
            });
        }

        public static bool IsHeretic(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Inquisitor).Any(role =>
            {
                var inquisitor = (Inquisitor)role;
                return inquisitor != null && inquisitor.heretics.Contains(player.PlayerId);
            });
        }

        public static bool IsInVision(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Mystic).Any(role =>
            {
                var visionPlayer = ((Mystic)role).VisionPlayer;
                return player.PlayerId == visionPlayer;
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
                var crusader = (Crusader)role;
                return crusader != null && crusader.FortifiedPlayers.Contains(player.PlayerId);
            });
        }

        public static Color? GetPlayerNameColor(this PlayerControl player)
        {
            Color? result = null;
            var role = Role.GetRole(player);
            if (role.ColorCriteria())
                result = role.Color;
            if (role.RoleType == RoleEnum.Undercover)
            {
                if (((((Undercover)role).UndercoverImpostor && (PlayerControl.LocalPlayer.Data.IsImpostor() || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ImpostorAgent)) && !PlayerControl.LocalPlayer.Is((RoleEnum)254)) || (((Undercover)role).UndercoverApocalypse) && (PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse) || PlayerControl.LocalPlayer.Is(ObjectiveEnum.ApocalypseAgent))) && !(PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.DeadSeeRoles && Utils.ShowDeadBodies) && !(PlayerControl.LocalPlayer.Is(role.FactionOverride) && role.FactionOverride != FactionOverride.None))
                    result = ((Undercover)role).UndercoverRole.GetRoleColor();
            }
            else if (role.Faction == Faction.Impostors && PlayerControl.LocalPlayer.Data.IsImpostor() && !PlayerControl.LocalPlayer.Is((RoleEnum)254) && role.RoleType != (RoleEnum)254)
                result = Patches.Colors.Impostor;
            else if (PlayerControl.LocalPlayer.Data.IsImpostor() && role.RoleType == (RoleEnum)254 && player != PlayerControl.LocalPlayer)
                result = null;
            return result;
        }

        public static bool IsSpectator(this PlayerControl player)
        {
            return SpectatorPatch.Spectators.Contains(player.PlayerId);
        }

        public static bool IsRoleC(this PlayerControl player)
        {
            return Role.GetRoles((RoleEnum)253).Any(role =>
            {
                var target = ((RoleC)role).AbilityA0;
                return player.PlayerId == target;
            });
        }

        public static bool IsRoleD(this PlayerControl player)
        {
            return Role.GetRoles((RoleEnum)252).Any(role =>
            {
                var roled = (RoleD)role;
                return roled != null && roled.AbilityA0.Contains(player.PlayerId);
            });
        }

        public static bool IsRoleF(this PlayerControl player)
        {
            return Role.GetRoles((RoleEnum)250).Any(role =>
            {
                var rolef = (RoleF)role;
                return rolef != null && rolef.Player != null && !rolef.Player.Data.IsDead && !rolef.Player.Data.Disconnected && rolef.AbilityA0.Contains(player.PlayerId);
            });
        }

        public static bool PoltergeistTasks()
        {
            return Role.GetRoles(RoleEnum.Poltergeist).Any(role =>
            {
                return ((Poltergeist)role).CompletedTasks;
            });
        }

        public static Pirate GetPirate(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Pirate).FirstOrDefault(role =>
            {
                var dueledPlayer = ((Pirate)role).DueledPlayer;
                return dueledPlayer != null && player.PlayerId == dueledPlayer.PlayerId;
            }) as Pirate;
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

        public static RoleC GetRoleC(this PlayerControl player)
        {
            return Role.GetRoles((RoleEnum)253).FirstOrDefault(role =>
            {
                var target = ((RoleC)role).AbilityA0;
                return player.PlayerId == target;
            }) as RoleC;
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
            }) || Role.GetRoles((RoleEnum)249).Any(x => 
            ((RoleG)x).AbilityA0.Any(y => Vector2.Distance(player.transform.position,
            y.transform.position + new Vector3(0f, 0.1f, 0f)) <= 3.3f));
        }

        public static bool IsInfected(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Plaguebearer).Any(role =>
            {
                var plaguebearer = (Plaguebearer)role;
                return plaguebearer != null && (plaguebearer.InfectedPlayers.Contains(player.PlayerId) || player.PlayerId == plaguebearer.Player.PlayerId);
            });
        }

        public static bool IsConvinced(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Demagogue).Any(role =>
            {
                var demagogue = (Demagogue)role;
                return demagogue != null && demagogue.Convinced.Contains(player.PlayerId);
            });
        }

        public static bool IsMarked(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Occultist).Any(role =>
            {
                var occultist = (Occultist)role;
                return occultist != null && occultist.MarkedPlayers.Contains(player.PlayerId);
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
            if (player.IsRoleblocked())
            {
                if (player == PlayerControl.LocalPlayer)
                {
                    Coroutines.Start(Utils.FlashCoroutine(Color.white));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                }
                zeroSecReset = true;
            }
            else
            {
                if (target.IsInfected() || player.IsInfected())
                {
                    foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(target, player);
                }
                if (target.IsInVision() || player.IsInVision())
                {
                    Rpc(CustomRPC.VisionInteract, player.PlayerId, target.PlayerId);
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
                        else if (target.IsFortified() && player.PlayerId != target.GetCrusader().Player.PlayerId)
                        {
                            if (player.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, player.PlayerId, (byte)RoleEnum.Crusader, (byte)1);
                            if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                            var crus = target.GetCrusader();
                            if (!player.Is(RoleEnum.Pestilence) && !player.Is(RoleEnum.Famine) && !player.Is(RoleEnum.War) && !player.Is(RoleEnum.Death))
                            {
                                if (player.IsShielded())
                                {
                                    Rpc(CustomRPC.AttemptSound, player.GetMedic().Player.PlayerId, player.PlayerId);

                                    System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                                    StopKill.BreakShield(player.GetMedic().Player.PlayerId, player.PlayerId, CustomGameOptions.ShieldBreaks);
                                }
                                else if (player.IsFortified())
                                {
                                    var crus1 = player.GetCrusader();
                                    crus1.FortifiedPlayers.Remove(player.PlayerId);
                                    Rpc(CustomRPC.Unfortify, crus1.Player.PlayerId, player.PlayerId);
                                }
                                else if (player.IsBarriered())
                                {
                                    var cleric = player.GetCleric();
                                    cleric.BarrieredPlayer = null;
                                    Rpc(CustomRPC.Unbarrier, cleric.Player.PlayerId);
                                }
                                else if (player.IsRoleC())
                                {
                                    var rolec = player.GetRoleC();
                                    rolec.AbilityB0 = true;
                                    rolec.AbilityA0 = byte.MaxValue;
                                    Utils.Rpc((CustomRPC)251, rolec.Player.PlayerId, rolec.AbilityA0, true);
                                }
                                else if (!player.IsProtected())
                                {
                                    RpcMultiMurderPlayer(crus.Player, player);
                                }
                            }
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
                            if (player.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, player.PlayerId, (byte)RoleEnum.Bodyguard, (byte)1);
                            var bg = target.GetBodyguard().Player;
                            var tpPos = Vector3.Lerp(player.transform.position, target.transform.position, 0.5f);
                            bg.TeleportRpc(new Vector2(tpPos.x, tpPos.y));
                            if (bg.IsShielded())
                            {
                                if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                                Rpc(CustomRPC.AttemptSound, bg.GetMedic().Player.PlayerId, bg.PlayerId);

                                System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                                StopKill.BreakShield(bg.GetMedic().Player.PlayerId, bg.PlayerId, CustomGameOptions.ShieldBreaks);
                                fullCooldownReset = true;
                            }
                            else if (bg.IsFortified())
                            {
                                if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                                var crus1 = bg.GetCrusader();
                                crus1.FortifiedPlayers.Remove(bg.PlayerId);
                                Rpc(CustomRPC.Unfortify, crus1.Player.PlayerId, bg.PlayerId);
                                fullCooldownReset = true;
                            }
                            else if (bg.IsBarriered() && toKill)
                            {
                                if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                                var cleric = bg.GetCleric();
                                cleric.BarrieredPlayer = null;
                                barrierCdReset = true;
                            }
                            else if (bg.IsProtected())
                            {
                                if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                                gaReset = true;
                                fullCooldownReset = true;
                            }
                            else if (bg.IsRoleC())
                            {
                                var rolec = bg.GetRoleC();
                                rolec.AbilityB0 = true;
                                rolec.AbilityA0 = byte.MaxValue;
                                Utils.Rpc((CustomRPC)251, rolec.Player.PlayerId, rolec.AbilityA0, true);
                                fullCooldownReset = true;
                            }
                            else
                            {
                                if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills += 1;
                                if (player.Is(RoleEnum.Juggernaut)) Role.GetRole<Juggernaut>(player).JuggKills += 1;
                                if (player.Is(RoleEnum.Berserker)) Role.GetRole<Berserker>(player).KilledPlayers += 1;
                                RpcMultiMurderPlayer(player, bg);
                                fullCooldownReset = true;
                            }
                            if (!player.Is(RoleEnum.Pestilence) && !player.Is(RoleEnum.Famine) && !player.Is(RoleEnum.War) && !player.Is(RoleEnum.Death))
                            {
                                if (player.IsShielded())
                                {
                                    Rpc(CustomRPC.AttemptSound, player.GetMedic().Player.PlayerId, player.PlayerId);

                                    System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                                    StopKill.BreakShield(player.GetMedic().Player.PlayerId, player.PlayerId, CustomGameOptions.ShieldBreaks);
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
                                else if (player.IsRoleC())
                                {
                                    var rolec = player.GetRoleC();
                                    rolec.AbilityB0 = true;
                                    rolec.AbilityA0 = byte.MaxValue;
                                    Utils.Rpc((CustomRPC)251, rolec.Player.PlayerId, rolec.AbilityA0, true);
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
                        else if (target.IsRoleC() && toKill)
                        {
                            var rolec = target.GetRoleC();
                            rolec.AbilityB0 = true;
                            rolec.AbilityA0 = byte.MaxValue;
                            Utils.Rpc((CustomRPC)251, rolec.Player.PlayerId, rolec.AbilityA0, true);
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
                            else if (player.Is((RoleEnum)255))
                            {
                                var rolea = Role.GetRole<Roles.RoleA>(player);
                                rolea.LastA = DateTime.UtcNow;
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
                    if (player.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, player.PlayerId, (byte)RoleEnum.Crusader, (byte)1);
                    if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                    var crus = target.GetCrusader();
                    if (!player.Is(RoleEnum.Pestilence) && !player.Is(RoleEnum.Famine) && !player.Is(RoleEnum.War) && !player.Is(RoleEnum.Death))
                    {
                        if (player.IsShielded())
                        {
                            Rpc(CustomRPC.AttemptSound, player.GetMedic().Player.PlayerId, player.PlayerId);

                            System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                            StopKill.BreakShield(player.GetMedic().Player.PlayerId, player.PlayerId, CustomGameOptions.ShieldBreaks);
                        }
                        else if (player.IsFortified())
                        {
                            var crus1 = player.GetCrusader();
                            crus1.FortifiedPlayers.Remove(player.PlayerId);
                            Rpc(CustomRPC.Unfortify, crus1.Player.PlayerId, player.PlayerId);
                        }
                        else if (player.IsBarriered())
                        {
                            var cleric = player.GetCleric();
                            cleric.BarrieredPlayer = null;
                            Rpc(CustomRPC.Unbarrier, cleric.Player.PlayerId);
                        }
                        else if (player.IsRoleC())
                        {
                            var rolec = player.GetRoleC();
                            rolec.AbilityB0 = true;
                            rolec.AbilityA0 = byte.MaxValue;
                            Utils.Rpc((CustomRPC)251, rolec.Player.PlayerId, rolec.AbilityA0, true);
                        }
                        else if (!player.IsProtected())
                        {
                            RpcMultiMurderPlayer(crus.Player, player);
                        }
                    }
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
                    if (player.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, player.PlayerId, (byte)RoleEnum.Bodyguard, (byte)1);
                    var bg = target.GetBodyguard().Player;
                    var tpPos = Vector3.Lerp(player.transform.position, target.transform.position, 0.5f);
                    bg.TeleportRpc(new Vector2(tpPos.x, tpPos.y));
                    if (bg.IsShielded())
                    {
                        if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                        Rpc(CustomRPC.AttemptSound, bg.GetMedic().Player.PlayerId, bg.PlayerId);

                        System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                        StopKill.BreakShield(bg.GetMedic().Player.PlayerId, bg.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (bg.IsFortified())
                    {
                        if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                        var crus1 = bg.GetCrusader();
                        crus1.FortifiedPlayers.Remove(bg.PlayerId);
                        Rpc(CustomRPC.Unfortify, crus1.Player.PlayerId, bg.PlayerId);
                        fullCooldownReset = true;
                    }
                    else if (bg.IsBarriered() && toKill)
                    {
                        if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                        var cleric = bg.GetCleric();
                        cleric.BarrieredPlayer = null;
                        barrierCdReset = true;
                    }
                    else if (bg.IsRoleC() && toKill)
                    {
                        var rolec = bg.GetRoleC();
                        rolec.AbilityB0 = true;
                        rolec.AbilityA0 = byte.MaxValue;
                        Utils.Rpc((CustomRPC)251, rolec.Player.PlayerId, rolec.AbilityA0, true);
                        fullCooldownReset = true;
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
                            Rpc(CustomRPC.AttemptSound, player.GetMedic().Player.PlayerId, player.PlayerId);

                            System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");
                            StopKill.BreakShield(player.GetMedic().Player.PlayerId, player.PlayerId, CustomGameOptions.ShieldBreaks);
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
                        else if (player.IsRoleC())
                        {
                            var rolec = player.GetRoleC();
                            rolec.AbilityB0 = true;
                            rolec.AbilityA0 = byte.MaxValue;
                            Utils.Rpc((CustomRPC)251, rolec.Player.PlayerId, rolec.AbilityA0, true);
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
                else if (target.IsRoleC() && toKill)
                {
                    var rolec = target.GetRoleC();
                    rolec.AbilityB0 = true;
                    rolec.AbilityA0 = byte.MaxValue;
                    Utils.Rpc((CustomRPC)251, rolec.Player.PlayerId, rolec.AbilityA0, true);
                    fullCooldownReset = true;
                }
                else if (target.IsProtected() && toKill)
                {
                    if (player.Is(RoleEnum.SerialKiller)) Role.GetRole<SerialKiller>(player).SKKills = 0;
                    gaReset = true;
                }
                else if (target.Is((RoleEnum)253) && toKill)
                {
                    var rolec = Role.GetRole<RoleC>(target);
                    rolec.AbilityB0 = true;
                    rolec.AbilityA0 = byte.MaxValue;
                    Utils.Rpc((CustomRPC)251, rolec.Player.PlayerId, rolec.AbilityA0, true);
                    fullCooldownReset = true;
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
                    else if (player.Is((RoleEnum)255))
                    {
                        var rolea = Role.GetRole<Roles.RoleA>(player);
                        rolea.LastA = DateTime.UtcNow;
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
                (targets ?? PlayerControl.AllPlayerControls.ToArray().ToList()).ToArray().Where(x => !x.IsRoleD() && !PlayerControl.LocalPlayer.IsRoleD()).ToList()
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
                (targets ?? PlayerControl.AllPlayerControls.ToArray().ToList()).ToArray().Where(x => !x.IsRoleD() && !PlayerControl.LocalPlayer.IsRoleD()).ToList()
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

                if (killer == PlayerControl.LocalPlayer || AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Started)
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);

                if (!killer.Is(Faction.Crewmates) && killer != target
                    && GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.Normal && AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started) Role.GetRole(killer).Kills += 1;

                if (killer.Is(RoleEnum.Sheriff))
                {
                    var sheriff = Role.GetRole<Sheriff>(killer);
                    if (sheriff.FactionOverride != FactionOverride.None || killer.Is(ObjectiveEnum.ImpostorAgent) || killer.Is(ObjectiveEnum.ApocalypseAgent)) sheriff.Kills += 1;
                    else if (target.Is(Faction.Impostors) ||
                        target.Is(RoleEnum.Glitch) && CustomGameOptions.SheriffKillsGlitch ||
                        target.Is(RoleEnum.Arsonist) && CustomGameOptions.SheriffKillsArsonist ||
                        target.Is(RoleEnum.Plaguebearer) && CustomGameOptions.SheriffKillsPlaguebearer ||
                        target.Is(RoleEnum.Pestilence) && CustomGameOptions.SheriffKillsPlaguebearer ||
                        target.Is(RoleEnum.Baker) && CustomGameOptions.SheriffKillsBaker ||
                        target.Is(RoleEnum.Famine) && CustomGameOptions.SheriffKillsBaker ||
                        target.Is(RoleEnum.Berserker) && CustomGameOptions.SheriffKillsBerserker ||
                        target.Is(RoleEnum.War) && CustomGameOptions.SheriffKillsBerserker ||
                        target.Is(RoleEnum.SoulCollector) && CustomGameOptions.SheriffKillsSoulCollector ||
                        target.Is(RoleEnum.Death) && CustomGameOptions.SheriffKillsSoulCollector ||
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
                if (killer.Is(RoleEnum.Crusader))
                {
                    var crus = Role.GetRole<Crusader>(killer);
                    if (target.Is(Faction.Crewmates) && target.Is(FactionOverride.None) && !target.Is(ObjectiveEnum.ImpostorAgent) && !target.Is(ObjectiveEnum.ApocalypseAgent))
                    {
                        crus.IncorrectKills += 1;
                    }
                    else
                    {
                        crus.CorrectKills += 1;
                    }
                }

                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                target.Visible = false;

                if (target.Is(ModifierEnum.Famous))
                {
                    Coroutines.Start(FlashCoroutine(Patches.Colors.Famous));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Famous Has Died!" : "Famous Zginal!", 1000 * CustomGameOptions.NotificationDuration);
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    Coroutines.Start(FlashCoroutine(Patches.Colors.Mystic));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Someone Have Died!" : "Ktos Zginal!", 1000 * CustomGameOptions.NotificationDuration);
                }
                else if (PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector) && !PlayerControl.LocalPlayer.Data.IsDead && CustomGameOptions.SCKillNotif)
                {
                    Coroutines.Start(FlashCoroutine(Patches.Colors.SoulCollector));
                    NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Someone Have Died!" : "Ktos Zginal!", 1000 * CustomGameOptions.NotificationDuration);
                    Role.GetRole<SoulCollector>(PlayerControl.LocalPlayer);
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
                if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started) Role.GetRole(killer).LastBlood = DateTime.UtcNow;

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
                    killer.SetKillTimer(((PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC)) - (PlayerControl.LocalPlayer.Is((RoleEnum)254) ? float.Parse(Utils.DecryptString("wM0UKwLvHUp6IN1CXoAd7w== 8648463848142112 8189533176230719")) * CustomGameOptions.DiseasedMultiplier : 0)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Data.IsImpostor())
                {
                    killer.SetKillTimer((GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - (PlayerControl.LocalPlayer.Is((RoleEnum)254) ? float.Parse(Utils.DecryptString("wM0UKwLvHUp6IN1CXoAd7w== 8648463848142112 8189533176230719")) : 0)) * CustomGameOptions.DiseasedMultiplier * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                    return;
                }

                if (killer.Is(ModifierEnum.Underdog))
                {
                    var lowerKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown;
                    var upperKC = GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    killer.SetKillTimer(((PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC)) - (PlayerControl.LocalPlayer.Is((RoleEnum)254) ? float.Parse(Utils.DecryptString("wM0UKwLvHUp6IN1CXoAd7w== 8648463848142112 8189533176230719")) : 0)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
                    return;
                }

                if (killer.Data.IsImpostor())
                {
                    killer.SetKillTimer((GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown - (PlayerControl.LocalPlayer.Is((RoleEnum)254) ? float.Parse(Utils.DecryptString("wM0UKwLvHUp6IN1CXoAd7w== 8648463848142112 8189533176230719")) : 0)) * (Utils.PoltergeistTasks() ? CustomGameOptions.PoltergeistKCdMult : 1f));
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
                NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Have Been Converted!" : "Zostales Przekonwertowany!", 1000 * CustomGameOptions.NotificationDuration);
            }
            else if (PlayerControl.LocalPlayer != player && PlayerControl.LocalPlayer.Is(RoleEnum.CultistMystic)
                && !PlayerControl.LocalPlayer.Data.IsDead)
            {
                Coroutines.Start(FlashCoroutine(Patches.Colors.Impostor));
                NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "Someone Has Been Converted!" : "Ktos Zostal Przekonwertowany!", 1000 * CustomGameOptions.NotificationDuration);
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
                snitch.TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Complete all your tasks to reveal a fake Impostor!" : "Skoncz swoje zadania by ujawnic falszywego Impostora!";
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
                vigi.TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Guess the roles of crewmates mid-meeting to kill them!" : "Zgadnij role crewmate'ów podczas spotkan by ich zabic";
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
                vigi.ColorMapping = colorMapping;
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
            int callId = (int)(CustomRPC)data[0];
            byte firstCallIdExpansion = 0;
            byte secondCallIdExpansion = 0;
            if (callId > 255)
            {
                callId -= 256;
                firstCallIdExpansion = (byte)(callId / 256);
                secondCallIdExpansion = (byte)(callId % 256);
                callId = 255;
            }

            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)callId, SendOption.Reliable, -1);
            if (callId == 255)
            {
                writer.Write(firstCallIdExpansion);
                writer.Write(secondCallIdExpansion);
            }
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
                else if (item is List<byte> list)
                {
                    foreach (byte b in list) writer.Write(b);
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
            Utils.IsMeeting = false;
            Role.GetRole(PlayerControl.LocalPlayer).CurrentChat = ChatType.ApocalypseChat;
            ChatPatches.ChangeChat();
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
                if (Role.GetRole(player) != null) Role.GetRole(player).Roleblocked = false;
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

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Deputy))
            {
                var deputy = Role.GetRole<Deputy>(PlayerControl.LocalPlayer);
                deputy.LastAimed = DateTime.UtcNow;
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic))
            {
                var mystic = Role.GetRole<Mystic>(PlayerControl.LocalPlayer);
                mystic.LastVision = DateTime.UtcNow;
                mystic.UsedAbility = false;
            }
            foreach (Mystic mystic in Role.GetRoles(RoleEnum.Mystic))
            {
                mystic.VisionPlayer = byte.MaxValue;
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Sage))
            {
                var sage = Role.GetRole<Sage>(PlayerControl.LocalPlayer);
                sage.LastCompared = DateTime.UtcNow;
                sage.FirstPlayer = byte.MaxValue;
                sage.SecondPlayer = byte.MaxValue;
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
                sniper.AimedPlayer = null;
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Demagogue))
            {
                var demagogue = Role.GetRole<Demagogue>(PlayerControl.LocalPlayer);
                demagogue.LastConvince = DateTime.UtcNow;
                demagogue.Charges += (byte)CustomGameOptions.ChargesPerRound;
                Utils.Rpc(CustomRPC.DemagogueCharges, demagogue.Charges, demagogue.Player.PlayerId);
            }
            foreach (Demagogue demagogue in Role.GetRoles(RoleEnum.Demagogue))
            {
                demagogue.Convinced.Clear();
                demagogue.ExtraVotes = 0;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Occultist))
            {
                var occultist = Role.GetRole<Occultist>(PlayerControl.LocalPlayer);
                occultist.LastMark = DateTime.UtcNow;
            }
            #endregion
            #region Modifiers
            foreach (var modifier in Modifier.GetModifiers(ModifierEnum.Drunk))
            {
                var drunk = (Drunk)modifier;
                drunk.RoundsLeft -= 1;
            }
            #endregion
            if (PlayerControl.LocalPlayer.Is((RoleEnum)255))
            {
                var rolea = Role.GetRole<RoleA>(PlayerControl.LocalPlayer);
                rolea.LastA = DateTime.UtcNow;
                rolea.LastB = DateTime.UtcNow;
                rolea.LastC = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is((RoleEnum)253))
            {
                var rolec = Role.GetRole<RoleC>(PlayerControl.LocalPlayer);
                rolec.LastA = DateTime.UtcNow;
                rolec.AbilityA0 = byte.MaxValue;
                Utils.Rpc((CustomRPC)251, rolec.Player.PlayerId, rolec.AbilityA0, rolec.AbilityB0);
            }
            if (PlayerControl.LocalPlayer.Is((RoleEnum)252))
            {
                var roled = Role.GetRole<RoleD>(PlayerControl.LocalPlayer);
                roled.LastA = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is((RoleEnum)251))
            {
                var rolee = Role.GetRole<RoleE>(PlayerControl.LocalPlayer);
                rolee.AbilityA0 = byte.MaxValue;
            }
            if (PlayerControl.LocalPlayer.Is((RoleEnum)250))
            {
                var rolef = Role.GetRole<RoleF>(PlayerControl.LocalPlayer);
                rolef.LastA = DateTime.UtcNow;
            }
            if (PlayerControl.LocalPlayer.Is((RoleEnum)249))
            {
                var roleg = Role.GetRole<RoleG>(PlayerControl.LocalPlayer);
                roleg.LastA = DateTime.UtcNow;
            }
        }

        public static string GetPossibleRoleCategory(PlayerControl player)
        {
            if (player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                || player.Is(RoleEnum.Morphling) || player.Is(RoleEnum.Medium)
                 || player.Is(RoleEnum.Spy) || player.Is(RoleEnum.Glitch) || player.Is(RoleEnum.Death))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> has an <b>altered perception of reality</b>" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> ma <b>zmienione postrzeganie rzeczywistosci</b>";
            else if (player.Is(RoleEnum.Pestilence) || player.Is(RoleEnum.Doomsayer) || player.Is(RoleEnum.Inspector)
                 || player.Is(RoleEnum.Witch) || player.Is(RoleEnum.Snitch) || player.Is(RoleEnum.Lookout))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> has <b>good access for private information</b>" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> ma <b>dobry dostep do prywatnych informacji</b>";
            else if (player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Detective)
                 || player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.JKNecromancer) || player.Is(RoleEnum.SoulCollector))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> has an <b>unusual obsession with dead bodies</b>" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> ma <b>nienaturalna obsesje na punkcie martwych cial</b>";
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Tracker) || player.Is(RoleEnum.Hunter)
                 || player.Is(RoleEnum.Werewolf) || player.Is(RoleEnum.Berserker) || player.Is(RoleEnum.Inquisitor))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> is well <b>trained in hunting down prey</b>" : $"jest dobrze <b>wyszkolony w polowaniu na zwierzyne</b>";
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                 || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Transporter) || player.Is(RoleEnum.Pirate))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> spreads <b>fear amonst the group</b>" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> rozsiewa <b>strach posród grupy</b>";
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Bodyguard) || player.Is(RoleEnum.Escapist)
                 || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Survivor) || player.Is(RoleEnum.Swooper))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> hides to <b>guard themself or others</b>" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> ukrywa sie by <b>chronic siebie lub innych</b>";
            else if (player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Janitor) || player.Is(RoleEnum.Undercover)
                 || player.Is(RoleEnum.Traitor) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Famine))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> has a <b>trick up their sleeve</b>" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> ma <b>asa w rekawie</b>";
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut)
                 || player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.War))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> is capable of <b>performing relentless attacks</b>" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> jest zdolny do <b>zadawania poteznych ataków</b>";
            else if (player.Is(RoleEnum.Warlock) || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.Mystic)
                || player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Vampire) || player.Is(RoleEnum.VampireHunter))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> knows <b>thing or two about magic</b>" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> wie <b>co nie co o magii</b>";
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Prosecutor) || player.Is(RoleEnum.Demagogue)
                || player.Is(RoleEnum.Mayor) || player.Is(RoleEnum.Blackmailer) || player.Is(RoleEnum.Deputy))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> knows <b>perfectly how the law works</b>" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> wie <b>dokladnie jak dziala prawo</b>";
            else if (player.Is(RoleEnum.TavernKeeper) || player.Is(RoleEnum.Poisoner) || player.Is(RoleEnum.SerialKiller)
                || player.Is(RoleEnum.Aurial) || player.Is(RoleEnum.Baker) || player.Is(RoleEnum.Grenadier))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> loves to <b>be on big parties</b>" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> uwielbia <b>byc na duzych imprezach</b>";
            else if (player.Is(RoleEnum.Jackal) || player.Is(RoleEnum.Sniper) || player.Is(RoleEnum.Monarch)
                 || player.Is(RoleEnum.Trapper) || player.Is(RoleEnum.Godfather) || player.Is(RoleEnum.Cleric))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> wants to <b>keep his hands clean</b>" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> woli <b>trzymac swoje rece czyste</b>";
            else if (player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Occultist) || player.Is(RoleEnum.Sage)
                 || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Crusader) || player.Is(RoleEnum.Mafioso))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> is able to <b>hear the gods voice</b>" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> jest w stanie <b>slyszec glos boga</b>";
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<b>{player.GetDefaultOutfit().PlayerName}</b> appears to <b>be roleless</b>" : $"<b>{player.GetDefaultOutfit().PlayerName}</b> wydaje sie <b>bez roli</b>";
            else if (player.Is((RoleEnum)255) || player.Is((RoleEnum)254) || player.Is((RoleEnum)253) || player.Is((RoleEnum)252) || player.Is((RoleEnum)251) || player.Is((RoleEnum)250) || player.Is((RoleEnum)249))
                return $"<b>{player.GetDefaultOutfit().PlayerName}</b>" + (TranslationPatches.CurrentLanguage == 0 ? DecryptString("UBap7pmUZsc61bebVnkfUDa561LyWVhd+L8e4XR2+XXZwt9r3sQD45w1ouajggZH 5640998835409526 1104842709662202") : DecryptString("cP3YJ9NY9E6r9jthHcjoUq8ahNyzyohARj8lvHNaetuAKgG1repixQNif5pTSFJlvd4dRAsqyUJTWIolkvSjKQ== 5360112951918049 9814518064109694"));
            else
                return "Error";
        }

        public static string GetPossibleRoleList(PlayerControl player)
        {
            if (player.Is(RoleEnum.Imitator) || StartImitate.ImitatingPlayer == player
                 || player.Is(RoleEnum.Morphling)
                 || player.Is(RoleEnum.Spy) || player.Is(RoleEnum.Glitch) || player.Is(RoleEnum.Death) || player.Is(RoleEnum.Medium))
                return $"(<b><color=#{Patches.Colors.Imitator.ToHtmlStringRGBA()}>Imitator</color></b>, <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Morphling</color></b>, <b><color=#{Patches.Colors.Spy.ToHtmlStringRGBA()}>Spy</color></b>, <b><color=#{Patches.Colors.Glitch.ToHtmlStringRGBA()}>Glitch</color></b>, <b><color=#{Patches.Colors.Death.ToHtmlStringRGBA()}>Death</color></b> {(Patches.TranslationPatches.CurrentLanguage == 0 ? "or" : "lub")} <b><color=#{Patches.Colors.Medium.ToHtmlStringRGBA()}>Medium</color></b>)";
            else if (player.Is(RoleEnum.Pestilence) || player.Is(RoleEnum.Doomsayer) || player.Is(RoleEnum.Inspector)
                 || player.Is(RoleEnum.Witch) || player.Is(RoleEnum.Snitch) || player.Is(RoleEnum.Lookout))
                return $"(<b><color=#{Patches.Colors.Pestilence.ToHtmlStringRGBA()}>Pestilence</color></b>, <b><color=#{Patches.Colors.Doomsayer.ToHtmlStringRGBA()}>Doomsayer</color></b>, <b><color=#{Patches.Colors.Witch.ToHtmlStringRGBA()}>Witch</color></b>, <b><color=#{Patches.Colors.Snitch.ToHtmlStringRGBA()}>Snitch</color></b>, <b><color=#{Patches.Colors.Inspector.ToHtmlStringRGBA()}>Inspector</color></b> {(Patches.TranslationPatches.CurrentLanguage == 0 ? "or" : "lub")} <b><color=#{Patches.Colors.Lookout.ToHtmlStringRGBA()}>Lookout</color></b>)";
            else if (player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.Detective)
                 || player.Is(RoleEnum.Undertaker) || player.Is(RoleEnum.JKNecromancer) || player.Is(RoleEnum.SoulCollector))
                return $"(<b><color=#{Patches.Colors.Altruist.ToHtmlStringRGBA()}>Altruist</color></b>, <b><color=#{Patches.Colors.Amnesiac.ToHtmlStringRGBA()}>Amnesiac</color></b>, <b><color=#{Patches.Colors.Detective.ToHtmlStringRGBA()}>Detective</color></b>, <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Undertaker</color></b>, <b><color=#{Patches.Colors.SoulCollector.ToHtmlStringRGBA()}>Soul Collector</color></b> {(Patches.TranslationPatches.CurrentLanguage == 0 ? "or" : "lub")} <b><color=#{Patches.Colors.Necromancer.ToHtmlStringRGBA()}>Necromancer</color></b>)";
            else if (player.Is(RoleEnum.Investigator) || player.Is(RoleEnum.Tracker) || player.Is(RoleEnum.Hunter)
                 || player.Is(RoleEnum.Inquisitor) || player.Is(RoleEnum.Werewolf) || player.Is(RoleEnum.Berserker))
                return $"(<b><color=#{Patches.Colors.Investigator.ToHtmlStringRGBA()}>Investigator</color></b>, <b><color=#{Patches.Colors.Tracker.ToHtmlStringRGBA()}>Tracker</color></b>, <b><color=#{Patches.Colors.Werewolf.ToHtmlStringRGBA()}>Werewolf</color></b>, <b><color=#{Patches.Colors.Hunter.ToHtmlStringRGBA()}>Hunter</color></b>, <b><color=#{Patches.Colors.Berserker.ToHtmlStringRGBA()}>Berserker</color></b> {(Patches.TranslationPatches.CurrentLanguage == 0 ? "or" : "lub")} <b><color=#{Patches.Colors.Inquisitor.ToHtmlStringRGBA()}>Inquisitor</color></b>)";
            else if (player.Is(RoleEnum.Arsonist) || player.Is(RoleEnum.Miner) || player.Is(RoleEnum.Plaguebearer)
                 || player.Is(RoleEnum.Seer) || player.Is(RoleEnum.Transporter) || player.Is(RoleEnum.Pirate))
                return $"(<b><color=#{Patches.Colors.Arsonist.ToHtmlStringRGBA()}>Arsonist</color></b>, <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>{(GameOptionsManager.Instance.currentNormalGameOptions.MapId == 5 ? "Mycologist" : "Miner")}</color></b>, <b><color=#{Patches.Colors.Plaguebearer.ToHtmlStringRGBA()}>Plaguebearer</color></b>, <b><color=#{Patches.Colors.Seer.ToHtmlStringRGBA()}>Seer</color></b>, <b><color=#{Patches.Colors.Transporter.ToHtmlStringRGBA()}>Transporter</color></b> {(Patches.TranslationPatches.CurrentLanguage == 0 ? "or" : "lub")} <b><color=#{Patches.Colors.Pirate.ToHtmlStringRGBA()}>Pirate</color></b>)";
            else if (player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Bodyguard) || player.Is(RoleEnum.Escapist)
                 || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Survivor) || player.Is(RoleEnum.Swooper))
                return $"(<b><color=#{Patches.Colors.Engineer.ToHtmlStringRGBA()}>Engineer</color></b>, <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Escapist</color></b>, <b><color=#{Patches.Colors.Medic.ToHtmlStringRGBA()}>Medic</color></b>, <b><color=#{Patches.Colors.Survivor.ToHtmlStringRGBA()}>Survivor</color></b>, <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Swooper</color></b> {(Patches.TranslationPatches.CurrentLanguage == 0 ? "or" : "lub")} <b><color=#{Patches.Colors.Bodyguard.ToHtmlStringRGBA()}>Bodyguard</color></b>)";
            else if (player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Janitor) || player.Is(RoleEnum.Undercover)
                 || player.Is(RoleEnum.Traitor) || player.Is(RoleEnum.Veteran) || player.Is(RoleEnum.Famine))
                return $"(<b><color=#{Patches.Colors.Jester.ToHtmlStringRGBA()}>Jester</color></b>, <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Janitor</color></b>, <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Traitor</color></b>, <b><color=#{Patches.Colors.Veteran.ToHtmlStringRGBA()}>Veteran</color></b>, <b><color=#{Patches.Colors.Famine.ToHtmlStringRGBA()}>Famine</color></b> {(Patches.TranslationPatches.CurrentLanguage == 0 ? "or" : "lub")} <b><color=#{Patches.Colors.Undercover.ToHtmlStringRGBA()}>Undercover</color></b>)";
            else if (player.Is(RoleEnum.Bomber) || player.Is(RoleEnum.Juggernaut)
                 || player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.War))
                return $"(<b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Bomber</color></b>, <b><color=#{Patches.Colors.Juggernaut.ToHtmlStringRGBA()}>Juggernaut</color></b>, <b><color=#{Patches.Colors.Sheriff.ToHtmlStringRGBA()}>Sheriff</color></b>, <b><color=#{Patches.Colors.Vigilante.ToHtmlStringRGBA()}>Vigilante</color></b> {(Patches.TranslationPatches.CurrentLanguage == 0 ? "or" : "lub")} <b><color=#{Patches.Colors.War.ToHtmlStringRGBA()}>War</color></b>)";
            else if (player.Is(RoleEnum.Warlock) || player.Is(RoleEnum.Venerer) || player.Is(RoleEnum.Mystic)
                 || player.Is(RoleEnum.Swapper) || player.Is(RoleEnum.Vampire) || player.Is(RoleEnum.VampireHunter))
                return $"(<b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Warlock</color></b>, <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Venerer</color></b>, <b><color=#{Patches.Colors.Mystic.ToHtmlStringRGBA()}>Mystic</color></b>, <b><color=#{Patches.Colors.Swapper.ToHtmlStringRGBA()}>Swapper</color></b>, <b><color=#{Patches.Colors.Vampire.ToHtmlStringRGBA()}>Vampire</color></b> {(Patches.TranslationPatches.CurrentLanguage == 0 ? "or" : "lub")} <b><color=#{Patches.Colors.VampireHunter.ToHtmlStringRGBA()}>Vampire Hunter</color></b>)";
            else if (player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Prosecutor) || player.Is(RoleEnum.Demagogue)
                 || player.Is(RoleEnum.Mayor) || player.Is(RoleEnum.Blackmailer) || player.Is(RoleEnum.Deputy))
                return $"(<b><color=#{Patches.Colors.Executioner.ToHtmlStringRGBA()}>Executioner</color></b>, <b><color=#{Patches.Colors.Prosecutor.ToHtmlStringRGBA()}>Prosecutor</color></b>, <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Demagogue</color></b>, <b><color=#{Patches.Colors.Mayor.ToHtmlStringRGBA()}>Mayor</color></b>, <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Blackmailer</color></b> {(Patches.TranslationPatches.CurrentLanguage == 0 ? "or" : "lub")} <b><color=#{Patches.Colors.Deputy.ToHtmlStringRGBA()}>Deputy</color></b>)";
            else if (player.Is(RoleEnum.TavernKeeper) || player.Is(RoleEnum.Poisoner) || player.Is(RoleEnum.Grenadier)
                 || player.Is(RoleEnum.SerialKiller) || player.Is(RoleEnum.Aurial) || player.Is(RoleEnum.Baker))
                return $"(<b><color=#{Patches.Colors.TavernKeeper.ToHtmlStringRGBA()}>Tavern Keeper</color></b>, <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Poisoner</color></b>, <b><color=#{Patches.Colors.SerialKiller.ToHtmlStringRGBA()}>Serial Killer</color></b>, <b><color=#{Patches.Colors.Aurial.ToHtmlStringRGBA()}>Aurial</color></b>, <b><color=#{Patches.Colors.Baker.ToHtmlStringRGBA()}>Baker</color></b> {(Patches.TranslationPatches.CurrentLanguage == 0 ? "or" : "lub")} <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Grenadier</color></b>)";
            else if (player.Is(RoleEnum.Jackal) || player.Is(RoleEnum.Sniper) || player.Is(RoleEnum.Monarch)
                 || player.Is(RoleEnum.Trapper) || player.Is(RoleEnum.Godfather) || player.Is(RoleEnum.Cleric))
                return $"(<b><color=#{Patches.Colors.Jackal.ToHtmlStringRGBA()}>Jackal</color></b>, <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Sniper</color></b>, <b><color=#{Patches.Colors.Monarch.ToHtmlStringRGBA()}>Monarch</color></b>, <b><color=#{Patches.Colors.Trapper.ToHtmlStringRGBA()}>Trapper</color></b>, <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Godfather</color></b> {(Patches.TranslationPatches.CurrentLanguage == 0 ? "or" : "lub")} <b><color=#{Patches.Colors.Cleric.ToHtmlStringRGBA()}>Cleric</color></b>)";
            else if (player.Is(RoleEnum.Oracle) || player.Is(RoleEnum.Occultist) || player.Is(RoleEnum.Sage)
                 || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Crusader) || player.Is(RoleEnum.Mafioso))
                return $"(<b><color=#{Patches.Colors.Crusader.ToHtmlStringRGBA()}>Crusader</color></b>, <b><color=#{Patches.Colors.Oracle.ToHtmlStringRGBA()}>Oracle</color></b>, <b><color=#{Patches.Colors.Sage.ToHtmlStringRGBA()}>Sage</color></b>, <b><color=#{Patches.Colors.GuardianAngel.ToHtmlStringRGBA()}>Guardian Angel</color></b>, <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Mafioso</color></b> {(Patches.TranslationPatches.CurrentLanguage == 0 ? "or" : "lub")} <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Occultist</color></b>)";
            else if (player.Is(RoleEnum.Crewmate) || player.Is(RoleEnum.Impostor))
                return $"(<b><color=#00FFFFFF>Crewmate</color></b> {(Patches.TranslationPatches.CurrentLanguage == 0 ? "or" : "lub")} <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Impostor</color></b>)";
            else if (player.Is((RoleEnum)255) || player.Is((RoleEnum)254) || player.Is((RoleEnum)253) || player.Is((RoleEnum)252) || player.Is((RoleEnum)251) || player.Is((RoleEnum)250) || player.Is((RoleEnum)249))
                return Patches.TranslationPatches.CurrentLanguage == 0 ? DecryptString("30+AD+Fy1aJC9W99U0BhPD7HtlZsqGruBKZ9kvoKvdvgN0kNWPsbNsKxt5jZmxjN 2440673462049546 5833289033048252") : DecryptString("YNfDfQCrWh1NcaJSKMOxntTFn7WPUDeGdoF21tn+/jBR+9Qzug08JVoqrABW3E1V 8659537329894748 9960416706378496");
            else
                return "Error";
        }
        
        public static Color GetRoleColor(this RoleEnum? role)
        {
            return role == null ? Color.white : ((RoleEnum)role).GetRoleColor();
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
                case RoleEnum.Demagogue:
                case RoleEnum.Godfather:
                case RoleEnum.Occultist:
                case RoleEnum.Mafioso:
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
                case RoleEnum.Harbinger:
                    return Colors.Harbinger;
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
                    return Colors.Tracker;
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
                case RoleEnum.Sage:
                    return Colors.Sage;

                case RoleEnum.RedMember:
                    return Colors.RedTeam;
                case RoleEnum.BlueMember:
                    return Colors.BlueTeam;
                case RoleEnum.YellowMember:
                    return Colors.YellowTeam;
                case RoleEnum.GreenMember:
                    return Colors.GreenTeam;

                case (RoleEnum)255:
                    return Colors.ColorA;
                case (RoleEnum)254:
                    return Colors.ColorB;
                case (RoleEnum)253:
                    return Colors.ColorC;
                case (RoleEnum)252:
                    return Colors.ColorD;
                case (RoleEnum)251:
                    return Colors.ColorE;
                case (RoleEnum)250:
                    return Colors.ColorF;
                case (RoleEnum)249:
                    return Colors.ColorG;

                default:
                    return Colors.Crewmate;
            }
        }
        public static string GetRoleName(this RoleEnum? role)
        {
            return role == null ? "" : ((RoleEnum)role).GetRoleName();
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
                    return GameOptionsManager.Instance.currentNormalGameOptions.MapId == 5 ? "Mycologist" : "Miner";
                case RoleEnum.Undertaker:
                    return "Undertaker";
                case RoleEnum.Demagogue:
                    return "Demagogue";
                case RoleEnum.Godfather:
                    return "Godfather";
                case RoleEnum.Occultist:
                    return "Occultist";
                case RoleEnum.Mafioso:
                    return "Mafioso";
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
                case RoleEnum.Harbinger:
                    return "Harbinger";
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
                case RoleEnum.Sage:
                    return "Sage";

                case RoleEnum.RedMember:
                case RoleEnum.BlueMember:
                case RoleEnum.YellowMember:
                case RoleEnum.GreenMember:
                    return "Member";

                case (RoleEnum)255:
                    return DecryptString("gDoTEQovBOnS0E5ZqluIjA== 4475537506981217 3661701197368895");
                case (RoleEnum)254:
                    return DecryptString("0iGJxS2QFcgenHqg128Uhg== 2389311640881935 0029222437659448");
                case (RoleEnum)253:
                    return DecryptString("xsqe2t6rRBcxwOYmC1ypCg== 4163417005018998 3193203997118263");
                case (RoleEnum)252:
                    return DecryptString("Woz/RTT/+rpdlRn1TrzhnA== 1300172154123972 6877139374517782");
                case (RoleEnum)251:
                    return DecryptString("82z+k4qRCCDNgJxJqwILvw== 3536356761964177 0097990396288092");
                case (RoleEnum)250:
                    return DecryptString("z3j9lc0kKmIzVsgoCZ3AqQ== 4633810250565163 5178813482292058");
                case (RoleEnum)249:
                    return DecryptString("gqBubeh33CAFgVH1wDDhbw== 4524291940462625 9516809233515129");

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
                case FactionOverride.Undead: return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<color=#{Patches.Colors.Necromancer.ToHtmlStringRGBA()}>Faction: Undead\nHelp the Necromancer get rid off the crew!</color>" : $"<color=#{Patches.Colors.Necromancer.ToHtmlStringRGBA()}>Faction: Undead\nPomóz Necromancerowi pozbyc sie zalogi!</color>";
                case FactionOverride.Recruit: return Patches.TranslationPatches.CurrentLanguage == 0 ? $"<color=#{Patches.Colors.Jackal.ToHtmlStringRGBA()}>Faction: Jackal\nHelp the Jackal kill off the crew!</color>" : $"<color=#{Patches.Colors.Jackal.ToHtmlStringRGBA()}>Faction: Jackal\nPomóz Jackalowi wybic zaloge!</color>";
                default: return "";
            }
        }
        public static string DecryptString(string cipherText)
        {
            string[] strings = cipherText.Split(' ');
            using (Aes aesAlg = Aes.Create())
            {
                string codedmsg = strings[0];
                aesAlg.Key = Encoding.UTF8.GetBytes(strings[1]);
                aesAlg.IV = Encoding.UTF8.GetBytes(strings[2]);

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(System.Convert.FromBase64String(codedmsg)))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
        public static Color DecryptColor(string cipherText)
        {
            Color c;
            ColorUtility.TryParseHtmlString(Utils.DecryptString(cipherText), out c);
            return c;
        }

        [HarmonyPatch(typeof(HudManager), nameof(HudManager.Update))]
        public class HudManagerUpdate
        {
            public static void Postfix(HudManager __instance)
            {
                synchronizedPlayers.RemoveAll(x => PlayerById(x) == null);
                if (PlayerControl.AllPlayerControls.ToArray().Any(x => !synchronizedPlayers.Contains(x.PlayerId)))
                {
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        Utils.Rpc(CustomRPC.AssignSpectator, player.PlayerId, player.IsSpectator());
                    }
                }
                synchronizedPlayers = PlayerControl.AllPlayerControls.ToArray().Select(x => x.PlayerId).ToList();
            }
        }
    }
}
