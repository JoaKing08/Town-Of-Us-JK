using System;
using HarmonyLib;
using Hazel;
using TownOfUs;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.GuardianAngelMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;
using TownOfUs.Roles.Modifiers;
using TownOfUs.Roles.Teams;
using UnityEngine;
using Object = UnityEngine.Object;
using TownOfUs.Roles.Horseman;
using System.Collections.Generic;
using System.Linq;
using Necromancer = TownOfUs.Roles.Necromancer;
using CultistNecromancer = TownOfUs.Roles.Cultist.Necromancer;

namespace TownOfUs.Patches
{
    [HarmonyPatch(typeof(IntroCutscene._CoBegin_d__35), nameof(IntroCutscene._CoBegin_d__35.MoveNext))]
    public static class Start
    {
        public static Sprite Sprite => TownOfUs.Arrow;
        public static void Postfix(IntroCutscene._CoBegin_d__35 __instance)
        {
            Role.GetRole(PlayerControl.LocalPlayer).LastBlood = DateTime.UtcNow;
            Role.GetRole(PlayerControl.LocalPlayer).LastBlood.AddSeconds(-CustomGameOptions.BloodDuration);
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Detective))
            {
                var detective = Role.GetRole<Detective>(PlayerControl.LocalPlayer);
                detective.LastExamined = DateTime.UtcNow;
                detective.LastExamined = detective.LastExamined.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ExamineCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Medium))
            {
                var medium = Role.GetRole<Medium>(PlayerControl.LocalPlayer);
                medium.LastMediated = DateTime.UtcNow;
                medium.LastMediated = medium.LastMediated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MediateCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Seer))
            {
                var seer = Role.GetRole<Seer>(PlayerControl.LocalPlayer);
                seer.LastInvestigated = DateTime.UtcNow;
                seer.LastInvestigated = seer.LastInvestigated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SeerCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Oracle))
            {
                var oracle = Role.GetRole<Oracle>(PlayerControl.LocalPlayer);
                oracle.LastConfessed = DateTime.UtcNow;
                oracle.LastConfessed = oracle.LastConfessed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ConfessCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Aurial))
            {
                var aurial = Role.GetRole<Aurial>(PlayerControl.LocalPlayer);
                aurial.LastRadiated = DateTime.UtcNow;
                aurial.LastRadiated = aurial.LastRadiated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RadiateCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Sheriff))
            {
                var sheriff = Role.GetRole<Sheriff>(PlayerControl.LocalPlayer);
                sheriff.LastKilled = DateTime.UtcNow;
                sheriff.LastKilled = sheriff.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SheriffKillCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Tracker))
            {
                var tracker = Role.GetRole<Tracker>(PlayerControl.LocalPlayer);
                tracker.LastTracked = DateTime.UtcNow;
                tracker.LastTracked = tracker.LastTracked.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TrackCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Hunter))
            {
                var hunter = Role.GetRole<Hunter>(PlayerControl.LocalPlayer);
                hunter.LastKilled = DateTime.UtcNow;
                hunter.LastKilled = hunter.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HunterKillCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.VampireHunter))
            {
                var vh = Role.GetRole<VampireHunter>(PlayerControl.LocalPlayer);
                vh.LastStaked = DateTime.UtcNow;
                vh.LastStaked = vh.LastStaked.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.StakeCd);
            }

            if (CustomGameOptions.CanStakeRoundOne)
            {
                foreach (var vh in Role.GetRoles(RoleEnum.VampireHunter))
                {
                    var vhRole = (VampireHunter)vh;
                    vhRole.UsesLeft = CustomGameOptions.MaxFailedStakesPerGame;
                    vhRole.AddedStakes = true;
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
            {
                var transporter = Role.GetRole<Transporter>(PlayerControl.LocalPlayer);
                transporter.LastTransported = DateTime.UtcNow;
                transporter.LastTransported = transporter.LastTransported.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TransportCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Trapper))
            {
                var trapper = Role.GetRole<Trapper>(PlayerControl.LocalPlayer);
                trapper.LastTrapped = DateTime.UtcNow;
                trapper.LastTrapped = trapper.LastTrapped.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TrapCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Veteran))
            {
                var veteran = Role.GetRole<Veteran>(PlayerControl.LocalPlayer);
                veteran.LastAlerted = DateTime.UtcNow;
                veteran.LastAlerted = veteran.LastAlerted.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AlertCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Chameleon))
            {
                var chameleon = Role.GetRole<Chameleon>(PlayerControl.LocalPlayer);
                chameleon.LastSwooped = DateTime.UtcNow;
                chameleon.LastSwooped = chameleon.LastSwooped.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SwoopCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Necromancer))
            {
                var necro = Role.GetRole<CultistNecromancer>(PlayerControl.LocalPlayer);
                necro.LastRevived = DateTime.UtcNow;
                necro.LastRevived = necro.LastRevived.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ReviveCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.CultistSeer))
            {
                var seer = Role.GetRole<CultistSeer>(PlayerControl.LocalPlayer);
                seer.LastInvestigated = DateTime.UtcNow;
                seer.LastInvestigated = seer.LastInvestigated.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SeerCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Whisperer))
            {
                var whisperer = Role.GetRole<Whisperer>(PlayerControl.LocalPlayer);
                whisperer.LastWhispered = DateTime.UtcNow;
                whisperer.LastWhispered = whisperer.LastWhispered.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.WhisperCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Blackmailer))
            {
                var blackmailer = Role.GetRole<Blackmailer>(PlayerControl.LocalPlayer);
                blackmailer.LastBlackmailed = DateTime.UtcNow;
                blackmailer.LastBlackmailed = blackmailer.LastBlackmailed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BlackmailCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Escapist))
            {
                var escapist = Role.GetRole<Escapist>(PlayerControl.LocalPlayer);
                escapist.LastEscape = DateTime.UtcNow;
                escapist.LastEscape = escapist.LastEscape.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.EscapeCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Grenadier))
            {
                var grenadier = Role.GetRole<Grenadier>(PlayerControl.LocalPlayer);
                grenadier.LastFlashed = DateTime.UtcNow;
                grenadier.LastFlashed = grenadier.LastFlashed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GrenadeCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Miner))
            {
                var miner = Role.GetRole<Miner>(PlayerControl.LocalPlayer);
                miner.LastMined = DateTime.UtcNow;
                miner.LastMined = miner.LastMined.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MineCd);
                var vents = Object.FindObjectsOfType<Vent>();
                miner.VentSize =
                    Vector2.Scale(vents[0].GetComponent<BoxCollider2D>().size, vents[0].transform.localScale) * 0.75f;
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Morphling))
            {
                var morphling = Role.GetRole<Morphling>(PlayerControl.LocalPlayer);
                morphling.LastMorphed = DateTime.UtcNow;
                morphling.LastMorphed = morphling.LastMorphed.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MorphlingCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Swooper))
            {
                var swooper = Role.GetRole<Swooper>(PlayerControl.LocalPlayer);
                swooper.LastSwooped = DateTime.UtcNow;
                swooper.LastSwooped = swooper.LastSwooped.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SwoopCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Venerer))
            {
                var venerer = Role.GetRole<Venerer>(PlayerControl.LocalPlayer);
                venerer.LastCamouflaged = DateTime.UtcNow;
                venerer.LastCamouflaged = venerer.LastCamouflaged.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AbilityCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Undertaker))
            {
                var undertaker = Role.GetRole<Undertaker>(PlayerControl.LocalPlayer);
                undertaker.LastDragged = DateTime.UtcNow;
                undertaker.LastDragged = undertaker.LastDragged.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DragCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist))
            {
                var arsonist = Role.GetRole<Arsonist>(PlayerControl.LocalPlayer);
                arsonist.LastDoused = DateTime.UtcNow;
                arsonist.LastDoused = arsonist.LastDoused.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DouseCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Doomsayer))
            {
                var doomsayer = Role.GetRole<Doomsayer>(PlayerControl.LocalPlayer);
                doomsayer.LastObserved = DateTime.UtcNow;
                doomsayer.LastObserved = doomsayer.LastObserved.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ObserveCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Executioner))
            {
                var exe = Role.GetRole<Executioner>(PlayerControl.LocalPlayer);
                if (exe.target == null)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.ExecutionerToJester, SendOption.Reliable, -1);
                    writer.Write(exe.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    TargetColor.ExeToJes(exe.Player);
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Glitch))
            {
                var glitch = Role.GetRole< Glitch> (PlayerControl.LocalPlayer);
                glitch.LastKill = DateTime.UtcNow;
                glitch.LastHack = DateTime.UtcNow;
                glitch.LastMimic = DateTime.UtcNow;
                glitch.LastKill = glitch.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.GlitchKillCooldown);
                glitch.LastHack = glitch.LastHack.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.HackCooldown);
                glitch.LastMimic = glitch.LastMimic.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.MimicCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.GuardianAngel))
            {
                var ga = Role.GetRole<GuardianAngel>(PlayerControl.LocalPlayer);
                ga.LastProtected = DateTime.UtcNow;
                ga.LastProtected = ga.LastProtected.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ProtectCd);
                if (ga.target == null)
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                        (byte)CustomRPC.GAToSurv, SendOption.Reliable, -1);
                    writer.Write(ga.Player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    GATargetColor.GAToSurv(ga.Player);
                }
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut))
            {
                var juggernaut = Role.GetRole<Juggernaut>(PlayerControl.LocalPlayer);
                juggernaut.LastKill = DateTime.UtcNow;
                juggernaut.LastKill = juggernaut.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JuggKCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Plaguebearer))
            {
                var plaguebearer = Role.GetRole<Plaguebearer>(PlayerControl.LocalPlayer);
                plaguebearer.LastInfected = DateTime.UtcNow;
                plaguebearer.LastInfected = plaguebearer.LastInfected.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InfectCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Baker))
            {
                var baker = Role.GetRole<Baker>(PlayerControl.LocalPlayer);
                baker.LastBreaded = DateTime.UtcNow;
                baker.LastBreaded = baker.LastBreaded.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BakerCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Berserker))
            {
                var berserker = Role.GetRole<Berserker>(PlayerControl.LocalPlayer);
                berserker.LastKill = DateTime.UtcNow;
                berserker.LastKill = berserker.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BerserkerCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.SoulCollector))
            {
                var soulCollector = Role.GetRole<SoulCollector>(PlayerControl.LocalPlayer);
                soulCollector.LastReaped = DateTime.UtcNow;
                soulCollector.LastReaped = soulCollector.LastReaped.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SoulCollectorCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Survivor))
            {
                var surv = Role.GetRole<Survivor>(PlayerControl.LocalPlayer);
                surv.LastVested = DateTime.UtcNow;
                surv.LastVested = surv.LastVested.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.VestCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf))
            {
                var werewolf = Role.GetRole<Werewolf>(PlayerControl.LocalPlayer);
                werewolf.LastRampaged = DateTime.UtcNow;
                werewolf.LastKilled = DateTime.UtcNow;
                werewolf.LastRampaged = werewolf.LastRampaged.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RampageCd);
                werewolf.LastKilled = werewolf.LastKilled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.RampageKillCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Vampire))
            {
                var vamp = Role.GetRole<Vampire>(PlayerControl.LocalPlayer);
                vamp.LastBit = DateTime.UtcNow;
                vamp.LastBit = vamp.LastBit.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BiteCd);
            }

            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Radar))
            {
                var radar = Modifier.GetModifier<Radar>(PlayerControl.LocalPlayer);
                var gameObj = new GameObject();
                var arrow = gameObj.AddComponent<ArrowBehaviour>();
                gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                var renderer = gameObj.AddComponent<SpriteRenderer>();
                renderer.sprite = Sprite;
                renderer.color = Colors.Radar;
                arrow.image = renderer;
                gameObj.layer = 5;
                arrow.target = PlayerControl.LocalPlayer.transform.position;
                radar.RadarArrow.Add(arrow);
            }

            if (CustomGameOptions.GameMode == GameMode.Teams)
            {
                var member = Role.GetRole<TeamMember>(PlayerControl.LocalPlayer);
                member.LastKill = DateTime.UtcNow;
                member.LastKill = member.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.TeamsKCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.SoloKiller))
            {
                var killer = Role.GetRole<SoloKiller>(PlayerControl.LocalPlayer);
                killer.LastKill = DateTime.UtcNow;
                killer.LastKill = killer.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SoloKillerKCd);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Inspector))
            {
                var inspector = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);
                inspector.LastInspected = DateTime.UtcNow;
                inspector.LastInspected = inspector.LastInspected.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InspectCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Monarch))
            {
                var monarch = Role.GetRole<Monarch>(PlayerControl.LocalPlayer);
                monarch.LastKnighted = DateTime.UtcNow;
                monarch.LastKnighted = monarch.LastKnighted.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.KnightCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller))
            {
                var sk = Role.GetRole<SerialKiller>(PlayerControl.LocalPlayer);
                sk.LastKill = DateTime.UtcNow;
                sk.LastKill = sk.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SerialKillerCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Inquisitor))
            {
                var inquisitor = Role.GetRole<Inquisitor>(PlayerControl.LocalPlayer);
                inquisitor.LastAbility = DateTime.UtcNow;
                inquisitor.LastAbility = inquisitor.LastAbility.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InquisitorCooldown);
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Pirate))
            {
                var pirate = Role.GetRole<Pirate>(PlayerControl.LocalPlayer);
                pirate.LastDueled = DateTime.UtcNow;
                pirate.LastDueled = pirate.LastDueled.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.DuelCooldown);
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Sniper))
            {
                var sniper = Role.GetRole<Sniper>(PlayerControl.LocalPlayer);
                sniper.LastAim = DateTime.UtcNow;
                sniper.LastAim = sniper.LastAim.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.AimCooldown);
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Spy))
            {
                var spy = Role.GetRole<Spy>(PlayerControl.LocalPlayer);
                spy.LastBugged = DateTime.UtcNow;
                spy.LastBugged = spy.LastBugged.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.BugCooldown);
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Witch))
            {
                var witch = Role.GetRole<Witch>(PlayerControl.LocalPlayer);
                witch.LastControl = DateTime.UtcNow;
                witch.LastControl = witch.LastControl.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.ControlCooldown);
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.CursedSoul))
            {
                var cursedSoul = Role.GetRole<CursedSoul>(PlayerControl.LocalPlayer);
                cursedSoul.LastSwapped = DateTime.UtcNow;
                cursedSoul.LastSwapped = cursedSoul.LastSwapped.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.SoulSwapCooldown);
            }
            if (PlayerControl.LocalPlayer.Is(ModifierEnum.Drunk))
            {
                var drunk = Modifier.GetModifier<Drunk>(PlayerControl.LocalPlayer);
                drunk.RoundsLeft = CustomGameOptions.DrunkDuration;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Investigator))
            {
                var investigator = Role.GetRole<Investigator>(PlayerControl.LocalPlayer);
                investigator.LastInvestigate = DateTime.UtcNow;
                investigator.LastInvestigate = investigator.LastInvestigate.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.InvestigateCooldown);
                investigator.UsesLeft = CustomGameOptions.MaxInvestigates;
            }
            if (PlayerControl.LocalPlayer.Is(RoleEnum.Lookout))
            {
                var lookout = Role.GetRole<Lookout>(PlayerControl.LocalPlayer);
                lookout.LastWatched = DateTime.UtcNow;
                lookout.LastWatched = lookout.LastWatched.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.WatchCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.JKNecromancer))
            {
                var necro = Role.GetRole<Necromancer>(PlayerControl.LocalPlayer);
                necro.LastRevived = DateTime.UtcNow;
                necro.LastRevived = necro.LastRevived.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.NecromancerReviveCooldown);
            }

            if (PlayerControl.LocalPlayer.Is(RoleEnum.Jackal))
            {
                var jackal = Role.GetRole<Jackal>(PlayerControl.LocalPlayer);
                jackal.LastKill = DateTime.UtcNow;
                jackal.LastKill = jackal.LastKill.AddSeconds(CustomGameOptions.InitialCooldowns - CustomGameOptions.JackalKCd);
            }
        }
    }
}