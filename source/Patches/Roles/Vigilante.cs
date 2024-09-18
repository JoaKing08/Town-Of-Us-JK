using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.Patches;
using UnityEngine;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.GuardianAngelMod;
using TownOfUs.CrewmateRoles.VampireHunterMod;

namespace TownOfUs.Roles
{
    public class Vigilante : Role, IGuesser
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> Buttons { get; set; } = new();
        private Dictionary<string, Color> _colorMapping = null;
        public Dictionary<string, Color> ColorMapping
        {
            get
            {
                if (_colorMapping != null) return _colorMapping;
                var result = new Dictionary<string, Color>();
                if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.AllAny || CustomGameOptions.GameMode == GameMode.Horseman)
                {
                    if (Player.Is(ObjectiveEnum.ImpostorAgent) || Player.Is(ObjectiveEnum.ApocalypseAgent) || FactionOverride != FactionOverride.None)
                    {
                        if (CustomGameOptions.AssassinCrewmateGuess) result.Add("Crewmate", Colors.Crewmate);
                        if (CustomGameOptions.AssassinGuessCrewInvestigative)
                        {
                            if (CustomGameOptions.InvestigatorOn > 0) result.Add("Investigator", Colors.Investigator);
                            if (CustomGameOptions.SeerOn > 0) result.Add("Seer", Colors.Seer);
                            if (CustomGameOptions.SpyOn > 0) result.Add("Spy", Colors.Spy);
                            if (CustomGameOptions.SnitchOn > 0) result.Add("Snitch", Colors.Snitch);
                            if (CustomGameOptions.TrackerOn > 0) result.Add("Tracker", Colors.Tracker);
                            if (CustomGameOptions.TrapperOn > 0) result.Add("Trapper", Colors.Trapper);
                            if (CustomGameOptions.MysticOn > 0) result.Add("Mystic", Colors.Mystic);
                            if (CustomGameOptions.DetectiveOn > 0) result.Add("Detective", Colors.Detective);
                            if (CustomGameOptions.AurialOn > 0) result.Add("Aurial", Colors.Aurial);
                            if (CustomGameOptions.InspectorOn > 0) result.Add("Inspector", Colors.Inspector);
                            if (CustomGameOptions.LookoutOn > 0) result.Add("Lookout", Colors.Lookout);
                            if (CustomGameOptions.SageOn > 0) result.Add("Sage", Colors.Sage);
                        }
                        if (CustomGameOptions.MayorOn > 0) result.Add("Mayor", Colors.Mayor);
                        if (CustomGameOptions.SheriffOn > 0 || (CustomGameOptions.VampireHunterOn > 0 && (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Horseman) && (CustomGameOptions.VampireOn > 0 || CustomGameOptions.NecromancerOn > 0) && CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Sheriff)) result.Add("Sheriff", Colors.Sheriff);
                        if (CustomGameOptions.EngineerOn > 0) result.Add("Engineer", Colors.Engineer);
                        if (CustomGameOptions.SwapperOn > 0) result.Add("Swapper", Colors.Swapper);
                        if (CustomGameOptions.MedicOn > 0) result.Add("Medic", Colors.Medic);
                        if (CustomGameOptions.AltruistOn > 0) result.Add("Altruist", Colors.Altruist);
                        if (CustomGameOptions.VigilanteOn > 0 || (CustomGameOptions.VampireHunterOn > 0 && (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Horseman) && (CustomGameOptions.VampireOn > 0 || CustomGameOptions.NecromancerOn > 0) && CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Vigilante)) result.Add("Vigilante", Colors.Vigilante);
                        if (CustomGameOptions.VeteranOn > 0 || (CustomGameOptions.VampireHunterOn > 0 && (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Horseman) && (CustomGameOptions.VampireOn > 0 || CustomGameOptions.NecromancerOn > 0) && CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Veteran)) result.Add("Veteran", Colors.Veteran);
                        if (CustomGameOptions.HunterOn > 0 || (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.GameMode == GameMode.Classic && (CustomGameOptions.VampireOn > 0 || CustomGameOptions.NecromancerOn > 0) && CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Hunter)) result.Add("Hunter", Colors.Hunter);
                        if (CustomGameOptions.TransporterOn > 0) result.Add("Transporter", Colors.Transporter);
                        if (CustomGameOptions.MediumOn > 0) result.Add("Medium", Colors.Medium);
                        if (CustomGameOptions.ImitatorOn > 0) result.Add("Imitator", Colors.Imitator);
                        if (CustomGameOptions.VampireHunterOn > 0 && (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Horseman) && CustomGameOptions.VampireOn > 0) result.Add("Vampire Hunter", Colors.VampireHunter);
                        if (CustomGameOptions.ProsecutorOn > 0) result.Add("Prosecutor", Colors.Prosecutor);
                        if (CustomGameOptions.OracleOn > 0) result.Add("Oracle", Colors.Oracle);
                        if (CustomGameOptions.MonarchOn > 0) result.Add("Monarch", Colors.Monarch);
                        if (CustomGameOptions.TavernKeeperOn > 0) result.Add("Tavern Keeper", Colors.TavernKeeper);
                        if (CustomGameOptions.UndercoverOn > 0) result.Add("Undercover", Colors.Undercover);
                        if (CustomGameOptions.DeputyOn > 0) result.Add("Deputy", Colors.Deputy);
                        if (CustomGameOptions.BodyguardOn > 0) result.Add("Bodyguard", Colors.Bodyguard);
                        if (CustomGameOptions.CrusaderOn > 0) result.Add("Crusader", Colors.Crusader);
                        if (CustomGameOptions.ClericOn > 0) result.Add("Cleric", Colors.Cleric);
                    }
                    if (CustomGameOptions.GameMode != GameMode.Horseman)
                    {
                        result.Add("Impostor", Colors.Impostor);
                        if (CustomGameOptions.JanitorOn > 0) result.Add("Janitor", Colors.Impostor);
                        if (CustomGameOptions.MorphlingOn > 0) result.Add("Morphling", Colors.Impostor);
                        if (CustomGameOptions.MinerOn > 0) result.Add(GameOptionsManager.Instance.currentNormalGameOptions.MapId == 5 ? "Mycologist" : "Miner", Colors.Impostor);
                        if (CustomGameOptions.SwooperOn > 0) result.Add("Swooper", Colors.Impostor);
                        if (CustomGameOptions.UndertakerOn > 0) result.Add("Undertaker", Colors.Impostor);
                        if (CustomGameOptions.EscapistOn > 0) result.Add("Escapist", Colors.Impostor);
                        if (CustomGameOptions.GrenadierOn > 0) result.Add("Grenadier", Colors.Impostor);
                        if (CustomGameOptions.TraitorOn > 0) result.Add("Traitor", Colors.Impostor);
                        if (CustomGameOptions.BlackmailerOn > 0) result.Add("Blackmailer", Colors.Impostor);
                        if (CustomGameOptions.BomberOn > 0) result.Add("Bomber", Colors.Impostor);
                        if (CustomGameOptions.WarlockOn > 0) result.Add("Warlock", Colors.Impostor);
                        if (CustomGameOptions.VenererOn > 0) result.Add("Venerer", Colors.Impostor);
                        if (CustomGameOptions.PoisonerOn > 0) result.Add("Poisoner", Colors.Impostor);
                        if (CustomGameOptions.SniperOn > 0) result.Add("Sniper", Colors.Impostor);
                        if (CustomGameOptions.DemagogueOn > 0) result.Add("Demagogue", Colors.Impostor);
                        if (CustomGameOptions.GodfatherOn > 0) result.Add("Godfather", Colors.Impostor);
                        if (CustomGameOptions.GodfatherOn > 0) result.Add("Mafioso", Colors.Impostor);
                        if (CustomGameOptions.OccultistOn > 0) result.Add("Occultist", Colors.Impostor);
                        if (CustomGameOptions.ImpostorAgentOn > 0) result.Add("Agent (Imp)", Colors.ImpostorAgent);
                    }

                    if (CustomGameOptions.VigilanteGuessNeutralBenign)
                    {
                        if (CustomGameOptions.AmnesiacOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Amnesiac) || (CustomGameOptions.CursedSoulOn > 0 && CustomGameOptions.SwappedBecomes == SwappedBecomes.Amnesiac)) result.Add("Amnesiac", Colors.Amnesiac);
                        if (CustomGameOptions.GuardianAngelOn > 0) result.Add("Guardian Angel", Colors.GuardianAngel);
                        if (CustomGameOptions.SurvivorOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Survivor) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Survivor) || (CustomGameOptions.CursedSoulOn > 0 && (CustomGameOptions.SwappedBecomes == SwappedBecomes.Survivor || CustomGameOptions.SwappedBecomes == SwappedBecomes.DefaultRole))) result.Add("Survivor", Colors.Survivor);
                        if (CustomGameOptions.CursedSoulOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.CursedSoul) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.CursedSoul)) result.Add("Cursed Soul", Colors.CursedSoul);
                    }
                    if (CustomGameOptions.VigilanteGuessNeutralEvil)
                    {
                        if (CustomGameOptions.ExecutionerOn > 0) result.Add("Executioner", Colors.Executioner);
                        if (CustomGameOptions.JesterOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Jester) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Jester) || (CustomGameOptions.CursedSoulOn > 0 && CustomGameOptions.SwappedBecomes == SwappedBecomes.Jester)) result.Add("Jester", Colors.Jester);
                        if (CustomGameOptions.WitchOn > 0) result.Add("Witch", Colors.Witch);
                    }
                    if (CustomGameOptions.VigilanteGuessNeutralChaos)
                    {
                        if (CustomGameOptions.DoomsayerOn > 0) result.Add("Doomsayer", Colors.Doomsayer);
                        if (CustomGameOptions.PirateOn > 0) result.Add("Pirate", Colors.Pirate);
                        if (CustomGameOptions.InquisitorOn > 0) result.Add("Inquisitor", Colors.Inquisitor);
                    }
                    if (CustomGameOptions.VigilanteGuessNeutralKilling)
                    {
                        if (CustomGameOptions.ArsonistOn > 0) result.Add("Arsonist", Colors.Arsonist);
                        if (CustomGameOptions.GlitchOn > 0) result.Add("The Glitch", Colors.Glitch);
                        if (CustomGameOptions.WerewolfOn > 0) result.Add("Werewolf", Colors.Werewolf);
                        if (CustomGameOptions.JuggernautOn > 0) result.Add("Juggernaut", Colors.Juggernaut);
                        if (CustomGameOptions.SerialKillerOn > 0) result.Add("Serial Killer", Colors.SerialKiller);
                    }
                    if (CustomGameOptions.VigilanteGuessNeutralProselyte)
                    {
                        if ((CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Horseman) && CustomGameOptions.VampireOn > 0) result.Add("Vampire", Colors.Vampire);
                        if (CustomGameOptions.JackalOn > 0) result.Add("Jackal", Colors.Jackal);
                        if (CustomGameOptions.NecromancerOn > 0) result.Add("Necromancer", Colors.Necromancer);
                    }
                    if ((CustomGameOptions.PlaguebearerOn > 0 && CustomGameOptions.VigilanteGuessNeutralApocalypse) || CustomGameOptions.GameMode == GameMode.Horseman) result.Add("Plaguebearer", Colors.Plaguebearer);
                    if ((CustomGameOptions.BakerOn > 0 && CustomGameOptions.VigilanteGuessNeutralApocalypse) || CustomGameOptions.GameMode == GameMode.Horseman) result.Add("Baker", Colors.Baker);
                    if ((CustomGameOptions.BerserkerOn > 0 && CustomGameOptions.VigilanteGuessNeutralApocalypse) || CustomGameOptions.GameMode == GameMode.Horseman) result.Add("Berserker", Colors.Berserker);
                    if ((CustomGameOptions.SoulCollectorOn > 0 && CustomGameOptions.VigilanteGuessNeutralApocalypse) || CustomGameOptions.GameMode == GameMode.Horseman) result.Add("Soul Collector", Colors.SoulCollector);
                    if (CustomGameOptions.ApocalypseAgentOn > 0 && (((CustomGameOptions.PlaguebearerOn > 0 || CustomGameOptions.BakerOn > 0 || CustomGameOptions.BerserkerOn > 0 || CustomGameOptions.SoulCollectorOn > 0) && CustomGameOptions.VigilanteGuessNeutralApocalypse) || CustomGameOptions.GameMode == GameMode.Horseman)) result.Add("Agent (Apoc)", Colors.ApocalypseAgent);
                    if (CustomGameOptions.VigilanteGuessLovers && CustomGameOptions.LoversOn > 0) result.Add("Lover", Colors.Lovers);
                }
                else if (CustomGameOptions.GameMode == GameMode.KillingOnly)
                {
                    result.Add("Morphling", Colors.Impostor);
                    result.Add(GameOptionsManager.Instance.currentNormalGameOptions.MapId == 5 ? "Mycologist" : "Miner", Colors.Impostor);
                    result.Add("Swooper", Colors.Impostor);
                    result.Add("Undertaker", Colors.Impostor);
                    result.Add("Grenadier", Colors.Impostor);
                    result.Add("Traitor", Colors.Impostor);
                    result.Add("Escapist", Colors.Impostor);

                    if (CustomGameOptions.VigilanteGuessNeutralKilling)
                    {
                        if (CustomGameOptions.AddArsonist) result.Add("Arsonist", Colors.Arsonist);
                        if (CustomGameOptions.AddPlaguebearer) result.Add("Plaguebearer", Colors.Plaguebearer);
                        result.Add("The Glitch", Colors.Glitch);
                        result.Add("Werewolf", Colors.Werewolf);
                        result.Add("Juggernaut", Colors.Juggernaut);
                    }
                }
                else if (CustomGameOptions.GameMode == GameMode.RoleList)
                {
                    if (Player.Is(ObjectiveEnum.ImpostorAgent) || Player.Is(ObjectiveEnum.ApocalypseAgent) || FactionOverride != FactionOverride.None)
                    {
                        if (CustomGameOptions.AssassinCrewmateGuess) result.Add("Crewmate", Colors.Crewmate);
                        if (CustomGameOptions.AssassinGuessCrewInvestigative)
                        {
                            result.Add("Investigator", Colors.Investigator);
                            result.Add("Seer", Colors.Seer);
                            result.Add("Spy", Colors.Spy);
                            result.Add("Snitch", Colors.Snitch);
                            result.Add("Tracker", Colors.Tracker);
                            result.Add("Trapper", Colors.Trapper);
                            result.Add("Mystic", Colors.Mystic);
                            result.Add("Detective", Colors.Detective);
                            result.Add("Aurial", Colors.Aurial);
                            result.Add("Inspector", Colors.Inspector);
                            result.Add("Lookout", Colors.Lookout);
                            result.Add("Sage", Colors.Sage);
                        }
                        result.Add("Mayor", Colors.Mayor);
                        result.Add("Sheriff", Colors.Sheriff);
                        result.Add("Engineer", Colors.Engineer);
                        result.Add("Swapper", Colors.Swapper);
                        result.Add("Medic", Colors.Medic);
                        result.Add("Altruist", Colors.Altruist);
                        result.Add("Vigilante", Colors.Vigilante);
                        result.Add("Veteran", Colors.Veteran);
                        result.Add("Hunter", Colors.Hunter);
                        result.Add("Transporter", Colors.Transporter);
                        result.Add("Medium", Colors.Medium);
                        result.Add("Imitator", Colors.Imitator);
                        result.Add("Vampire Hunter", Colors.VampireHunter);
                        result.Add("Prosecutor", Colors.Prosecutor);
                        result.Add("Oracle", Colors.Oracle);
                        result.Add("Monarch", Colors.Monarch);
                        result.Add("Tavern Keeper", Colors.TavernKeeper);
                        result.Add("Undercover", Colors.Undercover);
                        result.Add("Deputy", Colors.Deputy);
                        result.Add("Bodyguard", Colors.Bodyguard);
                        result.Add("Crusader", Colors.Crusader);
                        result.Add("Cleric", Colors.Cleric);
                    }
                    result.Add("Impostor", Colors.Impostor);
                    result.Add("Janitor", Colors.Impostor);
                    result.Add("Morphling", Colors.Impostor);
                    result.Add(GameOptionsManager.Instance.currentNormalGameOptions.MapId == 5 ? "Mycologist" : "Miner", Colors.Impostor);
                    result.Add("Swooper", Colors.Impostor);
                    result.Add("Undertaker", Colors.Impostor);
                    result.Add("Escapist", Colors.Impostor);
                    result.Add("Grenadier", Colors.Impostor);
                    result.Add("Traitor", Colors.Impostor);
                    result.Add("Blackmailer", Colors.Impostor);
                    result.Add("Bomber", Colors.Impostor);
                    result.Add("Warlock", Colors.Impostor);
                    result.Add("Venerer", Colors.Impostor);
                    result.Add("Poisoner", Colors.Impostor);
                    result.Add("Sniper", Colors.Impostor);
                    result.Add("Demagogue", Colors.Impostor);
                    result.Add("Godfather", Colors.Impostor);
                    result.Add("Mafioso", Colors.Impostor);
                    result.Add("Occultist", Colors.Impostor);
                    if (CustomGameOptions.ImpostorAgentOn > 0) result.Add("Agent (Imp)", Colors.ImpostorAgent);

                    if (CustomGameOptions.VigilanteGuessNeutralBenign)
                    {
                        result.Add("Amnesiac", Colors.Amnesiac);
                        result.Add("Guardian Angel", Colors.GuardianAngel);
                        result.Add("Cursed Soul", Colors.CursedSoul);
                        result.Add("Survivor", Colors.Survivor);
                    }
                    if (CustomGameOptions.VigilanteGuessNeutralEvil)
                    {
                        result.Add("Executioner", Colors.Executioner);
                        result.Add("Jester", Colors.Jester);
                        result.Add("Witch", Colors.Witch);
                    }
                    if (CustomGameOptions.VigilanteGuessNeutralChaos)
                    {
                        result.Add("Doomsayer", Colors.Doomsayer);
                        result.Add("Pirate", Colors.Pirate);
                        result.Add("Inquisitor", Colors.Inquisitor);
                    }
                    if (CustomGameOptions.VigilanteGuessNeutralKilling)
                    {
                        result.Add("Arsonist", Colors.Arsonist);
                        result.Add("The Glitch", Colors.Glitch);
                        result.Add("Werewolf", Colors.Werewolf);
                        result.Add("Juggernaut", Colors.Juggernaut);
                        result.Add("Serial Killer", Colors.SerialKiller);
                    }
                    if (CustomGameOptions.VigilanteGuessNeutralProselyte)
                    {
                        result.Add("Vampire", Colors.Vampire);
                        result.Add("Jackal", Colors.Jackal);
                        result.Add("Necromancer", Colors.Necromancer);
                    }
                    if (CustomGameOptions.DoomsayerGuessNeutralApocalypse)
                    {
                        result.Add("Plaguebearer", Colors.Plaguebearer);
                        result.Add("Baker", Colors.Baker);
                        result.Add("Berserker", Colors.Berserker);
                        result.Add("Soul Collector", Colors.SoulCollector);
                        if (CustomGameOptions.ApocalypseAgentOn > 0) result.Add("Agent (Apoc)", Colors.ApocalypseAgent);
                    }
                }
                else
                {
                    result.Add("Necromancer", Colors.Impostor);
                    result.Add("Whisperer", Colors.Impostor);
                    if (CustomGameOptions.MaxChameleons > 0) result.Add("Swooper", Colors.Impostor);
                    if (CustomGameOptions.MaxEngineers > 0) result.Add("Demolitionist", Colors.Impostor);
                    if (CustomGameOptions.MaxInvestigators > 0) result.Add("Consigliere", Colors.Impostor);
                    if (CustomGameOptions.MaxMystics > 0) result.Add("Clairvoyant", Colors.Impostor);
                    if (CustomGameOptions.MaxSnitches > 0) result.Add("Informant", Colors.Impostor);
                    if (CustomGameOptions.MaxSpies > 0) result.Add("Rogue Agent", Colors.Impostor);
                    if (CustomGameOptions.MaxTransporters > 0) result.Add("Escapist", Colors.Impostor);
                    if (CustomGameOptions.MaxVigilantes > 1) result.Add("Assassin", Colors.Impostor);
                    result.Add("Impostor", Colors.Impostor);
                }
                return result;
            }
            set
            {
                _colorMapping = value;
            }
        }

        public Dictionary<string, Color> SortedColorMapping
        {
            get
            {
                return ColorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        public Dictionary<byte, string> Guesses = new();

        public Vigilante(PlayerControl player) : base(player)
        {
            Name = "Vigilante";
            ImpostorText = () => "Kill Impostors If You Can Guess Their Roles";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Guess the roles of impostors mid-meeting to kill them!" : "Zgaduj role impostorów podczas spotkania by ich zabic!";
            Color = Patches.Colors.Vigilante;
            RoleType = RoleEnum.Vigilante;
            AddToRoleHistory(RoleType);

            RemainingKills = CustomGameOptions.VigilanteKills;
        }

        public bool GuessedThisMeeting { get; set; } = false;

        public int RemainingKills { get; set; }

        public List<string> PossibleGuesses => SortedColorMapping.Keys.ToList();
    }
}
