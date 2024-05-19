using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.Patches;
using UnityEngine;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.GuardianAngelMod;
using System;
using TownOfUs.CrewmateRoles.VampireHunterMod;

namespace TownOfUs.Roles
{
    public class Doomsayer : Role, IGuesser
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> Buttons { get; set; } = new();

        private Dictionary<string, Color> ColorMapping = new();

        public Dictionary<string, Color> SortedColorMapping;

        public Dictionary<byte, string> Guesses = new();
        public DateTime LastObserved;
        public PlayerControl ClosestPlayer;
        public PlayerControl LastObservedPlayer;

        public Doomsayer(PlayerControl player) : base(player)
        {
            Name = "Doomsayer";
            ImpostorText = () => "Guess People's Roles To Win!";
            TaskText = () => "Win by guessing player's roles\nFake Tasks:";
            Color = Patches.Colors.Doomsayer;
            RoleType = RoleEnum.Doomsayer;
            LastObserved = DateTime.UtcNow;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralChaos;

            if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.AllAny || CustomGameOptions.GameMode == GameMode.Horseman)
            {
                ColorMapping.Add("Crewmate", Colors.Crewmate);
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
                if (CustomGameOptions.HunterOn > 0 || (CustomGameOptions.VampireHunterOn > 0 && (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Horseman) && (CustomGameOptions.VampireOn > 0 || CustomGameOptions.NecromancerOn > 0) && CustomGameOptions.BecomeOnVampDeaths == BecomeEnum.Hunter)) ColorMapping.Add("Hunter", Colors.Hunter);
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
                if (CustomGameOptions.DeputyOn > 0) ColorMapping.Add("Deputy", Colors.Deputy);
                if (CustomGameOptions.BodyguardOn > 0) ColorMapping.Add("Bodyguard", Colors.Bodyguard);
                if (CustomGameOptions.CrusaderOn > 0) ColorMapping.Add("Crusader", Colors.Crusader);
                if (CustomGameOptions.ClericOn > 0) ColorMapping.Add("Cleric", Colors.Cleric);

                if (CustomGameOptions.DoomsayerGuessImpostors && !PlayerControl.LocalPlayer.Is(Faction.Impostors))
                {
                    if (CustomGameOptions.GameMode != GameMode.Horseman)
                    {
                        ColorMapping.Add("Impostor", Colors.Impostor);
                        if (CustomGameOptions.JanitorOn > 0) ColorMapping.Add("Janitor", Colors.Impostor);
                        if (CustomGameOptions.MorphlingOn > 0) ColorMapping.Add("Morphling", Colors.Impostor);
                        if (CustomGameOptions.MinerOn > 0) ColorMapping.Add(GameOptionsManager.Instance.currentNormalGameOptions.MapId == 5 ? "Mycologist" : "Miner", Colors.Impostor);
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
                }

                if (CustomGameOptions.DoomsayerGuessNeutralBenign)
                {
                    if (CustomGameOptions.AmnesiacOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Amnesiac) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Amnesiac) || (CustomGameOptions.CursedSoulOn > 0 && CustomGameOptions.SwappedBecomes == SwappedBecomes.Amnesiac)) ColorMapping.Add("Amnesiac", Colors.Amnesiac);
                    if (CustomGameOptions.GuardianAngelOn > 0) ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);
                    if (CustomGameOptions.SurvivorOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Survivor) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Survivor) || (CustomGameOptions.CursedSoulOn > 0 && (CustomGameOptions.SwappedBecomes == SwappedBecomes.Survivor || CustomGameOptions.SwappedBecomes == SwappedBecomes.DefaultRole))) ColorMapping.Add("Cursed Soul", Colors.CursedSoul);
                    if (CustomGameOptions.CursedSoulOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.CursedSoul) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.CursedSoul)) ColorMapping.Add("Survivor", Colors.Survivor);
                }
                if (CustomGameOptions.DoomsayerGuessNeutralEvil)
                {
                    if (CustomGameOptions.ExecutionerOn > 0) ColorMapping.Add("Executioner", Colors.Executioner);
                    if (CustomGameOptions.JesterOn > 0 || (CustomGameOptions.ExecutionerOn > 0 && CustomGameOptions.OnTargetDead == OnTargetDead.Jester) || (CustomGameOptions.GuardianAngelOn > 0 && CustomGameOptions.GaOnTargetDeath == BecomeOptions.Jester) || (CustomGameOptions.CursedSoulOn > 0 && CustomGameOptions.SwappedBecomes == SwappedBecomes.Jester)) ColorMapping.Add("Jester", Colors.Jester);
                    if (CustomGameOptions.WitchOn > 0) ColorMapping.Add("Witch", Colors.Witch);
                }
                if (CustomGameOptions.DoomsayerGuessNeutralChaos)
                {
                    if (CustomGameOptions.GameMode == GameMode.AllAny) ColorMapping.Add("Doomsayer", Colors.Doomsayer);
                    if (CustomGameOptions.PirateOn > 0) ColorMapping.Add("Pirate", Colors.Pirate);
                    if (CustomGameOptions.InquisitorOn > 0) ColorMapping.Add("Inquisitor", Colors.Inquisitor);
                }
                if (CustomGameOptions.DoomsayerGuessNeutralKilling)
                {
                    if (CustomGameOptions.ArsonistOn > 0) ColorMapping.Add("Arsonist", Colors.Arsonist);
                    if (CustomGameOptions.GlitchOn > 0) ColorMapping.Add("The Glitch", Colors.Glitch);
                    if (CustomGameOptions.WerewolfOn > 0) ColorMapping.Add("Werewolf", Colors.Werewolf);
                    if (CustomGameOptions.JuggernautOn > 0 && CustomGameOptions.GameMode != GameMode.Horseman) ColorMapping.Add("Juggernaut", Colors.Juggernaut);
                    if (CustomGameOptions.SerialKillerOn > 0) ColorMapping.Add("Serial Killer", Colors.SerialKiller);
                }
                if (CustomGameOptions.DoomsayerGuessNeutralProselyte)
                {
                    if ((CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Horseman) && CustomGameOptions.VampireOn > 0) ColorMapping.Add("Vampire", Colors.Vampire);
                    if (CustomGameOptions.JackalOn > 0) ColorMapping.Add("Jackal", Colors.Jackal);
                    if (CustomGameOptions.NecromancerOn > 0) ColorMapping.Add("Necromancer", Colors.Necromancer);
                }
                if ((CustomGameOptions.PlaguebearerOn > 0 && CustomGameOptions.GameMode != GameMode.Horseman && CustomGameOptions.DoomsayerGuessNeutralKilling) || (CustomGameOptions.GameMode == GameMode.Horseman && CustomGameOptions.DoomsayerGuessImpostors)) ColorMapping.Add("Plaguebearer", Colors.Plaguebearer);
                if ((CustomGameOptions.BakerOn > 0 && CustomGameOptions.GameMode != GameMode.Horseman && CustomGameOptions.DoomsayerGuessNeutralKilling) || (CustomGameOptions.GameMode == GameMode.Horseman && CustomGameOptions.DoomsayerGuessImpostors)) ColorMapping.Add("Baker", Colors.Baker);
                if ((CustomGameOptions.PlaguebearerOn > 0 && CustomGameOptions.GameMode != GameMode.Horseman && CustomGameOptions.DoomsayerGuessNeutralKilling) || (CustomGameOptions.GameMode == GameMode.Horseman && CustomGameOptions.DoomsayerGuessImpostors)) ColorMapping.Add("Berserker", Colors.Berserker);
                if ((CustomGameOptions.PlaguebearerOn > 0 && CustomGameOptions.GameMode != GameMode.Horseman && CustomGameOptions.DoomsayerGuessNeutralKilling) || (CustomGameOptions.GameMode == GameMode.Horseman && CustomGameOptions.DoomsayerGuessImpostors)) ColorMapping.Add("Soul Collector", Colors.SoulCollector);
                if (CustomGameOptions.ApocalypseAgentOn > 0 && (((CustomGameOptions.PlaguebearerOn > 0 || CustomGameOptions.BakerOn > 0 || CustomGameOptions.BerserkerOn > 0 || CustomGameOptions.SoulCollectorOn > 0) && CustomGameOptions.GameMode != GameMode.Horseman && CustomGameOptions.DoomsayerGuessNeutralKilling) || (CustomGameOptions.GameMode == GameMode.Horseman && CustomGameOptions.DoomsayerGuessImpostors))) ColorMapping.Add("Agent (Apoc)", Colors.ApocalypseAgent);
            }
            else if (CustomGameOptions.GameMode == GameMode.RoleList)
            {
                ColorMapping.Add("Crewmate", Colors.Crewmate);
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

                if (CustomGameOptions.DoomsayerGuessImpostors && !PlayerControl.LocalPlayer.Is(Faction.Impostors))
                {
                    ColorMapping.Add("Impostor", Colors.Impostor);
                    ColorMapping.Add("Janitor", Colors.Impostor);
                    ColorMapping.Add("Morphling", Colors.Impostor);
                    ColorMapping.Add(GameOptionsManager.Instance.currentNormalGameOptions.MapId == 5 ? "Mycologist" : "Miner", Colors.Impostor);
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

                if (CustomGameOptions.DoomsayerGuessNeutralBenign)
                {
                    ColorMapping.Add("Amnesiac", Colors.Amnesiac);
                    ColorMapping.Add("Guardian Angel", Colors.GuardianAngel);
                    ColorMapping.Add("Cursed Soul", Colors.CursedSoul);
                    ColorMapping.Add("Survivor", Colors.Survivor);
                }
                if (CustomGameOptions.DoomsayerGuessNeutralEvil)
                {
                    ColorMapping.Add("Executioner", Colors.Executioner);
                    ColorMapping.Add("Jester", Colors.Jester);
                    ColorMapping.Add("Witch", Colors.Witch);
                }
                if (CustomGameOptions.DoomsayerGuessNeutralChaos)
                {
                    if (!CustomGameOptions.AllUnique) ColorMapping.Add("Doomsayer", Colors.Doomsayer);
                    ColorMapping.Add("Pirate", Colors.Pirate);
                    ColorMapping.Add("Inquisitor", Colors.Inquisitor);
                }
                if (CustomGameOptions.DoomsayerGuessNeutralKilling)
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
                if (CustomGameOptions.DoomsayerGuessNeutralProselyte)
                {
                    ColorMapping.Add("Vampire", Colors.Vampire);
                    ColorMapping.Add("Jackal", Colors.Jackal);
                    ColorMapping.Add("Necromancer", Colors.Necromancer);
                }
            }

            SortedColorMapping = ColorMapping.OrderBy(x => x.Key).ToDictionary(x => x.Key, x => x.Value);
        }

        public float ObserveTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastObserved;
            var num = CustomGameOptions.ObserveCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public int GuessedCorrectly = 0;
        public bool WonByGuessing = false;

        public List<string> PossibleGuesses => SortedColorMapping.Keys.ToList();

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var doomTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            doomTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = doomTeam;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead) return true;
            if (!CustomGameOptions.NeutralEvilWinEndsGame) return true;
            if (!WonByGuessing) return true;
            if (!Player.Is(FactionOverride.None)) return true;
            Utils.EndGame();
            return false;
        }
    }
}
