using HarmonyLib;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.SnitchMod;
using TownOfUs.CrewmateRoles.TrapperMod;
using TownOfUs.Roles;
using UnityEngine;
using System;
using TownOfUs.Extensions;
using TownOfUs.CrewmateRoles.ImitatorMod;
using AmongUs.GameOptions;
using TownOfUs.Roles.Modifiers;
using TownOfUs.ImpostorRoles.BomberMod;
using TownOfUs.CrewmateRoles.AurialMod;
using TownOfUs.Patches.ScreenEffects;
using TownOfUs.Roles.Horseman;
using Reactor.Utilities;
using System.Collections.Generic;
using Reactor.Utilities.Extensions;
using System.Linq;

namespace TownOfUs.NeutralRoles.AmnesiacMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKillButton
    {
        public static Sprite Sprite => TownOfUs.Arrow;
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton) return true;
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Amnesiac);
            if (!flag) return true;
            if (!PlayerControl.LocalPlayer.CanMove) return false;
            if (PlayerControl.LocalPlayer.Data.IsDead) return false;
            var role = Role.GetRole<Amnesiac>(PlayerControl.LocalPlayer);

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
            if (PlayerControl.LocalPlayer.IsRoleblocked())
            {
                Coroutines.Start(Utils.FlashCoroutine(Color.white));
                NotificationPatch.Notification(Patches.TranslationPatches.CurrentLanguage == 0 ? "You Are Roleblocked!" : "Twoja Rola Zostala Zablokowana!", 1000 * CustomGameOptions.NotificationDuration);
                return false;
            }
            var playerId = role.CurrentTarget.ParentId;
            var player = Utils.PlayerById(playerId);
            if (PlayerControl.LocalPlayer.IsInVision() || player.IsInVision())
            {
                Utils.Rpc(CustomRPC.VisionInteract, PlayerControl.LocalPlayer.PlayerId, player.PlayerId);
            }
            if (player.IsInfected() || role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer)) ((Plaguebearer)pb).RpcSpreadInfection(player, role.Player);
            }
            if (player.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, playerId, (byte)role.RoleType, (byte)0);

            Utils.Rpc(CustomRPC.Remember, PlayerControl.LocalPlayer.PlayerId, playerId);

            Remember(role, player);
            return false;
        }

        public static void Remember(Amnesiac amneRole, PlayerControl other)
        {
            var role = Utils.GetRole(other);
            var amnesiac = amneRole.Player;

            var rememberImp = true;
            var rememberNeut = true;
            var oldBread = amneRole.BreadLeft;
            var amneFactionOverride = amneRole.FactionOverride;
            var targetFactionOverride = Role.GetRole(other).FactionOverride;

            Role newRole;

            if (PlayerControl.LocalPlayer == amnesiac)
            {
                var amnesiacRole = Role.GetRole<Amnesiac>(amnesiac);
                amnesiacRole.BodyArrows.Values.DestroyAll();
                amnesiacRole.BodyArrows.Clear();
                foreach (var body in amnesiacRole.CurrentTarget.bodyRenderers) body.material.SetFloat("_Outline", 0f);
            }

            switch (role)
            {
                case RoleEnum.Sheriff:
                case RoleEnum.Engineer:
                case RoleEnum.Mayor:
                case RoleEnum.Swapper:
                case RoleEnum.Investigator:
                case RoleEnum.Medic:
                case RoleEnum.Seer:
                case RoleEnum.Spy:
                case RoleEnum.Snitch:
                case RoleEnum.Altruist:
                case RoleEnum.Vigilante:
                case RoleEnum.Veteran:
                case RoleEnum.Crewmate:
                case RoleEnum.Tracker:
                case RoleEnum.Hunter:
                case RoleEnum.Transporter:
                case RoleEnum.Medium:
                case RoleEnum.Mystic:
                case RoleEnum.Trapper:
                case RoleEnum.Detective:
                case RoleEnum.Imitator:
                case RoleEnum.VampireHunter:
                case RoleEnum.Prosecutor:
                case RoleEnum.Oracle:
                case RoleEnum.Aurial:
                case RoleEnum.Inspector:
                case RoleEnum.Monarch:
                case RoleEnum.TavernKeeper:
                case RoleEnum.Undercover:
                case RoleEnum.Lookout:
                case RoleEnum.Deputy:
                case RoleEnum.Bodyguard:
                case RoleEnum.Crusader:
                case RoleEnum.Cleric:
                case RoleEnum.Sage:

                    rememberImp = false;
                    rememberNeut = false;

                    break;

                case RoleEnum.Jester:
                case RoleEnum.Executioner:
                case RoleEnum.Arsonist:
                case RoleEnum.Amnesiac:
                case RoleEnum.Glitch:
                case RoleEnum.Juggernaut:
                case RoleEnum.Survivor:
                case RoleEnum.GuardianAngel:
                case RoleEnum.Plaguebearer:
                case RoleEnum.Pestilence:
                case RoleEnum.Baker:
                case RoleEnum.Famine:
                case RoleEnum.Berserker:
                case RoleEnum.War:
                case RoleEnum.SoulCollector:
                case RoleEnum.Death:
                case RoleEnum.Werewolf:
                case RoleEnum.Doomsayer:
                case RoleEnum.Vampire:
                case RoleEnum.Pirate:
                case RoleEnum.Inquisitor:
                case RoleEnum.SerialKiller:
                case RoleEnum.Witch:
                case RoleEnum.JKNecromancer:
                case RoleEnum.Jackal:

                    rememberImp = false;

                    break;
                default:
                    if (other.Is(Faction.Crewmates))
                    {
                        rememberImp = false;
                        rememberNeut = false;
                    }
                    else if (!other.Is(Faction.Impostors))
                    {
                        rememberImp = false;
                    }
                    break;
            }

            newRole = Role.GetRole(other);
            newRole.Player = amnesiac;

            if (role == RoleEnum.Aurial && PlayerControl.LocalPlayer == other)
            {
                var aurial = Role.GetRole<Aurial>(other);
                aurial.NormalVision = true;
                SeeAll.AllToNormal();
                CameraEffect.singleton.materials.Clear();
            }

            if ((role == RoleEnum.Glitch || role == RoleEnum.Juggernaut || role == RoleEnum.Pestilence ||
                role == RoleEnum.Werewolf || role == RoleEnum.Berserker || role == RoleEnum.War || role == RoleEnum.SerialKiller) &&
                PlayerControl.LocalPlayer == other)
            {
                HudManager.Instance.KillButton.buttonLabelText.gameObject.SetActive(false);
            }

            if (role == RoleEnum.Investigator) Footprint.DestroyAll(Role.GetRole<Investigator>(other));

            if (role == RoleEnum.Snitch) CompleteTask.Postfix(amnesiac);

            Role.RoleDictionary.Remove(amnesiac.PlayerId);
            Role.RoleDictionary.Remove(other.PlayerId);
            Role.RoleDictionary.Add(amnesiac.PlayerId, newRole);

            if (!(amnesiac.Is(RoleEnum.Crewmate) || amnesiac.Is(RoleEnum.Impostor))) newRole.RegenTask();

            if (other == StartImitate.ImitatingPlayer)
            {
                StartImitate.ImitatingPlayer = amneRole.Player;
                newRole.AddToRoleHistory(RoleEnum.Imitator);
            }
            else newRole.AddToRoleHistory(newRole.RoleType);

            if (rememberImp == false)
            {
                if (rememberNeut == false)
                {
                    new Crewmate(other);
                }
                else
                {
                    // If role is not Vampire, turn dead player into Survivor
                    if (role != RoleEnum.Vampire)
                    {
                        var survivor = new Survivor(other);
                        survivor.RegenTask();
                    }
                    // If role is Vampire, keep dead player as Vampire
                    if (role == RoleEnum.Vampire)
                    {
                        var vampire = new Vampire(other);
                        vampire.RegenTask();
                    }
                    if (role == RoleEnum.Arsonist || role == RoleEnum.Glitch || role == RoleEnum.Plaguebearer ||
                            role == RoleEnum.Pestilence || role == RoleEnum.Werewolf || role == RoleEnum.Juggernaut
                             || role == RoleEnum.Vampire || role == RoleEnum.Famine || role == RoleEnum.Baker
                             || role == RoleEnum.Berserker || role == RoleEnum.War || role == RoleEnum.SoulCollector
                             || role == RoleEnum.Death || role == RoleEnum.SerialKiller)
                    {
                        if (CustomGameOptions.AmneTurnNeutAssassin) new Assassin(amnesiac);
                        if (other.Is(AbilityEnum.Assassin)) Ability.AbilityDictionary.Remove(other.PlayerId);
                    }
                }
            }
            else if (rememberImp == true)
            {
                new Impostor(other);
                amnesiac.Data.Role.TeamType = RoleTeamTypes.Impostor;
                RoleManager.Instance.SetRole(amnesiac, RoleTypes.Impostor);
                amnesiac.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                foreach (var player in PlayerControl.AllPlayerControls)
                {
                    if (player.Data.IsImpostor() && PlayerControl.LocalPlayer.Data.IsImpostor())
                    {
                        player.nameText().color = Patches.Colors.Impostor;
                    }
                }
                if (CustomGameOptions.AmneTurnImpAssassin) new Assassin(amnesiac);
            }

            if (role == RoleEnum.Snitch)
            {
                var snitchRole = Role.GetRole<Snitch>(amnesiac);
                snitchRole.ImpArrows.DestroyAll();
                snitchRole.SnitchArrows.Values.DestroyAll();
                snitchRole.SnitchArrows.Clear();
                CompleteTask.Postfix(amnesiac);
                if (other.AmOwner)
                    foreach (var player in PlayerControl.AllPlayerControls)
                        player.nameText().color = Color.white;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Sheriff)
            {
                var sheriffRole = Role.GetRole<Sheriff>(amnesiac);
                sheriffRole.LastKilled = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Engineer)
            {
                var engiRole = Role.GetRole<Engineer>(amnesiac);
                engiRole.UsesLeft = CustomGameOptions.MaxFixes;
            }

            else if (role == RoleEnum.Medic)
            {
                var medicRole = Role.GetRole<Medic>(amnesiac);
                if (amnesiac != StartImitate.ImitatingPlayer) medicRole.UsedAbility = false;
                else medicRole.UsedAbility = true;
            }

            else if (role == RoleEnum.Mayor)
            {
                var mayorRole = Role.GetRole<Mayor>(amnesiac);
                mayorRole.Revealed = false;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Prosecutor)
            {
                var prosRole = Role.GetRole<Prosecutor>(amnesiac);
                prosRole.Revealed = false;
                prosRole.ProsecutionsLeft = CustomGameOptions.MaxProsecutions;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Vigilante)
            {
                var vigiRole = Role.GetRole<Vigilante>(amnesiac);
                vigiRole.RemainingKills = CustomGameOptions.VigilanteKills;
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }

            else if (role == RoleEnum.Veteran)
            {
                var vetRole = Role.GetRole<Veteran>(amnesiac);
                vetRole.UsesLeft = CustomGameOptions.MaxAlerts;
                vetRole.LastAlerted = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Tracker)
            {
                var trackerRole = Role.GetRole<Tracker>(amnesiac);
                trackerRole.TrackerArrows.Values.DestroyAll();
                trackerRole.TrackerArrows.Clear();
                trackerRole.UsesLeft = CustomGameOptions.MaxTracks;
                trackerRole.LastTracked = DateTime.UtcNow;
            }

            else if (role == RoleEnum.VampireHunter)
            {
                var vhRole = Role.GetRole<VampireHunter>(amnesiac);
                if (vhRole.AddedStakes) vhRole.UsesLeft = CustomGameOptions.MaxFailedStakesPerGame;
                else vhRole.UsesLeft = 0;
                vhRole.LastStaked = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Detective)
            {
                var detectiveRole = Role.GetRole<Detective>(amnesiac);
                detectiveRole.LastExamined = DateTime.UtcNow;
                detectiveRole.CurrentTarget = null;
                detectiveRole.LastKiller = null;
            }

            else if (role == RoleEnum.Mystic)
            {
                var mysticRole = Role.GetRole<Mystic>(amnesiac);
                mysticRole.BodyArrows.Values.DestroyAll();
                mysticRole.BodyArrows.Clear();
                mysticRole.InteractingPlayers.Clear();
                mysticRole.PlayersInteracted.Clear();
                mysticRole.VisionPlayer = byte.MaxValue;
                mysticRole.UsedAbility = false;
            }

            else if (role == RoleEnum.Transporter)
            {
                var tpRole = Role.GetRole<Transporter>(amnesiac);
                tpRole.PressedButton = false;
                tpRole.MenuClick = false;
                tpRole.LastMouse = false;
                tpRole.TransportList = null;
                tpRole.TransportPlayer1 = null;
                tpRole.TransportPlayer2 = null;
                tpRole.LastTransported = DateTime.UtcNow;
                tpRole.UsesLeft = CustomGameOptions.TransportMaxUses;
            }

            else if (role == RoleEnum.Medium)
            {
                var medRole = Role.GetRole<Medium>(amnesiac);
                medRole.MediatedPlayers.Values.DestroyAll();
                medRole.MediatedPlayers.Clear();
                medRole.LastMediated = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Seer)
            {
                var seerRole = Role.GetRole<Seer>(amnesiac);
                seerRole.Investigated.RemoveRange(0, seerRole.Investigated.Count);
                seerRole.LastInvestigated = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Oracle)
            {
                var oracleRole = Role.GetRole<Oracle>(amnesiac);
                oracleRole.Confessor = null;
                oracleRole.LastConfessed = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Aurial)
            {
                var aurialRole = Role.GetRole<Aurial>(amnesiac);
                aurialRole.LastRadiated = DateTime.UtcNow;
                aurialRole.NormalVision = false;
                aurialRole.knownPlayerRoles.Clear();
                if (amnesiac.AmOwner) aurialRole.ApplyEffect();
                aurialRole.Loaded = true;
            }

            else if (role == RoleEnum.Deputy)
            {
                var deputyRole = Role.GetRole<Deputy>(amnesiac);
                deputyRole.Revealed = false;
            }

            else if (role == RoleEnum.Cleric)
            {
                var clericRole = Role.GetRole<Cleric>(amnesiac);
                clericRole.BarrieredPlayer = null;
                clericRole.LastBarrier = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Crusader)
            {
                var crusaderRole = Role.GetRole<Crusader>(amnesiac);
                crusaderRole.FortifiedPlayers.Clear();
                crusaderRole.UsesLeft = CustomGameOptions.MaxFortify;
                crusaderRole.LastFortified = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Bodyguard)
            {
                var bodyguardRole = Role.GetRole<Bodyguard>(amnesiac);
                bodyguardRole.GuardedPlayer = null;
                bodyguardRole.LastGuard = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Arsonist)
            {
                var arsoRole = Role.GetRole<Arsonist>(amnesiac);
                arsoRole.DousedPlayers.RemoveRange(0, arsoRole.DousedPlayers.Count);
                arsoRole.LastDoused = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Survivor)
            {
                var survRole = Role.GetRole<Survivor>(amnesiac);
                survRole.LastVested = DateTime.UtcNow;
                survRole.UsesLeft = CustomGameOptions.MaxVests;
            }

            else if (role == RoleEnum.GuardianAngel)
            {
                var gaRole = Role.GetRole<GuardianAngel>(amnesiac);
                gaRole.LastProtected = DateTime.UtcNow;
                gaRole.UsesLeft = CustomGameOptions.MaxProtects;
            }

            else if (role == RoleEnum.Glitch)
            {
                var glitchRole = Role.GetRole<Glitch>(amnesiac);
                glitchRole.LastKill = DateTime.UtcNow;
                glitchRole.LastHack = DateTime.UtcNow;
                glitchRole.LastMimic = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Juggernaut)
            {
                var juggRole = Role.GetRole<Juggernaut>(amnesiac);
                juggRole.JuggKills = 0;
                juggRole.LastKill = DateTime.UtcNow;
            }

            else if (role == RoleEnum.SerialKiller)
            {
                var skRole = Role.GetRole<SerialKiller>(amnesiac);
                skRole.SKKills = 0;
                skRole.LastKill = DateTime.UtcNow;
                skRole.InBloodlust = false;
            }

            else if (role == RoleEnum.Grenadier)
            {
                var grenadeRole = Role.GetRole<Grenadier>(amnesiac);
                grenadeRole.LastFlashed = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Morphling)
            {
                var morphlingRole = Role.GetRole<Morphling>(amnesiac);
                morphlingRole.LastMorphed = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Escapist)
            {
                var escapistRole = Role.GetRole<Escapist>(amnesiac);
                escapistRole.LastEscape = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Swooper)
            {
                var swooperRole = Role.GetRole<Swooper>(amnesiac);
                swooperRole.LastSwooped = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Venerer)
            {
                var venererRole = Role.GetRole<Venerer>(amnesiac);
                venererRole.LastCamouflaged = DateTime.UtcNow;
                venererRole.KillsAtStartAbility = 0;
            }

            else if (role == RoleEnum.Blackmailer)
            {
                var blackmailerRole = Role.GetRole<Blackmailer>(amnesiac);
                blackmailerRole.LastBlackmailed = DateTime.UtcNow;
                blackmailerRole.Blackmailed = null;
            }

            else if (role == RoleEnum.Miner)
            {
                var minerRole = Role.GetRole<Miner>(amnesiac);
                minerRole.LastMined = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Undertaker)
            {
                var dienerRole = Role.GetRole<Undertaker>(amnesiac);
                dienerRole.LastDragged = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Sniper)
            {
                var sniperRole = Role.GetRole<Sniper>(amnesiac);
                sniperRole.LastAim = DateTime.UtcNow;
                sniperRole.AimedPlayer = null;
            }

            else if (role == RoleEnum.Werewolf)
            {
                var wwRole = Role.GetRole<Werewolf>(amnesiac);
                wwRole.LastRampaged = DateTime.UtcNow;
                wwRole.LastKilled = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Doomsayer)
            {
                var doomRole = Role.GetRole<Doomsayer>(amnesiac);
                doomRole.GuessedCorrectly = 0;
                doomRole.LastObserved = DateTime.UtcNow;
                doomRole.LastObservedPlayer = null;
            }

            else if (role == RoleEnum.Plaguebearer)
            {
                var plagueRole = Role.GetRole<Plaguebearer>(amnesiac);
                plagueRole.InfectedPlayers.RemoveRange(0, plagueRole.InfectedPlayers.Count);
                plagueRole.InfectedPlayers.Add(amnesiac.PlayerId);
                plagueRole.LastInfected = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Baker)
            {
                var bakerRole = Role.GetRole<Baker>(amnesiac);
                bakerRole.BreadPlayers.RemoveRange(0, bakerRole.BreadPlayers.Count);
                bakerRole.LastBreaded = DateTime.UtcNow;
            }

            else if (role == RoleEnum.SoulCollector)
            {
                var collectorRole = Role.GetRole<SoulCollector>(amnesiac);
                collectorRole.ReapedSouls = 0;
                collectorRole.LastReaped = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Berserker)
            {
                var berserkerRole = Role.GetRole<Berserker>(amnesiac);
                berserkerRole.KilledPlayers = 0;
                berserkerRole.LastKill = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Pestilence)
            {
                var pestRole = Role.GetRole<Pestilence>(amnesiac);
                pestRole.LastKill = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Famine)
            {
                var famRole = Role.GetRole<Famine>(amnesiac);
                famRole.LastStarved = DateTime.UtcNow;
            }

            else if (role == RoleEnum.War)
            {
                var warRole = Role.GetRole<War>(amnesiac);
                warRole.LastKill = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Death)
            {
                var deathRole = Role.GetRole<Death>(amnesiac);
                deathRole.LastApocalypse = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Vampire)
            {
                var vampRole = Role.GetRole<Vampire>(amnesiac);
                vampRole.LastBit = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Trapper)
            {
                var trapperRole = Role.GetRole<Trapper>(amnesiac);
                trapperRole.LastTrapped = DateTime.UtcNow;
                trapperRole.UsesLeft = CustomGameOptions.MaxTraps;
                trapperRole.trappedPlayers.Clear();
                trapperRole.traps.ClearTraps();
            }

            else if (role == RoleEnum.Bomber)
            {
                var bomberRole = Role.GetRole<Bomber>(amnesiac);
                bomberRole.Bomb.ClearBomb();
            }

            else if (role == RoleEnum.Monarch)
            {
                var monarchRole = Role.GetRole<Monarch>(amnesiac);
                monarchRole.LastKnighted = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Inspector)
            {
                var inspectorRole = Role.GetRole<Inspector>(amnesiac);
                inspectorRole.LastInspectedPlayer = null;
                inspectorRole.LastInspected = DateTime.UtcNow;
            }

            else if (role == RoleEnum.TavernKeeper)
            {
                var tavernKeeperRole = Role.GetRole<TavernKeeper>(amnesiac);
                tavernKeeperRole.DrunkPlayers = new List<PlayerControl>();
                foreach (var player in tavernKeeperRole.DrunkPlayers)
                {
                    Role.GetRole(player).Roleblocked = false;
                    Utils.Rpc(CustomRPC.UnroleblockPlayer, player.PlayerId, false);
                }
                tavernKeeperRole.LastDrink = DateTime.UtcNow;
            }
            else if (role ==RoleEnum.Spy)
            {
                var spy = Role.GetRole<Spy>(amnesiac);
                spy.BuggedPlayers = new List<byte>();
                spy.LastBugged = DateTime.UtcNow;
                spy.BugsLeft = CustomGameOptions.BugsPerGame;
                Utils.Rpc(CustomRPC.UnbugPlayers, PlayerControl.LocalPlayer.PlayerId);
            }

            else if (role == RoleEnum.Lookout)
            {
                var lookoutRole = Role.GetRole<Lookout>(amnesiac);
                lookoutRole.LastWatched = DateTime.UtcNow;
                lookoutRole.TimeRemaining = 0f;
            }

            else if (role == RoleEnum.Witch)
            {
                var witch = Role.GetRole<Witch>(amnesiac);
                witch.LastControl = DateTime.UtcNow;
                witch.ControledPlayer = null;
                witch.RevealedPlayers = new List<byte>();
            }

            else if (role == RoleEnum.Pirate)
            {
                var pirateRole = Role.GetRole<Pirate>(amnesiac);
                pirateRole.LastDueled = DateTime.UtcNow;
                pirateRole.DueledPlayer = null;
                pirateRole.DuelsWon = 0;
            }

            else if (role == RoleEnum.Inquisitor)
            {
                var inquisitorRole = Role.GetRole<Inquisitor>(amnesiac);
                inquisitorRole.LastAbility = DateTime.UtcNow;
                inquisitorRole.CanVanquish = true;
                if (inquisitorRole.heretics.Contains(amnesiac.PlayerId)) inquisitorRole.heretics.Remove(amnesiac.PlayerId);
            }

            else if (role == RoleEnum.Godfather)
            {
                var gfRole = Role.GetRole<Godfather>(amnesiac);
                gfRole.LastRecruit = DateTime.UtcNow.AddSeconds(-CustomGameOptions.InitialCooldowns);
            }

            else if (role == RoleEnum.Undertaker)
            {
                var dienerRole = Role.GetRole<Undertaker>(amnesiac);
                dienerRole.LastDragged = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Undertaker)
            {
                var dienerRole = Role.GetRole<Undertaker>(amnesiac);
                dienerRole.LastDragged = DateTime.UtcNow;
            }

            else if (role == RoleEnum.Demagogue)
            {
                var demagogueRole = Role.GetRole<Demagogue>(amnesiac);
                demagogueRole.LastConvince = DateTime.UtcNow;
                demagogueRole.Charges = (byte)CustomGameOptions.StartingCharges;
                demagogueRole.Convinced.Clear();
            }
            else if (role == RoleEnum.Occultist)
            {
                var occultistRole = Role.GetRole<Occultist>(amnesiac);
                occultistRole.LastMark = DateTime.UtcNow;
                occultistRole.MarkedPlayers.Clear();
            }
            else if (role == (RoleEnum)255)
            {
                var roleA = Role.GetRole<RoleA>(amnesiac);
                roleA.LastA = DateTime.UtcNow;
                roleA.LastB = DateTime.UtcNow;
                roleA.LastC = DateTime.UtcNow;
                if (roleA.AbilityB0)
                {
                    Utils.Unmorph(other);
                    other.myRend().color = Color.white;
                    roleA.AbilityB0 = false;
                    other.MyPhysics.ResetMoveState();
                }
                roleA.AbilityBActive = false;
            }
            else if (role == (RoleEnum)253)
            {
                var roleC = Role.GetRole<RoleC>(amnesiac);
                roleC.LastA = DateTime.UtcNow;
                roleC.AbilityA0 = byte.MaxValue;
            }
            else if (role == (RoleEnum)252)
            {
                var roleD = Role.GetRole<RoleD>(amnesiac);
                roleD.LastA = DateTime.UtcNow;
                roleD.AbilityA0.Clear();
            }
            else if (role == (RoleEnum)251)
            {
                var roleE = Role.GetRole<RoleE>(amnesiac);
                roleE.AbilityA0 = byte.MaxValue;
            }
            else if (role == (RoleEnum)250)
            {
                var roleF = Role.GetRole<RoleF>(amnesiac);
                roleF.LastA = DateTime.UtcNow;
            }
            else if (role == (RoleEnum)249)
            {
                var roleG = Role.GetRole<RoleG>(amnesiac);
                roleG.LastA = DateTime.UtcNow;
                foreach (var obj in roleG.AbilityA0)
                {
                    obj.Destroy();
                }
                roleG.AbilityA0.Clear();
                foreach (var obj in roleG.AbilityA1.Select(x => x.Item2))
                {
                    obj.Destroy();
                }
                roleG.AbilityA1.Clear();
            }

            else if (!(amnesiac.Is(RoleEnum.Altruist) || amnesiac.Is(RoleEnum.Amnesiac) || amnesiac.Is(Faction.Impostors)))
            {
                DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(false);
            }
            Role.GetRole(amnesiac).LastBlood = DateTime.UtcNow;
            Role.GetRole(amnesiac).LastBlood.AddSeconds(-CustomGameOptions.BloodDuration);
            Role.GetRole(amnesiac).Roleblocked = false;
            Role.GetRole(amnesiac).Reaped = false;
            Role.GetRole(amnesiac).BreadLeft = oldBread;
            if (!amnesiac.Is(RoleEnum.Vampire) && !amnesiac.Is(RoleEnum.JKNecromancer) && !amnesiac.Is(RoleEnum.Jackal))
            {
                Role.GetRole(amnesiac).FactionOverride = amneFactionOverride;
                Role.GetRole(other).FactionOverride = targetFactionOverride;
            }
            Role.GetRole(amnesiac).RegenTask();
            Role.GetRole(other).RegenTask();

            var killsList = (newRole.Kills, newRole.CorrectKills, newRole.IncorrectKills, newRole.CorrectAssassinKills, newRole.IncorrectAssassinKills);
            var otherRole = Role.GetRole(other);
            newRole.Kills = otherRole.Kills;
            newRole.CorrectKills = otherRole.CorrectKills;
            newRole.IncorrectKills = otherRole.IncorrectKills;
            newRole.CorrectAssassinKills = otherRole.CorrectAssassinKills;
            newRole.IncorrectAssassinKills = otherRole.IncorrectAssassinKills;
            otherRole.Kills = killsList.Kills;
            otherRole.CorrectKills = killsList.CorrectKills;
            otherRole.IncorrectKills = killsList.IncorrectKills;
            otherRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
            otherRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;

            if (amnesiac.Is(Faction.Impostors) && (!amnesiac.Is(RoleEnum.Traitor) || CustomGameOptions.SnitchSeesTraitor))
            {
                foreach (var snitch in Role.GetRoles(RoleEnum.Snitch))
                {
                    var snitchRole = (Snitch)snitch;
                    if (snitchRole.TasksDone && PlayerControl.LocalPlayer.Is(RoleEnum.Snitch))
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Sprite;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitchRole.SnitchArrows.Add(amnesiac.PlayerId, arrow);
                    }
                    else if (snitchRole.Revealed && PlayerControl.LocalPlayer == amnesiac)
                    {
                        var gameObj = new GameObject();
                        var arrow = gameObj.AddComponent<ArrowBehaviour>();
                        gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                        var renderer = gameObj.AddComponent<SpriteRenderer>();
                        renderer.sprite = Sprite;
                        arrow.image = renderer;
                        gameObj.layer = 5;
                        snitchRole.ImpArrows.Add(arrow);
                    }
                }
            }
        }
    }
}
