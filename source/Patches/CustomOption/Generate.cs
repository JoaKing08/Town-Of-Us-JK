using System;
using System.Collections.Generic;

namespace TownOfUs.CustomOption
{
    public class Generate
    {
        public static CustomHeaderOption CrewInvestigativeRoles;
        public static CustomNumberOption AurialOn;
        public static CustomNumberOption DetectiveOn;
        public static CustomNumberOption HaunterOn;
        public static CustomNumberOption InvestigatorOn;
        public static CustomNumberOption MysticOn;
        public static CustomNumberOption SageOn;
        public static CustomNumberOption SeerOn;
        public static CustomNumberOption SnitchOn;
        public static CustomNumberOption SpyOn;
        public static CustomNumberOption TrackerOn;
        public static CustomNumberOption TrapperOn;
        public static CustomNumberOption InspectorOn;
        public static CustomNumberOption LookoutOn;

        public static CustomHeaderOption CrewProtectiveRoles;
        public static CustomNumberOption AltruistOn;
        public static CustomNumberOption MedicOn;
        public static CustomNumberOption BodyguardOn;
        public static CustomNumberOption CrusaderOn;
        public static CustomNumberOption ClericOn;

        public static CustomHeaderOption CrewKillingRoles;
        public static CustomNumberOption SheriffOn;
        public static CustomNumberOption VampireHunterOn;
        public static CustomNumberOption VeteranOn;
        public static CustomNumberOption VigilanteOn;
        public static CustomNumberOption HunterOn;
        public static CustomNumberOption DeputyOn;

        public static CustomHeaderOption CrewSupportRoles;
        public static CustomNumberOption EngineerOn;
        public static CustomNumberOption ImitatorOn;
        public static CustomNumberOption MediumOn;
        public static CustomNumberOption TransporterOn;
        public static CustomNumberOption TavernKeeperOn;
        public static CustomNumberOption UndercoverOn;

        public static CustomHeaderOption CrewPowerRoles;
        public static CustomNumberOption MayorOn;
        public static CustomNumberOption OracleOn;
        public static CustomNumberOption ProsecutorOn;
        public static CustomNumberOption SwapperOn;
        public static CustomNumberOption MonarchOn;

        public static CustomHeaderOption NeutralBenignRoles;
        public static CustomNumberOption AmnesiacOn;
        public static CustomNumberOption GuardianAngelOn;
        public static CustomNumberOption SurvivorOn;
        public static CustomNumberOption CursedSoulOn;

        public static CustomHeaderOption NeutralEvilRoles;
        public static CustomNumberOption ExecutionerOn;
        public static CustomNumberOption JesterOn;
        public static CustomNumberOption PhantomOn;
        public static CustomNumberOption WitchOn;

        public static CustomHeaderOption NeutralChaosRoles;
        public static CustomNumberOption DoomsayerOn;
        public static CustomNumberOption PirateOn;
        public static CustomNumberOption InquisitorOn;

        public static CustomHeaderOption NeutralKillingRoles;
        public static CustomNumberOption ArsonistOn;
        public static CustomNumberOption GlitchOn;
        public static CustomNumberOption WerewolfOn;
        public static CustomNumberOption SerialKillerOn;
        public static CustomNumberOption JuggernautOn;

        public static CustomHeaderOption NeutralProselyteRoles;
        public static CustomNumberOption VampireOn;
        public static CustomNumberOption NecromancerOn;
        public static CustomNumberOption JackalOn;

        public static CustomHeaderOption NeutralApocalypseRoles;
        public static CustomNumberOption PlaguebearerOn;
        public static CustomNumberOption BakerOn;
        public static CustomNumberOption BerserkerOn;
        public static CustomNumberOption SoulCollectorOn;

        public static CustomHeaderOption ImpostorConcealingRoles;
        public static CustomNumberOption EscapistOn;
        public static CustomNumberOption MorphlingOn;
        public static CustomNumberOption SwooperOn;
        public static CustomNumberOption GrenadierOn;
        public static CustomNumberOption VenererOn;

        public static CustomHeaderOption ImpostorKillingRoles;
        public static CustomNumberOption BomberOn;
        public static CustomNumberOption TraitorOn;
        public static CustomNumberOption WarlockOn;
        public static CustomNumberOption PoisonerOn;
        public static CustomNumberOption SniperOn;

        public static CustomHeaderOption ImpostorSupportRoles;
        public static CustomNumberOption BlackmailerOn;
        public static CustomNumberOption JanitorOn;
        public static CustomNumberOption MinerOn;
        public static CustomNumberOption UndertakerOn;
        public static CustomNumberOption PoltergeistOn;

        public static CustomHeaderOption CrewmateModifiers;
        public static CustomNumberOption AftermathOn;
        public static CustomNumberOption BaitOn;
        public static CustomNumberOption DiseasedOn;
        public static CustomNumberOption FrostyOn;
        public static CustomNumberOption MultitaskerOn;
        public static CustomNumberOption TorchOn;
        public static CustomNumberOption FamousOn;

        public static CustomHeaderOption GlobalModifiers;
        public static CustomNumberOption ButtonBarryOn;
        public static CustomNumberOption FlashOn;
        public static CustomNumberOption GiantOn;
        public static CustomNumberOption RadarOn;
        public static CustomNumberOption SleuthOn;
        public static CustomNumberOption TiebreakerOn;
        public static CustomNumberOption DrunkOn;

        public static CustomHeaderOption ImpostorModifiers;
        public static CustomNumberOption DisperserOn;
        public static CustomNumberOption DoubleShotOn;
        public static CustomNumberOption UnderdogOn;
        public static CustomNumberOption TaskerOn;

        public static CustomHeaderOption ObjectiveModifiers;
        public static CustomNumberOption LoversOn;
        public static CustomNumberOption ImpostorAgentOn;
        public static CustomNumberOption ApocalypseAgentOn;

        public static CustomHeaderOption MapSettings;
        public static CustomToggleOption RandomMapEnabled;
        public static CustomNumberOption RandomMapSkeld;
        public static CustomNumberOption RandomMapMira;
        public static CustomNumberOption RandomMapPolus;
        public static CustomNumberOption RandomMapAirship;
        public static CustomNumberOption RandomMapFungle;
        public static CustomNumberOption RandomMapSubmerged;
        public static CustomToggleOption AutoAdjustSettings;
        public static CustomToggleOption SmallMapHalfVision;
        public static CustomNumberOption SmallMapDecreasedCooldown;
        public static CustomNumberOption LargeMapIncreasedCooldown;
        public static CustomNumberOption SmallMapIncreasedShortTasks;
        public static CustomNumberOption SmallMapIncreasedLongTasks;
        public static CustomNumberOption LargeMapDecreasedShortTasks;
        public static CustomNumberOption LargeMapDecreasedLongTasks;

        public static CustomHeaderOption CustomGameSettings;
        public static CustomToggleOption ColourblindComms;
        public static CustomToggleOption ImpostorSeeRoles;
        public static CustomToggleOption DeadSeeRoles;
        public static CustomNumberOption InitialCooldowns;
        public static CustomToggleOption ParallelMedScans;
        public static CustomStringOption SkipButtonDisable;
        public static CustomToggleOption FirstDeathShield;
        public static CustomToggleOption NeutralEvilWinEndsGame;
        public static CustomToggleOption GhostsDoTasks;
        public static CustomStringOption OvertakeWin;
        public static CustomNumberOption NotificationDuration;
        public static CustomToggleOption ShowImpostorsRemaining;
        public static CustomToggleOption ShowApocalypseRemaining;
        public static CustomToggleOption ShowUndeadRemaining;
        public static CustomToggleOption ShowKillingRemaining;
        public static CustomToggleOption ShowProselyteRemaining;
        public static CustomStringOption ImpostorsChat;
        public static CustomStringOption ApocalypseChat;

        public static CustomHeaderOption BetterPolusSettings;
        public static CustomToggleOption VentImprovements;
        public static CustomToggleOption VitalsLab;
        public static CustomToggleOption ColdTempDeathValley;
        public static CustomToggleOption WifiChartCourseSwap;

        public static CustomHeaderOption GameModeSettings;
        public static CustomStringOption GameMode;

        public static CustomHeaderOption ClassicSettings;
        public static CustomNumberOption MinNeutralBenignRoles;
        public static CustomNumberOption MaxNeutralBenignRoles;
        public static CustomNumberOption MinNeutralEvilRoles;
        public static CustomNumberOption MaxNeutralEvilRoles;
        public static CustomNumberOption MinNeutralChaosRoles;
        public static CustomNumberOption MaxNeutralChaosRoles;
        public static CustomNumberOption MinNeutralKillingRoles;
        public static CustomNumberOption MaxNeutralKillingRoles;
        public static CustomNumberOption MinNeutralProselyteRoles;
        public static CustomNumberOption MaxNeutralProselyteRoles;
        public static CustomNumberOption MinNeutralApocalypseRoles;
        public static CustomNumberOption MaxNeutralApocalypseRoles;

        public static CustomHeaderOption AllAnySettings;
        public static CustomToggleOption RandomNumberImps;

        public static CustomHeaderOption KillingOnlySettings;
        public static CustomNumberOption NeutralRoles;
        public static CustomNumberOption VeteranCount;
        public static CustomNumberOption VigilanteCount;
        public static CustomToggleOption AddArsonist;
        public static CustomToggleOption AddPlaguebearer;

        public static CustomHeaderOption CultistSettings;
        public static CustomNumberOption MayorCultistOn;
        public static CustomNumberOption SeerCultistOn;
        public static CustomNumberOption SheriffCultistOn;
        public static CustomNumberOption SurvivorCultistOn;
        public static CustomNumberOption NumberOfSpecialRoles;
        public static CustomNumberOption MaxChameleons;
        public static CustomNumberOption MaxEngineers;
        public static CustomNumberOption MaxInvestigators;
        public static CustomNumberOption MaxMystics;
        public static CustomNumberOption MaxSnitches;
        public static CustomNumberOption MaxSpies;
        public static CustomNumberOption MaxTransporters;
        public static CustomNumberOption MaxVigilantes;
        public static CustomNumberOption WhisperCooldown;
        public static CustomNumberOption IncreasedCooldownPerWhisper;
        public static CustomNumberOption WhisperRadius;
        public static CustomNumberOption ConversionPercentage;
        public static CustomNumberOption DecreasedPercentagePerConversion;
        public static CustomNumberOption ReviveCooldown;
        public static CustomNumberOption IncreasedCooldownPerRevive;
        public static CustomNumberOption MaxReveals;

        public static CustomHeaderOption TaskTrackingSettings;
        public static CustomToggleOption SeeTasksDuringRound;
        public static CustomToggleOption SeeTasksDuringMeeting;
        public static CustomToggleOption SeeTasksWhenDead;

        public static CustomHeaderOption TeamsSettings;
        public static CustomNumberOption TeamsKCd;
        public static CustomNumberOption TeamsAmount;
        public static CustomToggleOption TeamsVent;

        public static CustomHeaderOption SoloKillerSettings;
        public static CustomNumberOption SoloKillerKCd;
        public static CustomToggleOption SoloKillerVent;
        public static CustomStringOption SoloKillerPlayer;

        public static CustomHeaderOption Sheriff;
        public static CustomToggleOption SheriffKillOther;
        public static CustomToggleOption SheriffKillsExecutioner;
        public static CustomToggleOption SheriffKillsJester;
        public static CustomToggleOption SheriffKillsWitch;
        public static CustomToggleOption SheriffKillsDoomsayer;
        public static CustomToggleOption SheriffKillsPirate;
        public static CustomToggleOption SheriffKillsInquisitor;
        public static CustomToggleOption SheriffKillsArsonist;
        public static CustomToggleOption SheriffKillsJuggernaut;
        public static CustomToggleOption SheriffKillsPlaguebearer;
        public static CustomToggleOption SheriffKillsBaker;
        public static CustomToggleOption SheriffKillsBerserker;
        public static CustomToggleOption SheriffKillsSoulCollector;
        public static CustomToggleOption SheriffKillsGlitch;
        public static CustomToggleOption SheriffKillsWerewolf;
        public static CustomToggleOption SheriffKillsSerialKiller;
        public static CustomToggleOption SheriffKillsVampire;
        public static CustomToggleOption SheriffKillsNecromancer;
        public static CustomToggleOption SheriffKillsUndead;
        public static CustomToggleOption SheriffKillsJackal;
        public static CustomToggleOption SheriffKillsRecruits;
        public static CustomToggleOption SheriffKillsAgent;
        public static CustomNumberOption SheriffKillCd;
        public static CustomToggleOption SheriffBodyReport;

        public static CustomHeaderOption Hunter;
        public static CustomNumberOption HunterKillCd;
        public static CustomNumberOption HunterStalkCd;
        public static CustomNumberOption HunterStalkDuration;
        public static CustomNumberOption HunterStalkUses;
        public static CustomToggleOption HunterBodyReport;

        public static CustomHeaderOption Engineer;
        public static CustomNumberOption MaxFixes;

        public static CustomHeaderOption Investigator;
        public static CustomNumberOption FootprintSize;
        public static CustomNumberOption FootprintInterval;
        public static CustomNumberOption FootprintDuration;
        public static CustomToggleOption AnonymousFootPrint;
        public static CustomToggleOption VentFootprintVisible;
        public static CustomNumberOption InvestigateCooldown;
        public static CustomNumberOption MaxInvestigates;

        public static CustomHeaderOption Medic;
        public static CustomStringOption ShowShielded;
        public static CustomStringOption WhoGetsNotification;
        public static CustomToggleOption ShieldBreaks;
        public static CustomToggleOption MedicReportSwitch;
        public static CustomNumberOption MedicReportNameDuration;
        public static CustomNumberOption MedicReportColorDuration;

        public static CustomHeaderOption Seer;
        public static CustomNumberOption SeerCooldown;
        public static CustomToggleOption CrewKillingRed;
        public static CustomToggleOption NeutBenignRed;
        public static CustomToggleOption NeutEvilRed;
        public static CustomToggleOption NeutChaosRed;
        public static CustomToggleOption NeutKillingRed;
        public static CustomToggleOption NeutProselyteRed;
        public static CustomToggleOption NeutApocalypseRed;
        public static CustomToggleOption AgentRed;
        public static CustomToggleOption UndeadRed;
        public static CustomToggleOption RecruitRed;
        public static CustomToggleOption TraitorColourSwap;

        public static CustomHeaderOption Spy;
        public static CustomStringOption WhoSeesDead;
        public static CustomNumberOption BugsPerGame;
        public static CustomNumberOption BugCooldown;

        public static CustomHeaderOption Swapper;
        public static CustomToggleOption SwapperButton;

        public static CustomHeaderOption Transporter;
        public static CustomNumberOption TransportCooldown;
        public static CustomNumberOption TransportMaxUses;
        public static CustomToggleOption TransporterVitals;

        public static CustomHeaderOption Jester;
        public static CustomToggleOption JesterButton;
        public static CustomToggleOption JesterVent;
        public static CustomToggleOption JesterImpVision;
        public static CustomToggleOption JesterHaunt;

        public static CustomHeaderOption TheGlitch;
        public static CustomNumberOption MimicCooldownOption;
        public static CustomNumberOption MimicDurationOption;
        public static CustomNumberOption HackCooldownOption;
        public static CustomNumberOption HackDurationOption;
        public static CustomNumberOption GlitchKillCooldownOption;
        public static CustomStringOption GlitchHackDistanceOption;
        public static CustomToggleOption GlitchVent;

        public static CustomHeaderOption Juggernaut;
        public static CustomNumberOption JuggKillCooldown;
        public static CustomNumberOption ReducedKCdPerKill;
        public static CustomToggleOption JuggVent;

        public static CustomHeaderOption Morphling;
        public static CustomNumberOption MorphlingCooldown;
        public static CustomNumberOption MorphlingDuration;
        public static CustomToggleOption MorphlingVent;

        public static CustomHeaderOption Executioner;
        public static CustomStringOption OnTargetDead;
        public static CustomToggleOption ExecutionerButton;
        public static CustomToggleOption ExecutionerTorment;

        public static CustomHeaderOption Phantom;
        public static CustomNumberOption PhantomTasksRemaining;
        public static CustomToggleOption PhantomSpook;

        public static CustomHeaderOption Pirate;
        public static CustomNumberOption DuelCooldown;
        public static CustomNumberOption PirateDuelsToWin;

        public static CustomHeaderOption Snitch;
        public static CustomToggleOption SnitchSeesNeutrals;
        public static CustomNumberOption SnitchTasksRemaining;
        public static CustomToggleOption SnitchSeesImpInMeeting;
        public static CustomToggleOption SnitchSeesTraitor;

        public static CustomHeaderOption Altruist;
        public static CustomNumberOption ReviveDuration;
        public static CustomToggleOption AltruistTargetBody;

        public static CustomHeaderOption Miner;
        public static CustomNumberOption MineCooldown;
        public static CustomToggleOption InstantVent;

        public static CustomHeaderOption Swooper;
        public static CustomNumberOption SwoopCooldown;
        public static CustomNumberOption SwoopDuration;
        public static CustomToggleOption SwooperVent;

        public static CustomHeaderOption Arsonist;
        public static CustomNumberOption DouseCooldown;
        public static CustomNumberOption MaxDoused;
        public static CustomToggleOption ArsoImpVision;
        public static CustomToggleOption IgniteCdRemoved;

        public static CustomHeaderOption Undertaker;
        public static CustomNumberOption DragCooldown;
        public static CustomNumberOption UndertakerDragSpeed;
        public static CustomToggleOption UndertakerVent;
        public static CustomToggleOption UndertakerVentWithBody;

        public static CustomHeaderOption Assassin;
        public static CustomNumberOption NumberOfImpostorAssassins;
        public static CustomNumberOption NumberOfNeutralAssassins;
        public static CustomToggleOption AmneTurnImpAssassin;
        public static CustomToggleOption AmneTurnNeutAssassin;
        public static CustomToggleOption TraitorCanAssassin;
        public static CustomNumberOption AssassinKills;
        public static CustomToggleOption AssassinMultiKill;
        public static CustomToggleOption AssassinCrewmateGuess;
        public static CustomToggleOption AssassinGuessCrewInvestigative;
        public static CustomToggleOption AssassinGuessNeutralBenign;
        public static CustomToggleOption AssassinGuessNeutralEvil;
        public static CustomToggleOption AssassinGuessNeutralChaos;
        public static CustomToggleOption AssassinGuessNeutralKilling;
        public static CustomToggleOption AssassinGuessNeutralProselyte;
        public static CustomToggleOption AssassinGuessNeutralApocalypse;
        public static CustomToggleOption AssassinGuessImpostors;
        public static CustomToggleOption AssassinGuessModifiers;
        public static CustomToggleOption AssassinGuessLovers;
        public static CustomToggleOption AssassinateAfterVoting;

        public static CustomHeaderOption Underdog;
        public static CustomNumberOption UnderdogKillBonus;
        public static CustomToggleOption UnderdogIncreasedKC;

        public static CustomHeaderOption Vigilante;
        public static CustomNumberOption VigilanteKills;
        public static CustomToggleOption VigilanteMultiKill;
        public static CustomToggleOption VigilanteGuessNeutralBenign;
        public static CustomToggleOption VigilanteGuessNeutralEvil;
        public static CustomToggleOption VigilanteGuessNeutralChaos;
        public static CustomToggleOption VigilanteGuessNeutralKilling;
        public static CustomToggleOption VigilanteGuessNeutralProselyte;
        public static CustomToggleOption VigilanteGuessNeutralApocalypse;
        public static CustomToggleOption VigilanteGuessLovers;
        public static CustomToggleOption VigilanteAfterVoting;

        public static CustomHeaderOption Haunter;
        public static CustomNumberOption HaunterTasksRemainingClicked;
        public static CustomNumberOption HaunterTasksRemainingAlert;
        public static CustomToggleOption HaunterRevealsNeutrals;
        public static CustomStringOption HaunterCanBeClickedBy;

        public static CustomHeaderOption Grenadier;
        public static CustomNumberOption GrenadeCooldown;
        public static CustomNumberOption GrenadeDuration;
        public static CustomToggleOption GrenadierIndicators;
        public static CustomToggleOption GrenadierVent;
        public static CustomNumberOption FlashRadius;

        public static CustomHeaderOption Veteran;
        public static CustomToggleOption KilledOnAlert;
        public static CustomNumberOption AlertCooldown;
        public static CustomNumberOption AlertDuration;
        public static CustomNumberOption MaxAlerts;

        public static CustomHeaderOption Tracker;
        public static CustomNumberOption UpdateInterval;
        public static CustomNumberOption TrackCooldown;
        public static CustomToggleOption ResetOnNewRound;
        public static CustomNumberOption MaxTracks;

        public static CustomHeaderOption Trapper;
        public static CustomNumberOption TrapCooldown;
        public static CustomToggleOption TrapsRemoveOnNewRound;
        public static CustomNumberOption MaxTraps;
        public static CustomNumberOption MinAmountOfTimeInTrap;
        public static CustomNumberOption TrapSize;
        public static CustomNumberOption MinAmountOfPlayersInTrap;

        public static CustomHeaderOption Traitor;
        public static CustomNumberOption LatestSpawn;
        public static CustomToggleOption NeutralKillingStopsTraitor;

        public static CustomHeaderOption Amnesiac;
        public static CustomToggleOption RememberArrows;
        public static CustomNumberOption RememberArrowDelay;

        public static CustomHeaderOption Medium;
        public static CustomNumberOption MediateCooldown;
        public static CustomToggleOption ShowMediatePlayer;
        public static CustomToggleOption ShowMediumToDead;
        public static CustomStringOption DeadRevealed;

        public static CustomHeaderOption Survivor;
        public static CustomNumberOption VestCd;
        public static CustomNumberOption VestDuration;
        public static CustomNumberOption VestKCReset;
        public static CustomNumberOption MaxVests;

        public static CustomHeaderOption GuardianAngel;
        public static CustomNumberOption ProtectCd;
        public static CustomNumberOption ProtectDuration;
        public static CustomNumberOption ProtectKCReset;
        public static CustomNumberOption MaxProtects;
        public static CustomStringOption ShowProtect;
        public static CustomStringOption GaOnTargetDeath;
        public static CustomToggleOption GATargetKnows;
        public static CustomToggleOption GAKnowsTargetRole;
        public static CustomNumberOption EvilTargetPercent;

        public static CustomHeaderOption Mystic;
        public static CustomNumberOption MysticArrowDuration;
        public static CustomToggleOption AllowVision;
        public static CustomNumberOption VisionCooldown;

        public static CustomHeaderOption Blackmailer;
        public static CustomNumberOption BlackmailCooldown;
        public static CustomToggleOption BlackmailInvisible;
        public static CustomToggleOption BlackmailedVote;

        public static CustomHeaderOption Plaguebearer;
        public static CustomNumberOption InfectCooldown;
        public static CustomToggleOption PlaguebearerVent;
        public static CustomNumberOption PestKillCooldown;
        public static CustomToggleOption AnnouncePestilence;
        public static CustomToggleOption PestVent;

        public static CustomHeaderOption Baker;
        public static CustomNumberOption BreadNeeded;
        public static CustomNumberOption BakerCooldown;
        public static CustomNumberOption BreadSize;
        public static CustomToggleOption BakerVent;
        public static CustomNumberOption FamineCooldown;
        public static CustomToggleOption AnnounceFamine;
        public static CustomToggleOption FamineVent;

        public static CustomHeaderOption Berserker;
        public static CustomNumberOption KillsToWar;
        public static CustomNumberOption BerserkerCooldown;
        public static CustomNumberOption BerserkerCooldownBonus;
        public static CustomToggleOption BerserkerVent;
        public static CustomNumberOption WarCooldown;
        public static CustomNumberOption WarRampage;
        public static CustomToggleOption AnnounceWar;
        public static CustomToggleOption WarVent;

        public static CustomHeaderOption SoulCollector;
        public static CustomNumberOption SoulsNeeded;
        public static CustomNumberOption SoulCollectorCooldown;
        public static CustomToggleOption SoulCollectorVent;
        public static CustomNumberOption DeathCooldown;
        public static CustomToggleOption AnnounceDeath;
        public static CustomToggleOption DeathVent;

        public static CustomHeaderOption Werewolf;
        public static CustomNumberOption RampageCooldown;
        public static CustomNumberOption RampageDuration;
        public static CustomNumberOption RampageKillCooldown;
        public static CustomToggleOption WerewolfVent;

        public static CustomHeaderOption Detective;
        public static CustomNumberOption ExamineCooldown;
        public static CustomToggleOption DetectiveReportOn;
        public static CustomNumberOption DetectiveRoleDuration;
        public static CustomNumberOption DetectiveFactionDuration;
        public static CustomToggleOption CanDetectLastKiller;

        public static CustomHeaderOption Escapist;
        public static CustomNumberOption EscapeCooldown;
        public static CustomToggleOption EscapistVent;

        public static CustomHeaderOption Bomber;
        public static CustomNumberOption MaxKillsInDetonation;
        public static CustomNumberOption DetonateDelay;
        public static CustomNumberOption DetonateRadius;
        public static CustomToggleOption BomberVent;

        public static CustomHeaderOption Doomsayer;
        public static CustomNumberOption ObserveCooldown;
        public static CustomToggleOption DoomsayerGuessCrewInvestigative;
        public static CustomToggleOption DoomsayerGuessNeutralBenign;
        public static CustomToggleOption DoomsayerGuessNeutralEvil;
        public static CustomToggleOption DoomsayerGuessNeutralChaos;
        public static CustomToggleOption DoomsayerGuessNeutralKilling;
        public static CustomToggleOption DoomsayerGuessNeutralProselyte;
        public static CustomToggleOption DoomsayerGuessNeutralApocalypse;
        public static CustomToggleOption DoomsayerGuessImpostors;
        public static CustomToggleOption DoomsayerAfterVoting;
        public static CustomNumberOption DoomsayerGuessesToWin;
        public static CustomToggleOption DoomsayerCantObserve;

        public static CustomHeaderOption Vampire;
        public static CustomNumberOption BiteCooldown;
        public static CustomToggleOption VampImpVision;
        public static CustomToggleOption VampVent;
        public static CustomToggleOption NewVampCanAssassin;
        public static CustomNumberOption MaxVampiresPerGame;
        public static CustomToggleOption CanBiteNeutralBenign;
        public static CustomToggleOption CanBiteNeutralEvil;
        public static CustomToggleOption CanBiteNeutralChaos;
        public static CustomStringOption VampiresChat;

        public static CustomHeaderOption VampireHunter;
        public static CustomNumberOption StakeCooldown;
        public static CustomNumberOption MaxFailedStakesPerGame;
        public static CustomToggleOption CanStakeRoundOne;
        public static CustomToggleOption SelfKillAfterFinalStake;
        public static CustomStringOption BecomeOnVampDeaths;

        public static CustomHeaderOption Prosecutor;
        public static CustomToggleOption ProsDiesOnIncorrectPros;
        public static CustomNumberOption MaxProsecutions;
        public static CustomToggleOption RevealProsecutor;

        public static CustomHeaderOption Warlock;
        public static CustomNumberOption ChargeUpDuration;
        public static CustomNumberOption ChargeUseDuration;

        public static CustomHeaderOption Oracle;
        public static CustomNumberOption ConfessCooldown;
        public static CustomNumberOption RevealAccuracy;
        public static CustomToggleOption NeutralBenignShowsEvil;
        public static CustomToggleOption NeutralEvilShowsEvil;
        public static CustomToggleOption NeutralChaosShowsEvil;
        public static CustomToggleOption NeutralKillingShowsEvil;
        public static CustomToggleOption NeutralProselyteShowsEvil;
        public static CustomToggleOption NeutralApocalypseShowsEvil;

        public static CustomHeaderOption Venerer;
        public static CustomNumberOption AbilityCooldown;
        public static CustomNumberOption AbilityDuration;
        public static CustomNumberOption SprintSpeed;
        public static CustomNumberOption FreezeSpeed;

        public static CustomHeaderOption Aurial;
        public static CustomNumberOption RadiateRange;
        public static CustomNumberOption RadiateCooldown;
        public static CustomNumberOption RadiateSucceedChance;
        public static CustomNumberOption RadiateCount;
        public static CustomNumberOption RadiateInvis;
        public static CustomNumberOption AurialVisionMultiplier;

        public static CustomHeaderOption Giant;
        public static CustomNumberOption GiantSlow;

        public static CustomHeaderOption Flash;
        public static CustomNumberOption FlashSpeed;

        public static CustomHeaderOption Diseased;
        public static CustomNumberOption DiseasedKillMultiplier;

        public static CustomHeaderOption Bait;
        public static CustomNumberOption BaitMinDelay;
        public static CustomNumberOption BaitMaxDelay;

        public static CustomHeaderOption Lovers;
        public static CustomToggleOption BothLoversDie;
        public static CustomNumberOption LovingImpPercent;
        public static CustomToggleOption NeutralLovers;
        public static CustomStringOption LoversChat;

        public static CustomHeaderOption Frosty;
        public static CustomNumberOption ChillDuration;
        public static CustomNumberOption ChillStartSpeed;

        public static CustomHeaderOption Inspector;
        public static CustomNumberOption InspectCooldown;
        public static CustomNumberOption BloodDuration;

        public static CustomHeaderOption Monarch;
        public static CustomNumberOption KnightCooldown;
        public static CustomNumberOption MaxKnights;
        public static CustomToggleOption KnightFirstRound;
        public static CustomToggleOption InstantKnight;

        public static CustomHeaderOption Inquisitor;
        public static CustomNumberOption InquisitorCooldown;
        public static CustomNumberOption NumberOfHeretics;
        public static CustomStringOption HereticsInfo;

        public static CustomHeaderOption SerialKiller;
        public static CustomNumberOption SerialKillerCooldown;
        public static CustomNumberOption BloodlustCooldown;
        public static CustomNumberOption BloodlustDuration;
        public static CustomNumberOption KillsToBloodlust;
        public static CustomToggleOption SerialKillerVent;

        public static CustomHeaderOption TavernKeeper;
        public static CustomNumberOption DrinkCooldown;
        public static CustomNumberOption DrinksPerRound;

        public static CustomHeaderOption Poisoner;
        public static CustomNumberOption PoisonDelay;
        public static CustomToggleOption PoisonerVent;

        public static CustomHeaderOption Sniper;
        public static CustomNumberOption AimCooldown;
        public static CustomToggleOption SniperVent;

        public static CustomHeaderOption Undercover;
        public static CustomToggleOption UndercoverKillEachother;
        public static CustomToggleOption UndercoverVent;
        public static CustomToggleOption UndercoverEscapist;
        public static CustomToggleOption UndercoverGrenadier;
        public static CustomToggleOption UndercoverMorphling;
        public static CustomToggleOption UndercoverSwooper;
        public static CustomToggleOption UndercoverVenerer;
        public static CustomToggleOption UndercoverBomber;
        public static CustomToggleOption UndercoverWarlock;
        public static CustomToggleOption UndercoverPoisoner;
        public static CustomToggleOption UndercoverSniper;
        public static CustomToggleOption UndercoverBlackmailer;
        public static CustomToggleOption UndercoverJanitor;
        public static CustomToggleOption UndercoverMiner;
        public static CustomToggleOption UndercoverUndertaker;
        public static CustomToggleOption UndercoverPlaguebearer;
        public static CustomToggleOption UndercoverBaker;
        public static CustomToggleOption UndercoverBerserker;
        public static CustomToggleOption UndercoverSoulCollector;

        public static CustomHeaderOption Drunk;
        public static CustomToggleOption DrunkWearsOff;
        public static CustomNumberOption DrunkDuration;

        public static CustomHeaderOption Poltergeist;
        public static CustomNumberOption PoltergeistTasksRemainingClicked;
        public static CustomNumberOption PoltergeistTasksRemainingAlert;
        public static CustomNumberOption PoltergeistKCdMult;
        public static CustomStringOption PoltergeistCanBeClickedBy;

        public static CustomHeaderOption Witch;
        public static CustomNumberOption ControlCooldown;
        public static CustomNumberOption OrderCooldown;
        public static CustomStringOption WitchLearns;

        public static CustomHeaderOption CursedSoul;
        public static CustomNumberOption SoulSwapCooldown;
        public static CustomNumberOption SoulSwapAccuracy;
        public static CustomToggleOption SoulSwapImp;
        public static CustomStringOption SwappedBecomes;

        public static CustomHeaderOption Lookout;
        public static CustomNumberOption WatchCooldown;
        public static CustomNumberOption WatchDuration;
        public static CustomNumberOption WatchVisionMultiplier;

        public static CustomHeaderOption Necromancer;
        public static CustomNumberOption NecromancerReviveCooldown;
        public static CustomNumberOption ReviveCooldownIncrease;
        public static CustomNumberOption RitualKillCooldown;
        public static CustomNumberOption RitualKillCooldownIncrease;
        public static CustomNumberOption MaxNumberOfUndead;
        public static CustomToggleOption NecromancerVent;
        public static CustomStringOption UndeadChat;

        public static CustomHeaderOption Jackal;
        public static CustomNumberOption JackalKCd;
        public static CustomToggleOption RecruitsLifelink;
        public static CustomToggleOption RecruitsSeeJackal;
        public static CustomToggleOption JackalVent;
        public static CustomStringOption RecruitsChat;

        public static CustomHeaderOption Deputy;
        public static CustomToggleOption RevealDeputy;
        public static CustomToggleOption MisfireKillsDeputy;
        public static CustomNumberOption DeputyAimCooldown;
        public static CustomNumberOption MaxDeputyTargets;

        public static CustomHeaderOption Bodyguard;
        public static CustomNumberOption GuardCooldown;

        public static CustomHeaderOption Crusader;
        public static CustomNumberOption FortifyCooldown;
        public static CustomNumberOption MaxFortify;

        public static CustomHeaderOption Cleric;
        public static CustomNumberOption BarrierCooldown;
        public static CustomNumberOption BarrierCooldownReset;

        public static CustomHeaderOption Sage;
        public static CustomNumberOption CompareCooldown;
        public static CustomNumberOption CompareAccuracy;

        public static CustomHeaderOption RoleListSettings;
        public static Dictionary<int, CustomStringOption> RoleEntries;
        public static CustomHeaderOption RoleListSpacing0;
        public static Dictionary<int, CustomStringOption> BanEntries;
        public static CustomHeaderOption RoleListSpacing1;
        public static CustomNumberOption MaxImps;
        public static CustomToggleOption AllUnique;

        public static string[] RoleEntriesData => new string[]
    {
        "None",
        "Any",


        "<color=#0000FFFF>Random</color> <color=#00FFFFFF>Crewmate</color>",
        "<color=#0000FFFF>Common</color> <color=#00FFFFFF>Crewmate</color>",
        "<color=#0000FFFF>Uncommon</color> <color=#00FFFFFF>Crewmate</color>",
        "<color=#80FFFFFF>Crewmate</color>",

        "<color=#00FFFFFF>Crewmate</color> <color=#0000FFFF>Investigative</color>",
        "<color=#B34D99FF>Aurial</color>",
        "<color=#4D4DFFFF>Detective</color>",
        "<color=#8BFFDBFF>Inspector</color>",
        "<color=#00B3B3FF>Investigator</color>",
        "<color=#80DFDFFF>Lookout</color>",
        "<color=#4D99E6FF>Mystic</color>",
        "<color=#4B0082FF>Sage</color>",
        "<color=#FFCC80FF>Seer</color>",
        "<color=#D4AF37FF>Snitch</color>",
        "<color=#CCA3CCFF>Spy</color>",
        "<color=#009900FF>Tracker</color>",
        "<color=#A7D1B3FF>Trapper</color>",

        "<color=#00FFFFFF>Crewmate</color> <color=#0000FFFF>Killing</color>",
        "<color=#AAAAAAFF>Deputy</color>",
        "<color=#29AB87FF>Hunter</color>",
        "<color=#FFFF00FF>Sheriff</color>",
        "<color=#B3B3E6FF>Vampire Hunter</color>",
        "<color=#998040FF>Veteran</color>",
        "<color=#FFFF99FF>Vigilante</color>",

        "<color=#00FFFFFF>Crewmate</color> <color=#0000FFFF>Protective</color>",
        "<color=#660000FF>Altruist</color>",
        "<color=#36454FFF>Bodyguard</color>",
        "<color=#90EE90FF>Cleric</color>",
        "<color=#EFEFEFFF>Crusader</color>",
        "<color=#006600FF>Medic</color>",

        "<color=#00FFFFFF>Crewmate</color> <color=#0000FFFF>Support</color>",
        "<color=#FFA60AFF>Engineer</color>",
        "<color=#B3D94DFF>Imitator</color>",
        "<color=#A680FFFF>Medium</color>",
        "<color=#8B4513FF>Tavern Keeper</color>",
        "<color=#00EEFFFF>Transporter</color>",
        "<color=#002600FF>Undercover</color>",

        "<color=#00FFFFFF>Crewmate</color> <color=#0000FFFF>Power</color>",
        "<color=#704FA8FF>Mayor</color>",
        "<color=#9628C8FF>Monarch</color>",
        "<color=#BF00BFFF>Oracle</color>",
        "<color=#B38000FF>Prosecutor</color>",
        "<color=#66E666FF>Swapper</color>",


        "<color=#0000FFFF>Random</color> <color=#808080FF>Neutral</color>",
        "<color=#0000FFFF>Common</color> <color=#808080FF>Neutral</color>",
        "<color=#0000FFFF>Uncommon</color> <color=#808080FF>Neutral</color>",

        "<color=#808080FF>Neutral</color> <color=#0000FFFF>Benign</color>",
        "<color=#80B2FFFF>Amnesiac</color>",
        "<color=#8000FFFF>Cursed Soul</color>",
        "<color=#B3FFFFFF>Guardian Angel</color>",
        "<color=#FFE64DFF>Survivor</color>",

        "<color=#808080FF>Neutral</color> <color=#0000FFFF>Evil</color>",
        "<color=#8C4005FF>Executioner</color>",
        "<color=#FFBFCCFF>Jester</color>",
        "<color=#C060FFFF>Witch</color>",

        "<color=#808080FF>Neutral</color> <color=#0000FFFF>Chaos</color>",
        "<color=#00FF80FF>Doomsayer</color>",
        "<color=#821252FF>Inquisitor</color>",
        "<color=#ECC23EFF>Pirate</color>",

        "<color=#808080FF>Neutral</color> <color=#0000FFFF>Killing</color>",
        "<color=#FF4D00FF>Arsonist</color>",
        "<color=#8C004DFF>Juggernaut</color>",
        "<color=#1D4DFCFF>Serial Killer</color>",
        "<color=#00FF00FF>The Glitch</color>",
        "<color=#A86629FF>Werewolf</color>",

        "<color=#808080FF>Neutral</color> <color=#0000FFFF>Proselyte</color>",
        "<color=#666666FF>Jackal</color>",
        "<color=#679556FF>Necromancer</color>",
        "<color=#262626FF>Vampire</color>",

        "<color=#808080FF>Neutral</color> <color=#0000FFFF>Apocalypse</color>",
        "<color=#FFC080FF>Baker</color>",
        "<color=#FF4F00FF>Berserker</color>",
        "<color=#E6FFB3FF>Plaguebearer</color>",
        "<color=#E000FFFF>Soul Collector</color>",


        "<color=#0000FFFF>Random</color> <color=#FF0000FF>Impostor</color>",
        "<color=#0000FFFF>Common</color> <color=#FF0000FF>Impostor</color>",
        "<color=#0000FFFF>Uncommon</color> <color=#FF0000FF>Impostor</color>",
        "<color=#FF0000FF>Impostor</color>",

        "<color=#FF0000FF>Impostor</color> <color=#0000FFFF>Concealing</color>",
        "<color=#FF0000FF>Escapist</color>",
        "<color=#FF0000FF>Grenadier</color>",
        "<color=#FF0000FF>Morphling</color>",
        "<color=#FF0000FF>Swooper</color>",
        "<color=#FF0000FF>Venerer</color>",

        "<color=#FF0000FF>Impostor</color> <color=#0000FFFF>Killing</color>",
        "<color=#FF0000FF>Bomber</color>",
        "<color=#FF0000FF>Poisoner</color>",
        "<color=#FF0000FF>Sniper</color>",
        "<color=#FF0000FF>Warlock</color>",

        "<color=#FF0000FF>Impostor</color> <color=#0000FFFF>Support</color>",
        "<color=#FF0000FF>Blackmailer</color>",
        "<color=#FF0000FF>Janitor</color>",
        "<color=#FF0000FF>Miner</color>",
        "<color=#FF0000FF>Undertaker</color>",


        "<color=#0000FFFF>Random</color> <color=#FF0000FF>Killer</color>"
    };
        public static string[] BanEntriesData => new string[]
    {
        "None",


        "<color=#80FFFFFF>Crewmate</color>",

        "<color=#B34D99FF>Aurial</color>",
        "<color=#4D4DFFFF>Detective</color>",
        "<color=#8BFFDBFF>Inspector</color>",
        "<color=#00B3B3FF>Investigator</color>",
        "<color=#80DFDFFF>Lookout</color>",
        "<color=#4D99E6FF>Mystic</color>",
        "<color=#4B0082FF>Sage</color>",
        "<color=#FFCC80FF>Seer</color>",
        "<color=#D4AF37FF>Snitch</color>",
        "<color=#CCA3CCFF>Spy</color>",
        "<color=#009900FF>Tracker</color>",
        "<color=#A7D1B3FF>Trapper</color>",

        "<color=#AAAAAAFF>Deputy</color>",
        "<color=#29AB87FF>Hunter</color>",
        "<color=#FFFF00FF>Sheriff</color>",
        "<color=#B3B3E6FF>Vampire Hunter</color>",
        "<color=#998040FF>Veteran</color>",
        "<color=#FFFF99FF>Vigilante</color>",

        "<color=#660000FF>Altruist</color>",
        "<color=#36454FFF>Bodyguard</color>",
        "<color=#90EE90FF>Cleric</color>",
        "<color=#EFEFEFFF>Crusader</color>",
        "<color=#006600FF>Medic</color>",

        "<color=#FFA60AFF>Engineer</color>",
        "<color=#B3D94DFF>Imitator</color>",
        "<color=#A680FFFF>Medium</color>",
        "<color=#8B4513FF>Tavern Keeper</color>",
        "<color=#00EEFFFF>Transporter</color>",
        "<color=#002600FF>Undercover</color>",

        "<color=#704FA8FF>Mayor</color>",
        "<color=#9628C8FF>Monarch</color>",
        "<color=#BF00BFFF>Oracle</color>",
        "<color=#B38000FF>Prosecutor</color>",
        "<color=#66E666FF>Swapper</color>",


        "<color=#80B2FFFF>Amnesiac</color>",
        "<color=#8000FFFF>Cursed Soul</color>",
        "<color=#B3FFFFFF>Guardian Angel</color>",
        "<color=#FFE64DFF>Survivor</color>",

        "<color=#8C4005FF>Executioner</color>",
        "<color=#FFBFCCFF>Jester</color>",
        "<color=#C060FFFF>Witch</color>",

        "<color=#00FF80FF>Doomsayer</color>",
        "<color=#821252FF>Inquisitor</color>",
        "<color=#ECC23EFF>Pirate</color>",

        "<color=#FF4D00FF>Arsonist</color>",
        "<color=#8C004DFF>Juggernaut</color>",
        "<color=#1D4DFCFF>Serial Killer</color>",
        "<color=#00FF00FF>The Glitch</color>",
        "<color=#A86629FF>Werewolf</color>",

        "<color=#666666FF>Jackal</color>",
        "<color=#679556FF>Necromancer</color>",
        "<color=#262626FF>Vampire</color>",

        "<color=#FFC080FF>Baker</color>",
        "<color=#FF4F00FF>Berserker</color>",
        "<color=#E6FFB3FF>Plaguebearer</color>",
        "<color=#E000FFFF>Soul Collector</color>",


        "<color=#FF0000FF>Impostor</color>",

        "<color=#FF0000FF>Escapist</color>",
        "<color=#FF0000FF>Grenadier</color>",
        "<color=#FF0000FF>Morphling</color>",
        "<color=#FF0000FF>Swooper</color>",
        "<color=#FF0000FF>Venerer</color>",

        "<color=#FF0000FF>Bomber</color>",
        "<color=#FF0000FF>Poisoner</color>",
        "<color=#FF0000FF>Sniper</color>",
        "<color=#FF0000FF>Warlock</color>",

        "<color=#FF0000FF>Blackmailer</color>",
        "<color=#FF0000FF>Janitor</color>",
        "<color=#FF0000FF>Miner</color>",
        "<color=#FF0000FF>Undertaker</color>"
    };

        public static Func<object, string> PercentFormat { get; } = value => $"{value:0}%";
        private static Func<object, string> CooldownFormat { get; } = value => $"{value:0.0#}s";
        private static Func<object, string> MultiplierFormat { get; } = value => $"{value:0.0#}x";
        private static Func<object, string> RoundsFormat { get; } = value => $"{value:0} Rounds";


        public static void GenerateAll()
        {
            var num = 0;

            Patches.ExportButton = new Export(num++);
            Patches.ImportButton = new Import(num++);

            CrewInvestigativeRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "Crewmate Investigative Roles");
            AurialOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#B34D99FF>Aurial</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DetectiveOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#4D4DFFFF>Detective</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            HaunterOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#D3D3D3FF>Haunter</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            InspectorOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#8BFFDBFF>Inspector</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            InvestigatorOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#00B3B3FF>Investigator</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            LookoutOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#80DFDFFF>Lookout</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MysticOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#4D99E6FF>Mystic</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SageOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#4B0082FF>Sage</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SeerOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#FFCC80FF>Seer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SnitchOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#D4AF37FF>Snitch</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SpyOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#CCA3CCFF>Spy</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TrackerOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#009900FF>Tracker</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TrapperOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#A7D1B3FF>Trapper</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            CrewKillingRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "Crewmate Killing Roles");
            DeputyOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#AAAAAAFF>Deputy</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            HunterOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#29AB87FF>Hunter</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SheriffOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#FFFF00FF>Sheriff</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VampireHunterOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#B3B3E6FF>Vampire Hunter</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VeteranOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#998040FF>Veteran</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VigilanteOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#FFFF99FF>Vigilante</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            CrewProtectiveRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "Crewmate Protective Roles");
            AltruistOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#660000FF>Altruist</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            BodyguardOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#36454FFF>Bodyguard</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ClericOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#90EE90FF>Cleric</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            CrusaderOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#EFEFEFFF>Crusader</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MedicOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#006600FF>Medic</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            CrewSupportRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "Crewmate Support Roles");
            EngineerOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#FFA60AFF>Engineer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ImitatorOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#B3D94DFF>Imitator</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MediumOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#A680FFFF>Medium</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TavernKeeperOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#8B4513FF>Tavern Keeper</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TransporterOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#00EEFFFF>Transporter</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            UndercoverOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#002600FF>Undercover</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            CrewPowerRoles = new CustomHeaderOption(num++, MultiMenu.crewmate, "Crewmate Power Roles");
            MayorOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#704FA8FF>Mayor</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MonarchOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#9628C8FF>Monarch</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            OracleOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#BF00BFFF>Oracle</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            ProsecutorOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#B38000FF>Prosecutor</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SwapperOn = new CustomNumberOption(num++, MultiMenu.crewmate, "<color=#66E666FF>Swapper</color>", 0f, 0f, 100f, 10f,
                PercentFormat);


            NeutralBenignRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "Neutral Benign Roles");
            AmnesiacOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#80B2FFFF>Amnesiac</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            CursedSoulOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#8000FFFF>Cursed Soul</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GuardianAngelOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#B3FFFFFF>Guardian Angel</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SurvivorOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#FFE64DFF>Survivor</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            NeutralEvilRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "Neutral Evil Roles");
            ExecutionerOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#8C4005FF>Executioner</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            JesterOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#FFBFCCFF>Jester</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            PhantomOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#662962FF>Phantom</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            WitchOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#C060FFFF>Witch</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            NeutralChaosRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "Neutral Chaos Roles");
            DoomsayerOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#00FF80FF>Doomsayer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            InquisitorOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#821252FF>Inquisitor</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            PirateOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#ECC23EFF>Pirate</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            NeutralKillingRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "Neutral Killing Roles");
            ArsonistOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#FF4D00FF>Arsonist</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            JuggernautOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#8C004DFF>Juggernaut</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SerialKillerOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#1D4DFCFF>Serial Killer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GlitchOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#00FF00FF>The Glitch</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            WerewolfOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#A86629FF>Werewolf</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            NeutralKillingRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "Neutral Proselyte Roles");
            JackalOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#666666FF>Jackal</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            NecromancerOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#679556FF>Necromancer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VampireOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#262626FF>Vampire</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            NeutralApocalypseRoles = new CustomHeaderOption(num++, MultiMenu.neutral, "Neutral Apocalypse Roles");
            BakerOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#FFC080FF>Baker</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            BerserkerOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#FF4F00FF>Berserker</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            PlaguebearerOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#E6FFB3FF>Plaguebearer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SoulCollectorOn = new CustomNumberOption(num++, MultiMenu.neutral, "<color=#E000FFFF>Soul Collector</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            ImpostorConcealingRoles = new CustomHeaderOption(num++, MultiMenu.imposter, "Impostor Concealing Roles");
            EscapistOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Escapist</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GrenadierOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Grenadier</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MorphlingOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Morphling</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SwooperOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Swooper</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            VenererOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Venerer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            ImpostorKillingRoles = new CustomHeaderOption(num++, MultiMenu.imposter, "Impostor Killing Roles");
            BomberOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Bomber</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            PoisonerOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Poisoner</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SniperOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Sniper</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TraitorOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Traitor</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            WarlockOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Warlock</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            ImpostorSupportRoles = new CustomHeaderOption(num++, MultiMenu.imposter, "Impostor Support Roles");
            BlackmailerOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Blackmailer</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            JanitorOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Janitor</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MinerOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Miner</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            PoltergeistOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Poltergeist</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            UndertakerOn = new CustomNumberOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Undertaker</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            CrewmateModifiers = new CustomHeaderOption(num++, MultiMenu.modifiers, "Crewmate Modifiers");
            AftermathOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#A6FFA6FF>Aftermath</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            BaitOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#00B3B3FF>Bait</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DiseasedOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#808080FF>Diseased</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            FamousOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FFC000FF>Famous</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            FrostyOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#99FFFFFF>Frosty</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            MultitaskerOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF804DFF>Multitasker</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TorchOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FFFF99FF>Torch</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            GlobalModifiers = new CustomHeaderOption(num++, MultiMenu.modifiers, "Global Modifiers");
            ButtonBarryOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#E600FFFF>Button Barry</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DrunkOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#758000FF>Drunk</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            FlashOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF8080FF>Flash</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            GiantOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FFB34DFF>Giant</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            RadarOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF0080FF>Radar</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            SleuthOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#803333FF>Sleuth</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TiebreakerOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#99E699FF>Tiebreaker</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            ImpostorModifiers = new CustomHeaderOption(num++, MultiMenu.modifiers, "Impostor Modifiers");
            DisperserOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF0000FF>Disperser</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            DoubleShotOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF0000FF>Double Shot</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            TaskerOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF0000FF>Tasker</color>", 0f, 0f, 100f, 10f,
                PercentFormat);
            UnderdogOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF0000FF>Underdog</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            ObjectiveModifiers = new CustomHeaderOption(num++, MultiMenu.modifiers, "Objective Modifiers");
            ImpostorAgentOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF4040FF>Agent</color> (<color=#FF0000FF>Imp</color>)", 0f, 0f, 100f, 10f,
                PercentFormat);
            ApocalypseAgentOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#A0A0A0FF>Agent</color> (<color=#808080FF>Apoc</color>)", 0f, 0f, 100f, 10f,
                PercentFormat);
            LoversOn = new CustomNumberOption(num++, MultiMenu.modifiers, "<color=#FF66CCFF>Lovers</color>", 0f, 0f, 100f, 10f,
                PercentFormat);

            GameModeSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Game Mode Settings");
            GameMode = new CustomStringOption(num++, MultiMenu.main, "Game Mode", new[] { "Classic", "All Any", "Killing Only", "Cultist", "Teams", "Solo Killer", "Horseman", "Role List" });

            ClassicSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Classic Game Mode Settings");
            MinNeutralBenignRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Min Neutral Benign Roles", 1, 0, 4, 1);
            MaxNeutralBenignRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Max Neutral Benign Roles", 1, 0, 4, 1);
            MinNeutralEvilRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Min Neutral Evil Roles", 1, 0, 3, 1);
            MaxNeutralEvilRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Max Neutral Evil Roles", 1, 0, 3, 1);
            MinNeutralChaosRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Min Neutral Chaos Roles", 1, 0, 3, 1);
            MaxNeutralChaosRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Max Neutral Chaos Roles", 1, 0, 3, 1);
            MinNeutralKillingRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Min Neutral Killing Roles", 1, 0, 4, 1);
            MaxNeutralKillingRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Max Neutral Killing Roles", 1, 0, 4, 1);
            MinNeutralProselyteRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Min Neutral Proselyte Roles", 1, 0, 3, 1);
            MaxNeutralProselyteRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Max Neutral Proselyte Roles", 1, 0, 3, 1);
            MinNeutralApocalypseRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Min Neutral Apocalypse Roles", 1, 0, 4, 1);
            MaxNeutralApocalypseRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Max Neutral Apocalypse Roles", 1, 0, 4, 1);

            AllAnySettings =
                new CustomHeaderOption(num++, MultiMenu.main, "All Any Settings");
            RandomNumberImps = new CustomToggleOption(num++, MultiMenu.main, "Random Number Of Impostors", true);

            KillingOnlySettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Killing Only Settings");
            NeutralRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Neutral Roles", 1, 0, 5, 1);
            VeteranCount =
                new CustomNumberOption(num++, MultiMenu.main, "Veteran Count", 1, 0, 5, 1);
            VigilanteCount =
                new CustomNumberOption(num++, MultiMenu.main, "Vigilante Count", 1, 0, 5, 1);
            AddArsonist = new CustomToggleOption(num++, MultiMenu.main, "Add Arsonist", true);
            AddPlaguebearer = new CustomToggleOption(num++, MultiMenu.main, "Add Plaguebearer", true);

            CultistSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Cultist Settings");
            MayorCultistOn = new CustomNumberOption(num++, MultiMenu.main, "<color=#704FA8FF>Mayor</color> (Cultist Mode)", 100f, 0f, 100f, 10f,
                PercentFormat);
            SeerCultistOn = new CustomNumberOption(num++, MultiMenu.main, "<color=#FFCC80FF>Seer</color> (Cultist Mode)", 100f, 0f, 100f, 10f,
                PercentFormat);
            SheriffCultistOn = new CustomNumberOption(num++, MultiMenu.main, "<color=#FFFF00FF>Sheriff</color> (Cultist Mode)", 100f, 0f, 100f, 10f,
                PercentFormat);
            SurvivorCultistOn = new CustomNumberOption(num++, MultiMenu.main, "<color=#FFE64DFF>Survivor</color> (Cultist Mode)", 100f, 0f, 100f, 10f,
                PercentFormat);
            NumberOfSpecialRoles =
                new CustomNumberOption(num++, MultiMenu.main, "Number Of Special Roles", 4, 0, 4, 1);
            MaxChameleons =
                new CustomNumberOption(num++, MultiMenu.main, "Max Chameleons", 3, 0, 5, 1);
            MaxEngineers =
                new CustomNumberOption(num++, MultiMenu.main, "Max Engineers", 3, 0, 5, 1);
            MaxInvestigators =
                new CustomNumberOption(num++, MultiMenu.main, "Max Investigators", 3, 0, 5, 1);
            MaxMystics =
                new CustomNumberOption(num++, MultiMenu.main, "Max Mystics", 3, 0, 5, 1);
            MaxSnitches =
                new CustomNumberOption(num++, MultiMenu.main, "Max Snitches", 3, 0, 5, 1);
            MaxSpies =
                new CustomNumberOption(num++, MultiMenu.main, "Max Spies", 3, 0, 5, 1);
            MaxTransporters =
                new CustomNumberOption(num++, MultiMenu.main, "Max Transporters", 3, 0, 5, 1);
            MaxVigilantes =
                new CustomNumberOption(num++, MultiMenu.main, "Max Vigilantes", 3, 0, 5, 1);
            WhisperCooldown =
                new CustomNumberOption(num++, MultiMenu.main, "Initial Whisper Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            IncreasedCooldownPerWhisper =
                new CustomNumberOption(num++, MultiMenu.main, "Increased Cooldown Per Whisper", 5f, 0f, 15f, 0.5f, CooldownFormat);
            WhisperRadius =
                new CustomNumberOption(num++, MultiMenu.main, "Whisper Radius", 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
            ConversionPercentage = new CustomNumberOption(num++, MultiMenu.main, "Conversion Percentage", 25f, 0f, 100f, 5f,
                PercentFormat);
            DecreasedPercentagePerConversion = new CustomNumberOption(num++, MultiMenu.main, "Decreased Conversion Percentage Per Conversion", 5f, 0f, 15f, 1f,
                PercentFormat);
            ReviveCooldown =
                new CustomNumberOption(num++, MultiMenu.main, "Initial Revive Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            IncreasedCooldownPerRevive =
                new CustomNumberOption(num++, MultiMenu.main, "Increased Cooldown Per Revive", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MaxReveals = new CustomNumberOption(num++, MultiMenu.main, "Maximum Number Of Reveals", 5, 1, 15, 1);

            MapSettings = new CustomHeaderOption(num++, MultiMenu.main, "Map Settings");
            RandomMapEnabled = new CustomToggleOption(num++, MultiMenu.main, "Choose Random Map", false);
            RandomMapSkeld = new CustomNumberOption(num++, MultiMenu.main, "Skeld Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapMira = new CustomNumberOption(num++, MultiMenu.main, "Mira Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapPolus = new CustomNumberOption(num++, MultiMenu.main, "Polus Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapAirship = new CustomNumberOption(num++, MultiMenu.main, "Airship Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapFungle = new CustomNumberOption(num++, MultiMenu.main, "Fungle Chance", 0f, 0f, 100f, 10f, PercentFormat);
            RandomMapSubmerged = new CustomNumberOption(num++, MultiMenu.main, "Submerged Chance", 0f, 0f, 100f, 10f, PercentFormat);
            AutoAdjustSettings = new CustomToggleOption(num++, MultiMenu.main, "Auto Adjust Settings", false);
            SmallMapHalfVision = new CustomToggleOption(num++, MultiMenu.main, "Half Vision On Skeld/Mira HQ", false);
            SmallMapDecreasedCooldown =
                new CustomNumberOption(num++, MultiMenu.main, "Mira HQ Decreased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat);
            LargeMapIncreasedCooldown =
                new CustomNumberOption(num++, MultiMenu.main, "Airship/Submerged Increased Cooldowns", 0f, 0f, 15f, 2.5f, CooldownFormat);
            SmallMapIncreasedShortTasks =
                 new CustomNumberOption(num++, MultiMenu.main, "Skeld/Mira HQ Increased Short Tasks", 0, 0, 5, 1);
            SmallMapIncreasedLongTasks =
                 new CustomNumberOption(num++, MultiMenu.main, "Skeld/Mira HQ Increased Long Tasks", 0, 0, 3, 1);
            LargeMapDecreasedShortTasks =
                 new CustomNumberOption(num++, MultiMenu.main, "Airship/Submerged Decreased Short Tasks", 0, 0, 5, 1);
            LargeMapDecreasedLongTasks =
                 new CustomNumberOption(num++, MultiMenu.main, "Airship/Submerged Decreased Long Tasks", 0, 0, 3, 1);

            BetterPolusSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Better Polus Settings");
            VentImprovements = new CustomToggleOption(num++, MultiMenu.main, "Better Polus Vent Layout", false);
            VitalsLab = new CustomToggleOption(num++, MultiMenu.main, "Vitals Moved To Lab", false);
            ColdTempDeathValley = new CustomToggleOption(num++, MultiMenu.main, "Cold Temp Moved To Death Valley", false);
            WifiChartCourseSwap =
                new CustomToggleOption(num++, MultiMenu.main, "Reboot Wifi And Chart Course Swapped", false);

            CustomGameSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Custom Game Settings");
            ColourblindComms = new CustomToggleOption(num++, MultiMenu.main, "Camouflaged Comms", false);
            ImpostorSeeRoles = new CustomToggleOption(num++, MultiMenu.main, "Impostors Can See The Roles Of Their Team", false);
            DeadSeeRoles =
                new CustomToggleOption(num++, MultiMenu.main, "Dead Can See Everyone's Roles/Votes", false);
            InitialCooldowns =
                new CustomNumberOption(num++, MultiMenu.main, "Game Start Cooldowns", 10f, 0f, 30f, 2.5f, CooldownFormat);
            ParallelMedScans = new CustomToggleOption(num++, MultiMenu.main, "Parallel Medbay Scans", false);
            SkipButtonDisable = new CustomStringOption(num++, MultiMenu.main, "Disable Meeting Skip Button", new[] { "No", "Emergency", "Always" });
            FirstDeathShield = new CustomToggleOption(num++, MultiMenu.main, "First Death Shield Next Game", false);
            NeutralEvilWinEndsGame = new CustomToggleOption(num++, MultiMenu.main, "Neutral Evil/Chaos Win Ends Game", true);
            GhostsDoTasks = new CustomToggleOption(num++, MultiMenu.main, "Ghosts Do Tasks", true);
            OvertakeWin = new CustomStringOption(num++, MultiMenu.main, "Overtake Win", new[] { "On", "Without CK", "Off" });
            NotificationDuration =
                new CustomNumberOption(num++, MultiMenu.main, "Role Notification Duration", 2.5f, 0f, 10f, 0.25f, CooldownFormat);
            ShowImpostorsRemaining = new CustomToggleOption(num++, MultiMenu.main, "Show Impostors Remaining", false);
            ShowApocalypseRemaining = new CustomToggleOption(num++, MultiMenu.main, "Show Apocalypse Remaining", false);
            ShowUndeadRemaining = new CustomToggleOption(num++, MultiMenu.main, "Show Undead Remaining", false);
            ShowKillingRemaining = new CustomToggleOption(num++, MultiMenu.main, "Show Neutral Killing Remaining", false);
            ShowProselyteRemaining = new CustomToggleOption(num++, MultiMenu.main, "Show Neutral Proselyte Remaining", false);
            ImpostorsChat = new CustomStringOption(num++, MultiMenu.main, "Impostors Chat", new[] { "Off", "Rounds", "Meeting", "Both" });
            ApocalypseChat = new CustomStringOption(num++, MultiMenu.main, "Apocalypse Chat", new[] { "Off", "Rounds", "Meeting", "Both" });

            TaskTrackingSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Task Tracking Settings");
            SeeTasksDuringRound = new CustomToggleOption(num++, MultiMenu.main, "See Tasks During Round", false);
            SeeTasksDuringMeeting = new CustomToggleOption(num++, MultiMenu.main, "See Tasks During Meetings", false);
            SeeTasksWhenDead = new CustomToggleOption(num++, MultiMenu.main, "See Tasks When Dead", true);

            TeamsSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Teams Settings");
            TeamsKCd =
                new CustomNumberOption(num++, MultiMenu.main, "Kill Cooldown", 10f, 0f, 60f, 1f, CooldownFormat);
            TeamsAmount =
                new CustomNumberOption(num++, MultiMenu.main, "Teams Amount", 2, 2, 4, 1);
            TeamsVent = new CustomToggleOption(num++, MultiMenu.main, "Allow Venting");

            SoloKillerSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Solo Killer Settings");
            SoloKillerKCd =
                new CustomNumberOption(num++, MultiMenu.main, "Solo Killer Kill Cooldown", 10f, 0f, 60f, 1f, CooldownFormat);
            SoloKillerVent = new CustomToggleOption(num++, MultiMenu.main, "Solo Killer Vent");
            SoloKillerPlayer =
                new CustomStringOption(num++, MultiMenu.main, "Solo Killer", new[] { "Random", "Host", "Player 0", "Player 1", "Player 2", "Player 3", "Player 4", "Player 5", "Player 6", "Player 7", "Player 8", "Player 9", "Player 10", "Player 11", "Player 12", "Player 13", "Player 14" });

            RoleListSettings =
                new CustomHeaderOption(num++, MultiMenu.main, "Role List Settings");
            RoleEntries = new Dictionary<int, CustomStringOption>();
            for (int i = 0; i < 15; i++)
            {
                RoleEntries.Add(i, new CustomStringOption(num++, MultiMenu.main, "Role Entry " + (i + 1), RoleEntriesData));
            }
            RoleListSpacing0 =
                new CustomHeaderOption(num++, MultiMenu.main, "");
            BanEntries = new Dictionary<int, CustomStringOption>();
            for (int i = 0; i < 15; i++)
            {
                BanEntries.Add(i, new CustomStringOption(num++, MultiMenu.main, "Ban Entry " + (i + 1), BanEntriesData));
            }
            RoleListSpacing1 =
                new CustomHeaderOption(num++, MultiMenu.main, "");
            MaxImps = new CustomNumberOption(num++, MultiMenu.main, "Maximum Number Of Impostors", 4, 0, 15, 1);
            AllUnique = new CustomToggleOption(num++, MultiMenu.main, "All Roles Are Unique", false);

            Assassin = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Assassin Ability</color>");
            NumberOfImpostorAssassins = new CustomNumberOption(num++, MultiMenu.imposter, "Number Of Impostor Assassins", 1, 0, 15, 1);
            NumberOfNeutralAssassins = new CustomNumberOption(num++, MultiMenu.imposter, "Number Of Neutral Assassins", 1, 0, 15, 1);
            AmneTurnImpAssassin = new CustomToggleOption(num++, MultiMenu.imposter, "Amnesiac Turned Impostor Gets Ability", false);
            AmneTurnNeutAssassin = new CustomToggleOption(num++, MultiMenu.imposter, "Amnesiac Turned Neutral Killing Gets Ability", false);
            TraitorCanAssassin = new CustomToggleOption(num++, MultiMenu.imposter, "Traitor Gets Ability", false);
            AssassinKills = new CustomNumberOption(num++, MultiMenu.imposter, "Number Of Assassin Kills", 1, 1, 15, 1);
            AssassinMultiKill = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Kill More Than Once Per Meeting", false);
            AssassinCrewmateGuess = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess \"Crewmate\"", false);
            AssassinGuessCrewInvestigative = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Crew Investigative Roles", false);
            AssassinGuessNeutralBenign = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Neutral Benign Roles", false);
            AssassinGuessNeutralEvil = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Neutral Evil Roles", false);
            AssassinGuessNeutralChaos = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Neutral Chaos Roles", false);
            AssassinGuessNeutralKilling = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Neutral Killing Roles", false);
            AssassinGuessNeutralProselyte = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Neutral Proselyte Roles", false);
            AssassinGuessNeutralApocalypse = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Neutral Apocalypse Roles", false);
            AssassinGuessImpostors = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Impostor Roles", false);
            AssassinGuessModifiers = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Crewmate Modifiers", false);
            AssassinGuessLovers = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess Lovers", false);
            AssassinateAfterVoting = new CustomToggleOption(num++, MultiMenu.imposter, "Assassin Can Guess After Voting", false);

            Aurial =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#B34D99FF>Aurial</color>");
            RadiateRange =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Radiate Range", 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
            RadiateCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Radiate Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RadiateInvis =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Radiate See Delay", 10f, 0f, 15f, 1f, CooldownFormat);
            RadiateCount =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Radiate Uses To See", 3, 1, 5, 1);
            RadiateSucceedChance =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Radiate Succeed Chance", 100f, 0f, 100f, 10f, PercentFormat);
            AurialVisionMultiplier =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Aurial Vision Multiplier", 1.25f, 1f, 1.5f, 0.05f, MultiplierFormat);

            Detective =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#4D4DFFFF>Detective</color>");
            ExamineCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Examine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            DetectiveReportOn = new CustomToggleOption(num++, MultiMenu.crewmate, "Show Detective Reports", true);
            DetectiveRoleDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Time Where Detective Will Have Role", 15f, 0f, 60f, 2.5f,
                    CooldownFormat);
            DetectiveFactionDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Time Where Detective Will Have Faction", 30f, 0f, 60f, 2.5f,
                    CooldownFormat);
            CanDetectLastKiller = new CustomToggleOption(num++, MultiMenu.crewmate, "Can Detect Last Killer", false);

            Haunter =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#d3d3d3FF>Haunter</color>");
            HaunterTasksRemainingClicked =
                 new CustomNumberOption(num++, MultiMenu.crewmate, "Tasks Remaining When Haunter Can Be Clicked", 5, 1, 15, 1);
            HaunterTasksRemainingAlert =
                 new CustomNumberOption(num++, MultiMenu.crewmate, "Tasks Remaining When Alert Is Sent", 1, 1, 5, 1);
            HaunterRevealsNeutrals = new CustomToggleOption(num++, MultiMenu.crewmate, "Haunter Reveals Neutral Roles", false);
            HaunterCanBeClickedBy = new CustomStringOption(num++, MultiMenu.crewmate, "Who Can Click Haunter", new[] { "All", "Non-Crew", "Imps Only" });

            Inspector =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#8BFFDBFF>Inspector</color>");
            InspectCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Inspect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            BloodDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "How Long Blood Stays", 30f, 10f, 120f, 2.5f, CooldownFormat);

            Investigator =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#00B3B3FF>Investigator</color>");
            FootprintSize = new CustomNumberOption(num++, MultiMenu.crewmate, "Footprint Size", 4f, 1f, 10f, 1f);
            FootprintInterval =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Footprint Interval", 0.1f, 0.05f, 1f, 0.05f, CooldownFormat);
            FootprintDuration = new CustomNumberOption(num++, MultiMenu.crewmate, "Footprint Duration", 10f, 1f, 15f, 0.5f, CooldownFormat);
            AnonymousFootPrint = new CustomToggleOption(num++, MultiMenu.crewmate, "Anonymous Footprint", false);
            VentFootprintVisible = new CustomToggleOption(num++, MultiMenu.crewmate, "Footprint Vent Visible", false);
            InvestigateCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Investigate Cooldown", 10f, 2.5f, 60f, 2.5f, CooldownFormat);
            MaxInvestigates =
                 new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Investigates Per Round", 3, 0, 15, 1);

            Lookout =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#80DFDFFF>Lookout</color>");
            WatchCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Watch Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            WatchDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Watch Duration", 10f, 5f, 20f, 2.5f, CooldownFormat);
            WatchVisionMultiplier =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Vision Multiplier On Watch", 1.5f, 1f, 3f, 0.25f, MultiplierFormat);

            Mystic =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#4D99E6FF>Mystic</color>");
            MysticArrowDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Dead Body Arrow Duration", 0.1f, 0f, 1f, 0.05f, CooldownFormat);
            AllowVision =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Allow Vision");
            VisionCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Vision Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Sage =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#4B0082FF>Sage</color>");
            CompareCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Compare Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            CompareAccuracy =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Compare Accuracy", 90f, 0f, 100f, 10f, PercentFormat);

            Seer =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#FFCC80FF>Seer</color>");
            SeerCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Seer Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            CrewKillingRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Crewmate Killing Roles Are Red", false);
            NeutBenignRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Benign Roles Are Red", false);
            NeutEvilRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Evil Roles Are Red", false);
            NeutChaosRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Chaos Roles Are Red", false);
            NeutKillingRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Killing Roles Are Red", true);
            NeutProselyteRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Proselyte Roles Are Red", true);
            NeutApocalypseRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Apocalypse Roles Are Red", true);
            AgentRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Agents Are Red", true);
            UndeadRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undead Are Red", true);
            RecruitRed =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Recruits Are Red", true);
            TraitorColourSwap =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Traitor Does Not Swap Colours", false);

            Snitch = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#D4AF37FF>Snitch</color>");
            SnitchSeesNeutrals = new CustomToggleOption(num++, MultiMenu.crewmate, "Snitch Sees Neutral Roles", false);
            SnitchTasksRemaining =
                 new CustomNumberOption(num++, MultiMenu.crewmate, "Tasks Remaining When Revealed", 1, 1, 5, 1);
            SnitchSeesImpInMeeting = new CustomToggleOption(num++, MultiMenu.crewmate, "Snitch Sees Impostors In Meetings", true);
            SnitchSeesTraitor = new CustomToggleOption(num++, MultiMenu.crewmate, "Snitch Sees Traitor", true);

            Spy =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#CCA3CCFF>Spy</color>");
            WhoSeesDead = new CustomStringOption(num++, MultiMenu.crewmate, "Who Sees Dead Bodies On Admin",
                new[] { "Nobody", "Spy", "Everyone But Spy", "Everyone" });
            BugsPerGame =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Max Bugs Per Game", 3, 0, 15, 1);
            BugCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Bug Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Tracker =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#009900FF>Tracker</color>");
            UpdateInterval =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Arrow Update Interval", 5f, 0.5f, 15f, 0.5f, CooldownFormat);
            TrackCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Track Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ResetOnNewRound = new CustomToggleOption(num++, MultiMenu.crewmate, "Tracker Arrows Reset After Each Round", false);
            MaxTracks = new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Number Of Tracks Per Round", 5, 1, 15, 1);

            Trapper =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#A7D1B3FF>Trapper</color>");
            MinAmountOfTimeInTrap =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Min Amount Of Time In Trap To Register", 1f, 0f, 15f, 0.5f, CooldownFormat);
            TrapCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Trap Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            TrapsRemoveOnNewRound =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Traps Removed After Each Round", true);
            MaxTraps =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Number Of Traps Per Game", 5, 1, 15, 1);
            TrapSize =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Trap Size", 0.25f, 0.05f, 1f, 0.05f, MultiplierFormat);
            MinAmountOfPlayersInTrap =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Minimum Number Of Roles Required To Trigger Trap", 3, 1, 5, 1);

            Deputy = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#AAAAAAFF>Deputy</color>");
            RevealDeputy = new CustomToggleOption(num++, MultiMenu.crewmate, "Reveal Deputy While Shooting");
            MisfireKillsDeputy = new CustomToggleOption(num++, MultiMenu.crewmate, "Misfire Causes Deputy Death");
            DeputyAimCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Aim Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MaxDeputyTargets = new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Alive Aimed Players", 3, 1, 15, 1);

            Hunter =
               new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#29AB87FF>Hunter</color>");
            HunterKillCd =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Hunter Kill Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            HunterStalkCd =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Hunter Stalk Cooldown", 10f, 0f, 40f, 2.5f, CooldownFormat);
            HunterStalkDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Hunter Stalk Duration", 25f, 5f, 40f, 1f, CooldownFormat);
            HunterStalkUses =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Stalk Uses", 5, 1, 15, 1);
            HunterBodyReport =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Hunter Can Report Who They've Killed");

            Sheriff =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#FFFF00FF>Sheriff</color>");
            SheriffKillOther =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Miskill Kills Crewmate", false);
            SheriffKillsAgent =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Agent", false);
            SheriffKillsExecutioner =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Executioner", false);
            SheriffKillsJester =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Jester", false);
            SheriffKillsWitch =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Witch", false);
            SheriffKillsDoomsayer =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Doomsayer", false);
            SheriffKillsPirate =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Pirate", false);
            SheriffKillsInquisitor =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Inquisitor", false);
            SheriffKillsArsonist =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Arsonist", false);
            SheriffKillsGlitch =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills The Glitch", false);
            SheriffKillsJuggernaut =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Juggernaut", false);
            SheriffKillsWerewolf =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Werewolf", false);
            SheriffKillsSerialKiller =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Serial Killer", false);
            SheriffKillsVampire =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Vampire", false);
            SheriffKillsNecromancer =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Necromancer", false);
            SheriffKillsUndead =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Undead", false);
            SheriffKillsJackal =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Jackal", false);
            SheriffKillsRecruits =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Recruits", false);
            SheriffKillsBaker =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Baker", false);
            SheriffKillsBerserker =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Berserker", false);
            SheriffKillsPlaguebearer =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Plaguebearer", false);
            SheriffKillsSoulCollector =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Kills Soul Collector", false);
            SheriffKillCd =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Sheriff Kill Cooldown", 25f, 10f, 40f, 2.5f, CooldownFormat);
            SheriffBodyReport = new CustomToggleOption(num++, MultiMenu.crewmate, "Sheriff Can Report Who They've Killed");

            VampireHunter =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#B3B3E6FF>Vampire Hunter</color>");
            StakeCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Stake Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MaxFailedStakesPerGame = new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Failed Stakes Per Game", 5, 1, 15, 1);
            CanStakeRoundOne = new CustomToggleOption(num++, MultiMenu.crewmate, "Can Stake Round One", false);
            SelfKillAfterFinalStake = new CustomToggleOption(num++, MultiMenu.crewmate, "Self Kill On Failure To Kill A Vamp With All Stakes", false);
            BecomeOnVampDeaths =
                new CustomStringOption(num++, MultiMenu.crewmate, "What Vampire Hunter Becomes On All Vampire Deaths", new[] { "Crewmate", "Sheriff", "Veteran", "Vigilante", "Hunter" });

            Veteran =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#998040FF>Veteran</color>");
            KilledOnAlert =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Can Be Killed On Alert", false);
            AlertCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Alert Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            AlertDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Alert Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            MaxAlerts = new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Number Of Alerts", 5, 1, 15, 1);

            Vigilante = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#FFFF99FF>Vigilante</color>");
            VigilanteKills = new CustomNumberOption(num++, MultiMenu.crewmate, "Number Of Vigilante Kills", 1, 1, 15, 1);
            VigilanteMultiKill = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Kill More Than Once Per Meeting", false);
            VigilanteGuessNeutralBenign = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Guess Neutral Benign Roles", false);
            VigilanteGuessNeutralEvil = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Guess Neutral Evil Roles", false);
            VigilanteGuessNeutralChaos = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Guess Neutral Chaos Roles", false);
            VigilanteGuessNeutralKilling = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Guess Neutral Killing Roles", false);
            VigilanteGuessNeutralProselyte = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Guess Neutral Proselyte Roles", false);
            VigilanteGuessNeutralApocalypse = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Guess Neutral Apocalypse Roles", false);
            VigilanteGuessLovers = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Guess Lovers", false);
            VigilanteAfterVoting = new CustomToggleOption(num++, MultiMenu.crewmate, "Vigilante Can Guess After Voting", false);

            Altruist = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#660000FF>Altruist</color>");
            ReviveDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Altruist Revive Duration", 10f, 1f, 15f, 1f, CooldownFormat);
            AltruistTargetBody =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Target's Body Disappears On Beginning Of Revive", false);

            Bodyguard =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#36454FFF>Bodyguard</color>");
            GuardCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Guard Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);

            Cleric =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#90EE90FF>Cleric</color>");
            BarrierCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Barrier Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            BarrierCooldownReset =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Barrier Kill Cooldown Reset", 10f, 2.5f, 60f, 0.5f, CooldownFormat);

            Crusader =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#EFEFEFFF>Crusader</color>");
            FortifyCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Fortify Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MaxFortify =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Uses Of Fortify", 3, 1, 15, 1);

            Medic =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#006600FF>Medic</color>");
            ShowShielded =
                new CustomStringOption(num++, MultiMenu.crewmate, "Show Shielded Player",
                    new[] { "Self", "Medic", "Self+Medic", "Everyone" });
            WhoGetsNotification =
                new CustomStringOption(num++, MultiMenu.crewmate, "Who Gets Murder Attempt Indicator",
                    new[] { "Medic", "Shielded", "Everyone", "Nobody" });
            ShieldBreaks = new CustomToggleOption(num++, MultiMenu.crewmate, "Shield Breaks On Murder Attempt", false);
            MedicReportSwitch = new CustomToggleOption(num++, MultiMenu.crewmate, "Show Medic Reports");
            MedicReportNameDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Time Where Medic Will Have Name", 0f, 0f, 60f, 2.5f,
                    CooldownFormat);
            MedicReportColorDuration =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Time Where Medic Will Have Color Type", 15f, 0f, 60f, 2.5f,
                    CooldownFormat);

            Engineer =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#FFA60AFF>Engineer</color>");
            MaxFixes =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Number Of Fixes", 5, 1, 15, 1);

            Medium =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#A680FFFF>Medium</color>");
            MediateCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Mediate Cooldown", 10f, 1f, 15f, 1f, CooldownFormat);
            ShowMediatePlayer =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Reveal Appearance Of Mediate Target", true);
            ShowMediumToDead =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Reveal The Medium To The Mediate Target", true);
            DeadRevealed =
                new CustomStringOption(num++, MultiMenu.crewmate, "Who Is Revealed With Mediate", new[] { "Oldest Dead", "Newest Dead", "All Dead" });

            TavernKeeper =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#8B4513FF>Tavern Keeper</color>");
            DrinkCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Drink Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            DrinksPerRound =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Max Drinks Per Round", 2, 1, 5, 1);

            Transporter =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#00EEFFFF>Transporter</color>");
            TransportCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Transport Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            TransportMaxUses =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Number Of Transports", 5, 1, 15, 1);
            TransporterVitals =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Transporter Can Use Vitals", false);

            Undercover = new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#002600FF>Undercover</color>");
            UndercoverKillEachother =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Impostors Can Kill Eachother When Undercover Exists");
            UndercoverVent =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Jump To Vents");
            UndercoverEscapist =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Escapist");
            UndercoverGrenadier =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Grenadier");
            UndercoverMorphling =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Morphling");
            UndercoverSwooper =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Swooper");
            UndercoverVenerer =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Venerer");
            UndercoverBomber =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Bomber");
            UndercoverWarlock =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Warlock");
            UndercoverPoisoner =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Poisoner");
            UndercoverSniper =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Sniper");
            UndercoverBlackmailer =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Blackmailer");
            UndercoverJanitor =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Janitor");
            UndercoverMiner =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Miner");
            UndercoverUndertaker =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Undertaker");
            UndercoverPlaguebearer =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Plaguebearer", false);
            UndercoverBaker =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Baker", false);
            UndercoverBerserker =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Berserker", false);
            UndercoverSoulCollector =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Undercover Can Be Soul Collector", false);

            Monarch =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#9628C8FF>Monarch</color>");
            KnightCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Knight Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MaxKnights =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Number Of Knights", 2, 1, 5, 1);
            KnightFirstRound =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Allow Knighting Round One", false);
            InstantKnight =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Show Knight Immediately", false);

            Oracle =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#BF00BFFF>Oracle</color>");
            ConfessCooldown =
                new CustomNumberOption(num++, MultiMenu.crewmate, "Confess Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RevealAccuracy = new CustomNumberOption(num++, MultiMenu.crewmate, "Reveal Accuracy", 80f, 0f, 100f, 10f,
                PercentFormat);
            NeutralBenignShowsEvil =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Benign Roles Show Evil", false);
            NeutralEvilShowsEvil =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Evil Roles Show Evil", false);
            NeutralChaosShowsEvil =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Chaos Roles Show Evil", false);
            NeutralKillingShowsEvil =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Killing Roles Show Evil", true);
            NeutralProselyteShowsEvil =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Proselyte Roles Show Evil", true);
            NeutralApocalypseShowsEvil =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Neutral Apocalypse Roles Show Evil", true);

            Prosecutor =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#B38000FF>Prosecutor</color>");
            ProsDiesOnIncorrectPros =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Prosecutor Dies When They Exile A Crewmate", false);
            MaxProsecutions = new CustomNumberOption(num++, MultiMenu.crewmate, "Maximum Amount Of Prosecutions", 1, 1, 5, 1);
            RevealProsecutor = new CustomToggleOption(num++, MultiMenu.crewmate, "Reveal Prosecutor While Prosecuting");

            Swapper =
                new CustomHeaderOption(num++, MultiMenu.crewmate, "<color=#66E666FF>Swapper</color>");
            SwapperButton =
                new CustomToggleOption(num++, MultiMenu.crewmate, "Swapper Can Button", true);

            Amnesiac = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#80B2FFFF>Amnesiac</color>");
            RememberArrows =
                new CustomToggleOption(num++, MultiMenu.neutral, "Amnesiac Gets Arrows Pointing To Dead Bodies", false);
            RememberArrowDelay =
                new CustomNumberOption(num++, MultiMenu.neutral, "Time After Death Arrow Appears", 5f, 0f, 15f, 1f, CooldownFormat);

            CursedSoul =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#8000FFFF>Cursed Soul</color>");
            SoulSwapCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Soul Swap Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            SoulSwapAccuracy = new CustomNumberOption(num++, MultiMenu.neutral, "Probability Of Soul Swapping With Random Other Player", 50f, 0f, 100f, 10f,
                PercentFormat);
            SoulSwapImp =
                new CustomToggleOption(num++, MultiMenu.neutral, "Cursed Soul Can Soul Swap With Impostor", false);
            SwappedBecomes = new CustomStringOption(num++, MultiMenu.neutral, "Swapped Player Becomes",
                new[] { "Cursed Soul", "Crew", "Amnesiac", "Survivor", "Jester", "Default" });

            GuardianAngel =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#B3FFFFFF>Guardian Angel</color>");
            ProtectCd =
                new CustomNumberOption(num++, MultiMenu.neutral, "Protect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ProtectDuration =
                new CustomNumberOption(num++, MultiMenu.neutral, "Protect Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            ProtectKCReset =
                new CustomNumberOption(num++, MultiMenu.neutral, "Kill Cooldown Reset When Protected", 2.5f, 0f, 15f, 0.5f, CooldownFormat);
            MaxProtects =
                new CustomNumberOption(num++, MultiMenu.neutral, "Maximum Number Of Protects", 5, 1, 15, 1);
            ShowProtect =
                new CustomStringOption(num++, MultiMenu.neutral, "Show Protected Player",
                    new[] { "Self", "Guardian Angel", "Self+GA", "Everyone" });
            GaOnTargetDeath = new CustomStringOption(num++, MultiMenu.neutral, "GA Becomes On Target Dead",
                new[] { "Crew", "Amnesiac", "Survivor", "Jester", "Cursed Soul" });
            GATargetKnows =
                new CustomToggleOption(num++, MultiMenu.neutral, "Target Knows GA Exists", false);
            GAKnowsTargetRole =
                new CustomToggleOption(num++, MultiMenu.neutral, "GA Knows Targets Role", false);
            EvilTargetPercent = new CustomNumberOption(num++, MultiMenu.neutral, "Odds Of Target Being Evil", 20f, 0f, 100f, 10f,
                PercentFormat);

            Survivor =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#FFE64DFF>Survivor</color>");
            VestCd =
                new CustomNumberOption(num++, MultiMenu.neutral, "Vest Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            VestDuration =
                new CustomNumberOption(num++, MultiMenu.neutral, "Vest Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            VestKCReset =
                new CustomNumberOption(num++, MultiMenu.neutral, "Kill Cooldown Reset On Attack", 2.5f, 0f, 15f, 0.5f, CooldownFormat);
            MaxVests =
                new CustomNumberOption(num++, MultiMenu.neutral, "Maximum Number Of Vests", 5, 1, 15, 1);

            Executioner =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#8C4005FF>Executioner</color>");
            OnTargetDead = new CustomStringOption(num++, MultiMenu.neutral, "Executioner Becomes On Target Dead",
                new[] { "Crew", "Amnesiac", "Survivor", "Jester", "Cursed Soul" });
            ExecutionerButton =
                new CustomToggleOption(num++, MultiMenu.neutral, "Executioner Can Button", true);
            ExecutionerTorment =
                new CustomToggleOption(num++, MultiMenu.neutral, "Executioner Torments Player On Victory", true);

            Jester =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#FFBFCCFF>Jester</color>");
            JesterButton =
                new CustomToggleOption(num++, MultiMenu.neutral, "Jester Can Button", true);
            JesterVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Jester Can Hide In Vents", false);
            JesterImpVision =
                new CustomToggleOption(num++, MultiMenu.neutral, "Jester Has Impostor Vision", false);
            JesterHaunt =
                new CustomToggleOption(num++, MultiMenu.neutral, "Jester Haunts Player On Victory", true);

            Phantom =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#662962FF>Phantom</color>");
            PhantomTasksRemaining =
                 new CustomNumberOption(num++, MultiMenu.neutral, "Tasks Remaining When Phantom Can Be Clicked", 5, 1, 15, 1);
            PhantomSpook =
                new CustomToggleOption(num++, MultiMenu.neutral, "Phantom Spooks Player On Victory", true);

            Witch = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#C060FFFF>Witch</color>");
            ControlCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Control Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            OrderCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Order Cooldown", 10f, 5f, 60f, 2.5f, CooldownFormat);
            WitchLearns = new CustomStringOption(num++, MultiMenu.neutral, "Witch Learns About Controled",
                new[] { "Nothing", "Faction", "Aligment", "Role" });

            Doomsayer = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#00FF80FF>Doomsayer</color>");
            ObserveCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Observe Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            DoomsayerGuessCrewInvestigative = new CustomToggleOption(num++, MultiMenu.neutral, "Doomsayer Can Guess Crew Investigative Roles", true);
            DoomsayerGuessNeutralBenign = new CustomToggleOption(num++, MultiMenu.neutral, "Doomsayer Can Guess Neutral Benign Roles", false);
            DoomsayerGuessNeutralEvil = new CustomToggleOption(num++, MultiMenu.neutral, "Doomsayer Can Guess Neutral Evil Roles", false);
            DoomsayerGuessNeutralChaos = new CustomToggleOption(num++, MultiMenu.neutral, "Doomsayer Can Guess Neutral Chaos Roles", false);
            DoomsayerGuessNeutralKilling = new CustomToggleOption(num++, MultiMenu.neutral, "Doomsayer Can Guess Neutral Killing Roles", false);
            DoomsayerGuessNeutralProselyte = new CustomToggleOption(num++, MultiMenu.neutral, "Doomsayer Can Guess Neutral Proselyte Roles", false);
            DoomsayerGuessNeutralApocalypse = new CustomToggleOption(num++, MultiMenu.neutral, "Doomsayer Can Guess Neutral Apocalypse Roles", false);
            DoomsayerGuessImpostors = new CustomToggleOption(num++, MultiMenu.neutral, "Doomsayer Can Guess Impostor Roles", false);
            DoomsayerAfterVoting = new CustomToggleOption(num++, MultiMenu.neutral, "Doomsayer Can Guess After Voting", false);
            DoomsayerGuessesToWin = new CustomNumberOption(num++, MultiMenu.neutral, "Number Of Doomsayer Kills To Win", 3, 1, 5, 1);
            DoomsayerCantObserve = new CustomToggleOption(num++, MultiMenu.neutral, "(Experienced) Doomsayer can't observe", false);

            Inquisitor =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#821252FF>Inquisitor</color>");
            InquisitorCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Inquisitor Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            NumberOfHeretics =
                 new CustomNumberOption(num++, MultiMenu.neutral, "Number Of Heretics", 3, 1, 7, 1);
            HereticsInfo =
                new CustomStringOption(num++, MultiMenu.neutral, "What Inquisitor Knows About Heretics", new[] { "Nothing", "Count", "Faction", "Aligment", "Role" });

            Pirate =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#ECC23EFF>Pirate</color>");
            DuelCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Duel Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            PirateDuelsToWin =
                 new CustomNumberOption(num++, MultiMenu.neutral, "Number Of Won Duels To Win", 2, 1, 5, 1);

            Arsonist = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#FF4D00FF>Arsonist</color>");
            DouseCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Douse Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MaxDoused =
                new CustomNumberOption(num++, MultiMenu.neutral, "Maximum Alive Players Doused", 5, 1, 15, 1);
            ArsoImpVision =
                new CustomToggleOption(num++, MultiMenu.neutral, "Arsonist Has Impostor Vision", false);
            IgniteCdRemoved =
                new CustomToggleOption(num++, MultiMenu.neutral, "Ignite Cooldown Removed When Arsonist Is Last Killer", false);

            Juggernaut =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#8C004DFF>Juggernaut</color>");
            JuggKillCooldown = new CustomNumberOption(num++, MultiMenu.neutral, "Juggernaut Initial Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ReducedKCdPerKill = new CustomNumberOption(num++, MultiMenu.neutral, "Reduced Kill Cooldown Per Kill", 5f, 2.5f, 10f, 2.5f, CooldownFormat);
            JuggVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Juggernaut Can Vent", false);

            SerialKiller = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#1D4DFCFF>Serial Killer</color>");
            SerialKillerCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Serial Killer Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            BloodlustCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Kill Cooldown On Bloodlust", 5f, 0.5f, 20f, 0.5f, CooldownFormat);
            BloodlustDuration =
                new CustomNumberOption(num++, MultiMenu.neutral, "Bloodlust Duration", 20f, 2.5f, 60f, 2.5f, CooldownFormat);
            KillsToBloodlust =
                new CustomNumberOption(num++, MultiMenu.neutral, "Kills To Bloodlust", 2, 1, 5, 1);
            SerialKillerVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Serial Killer Can Vent");

            TheGlitch =
                new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#00FF00FF>The Glitch</color>");
            MimicCooldownOption = new CustomNumberOption(num++, MultiMenu.neutral, "Mimic Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MimicDurationOption = new CustomNumberOption(num++, MultiMenu.neutral, "Mimic Duration", 10f, 1f, 15f, 1f, CooldownFormat);
            HackCooldownOption = new CustomNumberOption(num++, MultiMenu.neutral, "Hack Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            HackDurationOption = new CustomNumberOption(num++, MultiMenu.neutral, "Hack Duration", 10f, 1f, 15f, 1f, CooldownFormat);
            GlitchKillCooldownOption =
                new CustomNumberOption(num++, MultiMenu.neutral, "Glitch Kill Cooldown", 25f, 10f, 120f, 2.5f, CooldownFormat);
            GlitchHackDistanceOption =
                new CustomStringOption(num++, MultiMenu.neutral, "Glitch Hack Distance", new[] { "Short", "Normal", "Long" });
            GlitchVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Glitch Can Vent", false);

            Werewolf = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#A86629FF>Werewolf</color>");
            RampageCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Rampage Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RampageDuration =
                new CustomNumberOption(num++, MultiMenu.neutral, "Rampage Duration", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RampageKillCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Rampage Kill Cooldown", 10f, 0.5f, 15f, 0.5f, CooldownFormat);
            WerewolfVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Werewolf Can Vent When Rampaged", false);

            Jackal = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#666666FF>Jackal</color>");
            JackalKCd =
                new CustomNumberOption(num++, MultiMenu.neutral, "Jackal Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RecruitsLifelink =
                new CustomToggleOption(num++, MultiMenu.neutral, "Recruits Are Lifelinked");
            RecruitsSeeJackal =
                new CustomToggleOption(num++, MultiMenu.neutral, "Recruits See Who Is Jackal");
            JackalVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Jackal Can Vent", false);
            RecruitsChat = new CustomStringOption(num++, MultiMenu.neutral, "Recruits Chat", new[] { "Off", "Rounds", "Meeting", "Both" });

            Necromancer = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#679556FF>Necromancer</color>");
            NecromancerReviveCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Initial Revive Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ReviveCooldownIncrease =
                new CustomNumberOption(num++, MultiMenu.neutral, "Increased Cooldown Per Revive", 25f, 0f, 60f, 2.5f, CooldownFormat);
            RitualKillCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Initial Ritual Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            RitualKillCooldownIncrease =
                new CustomNumberOption(num++, MultiMenu.neutral, "Increased Cooldown Per Kill", 25f, 0f, 60f, 2.5f, CooldownFormat);
            MaxNumberOfUndead =
                new CustomNumberOption(num++, MultiMenu.neutral, "Maximum Number Of Undead", 3, 1, 7, 1);
            NecromancerVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Necromancer Can Vent", false);
            UndeadChat = new CustomStringOption(num++, MultiMenu.neutral, "Undead Chat", new[] { "Off", "Rounds", "Meeting", "Both" });

            Vampire = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#262626FF>Vampire</color>");
            BiteCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Vampire Bite Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            VampImpVision =
                new CustomToggleOption(num++, MultiMenu.neutral, "Vampires Have Impostor Vision", false);
            VampVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Vampires Can Vent", false);
            NewVampCanAssassin =
                new CustomToggleOption(num++, MultiMenu.neutral, "New Vampire Can Assassinate", false);
            MaxVampiresPerGame =
                new CustomNumberOption(num++, MultiMenu.neutral, "Maximum Vampires Per Game", 2, 2, 5, 1);
            CanBiteNeutralBenign =
                new CustomToggleOption(num++, MultiMenu.neutral, "Can Convert Neutral Benign Roles", false);
            CanBiteNeutralEvil =
                new CustomToggleOption(num++, MultiMenu.neutral, "Can Convert Neutral Evil Roles", false);
            CanBiteNeutralChaos =
                new CustomToggleOption(num++, MultiMenu.neutral, "Can Convert Neutral Chaos Roles", false);
            VampiresChat = new CustomStringOption(num++, MultiMenu.neutral, "Vampires Chat", new[] { "Off", "Rounds", "Meeting", "Both" });

            Baker = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#FFC080FF>Baker</color>");
            BreadNeeded =
                new CustomNumberOption(num++, MultiMenu.neutral, "Bread To Become Famine", 5f, 3f, 7f, 1f);
            BakerCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Baker Cooldown", 30f, 10f, 60f, 2.5f, CooldownFormat);
            BreadSize =
                new CustomNumberOption(num++, MultiMenu.neutral, "Bread Size", 3f, 1f, 5f, 1f);
            BakerVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Baker Can Vent", false);
            FamineCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Famine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            AnnounceFamine =
                new CustomToggleOption(num++, MultiMenu.neutral, "Announce Famine");
            FamineVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Famine Can Vent", false);

            Berserker = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#FF4F00FF>Berserker</color>");
            KillsToWar =
                new CustomNumberOption(num++, MultiMenu.neutral, "Kills To Become War", 4f, 2f, 10f, 1f);
            BerserkerCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Berserker Cooldown", 30f, 10f, 60f, 2.5f, CooldownFormat);
            BerserkerCooldownBonus =
                new CustomNumberOption(num++, MultiMenu.neutral, "Berserker Cooldown Bonus", 5f, 2.5f, 10f, 2.5f, CooldownFormat);
            BerserkerVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Berserker Can Vent", false);
            WarCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "War Cooldown", 15f, 5f, 30f, 2.5f, CooldownFormat);
            WarRampage =
                new CustomNumberOption(num++, MultiMenu.neutral, "War Rampage Duration", 0.3f, 0.1f, 1.5f, 0.05f, CooldownFormat);
            AnnounceWar =
                new CustomToggleOption(num++, MultiMenu.neutral, "Announce War");
            WarVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "War Can Vent", false);

            Plaguebearer = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#E6FFB3FF>Plaguebearer</color>");
            InfectCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Infect Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            PlaguebearerVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Plaguebearer Can Vent", false);
            PestKillCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Pestilence Kill Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            AnnouncePestilence =
                new CustomToggleOption(num++, MultiMenu.neutral, "Announce Pestilence");
            PestVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Pestilence Can Vent", false);

            SoulCollector = new CustomHeaderOption(num++, MultiMenu.neutral, "<color=#E000FFFF>Soul Collector</color>");
            SoulsNeeded =
                new CustomNumberOption(num++, MultiMenu.neutral, "Souls To Become Death", 4f, 2f, 10f, 1f);
            SoulCollectorCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Soul Collector Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            SoulCollectorVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Soul Collector Can Vent", false);
            DeathCooldown =
                new CustomNumberOption(num++, MultiMenu.neutral, "Death Cooldown", 30f, 10f, 60f, 2.5f, CooldownFormat);
            AnnounceDeath =
                new CustomToggleOption(num++, MultiMenu.neutral, "Announce Death");
            DeathVent =
                new CustomToggleOption(num++, MultiMenu.neutral, "Death Can Vent", false);

            Escapist =
                new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Escapist</color>");
            EscapeCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Recall Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            EscapistVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Escapist Can Vent", false);

            Grenadier =
                new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Grenadier</color>");
            GrenadeCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Flash Grenade Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            GrenadeDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, "Flash Grenade Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            FlashRadius =
                new CustomNumberOption(num++, MultiMenu.imposter, "Flash Radius", 1f, 0.25f, 5f, 0.25f, MultiplierFormat);
            GrenadierIndicators =
                new CustomToggleOption(num++, MultiMenu.imposter, "Indicate Flashed Crewmates", false);
            GrenadierVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Grenadier Can Vent", false);

            Morphling =
                new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Morphling</color>");
            MorphlingCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Morphling Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            MorphlingDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, "Morphling Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            MorphlingVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Morphling Can Vent", false);

            Swooper = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Swooper</color>");
            SwoopCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Swoop Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            SwoopDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, "Swoop Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            SwooperVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Swooper Can Vent", false);

            Venerer = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Venerer</color>");
            AbilityCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Ability Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            AbilityDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, "Ability Duration", 10f, 5f, 15f, 1f, CooldownFormat);
            SprintSpeed = new CustomNumberOption(num++, MultiMenu.imposter, "Sprint Speed", 1.25f, 1.05f, 2.5f, 0.05f, MultiplierFormat);
            FreezeSpeed = new CustomNumberOption(num++, MultiMenu.imposter, "Freeze Speed", 0.75f, 0.25f, 1f, 0.05f, MultiplierFormat);

            Bomber =
                new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Bomber</color>");
            DetonateDelay =
                new CustomNumberOption(num++, MultiMenu.imposter, "Detonate Delay", 5f, 1f, 15f, 1f, CooldownFormat);
            MaxKillsInDetonation =
                new CustomNumberOption(num++, MultiMenu.imposter, "Max Kills In Detonation", 5, 1, 15, 1);
            DetonateRadius =
                new CustomNumberOption(num++, MultiMenu.imposter, "Detonate Radius", 0.25f, 0.05f, 1f, 0.05f, MultiplierFormat);
            BomberVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Bomber Can Vent", false);

            Poisoner = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Poisoner</color>");
            PoisonDelay =
                new CustomNumberOption(num++, MultiMenu.imposter, "Poison Delay", 10f, 5f, 30f, 2.5f, CooldownFormat);
            PoisonerVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Poisoner Can Vent", false);

            Sniper = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Sniper</color>");
            AimCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Aiming Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            SniperVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Sniper Can Vent", false);

            Traitor = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Traitor</color>");
            LatestSpawn = new CustomNumberOption(num++, MultiMenu.imposter, "Minimum People Alive When Traitor Can Spawn", 5, 3, 15, 1);
            NeutralKillingStopsTraitor =
                new CustomToggleOption(num++, MultiMenu.imposter, "Traitor Won't Spawn If Any Neutral Killing Is Alive", false);

            Warlock = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Warlock</color>");
            ChargeUpDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, "Time It Takes To Fully Charge", 25f, 10f, 60f, 2.5f, CooldownFormat);
            ChargeUseDuration =
                new CustomNumberOption(num++, MultiMenu.imposter, "Time It Takes To Use Full Charge", 1f, 0.05f, 5f, 0.05f, CooldownFormat);

            Blackmailer = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Blackmailer</color>");
            BlackmailCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Initial Blackmail Cooldown", 10f, 1f, 15f, 1f, CooldownFormat);
            BlackmailInvisible =
                new CustomToggleOption(num++, MultiMenu.imposter, "Only Target Sees Blackmail", false);
            BlackmailedVote =
                new CustomToggleOption(num++, MultiMenu.imposter, "Blackmailed Vote Doesn't Count", false);

            Miner = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Miner</color>");
            MineCooldown =
                new CustomNumberOption(num++, MultiMenu.imposter, "Mine Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            InstantVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Show Vents Immediately", true);

            Poltergeist =
                new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Poltergeist</color>");
            PoltergeistTasksRemainingClicked =
                 new CustomNumberOption(num++, MultiMenu.imposter, "Tasks Remaining When Poltergeist Can Be Clicked", 5, 1, 15, 1);
            PoltergeistTasksRemainingAlert =
                 new CustomNumberOption(num++, MultiMenu.imposter, "Tasks Remaining When Alert Is Sent", 1, 0, 5, 1);
            PoltergeistKCdMult =
                new CustomNumberOption(num++, MultiMenu.imposter, "Poltergeist Cooldown Multiplier", 0.5f, 0.05f, 0.95f, 0.05f, MultiplierFormat);
            PoltergeistCanBeClickedBy = new CustomStringOption(num++, MultiMenu.imposter, "Who Can Click Poltergeist", new[] { "All", "Non-Imps", "Crew Only" });

            Undertaker = new CustomHeaderOption(num++, MultiMenu.imposter, "<color=#FF0000FF>Undertaker</color>");
            DragCooldown = new CustomNumberOption(num++, MultiMenu.imposter, "Drag Cooldown", 25f, 10f, 60f, 2.5f, CooldownFormat);
            UndertakerDragSpeed =
                new CustomNumberOption(num++, MultiMenu.imposter, "Undertaker Drag Speed", 0.75f, 0.25f, 1f, 0.05f, MultiplierFormat);
            UndertakerVent =
                new CustomToggleOption(num++, MultiMenu.imposter, "Undertaker Can Vent", false);
            UndertakerVentWithBody =
                new CustomToggleOption(num++, MultiMenu.imposter, "Undertaker Can Vent While Dragging", false);

            Bait = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#00B3B3FF>Bait</color>");
            BaitMinDelay = new CustomNumberOption(num++, MultiMenu.modifiers, "Minimum Delay for the Bait Report", 0f, 0f, 15f, 0.5f, CooldownFormat);
            BaitMaxDelay = new CustomNumberOption(num++, MultiMenu.modifiers, "Maximum Delay for the Bait Report", 1f, 0f, 15f, 0.5f, CooldownFormat);

            Diseased = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#808080FF>Diseased</color>");
            DiseasedKillMultiplier = new CustomNumberOption(num++, MultiMenu.modifiers, "Diseased Kill Multiplier", 3f, 1.5f, 5f, 0.5f, MultiplierFormat);

            Frosty = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#99FFFFFF>Frosty</color>");
            ChillDuration = new CustomNumberOption(num++, MultiMenu.modifiers, "Chill Duration", 10f, 1f, 15f, 1f, CooldownFormat);
            ChillStartSpeed = new CustomNumberOption(num++, MultiMenu.modifiers, "Chill Start Speed", 0.75f, 0.25f, 0.95f, 0.05f, MultiplierFormat);

            Drunk =
                new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#758000FF>Drunk</color>");
            DrunkWearsOff = new CustomToggleOption(num++, MultiMenu.modifiers, "Drunk Wears Off");
            DrunkDuration = new CustomNumberOption(num++, MultiMenu.modifiers, "Drunk Stays", 3, 1, 5, 1, RoundsFormat);

            Flash = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#FF8080FF>Flash</color>");
            FlashSpeed = new CustomNumberOption(num++, MultiMenu.modifiers, "Flash Speed", 1.25f, 1.05f, 2.5f, 0.05f, MultiplierFormat);

            Giant = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#FFB34DFF>Giant</color>");
            GiantSlow = new CustomNumberOption(num++, MultiMenu.modifiers, "Giant Speed", 0.75f, 0.25f, 1f, 0.05f, MultiplierFormat);

            Underdog = new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#FF0000FF>Underdog</color>");
            UnderdogKillBonus = new CustomNumberOption(num++, MultiMenu.modifiers, "Kill Cooldown Bonus", 5f, 2.5f, 10f, 2.5f, CooldownFormat);
            UnderdogIncreasedKC = new CustomToggleOption(num++, MultiMenu.modifiers, "Increased Kill Cooldown When 2+ Imps", true);

            Lovers =
                new CustomHeaderOption(num++, MultiMenu.modifiers, "<color=#FF66CCFF>Lovers</color>");
            BothLoversDie = new CustomToggleOption(num++, MultiMenu.modifiers, "Both Lovers Die");
            LovingImpPercent = new CustomNumberOption(num++, MultiMenu.modifiers, "Loving Impostor Probability", 20f, 0f, 100f, 10f,
                PercentFormat);
            NeutralLovers = new CustomToggleOption(num++, MultiMenu.modifiers, "Neutral Roles Can Be Lovers");
            LoversChat = new CustomStringOption(num++, MultiMenu.modifiers, "Lovers Chat", new[] { "Off", "Rounds", "Meeting", "Both" });
        }
    }
}