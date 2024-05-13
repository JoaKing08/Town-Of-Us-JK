using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.Patches;
using UnityEngine;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.GuardianAngelMod;
using TownOfUs.CrewmateRoles.VampireHunterMod;

namespace TownOfUs.Roles.Modifiers
{
    public class Assassin : Ability, IGuesser
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> Buttons { get; set; } = new();


        private Dictionary<string, Color> ColorMapping = new();

        public Dictionary<string, Color> SortedColorMapping;

        public Dictionary<byte, string> Guesses = new();


        public Assassin(PlayerControl player) : base(player)
        {
            Name = "Assassin";
            TaskText = () => "Guess the roles of the people and kill them mid-meeting";
            Color = Patches.Colors.Impostor;
            AbilityType = AbilityEnum.Assassin;

            RemainingKills = CustomGameOptions.AssassinKills;

            // Adds all the roles that have a non-zero chance of being in the game.
            if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.AllAny || CustomGameOptions.GameMode == GameMode.Horseman)
            {
                if (CustomGameOptions.MayorOn > 0) ColorMapping.Add("Mayor", Colors.Mayor);
                if (CustomGameOptions.SheriffOn > 0 || (CustomGameOptions.VampireHunterOn > 0 && (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Horseman) && (CustomGameOptions.VampireOn > 0 || CustomGameOptions.NecromancerOn > 0) && CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Sheriff)) ColorMapping.Add("Sheriff", Colors.Sheriff);
                if (CustomGameOptions.EngineerOn > 0) ColorMapping.Add("Engineer", Colors.Engineer);
                if (CustomGameOptions.SwapperOn > 0) ColorMapping.Add("Swapper", Colors.Swapper);
                if (CustomGameOptions.InvestigatorOn > 0) ColorMapping.Add("Investigator", Colors.Investigator);
                if (CustomGameOptions.MedicOn > 0) ColorMapping.Add("Medic", Colors.Medic);
                if (CustomGameOptions.SeerOn > 0) ColorMapping.Add("Seer", Colors.Seer);
                if (CustomGameOptions.SpyOn > 0) ColorMapping.Add("Spy", Colors.Spy);
                if (CustomGameOptions.SnitchOn > 0) ColorMapping.Add("Snitch", Colors.Snitch);
                if (CustomGameOptions.AltruistOn > 0) ColorMapping.Add("Altruist", Colors.Altruist);
                if (CustomGameOptions.VigilanteOn > 0 || (CustomGameOptions.VampireHunterOn > 0 && (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Horseman) && (CustomGameOptions.VampireOn > 0 || CustomGameOptions.NecromancerOn > 0) && CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Vigilante)) ColorMapping.Add("Vigilante", Colors.Vigilante);
                if (CustomGameOptions.VeteranOn > 0 || (CustomGameOptions.VampireHunterOn > 0 && (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Horseman) && (CustomGameOptions.VampireOn > 0 || CustomGameOptions.NecromancerOn > 0) && CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Veteran)) ColorMapping.Add("Veteran", Colors.Veteran);
                if (CustomGameOptions.HunterOn > 0 || (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.GameMode == GameMode.Classic && (CustomGameOptions.VampireOn > 0 || CustomGameOptions.NecromancerOn > 0) && CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Hunter)) ColorMapping.Add("Hunter", Colors.Hunter);
                if (CustomGameOptions.TrackerOn > 0) ColorMapping.Add("Tracker", Colors.Tracker);
                if (CustomGameOptions.TrapperOn > 0) ColorMapping.Add("Trapper", Colors.Trapper);
                if (CustomGameOptions.TransporterOn > 0) ColorMapping.Add("Transporter", Colors.Transporter);
                if (CustomGameOptions.MediumOn > 0) ColorMapping.Add("Medium", Colors.Medium);
                if (CustomGameOptions.MysticOn > 0) ColorMapping.Add("Mystic", Colors.Mystic);
                if (CustomGameOptions.DetectiveOn > 0) ColorMapping.Add("Detective", Colors.Detective);
                if (CustomGameOptions.ImitatorOn > 0) ColorMapping.Add("Imitator", Colors.Imitator);
                if (CustomGameOptions.VampireHunterOn > 0 && (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Horseman) && CustomGameOptions.VampireOn > 0) ColorMapping.Add("Vampire Hunter", Colors.VampireHunter);
                if (CustomGameOptions.ProsecutorOn > 0) ColorMapping.Add("Prosecutor", Colors.Prosecutor);
                if (CustomGameOptions.OracleOn > 0) ColorMapping.Add("Oracle", Colors.Oracle);
                if (CustomGameOptions.AurialOn > 0) ColorMapping.Add("Aurial", Colors.Aurial);
                if (CustomGameOptions.InspectorOn > 0) ColorMapping.Add("Inspector", Colors.Inspector);
                if (CustomGameOptions.MonarchOn > 0) ColorMapping.Add("Monarch", Colors.Monarch);
                if (CustomGameOptions.TavernKeeperOn > 0) ColorMapping.Add("Tavern Keeper", Colors.TavernKeeper);
                if (CustomGameOptions.UndercoverOn > 0) ColorMapping.Add("Undercover", Colors.Undercover);
                if (CustomGameOptions.LookoutOn > 0) ColorMapping.Add("Lookout", Colors.Lookout);

                // Add Neutral roles if enabled
                if (CustomGameOptions.AssassinGuessNeutralBenign)
                {
                    if (CustomGameOptions.AmnesiacOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Amnesiac) || (CustomGameOptions.CursedSoulOn > 0 && CustomGameOptions.SwappedBecomes == SwappedBecomes.Amnesiac)) ColorMapping.Add("Amnesiac", Colors.Amnesiac);
                    if (CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);
                    if (CustomGameOptions.SurvivorOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Survivor) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Survivor) || (CustomGameOptions.CursedSoulOn > 0 && (CustomGameOptions.SwappedBecomes == SwappedBecomes.Survivor || CustomGameOptions.SwappedBecomes == SwappedBecomes.DefaultRole))) ColorMapping.Add("Cursed Soul", Colors.CursedSoul);
                    if (CustomGameOptions.CursedSoulOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.CursedSoul) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.CursedSoul)) ColorMapping.Add("Survivor", Colors.Survivor);
                }
                if (CustomGameOptions.AssassinGuessNeutralEvil)
                {
                    if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", Colors.Executioner);
                    if (CustomGameOptions.JesterOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Jester) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Jester) || (CustomGameOptions.CursedSoulOn > 0 && CustomGameOptions.SwappedBecomes == SwappedBecomes.Jester)) ColorMapping.Add("Jester", Colors.Jester);
                    if (CustomGameOptions.WitchOn > 0) ColorMapping.Add("Witch", Colors.Witch);
                }
                if (CustomGameOptions.AssassinGuessNeutralChaos)
                {
                    if (CustomGameOptions.DoomsayerOn > 0) ColorMapping.Add("Doomsayer", Colors.Doomsayer);
                    if (CustomGameOptions.PirateOn > 0) ColorMapping.Add("Pirate", Colors.Pirate);
                    if (CustomGameOptions.InquisitorOn > 0) ColorMapping.Add("Inquisitor", Colors.Inquisitor);
                }
                if (CustomGameOptions.AssassinGuessNeutralKilling)
                {
                    if (CustomGameOptions.ArsonistOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist)) ColorMapping.Add("Arsonist", Colors.Arsonist);
                    if (CustomGameOptions.GlitchOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)) ColorMapping.Add("The Glitch", Colors.Glitch);
                    if (CustomGameOptions.WerewolfOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf)) ColorMapping.Add("Werewolf", Colors.Werewolf);
                    if (!PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut) && CustomGameOptions.HiddenRoles && CustomGameOptions.GameMode != GameMode.Horseman) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
                    if (CustomGameOptions.SerialKillerOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller)) ColorMapping.Add("Serial Killer", Colors.SerialKiller);
                }
                if (CustomGameOptions.AssassinGuessNeutralProselyte)
                {
                    if ((CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Horseman) && CustomGameOptions.VampireOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Vampire)) ColorMapping.Add("Vampire", Colors.Vampire);
                    if (CustomGameOptions.WerewolfOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf)) ColorMapping.Add("Jackal", Colors.Jackal);
                    if (CustomGameOptions.SerialKillerOn > 0 && !PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller)) ColorMapping.Add("Necromancer", Colors.Necromancer);
                }
                if (CustomGameOptions.AssassinGuessImpostors && !PlayerControl.LocalPlayer.Is(Faction.Impostors) && CustomGameOptions.GameMode != GameMode.Horseman)
                {
                    ColorMapping.Add("Impostor", Colors.Impostor);
                    if (CustomGameOptions.JanitorOn > 0) ColorMapping.Add("Janitor", Colors.Impostor);
                    if (CustomGameOptions.MorphlingOn > 0) ColorMapping.Add("Morphling", Colors.Impostor);
                    if (CustomGameOptions.MinerOn > 0) ColorMapping.Add("Miner", Colors.Impostor);
                    if (CustomGameOptions.SwooperOn > 0) ColorMapping.Add("Swooper", Colors.Impostor);
                    if (CustomGameOptions.UndertakerOn > 0) ColorMapping.Add("Undertaker", Colors.Impostor);
                    if (CustomGameOptions.EscapistOn > 0) ColorMapping.Add("Escapist", Colors.Impostor);
                    if (CustomGameOptions.GrenadierOn > 0) ColorMapping.Add("Grenadier", Colors.Impostor);
                    if (CustomGameOptions.TraitorOn > 0) ColorMapping.Add("Traitor", Colors.Impostor);
                    if (CustomGameOptions.BlackmailerOn > 0) ColorMapping.Add("Blackmailer", Colors.Impostor);
                    if (CustomGameOptions.BomberOn > 0) ColorMapping.Add("Bomber", Colors.Impostor);
                    if (CustomGameOptions.WarlockOn > 0) ColorMapping.Add("Warlock", Colors.Impostor);
                    if (CustomGameOptions.VenererOn > 0) ColorMapping.Add("Venerer", Colors.Impostor);
                    if (CustomGameOptions.PoisonerOn > 0) ColorMapping.Add("Poisoner", Colors.Impostor);
                    if (CustomGameOptions.SniperOn > 0) ColorMapping.Add("Sniper", Colors.Impostor);
                    if (CustomGameOptions.ImpostorAgentOn > 0) ColorMapping.Add("Agent (Imp)", Colors.Impostor);
                }
                if (((CustomGameOptions.PlaguebearerOn > 0 && CustomGameOptions.GameMode != GameMode.Horseman && CustomGameOptions.AssassinGuessNeutralKilling) || (CustomGameOptions.GameMode == GameMode.Horseman && CustomGameOptions.AssassinGuessImpostors)) && !PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse)) ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
                if (((CustomGameOptions.BakerOn > 0 && CustomGameOptions.GameMode != GameMode.Horseman && CustomGameOptions.AssassinGuessNeutralKilling) || (CustomGameOptions.GameMode == GameMode.Horseman && CustomGameOptions.AssassinGuessImpostors)) && !PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse)) ColorMapping.Add("Baker", Colors.Baker);
                if (((CustomGameOptions.BerserkerOn > 0 && CustomGameOptions.GameMode != GameMode.Horseman && CustomGameOptions.AssassinGuessNeutralKilling) || (CustomGameOptions.GameMode == GameMode.Horseman && CustomGameOptions.AssassinGuessImpostors)) && !PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse)) ColorMapping.Add("Berserker", Colors.Berserker);
                if (((CustomGameOptions.SoulCollectorOn > 0 && CustomGameOptions.GameMode != GameMode.Horseman && CustomGameOptions.AssassinGuessNeutralKilling) || (CustomGameOptions.GameMode == GameMode.Horseman && CustomGameOptions.AssassinGuessImpostors)) && !PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse)) ColorMapping.Add("Soul Collector", Colors.SoulCollector);
                if (CustomGameOptions.ApocalypseAgentOn > 0 && (((CustomGameOptions.PlaguebearerOn > 0 || CustomGameOptions.BakerOn > 0 || CustomGameOptions.BerserkerOn > 0 || CustomGameOptions.SoulCollectorOn > 0) && CustomGameOptions.GameMode != GameMode.Horseman && CustomGameOptions.DoomsayerGuessNeutralKilling) || (CustomGameOptions.GameMode == GameMode.Horseman && CustomGameOptions.DoomsayerGuessImpostors)) && !PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse)) ColorMapping.Add("Agent (Apoc)", Colors.ApocalypseAgent);

                // Add vanilla crewmate if enabled
                if (CustomGameOptions.AssassinCrewmateGuess) ColorMapping.Add("Crewmate", Colors.Crewmate);
            }
            else if (CustomGameOptions.GameMode == GameMode.RoleList)
            {
                if (CustomGameOptions.AssassinCrewmateGuess) ColorMapping.Add("Crewmate", Colors.Crewmate);
                ColorMapping.Add("Mayor", Colors.Mayor);
                ColorMapping.Add("Sheriff", Colors.Sheriff);
                ColorMapping.Add("Engineer", Colors.Engineer);
                ColorMapping.Add("Swapper", Colors.Swapper);
                ColorMapping.Add("Investigator", Colors.Investigator);
                ColorMapping.Add("Medic", Colors.Medic);
                ColorMapping.Add("Seer", Colors.Seer);
                ColorMapping.Add("Spy", Colors.Spy);
                ColorMapping.Add("Snitch", Colors.Snitch);
                ColorMapping.Add("Altruist", Colors.Altruist);
                ColorMapping.Add("Vigilante", Colors.Vigilante);
                ColorMapping.Add("Veteran", Colors.Veteran);
                ColorMapping.Add("Hunter", Colors.Hunter);
                ColorMapping.Add("Tracker", Colors.Tracker);
                ColorMapping.Add("Trapper", Colors.Trapper);
                ColorMapping.Add("Transporter", Colors.Transporter);
                ColorMapping.Add("Medium", Colors.Medium);
                ColorMapping.Add("Mystic", Colors.Mystic);
                ColorMapping.Add("Detective", Colors.Detective);
                ColorMapping.Add("Imitator", Colors.Imitator);
                ColorMapping.Add("Vampire Hunter", Colors.VampireHunter);
                ColorMapping.Add("Prosecutor", Colors.Prosecutor);
                ColorMapping.Add("Oracle", Colors.Oracle);
                ColorMapping.Add("Aurial", Colors.Aurial);
                ColorMapping.Add("Inspector", Colors.Inspector);
                ColorMapping.Add("Monarch", Colors.Monarch);
                ColorMapping.Add("Tavern Keeper", Colors.TavernKeeper);
                ColorMapping.Add("Undercover", Colors.Undercover);
                ColorMapping.Add("Lookout", Colors.Lookout);
                ColorMapping.Add("Deputy", Colors.Deputy);
                ColorMapping.Add("Bodyguard", Colors.Bodyguard);
                ColorMapping.Add("Crusader", Colors.Crusader);
                ColorMapping.Add("Cleric", Colors.Cleric);

                if (CustomGameOptions.AssassinGuessImpostors && !PlayerControl.LocalPlayer.Is(Faction.Impostors))
                {
                    ColorMapping.Add("Impostor", Colors.Impostor);
                    ColorMapping.Add("Janitor", Colors.Impostor);
                    ColorMapping.Add("Morphling", Colors.Impostor);
                    ColorMapping.Add("Miner", Colors.Impostor);
                    ColorMapping.Add("Swooper", Colors.Impostor);
                    ColorMapping.Add("Undertaker", Colors.Impostor);
                    ColorMapping.Add("Escapist", Colors.Impostor);
                    ColorMapping.Add("Grenadier", Colors.Impostor);
                    ColorMapping.Add("Traitor", Colors.Impostor);
                    ColorMapping.Add("Blackmailer", Colors.Impostor);
                    ColorMapping.Add("Bomber", Colors.Impostor);
                    ColorMapping.Add("Warlock", Colors.Impostor);
                    ColorMapping.Add("Venerer", Colors.Impostor);
                    ColorMapping.Add("Poisoner", Colors.Impostor);
                    ColorMapping.Add("Sniper", Colors.Impostor);
                    if (CustomGameOptions.ImpostorAgentOn > 0) ColorMapping.Add("Agent (Imp)", Colors.ImpostorAgent);
                }

                if (CustomGameOptions.AssassinGuessNeutralBenign)
                {
                    ColorMapping.Add("Amnesiac", Colors.Amnesiac);
                    ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);
                    ColorMapping.Add("Cursed Soul", Colors.CursedSoul);
                    ColorMapping.Add("Survivor", Colors.Survivor);
                }
                if (CustomGameOptions.AssassinGuessNeutralEvil)
                {
                    ColorMapping.Add("Executioner", Colors.Executioner);
                    ColorMapping.Add("Jester", Colors.Jester);
                    ColorMapping.Add("Witch", Colors.Witch);
                }
                if (CustomGameOptions.AssassinGuessNeutralChaos)
                {
                    ColorMapping.Add("Doomsayer", Colors.Doomsayer);
                    ColorMapping.Add("Pirate", Colors.Pirate);
                    ColorMapping.Add("Inquisitor", Colors.Inquisitor);
                }
                if (CustomGameOptions.AssassinGuessNeutralKilling)
                {
                    if (!PlayerControl.LocalPlayer.Is(RoleEnum.Arsonist)) ColorMapping.Add("Arsonist", Colors.Arsonist);
                    if (!PlayerControl.LocalPlayer.Is(RoleEnum.Glitch)) ColorMapping.Add("The Glitch", Colors.Glitch);
                    if (!PlayerControl.LocalPlayer.Is(RoleEnum.Werewolf)) ColorMapping.Add("Werewolf", Colors.Werewolf);
                    if (!PlayerControl.LocalPlayer.Is(RoleEnum.Juggernaut)) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
                    if (!PlayerControl.LocalPlayer.Is(RoleEnum.SerialKiller)) ColorMapping.Add("Serial Killer", Colors.SerialKiller);
                    if (!PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse)) ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
                    if (!PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse)) ColorMapping.Add("Baker", Colors.Baker);
                    if (!PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse)) ColorMapping.Add("Berserker", Colors.Berserker);
                    if (!PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse)) ColorMapping.Add("Soul Collector", Colors.SoulCollector);
                    if (CustomGameOptions.ApocalypseAgentOn > 0 && !PlayerControl.LocalPlayer.Is(Faction.NeutralApocalypse)) ColorMapping.Add("Agent (Apoc)", Colors.ApocalypseAgent);
                }
                if (CustomGameOptions.AssassinGuessNeutralProselyte)
                {
                    if (!PlayerControl.LocalPlayer.Is(FactionOverride.Vampires)) ColorMapping.Add("Vampire", Colors.Vampire);
                    if (!PlayerControl.LocalPlayer.Is(FactionOverride.Recruit)) ColorMapping.Add("Jackal", Colors.Jackal);
                    if (!PlayerControl.LocalPlayer.Is(FactionOverride.Undead)) ColorMapping.Add("Necromancer", Colors.Necromancer);
                }
            }
            //Add modifiers if enabled
            if (CustomGameOptions.AssassinGuessModifiers && CustomGameOptions.BaitOn > 0) ColorMapping.Add("Bait", Colors.Bait);
            if (CustomGameOptions.AssassinGuessModifiers && CustomGameOptions.AftermathOn > 0) ColorMapping.Add("Aftermath", Colors.Aftermath);
            if (CustomGameOptions.AssassinGuessModifiers && CustomGameOptions.DiseasedOn > 0) ColorMapping.Add("Diseased", Colors.Diseased);
            if (CustomGameOptions.AssassinGuessModifiers && CustomGameOptions.FrostyOn > 0) ColorMapping.Add("Frosty", Colors.Frosty);
            if (CustomGameOptions.AssassinGuessModifiers && CustomGameOptions.MultitaskerOn > 0) ColorMapping.Add("Multitasker", Colors.Multitasker);
            if (CustomGameOptions.AssassinGuessModifiers && CustomGameOptions.TorchOn > 0) ColorMapping.Add("Torch", Colors.Torch);
            if (CustomGameOptions.AssassinGuessModifiers && CustomGameOptions.FamousOn > 0) ColorMapping.Add("Famous", Colors.Famous);
            if (CustomGameOptions.AssassinGuessLovers && CustomGameOptions.LoversOn > 0) ColorMapping.Add("Lover", Colors.Lovers);

            // Sorts the list alphabetically. 
            SortedColorMapping = ColorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public bool GuessedThisMeeting { get; set; } = false;

        public int RemainingKills { get; set; }

        public List<string> PossibleGuesses => SortedColorMapping.Keys.ToList();
    }
}
