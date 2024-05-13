using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.Patches;
using UnityEngine;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.GuardianAngelMod;

namespace TownOfUs.Roles
{
    public class Vigilante : Role, IGuesser
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> Buttons { get; set; } = new();

        public Dictionary<string, Color> ColorMapping = new();

        public Dictionary<string, Color> SortedColorMapping;

        public Dictionary<byte, string> Guesses = new();

        public Vigilante(PlayerControl player) : base(player)
        {
            Name = "Vigilante";
            ImpostorText = () => "Kill Impostors If You Can Guess Their Roles";
            TaskText = () => "Guess the roles of impostors mid-meeting to kill them!";
            Color = Patches.Colors.Vigilante;
            RoleType = RoleEnum.Vigilante;
            AddToRoleHistory(RoleType);

            RemainingKills = CustomGameOptions.VigilanteKills;

            if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.AllAny || CustomGameOptions.GameMode == GameMode.Horseman)
            {
                if (CustomGameOptions.GameMode != GameMode.Horseman)
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
                    if (CustomGameOptions.ImpostorAgentOn > 0) ColorMapping.Add("Agent (Imp)", Colors.ImpostorAgent);
                }

                if (CustomGameOptions.VigilanteGuessNeutralBenign)
                {
                    if (CustomGameOptions.AmnesiacOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Amnesiac) || (CustomGameOptions.CursedSoulOn > 0 && CustomGameOptions.SwappedBecomes == SwappedBecomes.Amnesiac)) ColorMapping.Add("Amnesiac", Colors.Amnesiac);
                    if (CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);
                    if (CustomGameOptions.SurvivorOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Survivor) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Survivor) || (CustomGameOptions.CursedSoulOn > 0 && (CustomGameOptions.SwappedBecomes == SwappedBecomes.Survivor || CustomGameOptions.SwappedBecomes == SwappedBecomes.DefaultRole))) ColorMapping.Add("Cursed Soul", Colors.CursedSoul);
                    if (CustomGameOptions.CursedSoulOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.CursedSoul) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.CursedSoul)) ColorMapping.Add("Survivor", Colors.Survivor);
                }
                if (CustomGameOptions.VigilanteGuessNeutralEvil)
                {
                    if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", Colors.Executioner);
                    if (CustomGameOptions.JesterOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Jester) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Jester) || (CustomGameOptions.CursedSoulOn > 0 && CustomGameOptions.SwappedBecomes == SwappedBecomes.Jester)) ColorMapping.Add("Jester", Colors.Jester);
                    if (CustomGameOptions.WitchOn > 0) ColorMapping.Add("Witch", Colors.Witch);
                }
                if (CustomGameOptions.VigilanteGuessNeutralChaos)
                {
                    if (CustomGameOptions.DoomsayerOn > 0) ColorMapping.Add("Doomsayer", Colors.Doomsayer);
                    if (CustomGameOptions.PirateOn > 0) ColorMapping.Add("Pirate", Colors.Pirate);
                    if (CustomGameOptions.InquisitorOn > 0) ColorMapping.Add("Inquisitor", Colors.Inquisitor);
                }
                if (CustomGameOptions.VigilanteGuessNeutralKilling)
                {
                    if (CustomGameOptions.ArsonistOn > 0) ColorMapping.Add("Arsonist", Colors.Arsonist);
                    if (CustomGameOptions.GlitchOn > 0) ColorMapping.Add("The Glitch", Colors.Glitch);
                    if (CustomGameOptions.WerewolfOn > 0) ColorMapping.Add("Werewolf", Colors.Werewolf);
                    if (CustomGameOptions.HiddenRoles) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
                    if (CustomGameOptions.SerialKillerOn > 0) ColorMapping.Add("Serial Killer", Colors.SerialKiller);
                }
                if (CustomGameOptions.VigilanteGuessNeutralProselyte)
                {
                    if ((CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Horseman) && CustomGameOptions.VampireOn > 0) ColorMapping.Add("Vampire", Colors.Vampire);
                    if (CustomGameOptions.JackalOn > 0) ColorMapping.Add("Jackal", Colors.Jackal);
                    if (CustomGameOptions.NecromancerOn > 0) ColorMapping.Add("Necromancer", Colors.Necromancer);
                }
                if ((CustomGameOptions.PlaguebearerOn > 0 && CustomGameOptions.VigilanteGuessNeutralKilling) || CustomGameOptions.GameMode == GameMode.Horseman) ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
                if ((CustomGameOptions.BakerOn > 0 && CustomGameOptions.VigilanteGuessNeutralKilling) || CustomGameOptions.GameMode == GameMode.Horseman) ColorMapping.Add("Baker", Colors.Baker);
                if ((CustomGameOptions.BerserkerOn > 0 && CustomGameOptions.VigilanteGuessNeutralKilling) || CustomGameOptions.GameMode == GameMode.Horseman) ColorMapping.Add("Berserker", Colors.Berserker);
                if ((CustomGameOptions.SoulCollectorOn > 0 && CustomGameOptions.VigilanteGuessNeutralKilling) || CustomGameOptions.GameMode == GameMode.Horseman) ColorMapping.Add("Soul Collector", Colors.SoulCollector);
                if (CustomGameOptions.ApocalypseAgentOn > 0 && (((CustomGameOptions.PlaguebearerOn > 0 || CustomGameOptions.BakerOn > 0 || CustomGameOptions.BerserkerOn > 0 || CustomGameOptions.SoulCollectorOn > 0) && CustomGameOptions.VigilanteGuessNeutralKilling) || CustomGameOptions.GameMode == GameMode.Horseman)) ColorMapping.Add("Agent (Apoc)", Colors.ApocalypseAgent);
                if (CustomGameOptions.VigilanteGuessLovers && CustomGameOptions.LoversOn > 0) ColorMapping.Add("Lover", Colors.Lovers);
            }
            else if (CustomGameOptions.GameMode == GameMode.KillingOnly)
            {
                ColorMapping.Add("Morphling", Colors.Impostor);
                ColorMapping.Add("Miner", Colors.Impostor);
                ColorMapping.Add("Swooper", Colors.Impostor);
                ColorMapping.Add("Undertaker", Colors.Impostor);
                ColorMapping.Add("Grenadier", Colors.Impostor);
                ColorMapping.Add("Traitor", Colors.Impostor);
                ColorMapping.Add("Escapist", Colors.Impostor);

                if (CustomGameOptions.VigilanteGuessNeutralKilling)
                {
                    if (CustomGameOptions.AddArsonist) ColorMapping.Add("Arsonist", Colors.Arsonist);
                    if (CustomGameOptions.AddPlaguebearer) ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
                    ColorMapping.Add("The Glitch", Colors.Glitch);
                    ColorMapping.Add("Werewolf", Colors.Werewolf);
                    if (CustomGameOptions.HiddenRoles) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
                }
            }
            else if (CustomGameOptions.GameMode == GameMode.RoleList)
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

                if (CustomGameOptions.VigilanteGuessNeutralBenign)
                {
                    ColorMapping.Add("Amnesiac", Colors.Amnesiac);
                    ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);
                    ColorMapping.Add("Cursed Soul", Colors.CursedSoul);
                    ColorMapping.Add("Survivor", Colors.Survivor);
                }
                if (CustomGameOptions.VigilanteGuessNeutralEvil)
                {
                    ColorMapping.Add("Executioner", Colors.Executioner);
                    ColorMapping.Add("Jester", Colors.Jester);
                    ColorMapping.Add("Witch", Colors.Witch);
                }
                if (CustomGameOptions.VigilanteGuessNeutralChaos)
                {
                    ColorMapping.Add("Doomsayer", Colors.Doomsayer);
                    ColorMapping.Add("Pirate", Colors.Pirate);
                    ColorMapping.Add("Inquisitor", Colors.Inquisitor);
                }
                if (CustomGameOptions.VigilanteGuessNeutralKilling)
                {
                    ColorMapping.Add("Arsonist", Colors.Arsonist);
                    ColorMapping.Add("The Glitch", Colors.Glitch);
                    ColorMapping.Add("Werewolf", Colors.Werewolf);
                    ColorMapping.Add("Juggernaut", Colors.Juggernaut);
                    ColorMapping.Add("Serial Killer", Colors.SerialKiller);
                    ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
                    ColorMapping.Add("Baker", Colors.Baker);
                    ColorMapping.Add("Berserker", Colors.Berserker);
                    ColorMapping.Add("Soul Collector", Colors.SoulCollector);
                    if (CustomGameOptions.ApocalypseAgentOn > 0) ColorMapping.Add("Agent (Apoc)", Colors.ApocalypseAgent);
                }
                if (CustomGameOptions.VigilanteGuessNeutralProselyte)
                {
                    ColorMapping.Add("Vampire", Colors.Vampire);
                    ColorMapping.Add("Jackal", Colors.Jackal);
                    ColorMapping.Add("Necromancer", Colors.Necromancer);
                }
            }
            else
            {
                ColorMapping.Add("Necromancer", Colors.Impostor);
                ColorMapping.Add("Whisperer", Colors.Impostor);
                if (CustomGameOptions.MaxChameleons > 0) ColorMapping.Add("Swooper", Colors.Impostor);
                if (CustomGameOptions.MaxEngineers > 0) ColorMapping.Add("Demolitionist", Colors.Impostor);
                if (CustomGameOptions.MaxInvestigators > 0) ColorMapping.Add("Consigliere", Colors.Impostor);
                if (CustomGameOptions.MaxMystics > 0) ColorMapping.Add("Clairvoyant", Colors.Impostor);
                if (CustomGameOptions.MaxSnitches > 0) ColorMapping.Add("Informant", Colors.Impostor);
                if (CustomGameOptions.MaxSpies > 0) ColorMapping.Add("Rogue Agent", Colors.Impostor);
                if (CustomGameOptions.MaxTransporters > 0) ColorMapping.Add("Escapist", Colors.Impostor);
                if (CustomGameOptions.MaxVigilantes > 1) ColorMapping.Add("Assassin", Colors.Impostor);
                ColorMapping.Add("Impostor", Colors.Impostor);
            }

            SortedColorMapping = ColorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public bool GuessedThisMeeting { get; set; } = false;

        public int RemainingKills { get; set; }

        public List<string> PossibleGuesses => SortedColorMapping.Keys.ToList();
    }
}
