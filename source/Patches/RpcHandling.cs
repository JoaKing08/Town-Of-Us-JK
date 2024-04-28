using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using Reactor.Utilities.Extensions;
using Reactor.Networking.Extensions;
using TownOfUs.CrewmateRoles.AltruistMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.CrewmateRoles.SwapperMod;
using TownOfUs.CrewmateRoles.VigilanteMod;
using TownOfUs.NeutralRoles.DoomsayerMod;
using TownOfUs.CultistRoles.NecromancerMod;
using TownOfUs.CustomOption;
using TownOfUs.Extensions;
using TownOfUs.Modifiers.AssassinMod;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.GuardianAngelMod;
using TownOfUs.ImpostorRoles.MinerMod;
using TownOfUs.CrewmateRoles.HaunterMod;
using TownOfUs.NeutralRoles.PhantomMod;
using TownOfUs.ImpostorRoles.TraitorMod;
using TownOfUs.CrewmateRoles.ImitatorMod;
using TownOfUs.ImpostorRoles.PoltergeistMod;
using TownOfUs.NeutralRoles.CursedSoulMod;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.Roles;
using TownOfUs.Roles.Cultist;
using TownOfUs.Roles.Modifiers;
using TownOfUs.Roles.Horseman;
using TownOfUs.CrewmateRoles.SeerMod;
using UnityEngine;
using Coroutine = TownOfUs.ImpostorRoles.JanitorMod.Coroutine;
using Object = UnityEngine.Object;
using PerformKillButton = TownOfUs.NeutralRoles.AmnesiacMod.PerformKillButton;
using Random = UnityEngine.Random;
using TownOfUs.Patches;
using AmongUs.GameOptions;
using TownOfUs.NeutralRoles.VampireMod;
using TownOfUs.CrewmateRoles.MayorMod;
using System.Reflection;
using TownOfUs.Patches.NeutralRoles;
using BepInEx.Logging;
using TownOfUs.Roles.Teams;
using TownOfUs.NeutralRoles.PirateMod;

namespace TownOfUs
{
    public static class RpcHandling
    {
        private static readonly List<(Type, int, bool)> CrewmatesRoles = new();
        private static readonly List<(Type, int, bool)> NeutralBenignRoles = new();
        private static readonly List<(Type, int, bool)> NeutralEvilRoles = new();
        private static readonly List<(Type, int, bool)> NeutralChaosRoles = new();
        private static readonly List<(Type, int, bool)> NeutralKillingRoles = new();
        private static readonly List<(Type, int, bool)> NeutralApocalypseRoles = new();
        private static readonly List<(Type, int, bool)> ImpostorsRoles = new();
        private static readonly List<(Type, int)> ObjectiveCrewmateModifiers = new();
        private static readonly List<(Type, int)> ObjectiveGlobalModifiers = new();
        private static readonly List<(Type, int)> CrewmateModifiers = new();
        private static readonly List<(Type, int)> GlobalModifiers = new();
        private static readonly List<(Type, int)> ImpostorModifiers = new();
        private static readonly List<(Type, int)> ButtonModifiers = new();
        private static readonly List<(Type, int)> AssassinModifiers = new();
        private static readonly List<(Type, CustomRPC, int)> AssassinAbility = new();
        private static bool PhantomOn;
        private static bool HaunterOn;
        private static bool TraitorOn;
        private static bool PoltergeistOn;

        internal static bool Check(int probability)
        {
            if (probability == 0) return false;
            if (probability == 100) return true;
            var num = Random.RandomRangeInt(1, 101);
            return num <= probability;
        }
        internal static bool CheckJugg()
        {
            var num = Random.RandomRangeInt(1, 101);
            return num <= 10 * CustomGameOptions.MaxNeutralKillingRoles;
        }
        private static int PickRoleCount(int min, int max)
        {
            if (min > max) min = max;
            return Random.RandomRangeInt(min, max + 1);
        }

        private static void SortRoles(this List<(Type, int, bool)> roles, int max)
        {
            if (max <= 0)
            {
                roles.Clear();
                return;
            }

            var chosenRoles = roles.Where(x => x.Item2 == 100).ToList();
            // Shuffle to ensure that the same 100% roles do not appear in
            // every game if there are more than the maximum.
            chosenRoles.Shuffle();
            // Truncate the list if there are more 100% roles than the max.
            chosenRoles = chosenRoles.GetRange(0, Math.Min(max, chosenRoles.Count));

            if (chosenRoles.Count < max)
            {
                // These roles MAY appear in this game, but they may not.
                var potentialRoles = roles.Where(x => x.Item2 < 100).ToList();
                // Determine which roles appear in this game.
                var optionalRoles = potentialRoles.Where(x => Check(x.Item2)).ToList();
                potentialRoles = potentialRoles.Where(x => !optionalRoles.Contains(x)).ToList();

                optionalRoles.Shuffle();
                chosenRoles.AddRange(optionalRoles.GetRange(0, Math.Min(max - chosenRoles.Count, optionalRoles.Count)));

                // If there are not enough roles after that, randomly add
                // ones which were previously eliminated, up to the max.
                if (chosenRoles.Count < max)
                {
                    potentialRoles.Shuffle();
                    chosenRoles.AddRange(potentialRoles.GetRange(0, Math.Min(max - chosenRoles.Count, potentialRoles.Count)));
                }
            }

            // This list will be shuffled later in GenEachRole.
            roles.Clear();
            roles.AddRange(chosenRoles);
        }

        private static void SortModifiers(this List<(Type, int)> roles, int max)
        {
            var newList = roles.Where(x => x.Item2 == 100).ToList();
            newList.Shuffle();

            if (roles.Count < max)
                max = roles.Count;

            var roles2 = roles.Where(x => x.Item2 < 100).ToList();
            roles2.Shuffle();
            newList.AddRange(roles2.Where(x => Check(x.Item2)));

            while (newList.Count > max)
            {
                newList.Shuffle();
                newList.RemoveAt(newList.Count - 1);
            }

            roles = newList;
            roles.Shuffle();
        }

        private static void GenEachRole(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            // I do not shuffle impostors/crewmates because roles should be shuffled before they are assigned to them anyway.
            // Assigning shuffled roles across a shuffled list may mess with the statistics? I dunno, I didn't major in math.
            // One Fisher-Yates shuffle should have statistically equal permutation probability on its own, anyway.

            var crewRoles = new List<(Type, int, bool)>();
            var neutRoles = new List<(Type, int, bool)>();
            var impRoles = new List<(Type, int, bool)>();
            bool[] ChooseApocs = new bool[4] { true, true, true, true };

            if (CustomGameOptions.GameMode == GameMode.Classic)
            {
                var benign = PickRoleCount(CustomGameOptions.MinNeutralBenignRoles, Math.Min(CustomGameOptions.MaxNeutralBenignRoles, NeutralBenignRoles.Count));
                var evil = PickRoleCount(CustomGameOptions.MinNeutralEvilRoles, Math.Min(CustomGameOptions.MaxNeutralEvilRoles, NeutralEvilRoles.Count));
                var chaos = PickRoleCount(CustomGameOptions.MinNeutralChaosRoles, Math.Min(CustomGameOptions.MaxNeutralChaosRoles, NeutralChaosRoles.Count));
                var killing = PickRoleCount(CustomGameOptions.MinNeutralKillingRoles, Math.Min(CustomGameOptions.MaxNeutralKillingRoles, NeutralKillingRoles.Count));
                var apocalypse = PickRoleCount(CustomGameOptions.MinNeutralApocalypseRoles, Math.Min(CustomGameOptions.MaxNeutralApocalypseRoles, NeutralApocalypseRoles.Count));

                var canSubtract = (int faction, int minFaction) => { return faction > minFaction; };
                var factions = new List<string>() { "Benign", "Evil", "Chaos", "Killing", "Apocalypse" };

                // Crew must always start out outnumbering neutrals, so subtract roles until that can be guaranteed.
                while (Math.Ceiling((double)crewmates.Count/2) <= benign + evil + chaos + killing)
                {
                    bool canSubtractBenign = canSubtract(benign, CustomGameOptions.MinNeutralBenignRoles);
                    bool canSubtractEvil = canSubtract(evil, CustomGameOptions.MinNeutralEvilRoles);
                    bool canSubtractChaos = canSubtract(chaos, CustomGameOptions.MinNeutralChaosRoles);
                    bool canSubtractKilling = canSubtract(killing, CustomGameOptions.MinNeutralKillingRoles);
                    bool canSubtractApocalypse = canSubtract(apocalypse, CustomGameOptions.MinNeutralApocalypseRoles);
                    bool canSubtractNone = !canSubtractBenign && !canSubtractEvil && !canSubtractChaos && !canSubtractKilling && !canSubtractApocalypse;

                    factions.Shuffle();
                    switch(factions.First())
                    {
                        case "Benign":
                            if (benign > 0 && (canSubtractBenign || canSubtractNone))
                            {
                                benign -= 1;
                                break;
                            }
                            goto case "Evil";
                        case "Evil":
                            if (evil > 0 && (canSubtractEvil || canSubtractNone))
                            {
                                evil -= 1;
                                break;
                            }
                            goto case "Chaos";
                        case "Chaos":
                            if (chaos > 0 && (canSubtractChaos || canSubtractNone))
                            {
                                chaos -= 1;
                                break;
                            }
                            goto case "Killing";
                        case "Killing":
                            if (killing > 0 && (canSubtractKilling || canSubtractNone))
                            {
                                killing -= 1;
                                break;
                            }
                            goto case "Apocalypse";
                        case "Apocalypse":
                            if (apocalypse > 0 && (canSubtractApocalypse || canSubtractNone))
                            {
                                apocalypse -= 1;
                                break;
                            }
                            goto default;
                        default:
                            if (benign > 0)
                            {
                                benign -= 1;
                            }
                            else if (evil > 0)
                            {
                                evil -= 1;
                            }
                            else if (chaos > 0)
                            {
                                chaos -= 1;
                            }
                            else if (killing > 0)
                            {
                                killing -= 1;
                            }
                            else if (apocalypse > 0)
                            {
                                apocalypse -= 1;
                            }
                            break;
                    }

                    if (benign + evil + killing == 0)
                        break;
                }

                NeutralBenignRoles.SortRoles(benign);
                NeutralEvilRoles.SortRoles(evil);
                NeutralChaosRoles.SortRoles(chaos);
                NeutralKillingRoles.SortRoles(killing);
                NeutralApocalypseRoles.SortRoles(apocalypse);

                if (NeutralKillingRoles.Contains((typeof(Vampire), CustomGameOptions.VampireOn, true)) && CustomGameOptions.VampireHunterOn > 0)
                    CrewmatesRoles.Add((typeof(VampireHunter), CustomGameOptions.VampireHunterOn, true));
                if (NeutralApocalypseRoles.Count > 0 && CustomGameOptions.ApocalypseAgentOn > 0)
                    ObjectiveCrewmateModifiers.Add((typeof(ApocalypseAgent), CustomGameOptions.ApocalypseAgentOn));

                CrewmatesRoles.SortRoles(crewmates.Count - NeutralBenignRoles.Count - NeutralEvilRoles.Count - NeutralKillingRoles.Count - NeutralApocalypseRoles.Count - NeutralChaosRoles.Count);
                ImpostorsRoles.SortRoles(impostors.Count);

                crewRoles.AddRange(CrewmatesRoles);
                impRoles.AddRange(ImpostorsRoles);
            }

            if (CustomGameOptions.GameMode == GameMode.Horseman)
            {
                var apocalypse = Random.Range(CustomGameOptions.MinNeutralApocalypseRoles == 0 ? 1 : CustomGameOptions.MinNeutralApocalypseRoles, CustomGameOptions.MaxNeutralApocalypseRoles == 0 ? 2 : CustomGameOptions.MaxNeutralApocalypseRoles + 1);

                var benign = PickRoleCount(CustomGameOptions.MinNeutralBenignRoles, Math.Min(CustomGameOptions.MaxNeutralBenignRoles, NeutralBenignRoles.Count));
                var evil = PickRoleCount(CustomGameOptions.MinNeutralEvilRoles, Math.Min(CustomGameOptions.MaxNeutralEvilRoles, NeutralEvilRoles.Count));
                var chaos = PickRoleCount(CustomGameOptions.MinNeutralChaosRoles, Math.Min(CustomGameOptions.MaxNeutralChaosRoles, NeutralChaosRoles.Count));
                var killing = PickRoleCount(CustomGameOptions.MinNeutralKillingRoles, Math.Min(CustomGameOptions.MaxNeutralKillingRoles, NeutralKillingRoles.Count));

                var canSubtract = (int faction, int minFaction) => { return faction > minFaction; };
                var factions = new List<string>() { "Benign", "Evil", "Chaos", "Killing" };

                // Crew must always start out outnumbering neutrals, so subtract roles until that can be guaranteed.
                while (Math.Ceiling((double)crewmates.Count / 2) <= benign + evil + killing)
                {
                    bool canSubtractBenign = canSubtract(benign, CustomGameOptions.MinNeutralBenignRoles);
                    bool canSubtractEvil = canSubtract(evil, CustomGameOptions.MinNeutralEvilRoles);
                    bool canSubtractChaos = canSubtract(chaos, CustomGameOptions.MinNeutralChaosRoles);
                    bool canSubtractKilling = canSubtract(killing, CustomGameOptions.MinNeutralKillingRoles);
                    bool canSubtractNone = !canSubtractBenign && !canSubtractEvil && !canSubtractChaos && !canSubtractKilling;

                    factions.Shuffle();
                    switch (factions.First())
                    {
                        case "Benign":
                            if (benign > 0 && (canSubtractBenign || canSubtractNone))
                            {
                                benign -= 1;
                                break;
                            }
                            goto case "Evil";
                        case "Evil":
                            if (evil > 0 && (canSubtractEvil || canSubtractNone))
                            {
                                evil -= 1;
                                break;
                            }
                            goto case "Chaos";
                        case "Chaos":
                            if (chaos > 0 && (canSubtractChaos || canSubtractNone))
                            {
                                chaos -= 1;
                                break;
                            }
                            goto case "Killing";
                        case "Killing":
                            if (killing > 0 && (canSubtractKilling || canSubtractNone))
                            {
                                killing -= 1;
                                break;
                            }
                            goto default;
                        default:
                            if (benign > 0)
                            {
                                benign -= 1;
                            }
                            else if (evil > 0)
                            {
                                evil -= 1;
                            }
                            else if (chaos > 0)
                            {
                                chaos -= 1;
                            }
                            else if (killing > 0)
                            {
                                killing -= 1;
                            }
                            break;
                    }

                    if (benign + evil + killing == 0)
                        break;
                }

                NeutralBenignRoles.SortRoles(benign);
                NeutralEvilRoles.SortRoles(evil);
                NeutralChaosRoles.SortRoles(chaos);
                NeutralKillingRoles.SortRoles(killing);
                NeutralApocalypseRoles.SortRoles(apocalypse);

                if (NeutralKillingRoles.Contains((typeof(Vampire), CustomGameOptions.VampireOn, true)) && CustomGameOptions.VampireHunterOn > 0)
                    CrewmatesRoles.Add((typeof(VampireHunter), CustomGameOptions.VampireHunterOn, true));

                CrewmatesRoles.SortRoles(crewmates.Count - NeutralBenignRoles.Count - NeutralEvilRoles.Count - NeutralKillingRoles.Count - apocalypse - NeutralChaosRoles.Count);
                ImpostorsRoles.SortRoles(impostors.Count);

                crewRoles.AddRange(CrewmatesRoles);
                impRoles.AddRange(ImpostorsRoles);
            }
            neutRoles.AddRange(NeutralBenignRoles);
            neutRoles.AddRange(NeutralEvilRoles);
            neutRoles.AddRange(NeutralChaosRoles);
            neutRoles.AddRange(NeutralKillingRoles);
            neutRoles.AddRange(NeutralApocalypseRoles);
            // Roles are not, at this point, shuffled yet.

            // In All/Any mode, there is at least one neutral and one crewmate, but duplicates are allowed and probability is ignored.
            if (CustomGameOptions.GameMode == GameMode.AllAny)
            {
                // Add one neutral role to the game, if any are enabled.
                // This guarantees at least one neutral role's presence.
                if (neutRoles.Count > 0)
                {
                    neutRoles.Shuffle();
                    crewRoles.Add(neutRoles[0]);
                    // If it's unique, remove it from the list.
                    if (neutRoles[0].Item3 == true) neutRoles.Remove(neutRoles[0]);
                }
                // Add one crewmate role to the game, or vanilla Crewmate if none are enabled.
                // This guarantees at least one crewmate role's presence.
                if (CrewmatesRoles.Count > 0)
                {
                    CrewmatesRoles.Shuffle();
                    crewRoles.Add(CrewmatesRoles[0]);
                    if (CrewmatesRoles[0].Item3 == true) CrewmatesRoles.Remove(CrewmatesRoles[0]);
                }
                else
                {
                    crewRoles.Add((typeof(Crewmate), 100, false));
                }
                // Now add all the roles together.
                var allAnyRoles = new List<(Type, int, bool)>();
                allAnyRoles.AddRange(CrewmatesRoles);
                allAnyRoles.AddRange(neutRoles);
                allAnyRoles.Shuffle();
                // Add crew & neutral roles up to the crewmate count, including duplicates (unless defined as unique).
                while (crewRoles.Count < crewmates.Count && allAnyRoles.Count > 0)
                {
                    crewRoles.Add(allAnyRoles[0]);
                    if (allAnyRoles[0].Item3 == true) allAnyRoles.Remove(allAnyRoles[0]);
                }
                // Add impostor roles up to the impostor count, including duplicates (unless defined as unique).
                ImpostorsRoles.Shuffle();
                while (impRoles.Count < impostors.Count && ImpostorsRoles.Count > 0)
                {
                    impRoles.Add(ImpostorsRoles[0]);
                    if (ImpostorsRoles[0].Item3 == true) ImpostorsRoles.Remove(ImpostorsRoles[0]);
                }
            }
            else
            {
                // Roles have already been sorted for Classic mode.
                // So just add in the neutral roles.
                crewRoles.AddRange(neutRoles);
            }

            // Shuffle roles before handing them out.
            // This should ensure a statistically equal chance of all permutations of roles.
            crewRoles.Shuffle();
            impRoles.Shuffle();

            // Hand out appropriate roles to crewmates and impostors.
            foreach (var (type, _, unique) in crewRoles)
            {
                Role.GenRole<Role>(type, crewmates);
            }
            foreach (var (type, _, unique) in impRoles)
            {
                Role.GenRole<Role>(type, impostors);
            }

            // Assign vanilla roles to anyone who did not receive a role.
            foreach (var crewmate in crewmates)
                Role.GenRole<Role>(typeof(Crewmate), crewmate);

            foreach (var impostor in impostors)
                Role.GenRole<Role>(typeof(Impostor), impostor);

            var canHaveObjective = PlayerControl.AllPlayerControls.ToArray().ToList();
            canHaveObjective.Shuffle();

            foreach (var (type, id) in ObjectiveGlobalModifiers)
            {
                if (canHaveObjective.Count == 0) break;
                if (type.FullName.Contains("Lover"))
                {
                    if (canHaveObjective.Count == 1) continue;
                    Lover.Gen(canHaveObjective);
                }
                else
                {
                    Role.GenObjective<Objective>(type, canHaveObjective);
                }
            }
            canHaveObjective.RemoveAll(player => !player.Is(Faction.Crewmates) || player.Is(RoleEnum.Undercover));
            ObjectiveCrewmateModifiers.SortModifiers(canHaveObjective.Count);
            ObjectiveCrewmateModifiers.Shuffle();

            while (canHaveObjective.Count > 0 && ObjectiveCrewmateModifiers.Count > 0)
            {
                var (type, _) = ObjectiveCrewmateModifiers.TakeFirst();
                Role.GenObjective<Objective>(type, canHaveObjective.TakeFirst());
            }

            // Hand out assassin ability to killers according to the settings.
            var canHaveAbility = PlayerControl.AllPlayerControls.ToArray().Where(player => player.Is(Faction.Impostors) || (player.Is(Faction.NeutralApocalypse) && CustomGameOptions.GameMode == GameMode.Horseman)).ToList();
            canHaveAbility.Shuffle();
            var canHaveAbility2 = PlayerControl.AllPlayerControls.ToArray().Where(player => player.Is(Faction.NeutralKilling) || (player.Is(Faction.NeutralApocalypse) && CustomGameOptions.GameMode != GameMode.Horseman)).ToList();
            canHaveAbility2.Shuffle();

            var assassinConfig = new (List<PlayerControl>, int)[]
            {
                (canHaveAbility, CustomGameOptions.NumberOfImpostorAssassins),
                (canHaveAbility2, CustomGameOptions.NumberOfNeutralAssassins)
            };
            foreach ((var abilityList, int maxNumber) in assassinConfig)
            {
                int assassinNumber = maxNumber;
                while (abilityList.Count > 0 && assassinNumber > 0)
                {
                    var (type, rpc, _) = AssassinAbility.Ability();
                    Role.Gen<Ability>(type, abilityList.TakeFirst(), rpc);
                    assassinNumber -= 1;
                }
            }

            // Hand out assassin modifiers, if enabled, to impostor assassins.
            var canHaveAssassinModifier = PlayerControl.AllPlayerControls.ToArray().Where(player => player.Is(Faction.Impostors) && player.Is(AbilityEnum.Assassin)).ToList();
            canHaveAssassinModifier.Shuffle();
            AssassinModifiers.SortModifiers(canHaveAssassinModifier.Count);
            AssassinModifiers.Shuffle();

            foreach (var (type, _) in AssassinModifiers)
            {
                if (canHaveAssassinModifier.Count == 0) break;
                Role.GenModifier<Modifier>(type, canHaveAssassinModifier);
            }

            // Hand out impostor modifiers.
            var canHaveImpModifier = PlayerControl.AllPlayerControls.ToArray().Where(player => player.Is(Faction.Impostors) && !player.Is(ModifierEnum.DoubleShot)).ToList();
            canHaveImpModifier.Shuffle();
            ImpostorModifiers.SortModifiers(canHaveImpModifier.Count);
            ImpostorModifiers.Shuffle();

            foreach (var (type, _) in ImpostorModifiers)
            {
                if (canHaveImpModifier.Count == 0) break;
                Role.GenModifier<Modifier>(type, canHaveImpModifier);
            }

            // Hand out global modifiers.
            var canHaveModifier = PlayerControl.AllPlayerControls.ToArray()
                .Where(player => !player.Is(ModifierEnum.Disperser) && !player.Is(ModifierEnum.DoubleShot) && !player.Is(ModifierEnum.Underdog))
                .ToList();
            canHaveModifier.Shuffle();
            GlobalModifiers.SortModifiers(canHaveModifier.Count);
            GlobalModifiers.Shuffle();

            foreach (var (type, id) in GlobalModifiers)
            {
                if (canHaveModifier.Count == 0) break;
                Role.GenModifier<Modifier>(type, canHaveModifier);
            }

            // The Glitch cannot have Button Modifiers.
            canHaveModifier.RemoveAll(player => player.Is(RoleEnum.Glitch));
            ButtonModifiers.SortModifiers(canHaveModifier.Count);

            foreach (var (type, id) in ButtonModifiers)
            {
                if (canHaveModifier.Count == 0) break;
                Role.GenModifier<Modifier>(type, canHaveModifier);
            }

            // Now hand out Crewmate Modifiers to all remaining eligible players.
            canHaveModifier.RemoveAll(player => !player.Is(Faction.Crewmates) || player.Is(ObjectiveEnum.ImpostorAgent) || player.Is(ObjectiveEnum.ApocalypseAgent));
            CrewmateModifiers.SortModifiers(canHaveModifier.Count);
            CrewmateModifiers.Shuffle();

            while (canHaveModifier.Count > 0 && CrewmateModifiers.Count > 0)
            {
                var (type, _) = CrewmateModifiers.TakeFirst();
                Role.GenModifier<Modifier>(type, canHaveModifier.TakeFirst());
            }

            // Set the Traitor, if there is one enabled.
            var toChooseFromCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(RoleEnum.Mayor) && !x.Is(ObjectiveEnum.Lover) && !x.Is(ObjectiveEnum.ImpostorAgent) && !x.Is(ObjectiveEnum.ApocalypseAgent)).ToList();
            if (TraitorOn && toChooseFromCrew.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromCrew.Count);
                var pc = toChooseFromCrew[rand];

                SetTraitor.WillBeTraitor = pc;

                Utils.Rpc(CustomRPC.SetTraitor, pc.PlayerId);
            }
            else
            {
                Utils.Rpc(CustomRPC.SetTraitor, byte.MaxValue);
            }
            toChooseFromCrew.RemoveAll(player => SetTraitor.WillBeTraitor == player);

            // Set the Haunter, if there is one enabled.
            if (HaunterOn && toChooseFromCrew.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromCrew.Count);
                var pc = toChooseFromCrew[rand];

                SetHaunter.WillBeHaunter = pc;

                Utils.Rpc(CustomRPC.SetHaunter, pc.PlayerId);
            }
            else
            {
                Utils.Rpc(CustomRPC.SetHaunter, byte.MaxValue);
            }

            var toChooseFromNeut = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(Faction.NeutralBenign) || x.Is(Faction.NeutralEvil) || x.Is(Faction.NeutralKilling) || x.Is(Faction.NeutralBenign)) && !x.Is(ObjectiveEnum.Lover)).ToList();
            if (PhantomOn && toChooseFromNeut.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromNeut.Count);
                var pc = toChooseFromNeut[rand];

                SetPhantom.WillBePhantom = pc;

                Utils.Rpc(CustomRPC.SetPhantom, pc.PlayerId);
            }
            else
            {
                Utils.Rpc(CustomRPC.SetPhantom, byte.MaxValue);
            }

            var toChooseFromImps = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Data.IsImpostor() && !x.Is(ObjectiveEnum.Lover)).ToList();
            if (PoltergeistOn && toChooseFromImps.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromImps.Count);
                var pc = toChooseFromImps[rand];

                SetPoltergeist.WillBePoltergeist = pc;

                Utils.Rpc(CustomRPC.SetPoltergeist, pc.PlayerId);
            }
            else
            {
                Utils.Rpc(CustomRPC.SetPoltergeist, byte.MaxValue);
            }

            var exeTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(ObjectiveEnum.Lover) && !x.Is(RoleEnum.Mayor) && !x.Is(RoleEnum.Swapper) && !x.Is(RoleEnum.Vigilante) && x != SetTraitor.WillBeTraitor).ToList();
            foreach (var role in Role.GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;
                if (exeTargets.Count > 0)
                {
                    exe.target = exeTargets[Random.RandomRangeInt(0, exeTargets.Count)];
                    exeTargets.Remove(exe.target);

                    Utils.Rpc(CustomRPC.SetTarget, role.Player.PlayerId, exe.target.PlayerId);
                }
            }

            var undercoverRoles = new List<RoleEnum>();
            if (CustomGameOptions.UndercoverBaker && CustomGameOptions.BakerOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Baker)) == 0) undercoverRoles.Add(RoleEnum.Baker);
            if (CustomGameOptions.UndercoverBerserker && CustomGameOptions.BerserkerOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Berserker)) == 0) undercoverRoles.Add(RoleEnum.Berserker);
            if (CustomGameOptions.UndercoverBlackmailer && CustomGameOptions.BlackmailerOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Blackmailer)) == 0) undercoverRoles.Add(RoleEnum.Blackmailer);
            if (CustomGameOptions.UndercoverBomber && CustomGameOptions.BomberOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Bomber)) == 0) undercoverRoles.Add(RoleEnum.Bomber);
            if (CustomGameOptions.UndercoverEscapist && CustomGameOptions.EscapistOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Escapist)) == 0) undercoverRoles.Add(RoleEnum.Escapist);
            if (CustomGameOptions.UndercoverGrenadier && CustomGameOptions.GrenadierOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Grenadier)) == 0) undercoverRoles.Add(RoleEnum.Grenadier);
            if (CustomGameOptions.UndercoverJanitor && CustomGameOptions.JanitorOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Janitor)) == 0) undercoverRoles.Add(RoleEnum.Janitor);
            if (CustomGameOptions.UndercoverMiner && CustomGameOptions.MinerOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Miner)) == 0) undercoverRoles.Add(RoleEnum.Miner);
            if (CustomGameOptions.UndercoverMorphling && CustomGameOptions.MorphlingOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Morphling)) == 0) undercoverRoles.Add(RoleEnum.Morphling);
            if (CustomGameOptions.UndercoverPlaguebearer && CustomGameOptions.PlaguebearerOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Plaguebearer)) == 0) undercoverRoles.Add(RoleEnum.Plaguebearer);
            if (CustomGameOptions.UndercoverPoisoner && CustomGameOptions.PoisonerOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Poisoner)) == 0) undercoverRoles.Add(RoleEnum.Poisoner);
            if (CustomGameOptions.UndercoverSniper && CustomGameOptions.SniperOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Sniper)) == 0) undercoverRoles.Add(RoleEnum.Sniper);
            if (CustomGameOptions.UndercoverSoulCollector && CustomGameOptions.SoulCollectorOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.SoulCollector)) == 0) undercoverRoles.Add(RoleEnum.SoulCollector);
            if (CustomGameOptions.UndercoverSwooper && CustomGameOptions.SwooperOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Swooper)) == 0) undercoverRoles.Add(RoleEnum.Swooper);
            if (CustomGameOptions.UndercoverUndertaker && CustomGameOptions.UndertakerOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Undertaker)) == 0) undercoverRoles.Add(RoleEnum.Undertaker);
            if (CustomGameOptions.UndercoverVenerer && CustomGameOptions.VenererOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Venerer)) == 0) undercoverRoles.Add(RoleEnum.Venerer);
            if (CustomGameOptions.UndercoverWarlock && CustomGameOptions.WarlockOn > 0 && PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(RoleEnum.Warlock)) == 0) undercoverRoles.Add(RoleEnum.Warlock);
            foreach (var role in Role.GetRoles(RoleEnum.Undercover))
            {
                var undercover = (Undercover)role;
                if (undercoverRoles.Count > 0)
                {
                    undercover.UndercoverRole = undercoverRoles[Random.RandomRangeInt(0, undercoverRoles.Count)];
                    undercoverRoles.Remove((RoleEnum)undercover.UndercoverRole);
                }
                else
                {
                    undercover.UndercoverRole = RoleEnum.Impostor;
                }
                Utils.Rpc(CustomRPC.SetUndercover, role.Player.PlayerId, (byte)undercover.UndercoverRole);
            }
            foreach (var role in Role.GetRoles(RoleEnum.Inquisitor))
            {
                var inq = (Inquisitor)role;
                var hereticsRaw = PlayerControl.AllPlayerControls.ToArray().Where(x => x.PlayerId != inq.Player.PlayerId).ToList().OrderBy(x => new System.Random().Next()).Take(CustomGameOptions.NumberOfHeretics).ToList();
                var heretics = new Il2CppSystem.Collections.Generic.List<byte>();
                foreach (var heretic in hereticsRaw)
                {
                    heretics.Add(heretic.PlayerId);
                    Utils.Rpc(CustomRPC.SetHeretic, role.Player.PlayerId, heretic.PlayerId);
                }
                inq.heretics = heretics;
            }

            var goodGATargets = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(ObjectiveEnum.Lover)).ToList();
            var evilGATargets = PlayerControl.AllPlayerControls.ToArray().Where(x => (x.Is(Faction.Impostors) || x.Is(Faction.NeutralKilling)) && !x.Is(ObjectiveEnum.Lover)).ToList();
            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                if (!(goodGATargets.Count == 0 && CustomGameOptions.EvilTargetPercent == 0) ||
                    (evilGATargets.Count == 0 && CustomGameOptions.EvilTargetPercent == 100) ||
                    goodGATargets.Count == 0 && evilGATargets.Count == 0)
                {
                    if (goodGATargets.Count == 0)
                    {
                        ga.target = evilGATargets[Random.RandomRangeInt(0, evilGATargets.Count)];
                        evilGATargets.Remove(ga.target);
                    }
                    else if (evilGATargets.Count == 0 || !Check(CustomGameOptions.EvilTargetPercent))
                    {
                        ga.target = goodGATargets[Random.RandomRangeInt(0, goodGATargets.Count)];
                        goodGATargets.Remove(ga.target);
                    }
                    else
                    {
                        ga.target = evilGATargets[Random.RandomRangeInt(0, evilGATargets.Count)];
                        evilGATargets.Remove(ga.target);
                    }

                    Utils.Rpc(CustomRPC.SetGATarget, role.Player.PlayerId, ga.target.PlayerId);
                }
            }
        }
        private static void GenEachRoleKilling(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            crewmates.Shuffle();
            impostors.Shuffle();

            ImpostorsRoles.Add((typeof(Undertaker), 10, true));
            ImpostorsRoles.Add((typeof(Morphling), 10, false));
            ImpostorsRoles.Add((typeof(Escapist), 10, false));
            ImpostorsRoles.Add((typeof(Miner), 10, true));
            ImpostorsRoles.Add((typeof(Swooper), 10, false));
            ImpostorsRoles.Add((typeof(Grenadier), 10, true));

            ImpostorsRoles.SortRoles(impostors.Count);

            NeutralKillingRoles.Add((typeof(Glitch), 10, true));
            NeutralKillingRoles.Add((typeof(Werewolf), 10, true));
            NeutralKillingRoles.Add((typeof(Berserker), 10, true));
            if (CustomGameOptions.HiddenRoles)
                NeutralKillingRoles.Add((typeof(Juggernaut), 10, true));
            if (CustomGameOptions.AddArsonist)
                NeutralKillingRoles.Add((typeof(Arsonist), 10, true));
            if (CustomGameOptions.AddPlaguebearer)
                NeutralKillingRoles.Add((typeof(Plaguebearer), 10, true));

            var neutrals = 0;
            if (NeutralKillingRoles.Count < CustomGameOptions.NeutralRoles) neutrals = NeutralKillingRoles.Count;
            else neutrals = CustomGameOptions.NeutralRoles;
            var spareCrew = crewmates.Count - neutrals;
            if (spareCrew > 2) NeutralKillingRoles.SortRoles(neutrals);
            else NeutralKillingRoles.SortRoles(crewmates.Count - 3);

            var veterans = CustomGameOptions.VeteranCount;
            while (veterans > 0)
            {
                CrewmatesRoles.Add((typeof(Veteran), 10, false));
                veterans -= 1;
            }
            var vigilantes = CustomGameOptions.VigilanteCount;
            while (vigilantes > 0)
            {
                CrewmatesRoles.Add((typeof(Vigilante), 10, false));
                vigilantes -= 1;
            }
            if (CrewmatesRoles.Count + NeutralKillingRoles.Count > crewmates.Count)
            {
                CrewmatesRoles.SortRoles(crewmates.Count - NeutralKillingRoles.Count);
            }
            else if (CrewmatesRoles.Count + NeutralKillingRoles.Count < crewmates.Count)
            {
                var sheriffs = crewmates.Count - NeutralKillingRoles.Count - CrewmatesRoles.Count;
                while (sheriffs > 0)
                {
                    CrewmatesRoles.Add((typeof(Sheriff), 10, false));
                    sheriffs -= 1;
                }
            }

            var crewAndNeutralRoles = new List<(Type, int, bool)>();
            crewAndNeutralRoles.AddRange(CrewmatesRoles);
            crewAndNeutralRoles.AddRange(NeutralKillingRoles);
            crewAndNeutralRoles.Shuffle();
            ImpostorsRoles.Shuffle();

            foreach (var (type, _, _) in crewAndNeutralRoles)
            {
                Role.GenRole<Role>(type, crewmates);
            }
            foreach (var (type, _, _) in ImpostorsRoles)
            {
                Role.GenRole<Role>(type, impostors);
            }
        }
        private static void GenEachRoleCultist(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            crewmates.Shuffle();
            impostors.Shuffle();

            var specialRoles = new List<(Type, int, bool)>();
            var crewRoles = new List<(Type, int, bool)>();
            var impRole = new List<(Type, int, bool)>();
            if (CustomGameOptions.MayorCultistOn > 0) specialRoles.Add((typeof(Mayor), CustomGameOptions.MayorCultistOn, true));
            if (CustomGameOptions.SeerCultistOn > 0) specialRoles.Add((typeof(CultistSeer), CustomGameOptions.SeerCultistOn, true));
            if (CustomGameOptions.SheriffCultistOn > 0) specialRoles.Add((typeof(Sheriff), CustomGameOptions.SheriffCultistOn, true));
            if (CustomGameOptions.SurvivorCultistOn > 0) specialRoles.Add((typeof(Survivor), CustomGameOptions.SurvivorCultistOn, true));
            if (specialRoles.Count > CustomGameOptions.SpecialRoleCount) specialRoles.SortRoles(CustomGameOptions.SpecialRoleCount);
            if (specialRoles.Count > crewmates.Count) specialRoles.SortRoles(crewmates.Count);
            if (specialRoles.Count < crewmates.Count)
            {
                var chameleons = CustomGameOptions.MaxChameleons;
                var engineers = CustomGameOptions.MaxEngineers;
                var investigators = CustomGameOptions.MaxInvestigators;
                var mystics = CustomGameOptions.MaxMystics;
                var snitches = CustomGameOptions.MaxSnitches;
                var spies = CustomGameOptions.MaxSpies;
                var transporters = CustomGameOptions.MaxTransporters;
                var vigilantes = CustomGameOptions.MaxVigilantes;
                while (chameleons > 0)
                {
                    crewRoles.Add((typeof(Chameleon), 10, false));
                    chameleons--;
                }
                while (engineers > 0)
                {
                    crewRoles.Add((typeof(Engineer), 10, false));
                    engineers--;
                }
                while (investigators > 0)
                {
                    crewRoles.Add((typeof(Investigator), 10, false));
                    investigators--;
                }
                while (mystics > 0)
                {
                    crewRoles.Add((typeof(CultistMystic), 10, false));
                    mystics--;
                }
                while (snitches > 0)
                {
                    crewRoles.Add((typeof(CultistSnitch), 10, false));
                    snitches--;
                }
                while (spies > 0)
                {
                    crewRoles.Add((typeof(Spy), 10, false));
                    spies--;
                }
                while (transporters > 0)
                {
                    crewRoles.Add((typeof(Transporter), 10, false));
                    transporters--;
                }
                while (vigilantes > 0)
                {
                    crewRoles.Add((typeof(Vigilante), 10, false));
                    vigilantes--;
                }
                crewRoles.SortRoles(crewmates.Count - specialRoles.Count);
            }
            impRole.Add((typeof(Necromancer), 100, true));
            impRole.Add((typeof(Whisperer), 100, true));
            impRole.SortRoles(1);

            foreach (var (type, _, unique) in specialRoles)
            {
                Role.GenRole<Role>(type, crewmates);
            }
            foreach (var (type, _, unique) in crewRoles)
            {
                Role.GenRole<Role>(type, crewmates);
            }
            foreach (var (type, _, unique) in impRole)
            {
                Role.GenRole<Role>(type, impostors);
            }

            foreach (var crewmate in crewmates)
                Role.GenRole<Role>(typeof(Crewmate), crewmate);
        }
        private static void GenEachRoleTeams(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            crewmates.Shuffle();

            var roles = new List<(Type, int, bool)>();
            for (int i = 0; i < (crewmates.Count % CustomGameOptions.TeamsAmount >= 1 ? (int)(crewmates.Count / CustomGameOptions.TeamsAmount) + 1 : (int)(crewmates.Count / CustomGameOptions.TeamsAmount)); i++)
            {
                Role.GenRole<Role>(typeof(RedMember), crewmates);
            }
            for (int i = 0; i < (crewmates.Count % CustomGameOptions.TeamsAmount >= 2 ? (int)(crewmates.Count / CustomGameOptions.TeamsAmount) + 1 : (int)(crewmates.Count / CustomGameOptions.TeamsAmount)); i++)
            {
                Role.GenRole<Role>(typeof(BlueMember), crewmates);
            }
            if (CustomGameOptions.TeamsAmount >= 3) for (int i = 0; i < (crewmates.Count % CustomGameOptions.TeamsAmount >= 3 ? (int)(crewmates.Count / CustomGameOptions.TeamsAmount) + 1 : (int)(crewmates.Count / CustomGameOptions.TeamsAmount)); i++)
            {
                Role.GenRole<Role>(typeof(YellowMember), crewmates);
            }
            if (CustomGameOptions.TeamsAmount >= 4) for (int i = 0; i < (crewmates.Count % CustomGameOptions.TeamsAmount >= 4 ? (int)(crewmates.Count / CustomGameOptions.TeamsAmount) + 1 : (int)(crewmates.Count / CustomGameOptions.TeamsAmount)); i++)
            {
                Role.GenRole<Role>(typeof(GreenMember), crewmates);
            }

            foreach (var impostor in impostors)
                Role.GenRole<Role>(typeof(Impostor), impostor);

            foreach (var crewmate in crewmates)
                Role.GenRole<Role>(typeof(Crewmate), crewmate);
        }
        private static void GenEachRoleKiller(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            List<PlayerControl> killer;
            if (CustomGameOptions.SoloKillerPlayer == 0)
            {
                killer = new List<PlayerControl> { };
            }
            else
            {
                killer = new List<PlayerControl> { Utils.PlayerById(GameData.Instance.GetHost().PlayerId) };
            }
            foreach (var k in killer)
            {
                crewmates.Remove(k);
            }
            crewmates.Shuffle();
            var CrewmatesRoles = new List<(Type, int, bool)>();
            var killerRole = new List<(Type, int, bool)>();
            if (CustomGameOptions.SoloKillerPlayer == 0)
            {
                CrewmatesRoles.Add((typeof(SoloKiller), 10, false));
            }
            else
            {
                killerRole.Add((typeof(SoloKiller), 10, false));
            }
            CrewmatesRoles.SortRoles(crewmates.Count);
            foreach (var (type, _, unique) in killerRole)
            {
                Role.GenRole<Role>(type, killer);
            }
            foreach (var (type, _, unique) in CrewmatesRoles)
            {
                Role.GenRole<Role>(type, crewmates);
            }

            foreach (var impostor in impostors)
                Role.GenRole<Role>(typeof(Impostor), impostor);

            foreach (var crewmate in crewmates)
                Role.GenRole<Role>(typeof(Crewmate), crewmate);
        }


        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class HandleRpc
        {
            public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {
                Assembly asm = typeof(Role).Assembly;

                byte readByte, readByte1, readByte2;
                sbyte readSByte, readSByte2;
                switch ((CustomRPC) callId)
                {
                    case CustomRPC.SetRole:
                        var player = Utils.PlayerById(reader.ReadByte());
                        var rstring = reader.ReadString();
                        Activator.CreateInstance(asm.GetType(rstring), new object[] { player });
                        break;
                    case CustomRPC.SetModifier:
                        var player2 = Utils.PlayerById(reader.ReadByte());
                        var mstring = reader.ReadString();
                        Activator.CreateInstance(asm.GetType(mstring), new object[] { player2 });
                        break;
                    case CustomRPC.SetObjective:
                        var player3 = Utils.PlayerById(reader.ReadByte());
                        var ostring = reader.ReadString();
                        Activator.CreateInstance(asm.GetType(ostring), new object[] { player3 });
                        break;

                    case CustomRPC.LoveWin:
                        var winnerlover = Utils.PlayerById(reader.ReadByte());
                        Objective.GetObjective<Lover>(winnerlover).Win();
                        break;

                    case CustomRPC.NobodyWins:
                        Role.NobodyWinsFunc();
                        break;

                    case CustomRPC.SurvivorOnlyWin:
                        Role.SurvOnlyWin();
                        break;

                    case CustomRPC.VampireWin:
                        Role.VampWin();
                        break;

                    case CustomRPC.SetCouple:
                        var id = reader.ReadByte();
                        var id2 = reader.ReadByte();
                        var lover1 = Utils.PlayerById(id);
                        var lover2 = Utils.PlayerById(id2);

                        var modifierLover1 = new Lover(lover1);
                        var modifierLover2 = new Lover(lover2);

                        modifierLover1.OtherLover = modifierLover2;
                        modifierLover2.OtherLover = modifierLover1;

                        break;

                    case CustomRPC.Start:
                        readByte = reader.ReadByte();
                        Utils.ShowDeadBodies = false;
                        ShowRoundOneShield.FirstRoundShielded = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        ShowRoundOneShield.DiedFirst = "";
                        Murder.KilledPlayers.Clear();
                        Role.NobodyWins = false;
                        Role.SurvOnlyWins = false;
                        Role.VampireWins = false;
                        ExileControllerPatch.lastExiled = null;
                        PatchKillTimer.GameStarted = false;
                        StartImitate.ImitatingPlayer = null;
                        CrewmateRoles.AltruistMod.KillButtonTarget.DontRevive = byte.MaxValue;
                        ReviveHudManagerUpdate.DontRevive = byte.MaxValue;
                        AddHauntPatch.AssassinatedPlayers.Clear();
                        HudUpdate.Zooming = false;
                        HudUpdate.ZoomStart();
                        break;

                    case CustomRPC.JanitorClean:
                        readByte1 = reader.ReadByte();
                        var janitorPlayer = Utils.PlayerById(readByte1);
                        var janitorRole = Role.GetRole<Janitor>(janitorPlayer);
                        readByte = reader.ReadByte();
                        var deadBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in deadBodies)
                            if (body.ParentId == readByte)
                                Coroutines.Start(Coroutine.CleanCoroutine(body, janitorRole));

                        break;
                    case CustomRPC.EngineerFix:
                        if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.MushroomMixupSabotage))
                        {
                            var mushroom = ShipStatus.Instance.Systems[SystemTypes.MushroomMixupSabotage].Cast<MushroomMixupSabotageSystem>();
                            if (mushroom.IsActive) mushroom.currentSecondsUntilHeal = 0.1f;
                        }
                        break;

                    case CustomRPC.FixLights:
                        var lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                        lights.ActualSwitches = lights.ExpectedSwitches;
                        break;

                    case CustomRPC.Reveal:
                        var mayor = Utils.PlayerById(reader.ReadByte());
                        var mayorRole = Role.GetRole<Mayor>(mayor);
                        Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Mayor));
                        Role.GetRole(PlayerControl.LocalPlayer).Notification("Mayor Has Revealed!", 1000 * CustomGameOptions.NotificationDuration);
                        mayorRole.Revealed = true;
                        AddRevealButton.RemoveAssassin(mayorRole);
                        break;

                    case CustomRPC.Prosecute:
                        var host = reader.ReadBoolean();
                        if (host && AmongUsClient.Instance.AmHost)
                        {
                            var prosecutor = Utils.PlayerById(reader.ReadByte());
                            var prosRole = Role.GetRole<Prosecutor>(prosecutor);
                            prosRole.ProsecuteThisMeeting = true;
                        }
                        else if (!host && !AmongUsClient.Instance.AmHost)
                        {
                            var prosecutor = Utils.PlayerById(reader.ReadByte());
                            var prosRole = Role.GetRole<Prosecutor>(prosecutor);
                            prosRole.ProsecuteThisMeeting = true;
                        }
                        break;

                    case CustomRPC.Bite:
                        var newVamp = Utils.PlayerById(reader.ReadByte());
                        Bite.Convert(newVamp);
                        break;

                    case CustomRPC.SetSwaps:
                        readSByte = reader.ReadSByte();
                        SwapVotes.Swap1 =
                            MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte);
                        readSByte2 = reader.ReadSByte();
                        SwapVotes.Swap2 =
                            MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte2);
                        PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Bytes received - " + readSByte + " - " +
                                                                          readSByte2);
                        break;

                    case CustomRPC.Imitate:
                        var imitator = Utils.PlayerById(reader.ReadByte());
                        var imitatorRole = Role.GetRole<Imitator>(imitator);
                        var imitateTarget = Utils.PlayerById(reader.ReadByte());
                        imitatorRole.ImitatePlayer = imitateTarget;
                        break;
                    case CustomRPC.StartImitate:
                        var imitator2 = Utils.PlayerById(reader.ReadByte());
                        if (imitator2.Is(RoleEnum.Traitor)) break;
                        var imitatorRole2 = Role.GetRole<Imitator>(imitator2);
                        StartImitate.Imitate(imitatorRole2);
                        break;
                    case CustomRPC.Remember:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var amnesiac = Utils.PlayerById(readByte1);
                        var other = Utils.PlayerById(readByte2);
                        PerformKillButton.Remember(Role.GetRole<Amnesiac>(amnesiac), other);
                        break;
                    case CustomRPC.SoulSwap:
                        var cursedSoul = Utils.PlayerById(reader.ReadByte());
                        var target0 = Utils.PlayerById(reader.ReadByte());
                        NeutralRoles.CursedSoulMod.PerformKill.SoulSwap(cursedSoul, target0);
                        break;
                    case CustomRPC.Protect:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();

                        var medic = Utils.PlayerById(readByte1);
                        var shield = Utils.PlayerById(readByte2);
                        Role.GetRole<Medic>(medic).ShieldedPlayer = shield;
                        Role.GetRole<Medic>(medic).UsedAbility = true;
                        break;
                    case CustomRPC.AttemptSound:
                        var medicId = reader.ReadByte();
                        readByte = reader.ReadByte();
                        StopKill.BreakShield(medicId, readByte, CustomGameOptions.ShieldBreaks);
                        break;
                    case CustomRPC.BypassKill:
                        var killer = Utils.PlayerById(reader.ReadByte());
                        var target = Utils.PlayerById(reader.ReadByte());

                        Utils.MurderPlayer(killer, target, true);
                        break;
                    case CustomRPC.BypassMultiKill:
                        var killer2 = Utils.PlayerById(reader.ReadByte());
                        var target2 = Utils.PlayerById(reader.ReadByte());

                        Utils.MurderPlayer(killer2, target2, false);
                        break;
                    case CustomRPC.AssassinKill:
                        var toDie = Utils.PlayerById(reader.ReadByte());
                        var assassin = Utils.PlayerById(reader.ReadByte());
                        AssassinKill.MurderPlayer(toDie);
                        AssassinKill.AssassinKillCount(toDie, assassin);
                        break;
                    case CustomRPC.VigilanteKill:
                        var toDie2 = Utils.PlayerById(reader.ReadByte());
                        var vigi = Utils.PlayerById(reader.ReadByte());
                        VigilanteKill.MurderPlayer(toDie2);
                        VigilanteKill.VigiKillCount(toDie2, vigi);
                        break;
                    case CustomRPC.DoomsayerKill:
                        var toDie3 = Utils.PlayerById(reader.ReadByte());
                        var doom = Utils.PlayerById(reader.ReadByte());
                        DoomsayerKill.DoomKillCount(toDie3, doom);
                        DoomsayerKill.MurderPlayer(toDie3);
                        break;
                    case CustomRPC.PirateKill:
                        var toDie4 = Utils.PlayerById(reader.ReadByte());
                        PirateKill.MurderPlayer(toDie4);
                        break;
                    case CustomRPC.SetMimic:
                        var glitchPlayer = Utils.PlayerById(reader.ReadByte());
                        var mimicPlayer = Utils.PlayerById(reader.ReadByte());
                        var glitchRole = Role.GetRole<Glitch>(glitchPlayer);
                        glitchRole.MimicTarget = mimicPlayer;
                        glitchRole.IsUsingMimic = true;
                        Utils.Morph(glitchPlayer, mimicPlayer);
                        break;
                    case CustomRPC.RpcResetAnim:
                        var animPlayer = Utils.PlayerById(reader.ReadByte());
                        var theGlitchRole = Role.GetRole<Glitch>(animPlayer);
                        theGlitchRole.MimicTarget = null;
                        theGlitchRole.IsUsingMimic = false;
                        Utils.Unmorph(theGlitchRole.Player);
                        break;
                    case CustomRPC.GlitchWin:
                        var theGlitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
                        ((Glitch) theGlitch)?.Wins();
                        break;
                    case CustomRPC.JuggernautWin:
                        var juggernaut = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Juggernaut);
                        ((Juggernaut)juggernaut)?.Wins();
                        break;
                    case CustomRPC.SerialKillerWin:
                        var serialKiller = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.SerialKiller);
                        ((SerialKiller)serialKiller)?.Wins();
                        break;
                    case CustomRPC.SetHacked:
                        var hackPlayer = Utils.PlayerById(reader.ReadByte());
                        if (hackPlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        {
                            var glitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
                            ((Glitch) glitch)?.SetHacked(hackPlayer);
                        }

                        break;
                    case CustomRPC.Morph:
                        var morphling = Utils.PlayerById(reader.ReadByte());
                        var morphTarget = Utils.PlayerById(reader.ReadByte());
                        var morphRole = Role.GetRole<Morphling>(morphling);
                        morphRole.TimeRemaining = CustomGameOptions.MorphlingDuration;
                        morphRole.MorphedPlayer = morphTarget;
                        break;
                    case CustomRPC.SetTarget:
                        var exe = Utils.PlayerById(reader.ReadByte());
                        var exeTarget = Utils.PlayerById(reader.ReadByte());
                        var exeRole = Role.GetRole<Executioner>(exe);
                        exeRole.target = exeTarget;
                        break;
                    case CustomRPC.SetGATarget:
                        var ga = Utils.PlayerById(reader.ReadByte());
                        var gaTarget = Utils.PlayerById(reader.ReadByte());
                        var gaRole = Role.GetRole<GuardianAngel>(ga);
                        gaRole.target = gaTarget;
                        break;
                    case CustomRPC.SetHeretic:
                        var inq = Utils.PlayerById(reader.ReadByte());
                        var heretic = reader.ReadByte();
                        var inqRole = Role.GetRole<Inquisitor>(inq);
                        if (!inqRole.heretics.Contains(heretic)) inqRole.heretics.Add(heretic);
                        break;
                    case CustomRPC.SetUndercover:
                        var undercover = Utils.PlayerById(reader.ReadByte());
                        var undercoveredRole = (RoleEnum)reader.ReadByte();
                        var undercoverRole = Role.GetRole<Undercover>(undercover);
                        undercoverRole.UndercoverRole = undercoveredRole;
                        break;
                    case CustomRPC.MonarchKnight:
                        var mon = Utils.PlayerById(reader.ReadByte());
                        var knight = Utils.PlayerById(reader.ReadByte());
                        var monRole = Role.GetRole<Monarch>(mon);
                        if (!monRole.Knights.Contains(knight.PlayerId)) monRole.Knights.Add(knight.PlayerId);
                        break;
                    case CustomRPC.BugPlayer:
                        var spy = Utils.PlayerById(reader.ReadByte());
                        var bugged = Utils.PlayerById(reader.ReadByte());
                        var spyRole = Role.GetRole<Spy>(spy);
                        if (!spyRole.BuggedPlayers.Contains(bugged.PlayerId)) spyRole.BuggedPlayers.Add(bugged.PlayerId);
                        break;
                    case CustomRPC.UnbugPlayers:
                        var spyR = Role.GetRole<Spy>(Utils.PlayerById(reader.ReadByte()));
                        spyR.BuggedPlayers = new List<byte>();
                        break;
                    case CustomRPC.Blackmail:
                        var blackmailer = Role.GetRole<Blackmailer>(Utils.PlayerById(reader.ReadByte()));
                        blackmailer.Blackmailed = Utils.PlayerById(reader.ReadByte());
                        break;
                    case CustomRPC.ChangeDefence:
                        var defRole = Role.GetRole(Utils.PlayerById(reader.ReadByte()));
                        defRole.Defense = reader.ReadByte();
                        break;
                    case CustomRPC.Duel:
                        var pirate = Role.GetRole<Pirate>(Utils.PlayerById(reader.ReadByte()));
                        pirate.DueledPlayer = Utils.PlayerById(reader.ReadByte());
                        break;
                    case CustomRPC.SnitchCultistReveal:
                        var snitch = Role.GetRole<CultistSnitch>(Utils.PlayerById(reader.ReadByte()));
                        snitch.CompletedTasks = true;
                        snitch.RevealedPlayer = Utils.PlayerById(reader.ReadByte());
                        break;
                    case CustomRPC.Confess:
                        var oracle = Role.GetRole<Oracle>(Utils.PlayerById(reader.ReadByte()));
                        oracle.Confessor = Utils.PlayerById(reader.ReadByte());
                        var faction = reader.ReadInt32();
                        if (faction == 0) oracle.RevealedFaction = Faction.Crewmates;
                        else if (faction == 1) oracle.RevealedFaction = Faction.NeutralEvil;
                        else if (CustomGameOptions.GameMode == GameMode.Horseman) oracle.RevealedFaction = Faction.NeutralApocalypse;
                        else oracle.RevealedFaction = Faction.Impostors;
                        break;
                    case CustomRPC.Bless:
                        var oracle2 = Role.GetRole<Oracle>(Utils.PlayerById(reader.ReadByte()));
                        oracle2.SavedConfessor = true;
                        break;
                    case CustomRPC.ExecutionerToJester:
                        TargetColor.ExeToJes(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.GAToSurv:
                        GATargetColor.GAToSurv(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.Mine:
                        var ventId = reader.ReadInt32();
                        var miner = Utils.PlayerById(reader.ReadByte());
                        var minerRole = Role.GetRole<Miner>(miner);
                        var pos = reader.ReadVector2();
                        var zAxis = reader.ReadSingle();
                        PlaceVent.SpawnVent(ventId, minerRole, pos, zAxis);
                        break;
                    case CustomRPC.Swoop:
                        var swooper = Utils.PlayerById(reader.ReadByte());
                        var swooperRole = Role.GetRole<Swooper>(swooper);
                        swooperRole.TimeRemaining = CustomGameOptions.SwoopDuration;
                        swooperRole.Swoop();
                        break;
                    case CustomRPC.ChameleonSwoop:
                        var chameleon = Utils.PlayerById(reader.ReadByte());
                        var chameleonRole = Role.GetRole<Chameleon>(chameleon);
                        chameleonRole.TimeRemaining = CustomGameOptions.SwoopDuration;
                        chameleonRole.Swoop();
                        break;
                    case CustomRPC.Camouflage:
                        var venerer = Utils.PlayerById(reader.ReadByte());
                        var venererRole = Role.GetRole<Venerer>(venerer);
                        venererRole.TimeRemaining = CustomGameOptions.AbilityDuration;
                        venererRole.KillsAtStartAbility = reader.ReadInt32();
                        venererRole.Ability();
                        break;
                    case CustomRPC.Alert:
                        var veteran = Utils.PlayerById(reader.ReadByte());
                        var veteranRole = Role.GetRole<Veteran>(veteran);
                        veteranRole.TimeRemaining = CustomGameOptions.AlertDuration;
                        veteranRole.Alert();
                        break;
                    case CustomRPC.Vest:
                        var surv = Utils.PlayerById(reader.ReadByte());
                        var survRole = Role.GetRole<Survivor>(surv);
                        survRole.TimeRemaining = CustomGameOptions.VestDuration;
                        survRole.Vest();
                        break;
                    case CustomRPC.GAProtect:
                        var ga2 = Utils.PlayerById(reader.ReadByte());
                        var ga2Role = Role.GetRole<GuardianAngel>(ga2);
                        ga2Role.TimeRemaining = CustomGameOptions.ProtectDuration;
                        ga2Role.Protect();
                        break;
                    case CustomRPC.Transport:
                        Coroutines.Start(Transporter.TransportPlayers(reader.ReadByte(), reader.ReadByte(), reader.ReadBoolean()));
                        break;
                    case CustomRPC.SetUntransportable:
                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                        {
                            Role.GetRole<Transporter>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);
                        }
                        break;
                    case CustomRPC.Mediate:
                        var mediatedPlayer = Utils.PlayerById(reader.ReadByte());
                        var medium = Role.GetRole<Medium>(Utils.PlayerById(reader.ReadByte()));
                        if (PlayerControl.LocalPlayer.PlayerId != mediatedPlayer.PlayerId) break;
                        medium.AddMediatePlayer(mediatedPlayer.PlayerId);
                        break;
                    case CustomRPC.FlashGrenade:
                        var grenadier = Utils.PlayerById(reader.ReadByte());
                        var grenadierRole = Role.GetRole<Grenadier>(grenadier);
                        grenadierRole.TimeRemaining = CustomGameOptions.GrenadeDuration;
                        grenadierRole.Flash();
                        break;
                    case CustomRPC.ArsonistWin:
                        var theArsonistTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Arsonist);
                        ((Arsonist) theArsonistTheRole)?.Wins();
                        break;
                    case CustomRPC.WerewolfWin:
                        var theWerewolfTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Werewolf);
                        ((Werewolf)theWerewolfTheRole)?.Wins();
                        break;
                    case CustomRPC.RedWin:
                        var redTeamMember = Role.AllRoles.FirstOrDefault(x => x.Faction == Faction.RedTeam);
                        ((RedMember)redTeamMember)?.Wins();
                        break;
                    case CustomRPC.BlueWin:
                        var blueTeamMember = Role.AllRoles.FirstOrDefault(x => x.Faction == Faction.BlueTeam);
                        ((BlueMember)blueTeamMember)?.Wins();
                        break;
                    case CustomRPC.YellowWin:
                        var yellowTeamMember = Role.AllRoles.FirstOrDefault(x => x.Faction == Faction.YellowTeam);
                        ((YellowMember)yellowTeamMember)?.Wins();
                        break;
                    case CustomRPC.GreenWin:
                        var greenTeamMember = Role.AllRoles.FirstOrDefault(x => x.Faction == Faction.GreenTeam);
                        ((GreenMember)greenTeamMember)?.Wins();
                        break;
                    case CustomRPC.SoloKillerWin:
                        var soloKiller = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.SoloKiller);
                        ((SoloKiller)soloKiller)?.Wins();
                        break;
                    case CustomRPC.Infect:
                        var pb = Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte()));
                        pb.SpreadInfection(Utils.PlayerById(reader.ReadByte()), Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.TurnPestilence:
                        Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).TurnPestilence();
                        break;
                    case CustomRPC.TurnFamine:
                        Role.GetRole<Baker>(Utils.PlayerById(reader.ReadByte())).TurnFamine();
                        break;
                    case CustomRPC.TurnWar:
                        Role.GetRole<Berserker>(Utils.PlayerById(reader.ReadByte())).TurnWar();
                        break;
                    case CustomRPC.TurnDeath:
                        Role.GetRole<SoulCollector>(Utils.PlayerById(reader.ReadByte())).TurnDeath();
                        break;
                    case CustomRPC.ApocalypseWin0:
                        var theApocalypseTheRole0 = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Plaguebearer);
                        ((Plaguebearer)theApocalypseTheRole0)?.Wins();
                        break;
                    case CustomRPC.ApocalypseWin1:
                        var theApocalypseTheRole1 = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Pestilence);
                        ((Pestilence)theApocalypseTheRole1)?.Wins();
                        break;
                    case CustomRPC.ApocalypseWin2:
                        var theApocalypseTheRole2 = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Baker);
                        ((Baker)theApocalypseTheRole2)?.Wins();
                        break;
                    case CustomRPC.ApocalypseWin3:
                        var theApocalypseTheRole3 = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Famine);
                        ((Famine)theApocalypseTheRole3)?.Wins();
                        break;
                    case CustomRPC.ApocalypseWin4:
                        var theApocalypseTheRole4 = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Berserker);
                        ((Berserker)theApocalypseTheRole4)?.Wins();
                        break;
                    case CustomRPC.ApocalypseWin5:
                        var theApocalypseTheRole5 = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.War);
                        ((War)theApocalypseTheRole5)?.Wins();
                        break;
                    case CustomRPC.ApocalypseWin6:
                        var theApocalypseTheRole6 = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.SoulCollector);
                        ((SoulCollector)theApocalypseTheRole6)?.Wins();
                        break;
                    case CustomRPC.ApocalypseWin7:
                        var theApocalypseTheRole7 = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Death);
                        ((Death)theApocalypseTheRole7)?.Wins();
                        break;
                    case CustomRPC.SyncCustomSettings:
                        Rpc.ReceiveRpc(reader);
                        break;
                    case CustomRPC.AltruistRevive:
                        readByte1 = reader.ReadByte();
                        var altruistPlayer = Utils.PlayerById(readByte1);
                        var altruistRole = Role.GetRole<Altruist>(altruistPlayer);
                        readByte = reader.ReadByte();
                        var theDeadBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in theDeadBodies)
                            if (body.ParentId == readByte)
                            {
                                if (body.ParentId == PlayerControl.LocalPlayer.PlayerId)
                                    Coroutines.Start(Utils.FlashCoroutine(altruistRole.Color,
                                        CustomGameOptions.ReviveDuration, 0.5f));

                                Coroutines.Start(
                                    global::TownOfUs.CrewmateRoles.AltruistMod.Coroutine.AltruistRevive(body,
                                        altruistRole));
                            }

                        break;
                    case CustomRPC.FixAnimation:
                        var player4 = Utils.PlayerById(reader.ReadByte());
                        player4.MyPhysics.ResetMoveState();
                        player4.Collider.enabled = true;
                        player4.moveable = true;
                        player4.NetTransform.enabled = true;
                        break;
                    case CustomRPC.BarryButton:
                        var buttonBarry = Utils.PlayerById(reader.ReadByte());

                        if (AmongUsClient.Instance.AmHost)
                        {
                            MeetingRoomManager.Instance.reporter = buttonBarry;
                            MeetingRoomManager.Instance.target = null;
                            AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance
                                .Cast<IDisconnectHandler>());
                            if (GameManager.Instance.CheckTaskCompletion()) return;

                            DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(buttonBarry);
                            buttonBarry.RpcStartMeeting(null);
                        }
                        break;
                    case CustomRPC.Disperse:
                        byte teleports = reader.ReadByte();
                        Dictionary<byte, Vector2> coordinates = new Dictionary<byte, Vector2>();
                        for (int i = 0; i < teleports; i++)
                        {
                            byte playerId = reader.ReadByte();
                            Vector2 location = reader.ReadVector2();
                            coordinates.Add(playerId, location);
                        }
                        Disperser.DispersePlayersToCoordinates(coordinates);
                        break;
                    case CustomRPC.BaitReport:
                        var baitKiller = Utils.PlayerById(reader.ReadByte());
                        var bait = Utils.PlayerById(reader.ReadByte());
                        baitKiller.ReportDeadBody(bait.Data);
                        break;
                    case CustomRPC.CheckMurder:
                        var murderKiller = Utils.PlayerById(reader.ReadByte());
                        var murderTarget = Utils.PlayerById(reader.ReadByte());
                        murderKiller.CheckMurder(murderTarget);
                        break;
                    case CustomRPC.Drag:
                        readByte1 = reader.ReadByte();
                        var dienerPlayer = Utils.PlayerById(readByte1);
                        var dienerRole = Role.GetRole<Undertaker>(dienerPlayer);
                        readByte = reader.ReadByte();
                        var dienerBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in dienerBodies)
                            if (body.ParentId == readByte)
                                dienerRole.CurrentlyDragging = body;

                        break;
                    case CustomRPC.Drop:
                        readByte1 = reader.ReadByte();
                        var v2 = reader.ReadVector2();
                        var v2z = reader.ReadSingle();
                        var dienerPlayer2 = Utils.PlayerById(readByte1);
                        var dienerRole2 = Role.GetRole<Undertaker>(dienerPlayer2);
                        var body2 = dienerRole2.CurrentlyDragging;
                        dienerRole2.CurrentlyDragging = null;

                        body2.transform.position = new Vector3(v2.x, v2.y, v2z);

                        break;
                    case CustomRPC.SetAssassin:
                        new Assassin(Utils.PlayerById(reader.ReadByte()));
                        break;
                    case CustomRPC.SetPhantom:
                        readByte = reader.ReadByte();
                        SetPhantom.WillBePhantom = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;
                    case CustomRPC.CatchPhantom:
                        var phantomPlayer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Phantom>(phantomPlayer).Caught = true;
                        if (PlayerControl.LocalPlayer == phantomPlayer) HudManager.Instance.AbilityButton.gameObject.SetActive(true);
                        phantomPlayer.Exiled();
                        break;
                    case CustomRPC.PhantomWin:
                        var phantomWinner = Role.GetRole<Phantom>(Utils.PlayerById(reader.ReadByte()));
                        phantomWinner.CompletedTasks = true;
                        if (!CustomGameOptions.NeutralEvilWinEndsGame)
                        {
                            phantomWinner.Caught = true;
                            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Phantom) || !CustomGameOptions.PhantomSpook || MeetingHud.Instance) return;
                            byte[] toKill = MeetingHud.Instance.playerStates.Where(x => !Utils.PlayerById(x.TargetPlayerId).Is(RoleEnum.Pestilence) && !Utils.PlayerById(x.TargetPlayerId).Is(RoleEnum.Famine) && !Utils.PlayerById(x.TargetPlayerId).Is(RoleEnum.War) && !Utils.PlayerById(x.TargetPlayerId).Is(RoleEnum.Death)).Select(x => x.TargetPlayerId).ToArray();
                            Role.GetRole(PlayerControl.LocalPlayer).PauseEndCrit = true;
                            var pk = new PunishmentKill((x) => {
                                Utils.RpcMultiMurderPlayer(PlayerControl.LocalPlayer, x);
                                Role.GetRole(PlayerControl.LocalPlayer).PauseEndCrit = false;
                            }, (y) => {
                                return toKill.Contains(y.PlayerId);
                            });
                            Coroutines.Start(pk.Open(1f));
                        }
                        break;
                    case CustomRPC.SetHaunter:
                        readByte = reader.ReadByte();
                        SetHaunter.WillBeHaunter = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;
                    case CustomRPC.CatchHaunter:
                        var haunterPlayer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Haunter>(haunterPlayer).Caught = true;
                        if (PlayerControl.LocalPlayer == haunterPlayer) HudManager.Instance.AbilityButton.gameObject.SetActive(true);
                        haunterPlayer.Exiled();
                        break;
                    case CustomRPC.SetPoltergeist:
                        readByte = reader.ReadByte();
                        SetPoltergeist.WillBePoltergeist = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;
                    case CustomRPC.CatchPoltergeist:
                        var poltergeistPlayer = Utils.PlayerById(reader.ReadByte());
                        if (PlayerControl.LocalPlayer.Data.IsImpostor())
                        {
                            PlayerControl.LocalPlayer.SetKillTimer(PlayerControl.LocalPlayer.killTimer * 2);
                        }
                        Role.GetRole<Poltergeist>(poltergeistPlayer).Caught = true;
                        if (PlayerControl.LocalPlayer == poltergeistPlayer) HudManager.Instance.AbilityButton.gameObject.SetActive(true);
                        poltergeistPlayer.Exiled();
                        break;
                    case CustomRPC.SetTraitor:
                        readByte = reader.ReadByte();
                        SetTraitor.WillBeTraitor = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;
                    case CustomRPC.TraitorSpawn:
                        var traitor = SetTraitor.WillBeTraitor;
                        if (traitor == StartImitate.ImitatingPlayer) StartImitate.ImitatingPlayer = null;
                        var oldRole = Role.GetRole(traitor);
                        var killsList = (oldRole.CorrectKills, oldRole.IncorrectKills, oldRole.CorrectAssassinKills, oldRole.IncorrectAssassinKills);
                        Role.RoleDictionary.Remove(traitor.PlayerId);
                        var traitorRole = new Traitor(traitor);
                        traitorRole.formerRole = oldRole.RoleType;
                        traitorRole.CorrectKills = killsList.CorrectKills;
                        traitorRole.IncorrectKills = killsList.IncorrectKills;
                        traitorRole.CorrectAssassinKills = killsList.CorrectAssassinKills;
                        traitorRole.IncorrectAssassinKills = killsList.IncorrectAssassinKills;
                        traitorRole.RegenTask();
                        SetTraitor.TurnImp(traitor);
                        break;
                    case CustomRPC.Escape:
                        var escapist = Utils.PlayerById(reader.ReadByte());
                        var escapistRole = Role.GetRole<Escapist>(escapist);
                        var escapePos = reader.ReadVector2();
                        escapistRole.EscapePoint = escapePos;
                        Escapist.Escape(escapist);
                        break;
                    case CustomRPC.Revive:
                        var necromancer = Utils.PlayerById(reader.ReadByte());
                        var necromancerRole = Role.GetRole<Necromancer>(necromancer);
                        var revived = reader.ReadByte();
                        var theDeadBodies2 = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in theDeadBodies2)
                            if (body.ParentId == revived)
                            {
                                PerformRevive.Revive(body, necromancerRole);
                            }
                        break;
                    case CustomRPC.Convert:
                        var convertedPlayer = Utils.PlayerById(reader.ReadByte());
                        Utils.Convert(convertedPlayer);
                        break;
                    case CustomRPC.RemoveAllBodies:
                        var buggedBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in buggedBodies)
                            body.gameObject.Destroy();
                        break;
                    case CustomRPC.SubmergedFixOxygen:
                        Patches.SubmergedCompatibility.RepairOxygen();
                        break;
                    case CustomRPC.SetPos:
                        var setplayer = Utils.PlayerById(reader.ReadByte());
                        setplayer.transform.position = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                        break;
                    case CustomRPC.SetSettings:
                        readByte = reader.ReadByte();
                        GameOptionsManager.Instance.currentNormalGameOptions.MapId = readByte == byte.MaxValue ? (byte)0 : readByte;
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                        GameOptionsManager.Instance.currentNormalGameOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                        if (CustomGameOptions.AutoAdjustSettings) RandomMap.AdjustSettings(readByte);
                        break;
                    case CustomRPC.Roleblock:
                        Role.GetRole(Utils.PlayerById(reader.ReadByte())).Roleblocked = true;
                        break;
                    case CustomRPC.UnroleblockPlayer:
                        Role.GetRole(Utils.PlayerById(reader.ReadByte())).Roleblocked = false;
                        break;
                    case CustomRPC.Poison:
                        var poisoned = Utils.PlayerById(reader.ReadByte());
                        if (poisoned == PlayerControl.LocalPlayer)
                        {
                            Coroutines.Start(Utils.FlashCoroutine(Color.red));
                            Role.GetRole(poisoned).Notification("You Have Been Poisoned!", 1000 * CustomGameOptions.NotificationDuration);
                        }
                        break;
                    case CustomRPC.Shoot:
                        if (AmongUsClient.Instance.GameState == InnerNet.InnerNetClient.GameStates.Started)
                        {
                            Coroutines.Start(Utils.FlashCoroutine(Color.red));
                            Role.GetRole(PlayerControl.LocalPlayer).Notification("Sniper Has Shot!", 1000 * CustomGameOptions.NotificationDuration);
                            var r = Role.GetRole(PlayerControl.LocalPlayer);
                            var gameObj = new GameObject();
                            var arrow = gameObj.AddComponent<ArrowBehaviour>();
                            gameObj.transform.parent = PlayerControl.LocalPlayer.gameObject.transform;
                            var renderer = gameObj.AddComponent<SpriteRenderer>();
                            renderer.sprite = TownOfUs.Arrow;
                            arrow.image = renderer;
                            gameObj.layer = 5;
                            arrow.target = Utils.PlayerById(reader.ReadByte()).GetTruePosition();
                            r.SnipeArrows.Add(arrow);
                        }
                        break;
                    case CustomRPC.BugMessage:
                        var interacted = Utils.PlayerById(reader.ReadByte());
                        var interactorRole = (RoleEnum)reader.ReadByte();
                        var abilityId = reader.ReadByte();
                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Spy))
                        {
                            var localRole = Role.GetRole<Spy>(PlayerControl.LocalPlayer);
                            if (localRole.BuggedPlayers.Contains(interacted.PlayerId))
                            {
                                localRole.GenerateMessage(interacted, interactorRole, abilityId);
                            }
                        }
                        break;
                    case CustomRPC.ControlSet:
                        var witch = Utils.PlayerById(reader.ReadByte());
                        var controled = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Witch>(witch).ControledPlayer = controled;
                        break;
                    case CustomRPC.ControlPerform:
                        var witch1 = Utils.PlayerById(reader.ReadByte());
                        var controled1 = Utils.PlayerById(reader.ReadByte());
                        var target1 = Utils.PlayerById(reader.ReadByte());
                        if (controled1 == PlayerControl.LocalPlayer)
                        {
                            Coroutines.Start(Utils.FlashCoroutine(Patches.Colors.Witch));
                            Role.GetRole(PlayerControl.LocalPlayer).Notification("You Have Been Controled!", 1000 * CustomGameOptions.NotificationDuration);
                            var __instance = DestroyableSingleton<HudManager>.Instance.KillButton;
                            if (controled1.Data.IsImpostor())
                            {
                                __instance.SetTarget(target1);
                                StopImpKill.Prefix(__instance);
                            }
                            else switch (Role.GetRole(controled1).RoleType)
                                {
                                    case RoleEnum.Seer:
                                        Role.GetRole<Seer>(controled1).ClosestPlayer = target1;
                                        CrewmateRoles.SeerMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Medic:
                                        Role.GetRole<Medic>(controled1).ClosestPlayer = target1;
                                        CrewmateRoles.MedicMod.Protect.Prefix(__instance);
                                        break;
                                    case RoleEnum.Arsonist:
                                        Role.GetRole<Arsonist>(controled1).ClosestPlayerDouse = target1;
                                        NeutralRoles.ArsonistMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Altruist:
                                        CrewmateRoles.AltruistMod.PerformKillButton.Prefix(__instance);
                                        break;
                                    case RoleEnum.Veteran:
                                        CrewmateRoles.VeteranMod.Alert.Prefix(__instance);
                                        break;
                                    case RoleEnum.Amnesiac:
                                        NeutralRoles.AmnesiacMod.PerformKillButton.Prefix(__instance);
                                        break;
                                    case RoleEnum.Tracker:
                                        Role.GetRole<Tracker>(controled1).ClosestPlayer = target1;
                                        CrewmateRoles.TrackerMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Transporter:
                                        CrewmateRoles.TransporterMod.PerformKillButton.Prefix(__instance);
                                        if (Role.GetRole<Transporter>(controled1).TransportTimer() == 0f)
                                        {
                                            Role.GetRole<Transporter>(controled1).TransportPlayer1 = target1;
                                            Role.GetRole<Transporter>(controled1).TransportPlayer2 = controled1;
                                        }
                                        break;
                                    case RoleEnum.Medium:
                                        CrewmateRoles.MediumMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Survivor:
                                        NeutralRoles.SurvivorMod.Vest.Prefix(__instance);
                                        break;
                                    case RoleEnum.GuardianAngel:
                                        NeutralRoles.GuardianAngelMod.Protect.Prefix(__instance);
                                        break;
                                    case RoleEnum.Plaguebearer:
                                        Role.GetRole<Plaguebearer>(controled1).ClosestPlayer = target1;
                                        NeutralRoles.PlaguebearerMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Engineer:
                                        CrewmateRoles.EngineerMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Trapper:
                                        CrewmateRoles.TrapperMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Detective:
                                        Role.GetRole<Detective>(controled1).ClosestPlayer = target1;
                                        CrewmateRoles.DetectiveMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Doomsayer:
                                        Role.GetRole<Doomsayer>(controled1).ClosestPlayer = target1;
                                        NeutralRoles.DoomsayerMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Vampire:
                                        Role.GetRole<Vampire>(controled1).ClosestPlayer = target1;
                                        NeutralRoles.VampireMod.Bite.Prefix(__instance);
                                        break;
                                    case RoleEnum.VampireHunter:
                                        Role.GetRole<VampireHunter>(controled1).ClosestPlayer = target1;
                                        CrewmateRoles.VampireHunterMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Oracle:
                                        Role.GetRole<Oracle>(controled1).ClosestPlayer = target1;
                                        CrewmateRoles.OracleMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Aurial:
                                        CrewmateRoles.AurialMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Baker:
                                        Role.GetRole<Baker>(controled1).ClosestPlayer = target1;
                                        ApocalypseRoles.BakerMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Famine:
                                        Role.GetRole<Famine>(controled1).ClosestPlayer = target1;
                                        ApocalypseRoles.FamineMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.SoulCollector:
                                        ApocalypseRoles.SoulCollectorMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Death:
                                        ApocalypseRoles.DeathMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Pirate:
                                        Role.GetRole<Pirate>(controled1).ClosestPlayer = target1;
                                        NeutralRoles.PirateMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Inspector:
                                        Role.GetRole<Inspector>(controled1).ClosestPlayer = target1;
                                        CrewmateRoles.InspectorMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Monarch:
                                        Role.GetRole<Monarch>(controled1).ClosestPlayer = target1;
                                        CrewmateRoles.MonarchMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Inquisitor:
                                        Role.GetRole<Inquisitor>(controled1).ClosestPlayer = target1;
                                        NeutralRoles.InquisitorMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.TavernKeeper:
                                        Role.GetRole<TavernKeeper>(controled1).ClosestPlayer = target1;
                                        CrewmateRoles.TavernKeeperMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Spy:
                                        Role.GetRole<Spy>(controled1).ClosestPlayer = target1;
                                        CrewmateRoles.SpyMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Sheriff:
                                        Role.GetRole<Sheriff>(controled1).ClosestPlayer = target1;
                                        CrewmateRoles.SheriffMod.Kill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Pestilence:
                                        Role.GetRole<Pestilence>(controled1).ClosestPlayer = target1;
                                        NeutralRoles.PestilenceMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Werewolf:
                                        Role.GetRole<Werewolf>(controled1).ClosestPlayer = target1;
                                        NeutralRoles.WerewolfMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Juggernaut:
                                        Role.GetRole<Juggernaut>(controled1).ClosestPlayer = target1;
                                        NeutralRoles.JuggernautMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Berserker:
                                        Role.GetRole<Berserker>(controled1).ClosestPlayer = target1;
                                        ApocalypseRoles.BerserkerMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.War:
                                        Role.GetRole<War>(controled1).ClosestPlayer = target1;
                                        ApocalypseRoles.WarMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.SerialKiller:
                                        Role.GetRole<SerialKiller>(controled1).ClosestPlayer = target1;
                                        NeutralRoles.SerialKillerMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Glitch:
                                        Role.GetRole<Glitch>(controled1).KillTarget = target1;
                                        NeutralRoles.GlitchMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.CursedSoul:
                                        Role.GetRole<CursedSoul>(controled1).ClosestPlayer = target1;
                                        NeutralRoles.CursedSoulMod.PerformKill.Prefix(__instance);
                                        break;
                                    case RoleEnum.Hunter:
                                        Role.GetRole<Hunter>(controled1).ClosestStalkPlayer = target1;
                                        CrewmateRoles.HunterMod.Stalk.Prefix(__instance);
                                        break;
                                    case RoleEnum.Lookout:
                                        CrewmateRoles.LookoutMod.PerformKill.Prefix(__instance);
                                        break;
                                    default:
                                        Utils.Interact(controled1, target1);
                                        break;
                                }
                        }
                        break;
                    case CustomRPC.ControlCooldown:
                        var isEnabled = reader.ReadBoolean();
                        var cooldown = reader.ReadByte();
                        var isActive = reader.ReadBoolean();
                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Witch))
                        {
                            var witchRole = Role.GetRole<Witch>(PlayerControl.LocalPlayer);
                            witchRole.TargetIsEnabled = isEnabled;
                            witchRole.TargetCooldown = cooldown;
                            witchRole.TargetIsActive = isActive;
                        }
                        break;

                    case CustomRPC.HunterStalk:
                        var stalker = Utils.PlayerById(reader.ReadByte());
                        var stalked = Utils.PlayerById(reader.ReadByte());
                        Hunter hunterRole = Role.GetRole<Hunter>(stalker);
                        hunterRole.StalkDuration = CustomGameOptions.HunterStalkDuration;
                        hunterRole.StalkedPlayer = stalked;
                        hunterRole.Stalk();
                        break;
                    case CustomRPC.HunterCatchPlayer:
                        var hunter = Utils.PlayerById(reader.ReadByte());
                        var prey = Utils.PlayerById(reader.ReadByte());
                        Hunter hunter2 = Role.GetRole<Hunter>(hunter);
                        hunter2.CatchPlayer(prey);
                        break;
                    case CustomRPC.Retribution:
                        var lastVoted = Utils.PlayerById(reader.ReadByte());
                        AssassinKill.MurderPlayer(lastVoted);
                        break;
                    case CustomRPC.AbilityUsed:
                        var abilityPlayer = reader.ReadByte();
                        foreach (var kill in Murder.KilledPlayers)
                        {
                            if (abilityPlayer == kill.KillerId && (DateTime.UtcNow - kill.KillTime).TotalSeconds < 10)
                            {
                                kill.KillerEscapeAbility = true;
                            }
                        }
                        break;
                    case CustomRPC.KillAbilityUsed:
                        var abilityTarget = Role.GetRole(Utils.PlayerById(reader.ReadByte()));
                        abilityTarget.KilledByAbility = true;
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
        public static class RpcSetRole
        {
            public static void Postfix()
            {
                PluginSingleton<TownOfUs>.Instance.Log.LogMessage("RPC SET ROLE");
                var infected = GameData.Instance.AllPlayers.ToArray().Where(o => o.IsImpostor());

                Utils.ShowDeadBodies = false;
                if (ShowRoundOneShield.DiedFirst != null && CustomGameOptions.FirstDeathShield)
                {
                    var shielded = false;
                    foreach (var player in PlayerControl.AllPlayerControls)
                    {
                        if (player.name == ShowRoundOneShield.DiedFirst)
                        {
                            ShowRoundOneShield.FirstRoundShielded = player;
                            shielded = true;
                        }
                    }
                    if (!shielded) ShowRoundOneShield.FirstRoundShielded = null;
                }
                else ShowRoundOneShield.FirstRoundShielded = null;
                ShowRoundOneShield.DiedFirst = "";
                Role.NobodyWins = false;
                Role.SurvOnlyWins = false;
                Role.VampireWins = false;
                ExileControllerPatch.lastExiled = null;
                PatchKillTimer.GameStarted = false;
                StartImitate.ImitatingPlayer = null;
                AddHauntPatch.AssassinatedPlayers.Clear();
                CrewmatesRoles.Clear();
                NeutralBenignRoles.Clear();
                NeutralEvilRoles.Clear();
                NeutralChaosRoles.Clear();
                NeutralKillingRoles.Clear();
                NeutralApocalypseRoles.Clear();
                ImpostorsRoles.Clear();
                ObjectiveCrewmateModifiers.Clear();
                ObjectiveGlobalModifiers.Clear();
                CrewmateModifiers.Clear();
                GlobalModifiers.Clear();
                ImpostorModifiers.Clear();
                ButtonModifiers.Clear();
                AssassinModifiers.Clear();
                AssassinAbility.Clear();

                Murder.KilledPlayers.Clear();
                CrewmateRoles.AltruistMod.KillButtonTarget.DontRevive = byte.MaxValue;
                ReviveHudManagerUpdate.DontRevive = byte.MaxValue;
                HudUpdate.Zooming = false;
                HudUpdate.ZoomStart();

                if (ShowRoundOneShield.FirstRoundShielded != null)
                {
                    Utils.Rpc(CustomRPC.Start, ShowRoundOneShield.FirstRoundShielded.PlayerId);
                }
                else
                {
                    Utils.Rpc(CustomRPC.Start, byte.MaxValue);
                }

                if (GameOptionsManager.Instance.CurrentGameOptions.GameMode == GameModes.HideNSeek) return;

                if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.AllAny)
                {
                    PhantomOn = Check(CustomGameOptions.PhantomOn);
                    HaunterOn = Check(CustomGameOptions.HaunterOn);
                    TraitorOn = Check(CustomGameOptions.TraitorOn);
                    PoltergeistOn = Check(CustomGameOptions.PoltergeistOn);
                }
                else if (CustomGameOptions.GameMode == GameMode.Horseman)
                {
                    PhantomOn = Check(CustomGameOptions.PhantomOn);
                    HaunterOn = Check(CustomGameOptions.HaunterOn);
                    TraitorOn = false;
                    PoltergeistOn = false;
                }
                else
                {
                    PhantomOn = false;
                    HaunterOn = false;
                    TraitorOn = false;
                    PoltergeistOn = false;
                }

                if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.AllAny)
                {
                    #region Crewmate Roles
                    if (CustomGameOptions.MayorOn > 0)
                        CrewmatesRoles.Add((typeof(Mayor), CustomGameOptions.MayorOn, true));

                    if (CustomGameOptions.SheriffOn > 0)
                        CrewmatesRoles.Add((typeof(Sheriff), CustomGameOptions.SheriffOn, false));

                    if (CustomGameOptions.EngineerOn > 0)
                        CrewmatesRoles.Add((typeof(Engineer), CustomGameOptions.EngineerOn, false));

                    if (CustomGameOptions.SwapperOn > 0)
                        CrewmatesRoles.Add((typeof(Swapper), CustomGameOptions.SwapperOn, true));

                    if (CustomGameOptions.InvestigatorOn > 0)
                        CrewmatesRoles.Add((typeof(Investigator), CustomGameOptions.InvestigatorOn, false));

                    if (CustomGameOptions.MedicOn > 0)
                        CrewmatesRoles.Add((typeof(Medic), CustomGameOptions.MedicOn, true));

                    if (CustomGameOptions.SeerOn > 0)
                        CrewmatesRoles.Add((typeof(Seer), CustomGameOptions.SeerOn, false));

                    if (CustomGameOptions.SpyOn > 0)
                        CrewmatesRoles.Add((typeof(Spy), CustomGameOptions.SpyOn, false));

                    if (CustomGameOptions.SnitchOn > 0)
                        CrewmatesRoles.Add((typeof(Snitch), CustomGameOptions.SnitchOn, true));

                    if (CustomGameOptions.AltruistOn > 0)
                        CrewmatesRoles.Add((typeof(Altruist), CustomGameOptions.AltruistOn, true));

                    if (CustomGameOptions.VigilanteOn > 0)
                        CrewmatesRoles.Add((typeof(Vigilante), CustomGameOptions.VigilanteOn, false));

                    if (CustomGameOptions.VeteranOn > 0)
                        CrewmatesRoles.Add((typeof(Veteran), CustomGameOptions.VeteranOn, false));

                    if (CustomGameOptions.HunterOn > 0)
                        CrewmatesRoles.Add((typeof(Hunter), CustomGameOptions.HunterOn, false));

                    if (CustomGameOptions.TrackerOn > 0)
                        CrewmatesRoles.Add((typeof(Tracker), CustomGameOptions.TrackerOn, false));

                    if (CustomGameOptions.TransporterOn > 0)
                        CrewmatesRoles.Add((typeof(Transporter), CustomGameOptions.TransporterOn, false));

                    if (CustomGameOptions.MediumOn > 0)
                        CrewmatesRoles.Add((typeof(Medium), CustomGameOptions.MediumOn, false));

                    if (CustomGameOptions.MysticOn > 0)
                        CrewmatesRoles.Add((typeof(Mystic), CustomGameOptions.MysticOn, false));

                    if (CustomGameOptions.TrapperOn > 0)
                        CrewmatesRoles.Add((typeof(Trapper), CustomGameOptions.TrapperOn, false));

                    if (CustomGameOptions.DetectiveOn > 0)
                        CrewmatesRoles.Add((typeof(Detective), CustomGameOptions.DetectiveOn, false));

                    if (CustomGameOptions.ImitatorOn > 0)
                        CrewmatesRoles.Add((typeof(Imitator), CustomGameOptions.ImitatorOn, true));

                    if (CustomGameOptions.ProsecutorOn > 0)
                        CrewmatesRoles.Add((typeof(Prosecutor), CustomGameOptions.ProsecutorOn, true));

                    if (CustomGameOptions.OracleOn > 0)
                        CrewmatesRoles.Add((typeof(Oracle), CustomGameOptions.OracleOn, true));

                    if (CustomGameOptions.AurialOn > 0)
                        CrewmatesRoles.Add((typeof(Aurial), CustomGameOptions.AurialOn, false));

                    if (CustomGameOptions.InspectorOn > 0)
                        CrewmatesRoles.Add((typeof(Inspector), CustomGameOptions.InspectorOn, false));

                    if (CustomGameOptions.MonarchOn > 0)
                        CrewmatesRoles.Add((typeof(Monarch), CustomGameOptions.MonarchOn, true));

                    if (CustomGameOptions.TavernKeeperOn > 0)
                        CrewmatesRoles.Add((typeof(TavernKeeper), CustomGameOptions.TavernKeeperOn, false));

                    if (CustomGameOptions.UndercoverOn > 0)
                        CrewmatesRoles.Add((typeof(Undercover), CustomGameOptions.UndercoverOn, true));

                    if (CustomGameOptions.LookoutOn > 0)
                        CrewmatesRoles.Add((typeof(Lookout), CustomGameOptions.LookoutOn, false));
                    #endregion
                    #region Neutral Roles
                    if (CustomGameOptions.JesterOn > 0)
                        NeutralEvilRoles.Add((typeof(Jester), CustomGameOptions.JesterOn, false));

                    if (CustomGameOptions.AmnesiacOn > 0)
                        NeutralBenignRoles.Add((typeof(Amnesiac), CustomGameOptions.AmnesiacOn, false));

                    if (CustomGameOptions.CursedSoulOn > 0)
                        NeutralBenignRoles.Add((typeof(CursedSoul), CustomGameOptions.CursedSoulOn, false));

                    if (CustomGameOptions.ExecutionerOn > 0)
                        NeutralEvilRoles.Add((typeof(Executioner), CustomGameOptions.ExecutionerOn, false));

                    if (CustomGameOptions.DoomsayerOn > 0)
                        NeutralChaosRoles.Add((typeof(Doomsayer), CustomGameOptions.DoomsayerOn, false));

                    if (CustomGameOptions.PirateOn > 0)
                        NeutralChaosRoles.Add((typeof(Pirate), CustomGameOptions.PirateOn, true));

                    if (CustomGameOptions.InquisitorOn > 0)
                        NeutralChaosRoles.Add((typeof(Inquisitor), CustomGameOptions.InquisitorOn, true));

                    if (CustomGameOptions.WitchOn > 0)
                        NeutralEvilRoles.Add((typeof(Witch), CustomGameOptions.WitchOn, true));

                    if (CustomGameOptions.SurvivorOn > 0)
                        NeutralBenignRoles.Add((typeof(Survivor), CustomGameOptions.SurvivorOn, false));

                    if (CustomGameOptions.GuardianAngelOn > 0)
                        NeutralBenignRoles.Add((typeof(GuardianAngel), CustomGameOptions.GuardianAngelOn, false));

                    if (CustomGameOptions.GlitchOn > 0)
                        NeutralKillingRoles.Add((typeof(Glitch), CustomGameOptions.GlitchOn, true));

                    if (CustomGameOptions.ArsonistOn > 0)
                        NeutralKillingRoles.Add((typeof(Arsonist), CustomGameOptions.ArsonistOn, true));

                    if (CustomGameOptions.PlaguebearerOn > 0)
                        NeutralApocalypseRoles.Add((typeof(Plaguebearer), CustomGameOptions.PlaguebearerOn, true));

                    if (CustomGameOptions.BakerOn > 0)
                        NeutralApocalypseRoles.Add((typeof(Baker), CustomGameOptions.BakerOn, true));

                    if (CustomGameOptions.BerserkerOn > 0)
                        NeutralApocalypseRoles.Add((typeof(Berserker), CustomGameOptions.BerserkerOn, true));

                    if (CustomGameOptions.SoulCollectorOn > 0)
                        NeutralApocalypseRoles.Add((typeof(SoulCollector), CustomGameOptions.SoulCollectorOn, true));

                    if (CustomGameOptions.WerewolfOn > 0)
                        NeutralKillingRoles.Add((typeof(Werewolf), CustomGameOptions.WerewolfOn, true));

                    if (CustomGameOptions.GameMode == GameMode.Classic && CustomGameOptions.VampireOn > 0)
                        NeutralKillingRoles.Add((typeof(Vampire), CustomGameOptions.VampireOn, true));

                    if ((CheckJugg() || CustomGameOptions.GameMode == GameMode.AllAny) && CustomGameOptions.HiddenRoles)
                        NeutralKillingRoles.Add((typeof(Juggernaut), 100, true));

                    if (CustomGameOptions.SerialKillerOn > 0)
                        NeutralKillingRoles.Add((typeof(SerialKiller), CustomGameOptions.SerialKillerOn, true));
                    #endregion
                    #region Impostor Roles
                    if (CustomGameOptions.UndertakerOn > 0)
                        ImpostorsRoles.Add((typeof(Undertaker), CustomGameOptions.UndertakerOn, true));

                    if (CustomGameOptions.MorphlingOn > 0)
                        ImpostorsRoles.Add((typeof(Morphling), CustomGameOptions.MorphlingOn, false));

                    if (CustomGameOptions.BlackmailerOn > 0)
                        ImpostorsRoles.Add((typeof(Blackmailer), CustomGameOptions.BlackmailerOn, true));

                    if (CustomGameOptions.MinerOn > 0)
                        ImpostorsRoles.Add((typeof(Miner), CustomGameOptions.MinerOn, true));

                    if (CustomGameOptions.SwooperOn > 0)
                        ImpostorsRoles.Add((typeof(Swooper), CustomGameOptions.SwooperOn, false));

                    if (CustomGameOptions.JanitorOn > 0)
                        ImpostorsRoles.Add((typeof(Janitor), CustomGameOptions.JanitorOn, false));

                    if (CustomGameOptions.GrenadierOn > 0)
                        ImpostorsRoles.Add((typeof(Grenadier), CustomGameOptions.GrenadierOn, true));

                    if (CustomGameOptions.EscapistOn > 0)
                        ImpostorsRoles.Add((typeof(Escapist), CustomGameOptions.EscapistOn, false));

                    if (CustomGameOptions.BomberOn > 0)
                        ImpostorsRoles.Add((typeof(Bomber), CustomGameOptions.BomberOn, true));

                    if (CustomGameOptions.WarlockOn > 0)
                        ImpostorsRoles.Add((typeof(Warlock), CustomGameOptions.WarlockOn, false));

                    if (CustomGameOptions.VenererOn > 0)
                        ImpostorsRoles.Add((typeof(Venerer), CustomGameOptions.VenererOn, true));

                    if (CustomGameOptions.PoisonerOn > 0)
                        ImpostorsRoles.Add((typeof(Poisoner), CustomGameOptions.PoisonerOn, true));

                    if (CustomGameOptions.SniperOn > 0)
                        ImpostorsRoles.Add((typeof(Sniper), CustomGameOptions.SniperOn, true));
                    #endregion
                    #region Crewmate Modifiers
                    if (Check(CustomGameOptions.TorchOn))
                        CrewmateModifiers.Add((typeof(Torch), CustomGameOptions.TorchOn));

                    if (Check(CustomGameOptions.DiseasedOn))
                        CrewmateModifiers.Add((typeof(Diseased), CustomGameOptions.DiseasedOn));

                    if (Check(CustomGameOptions.BaitOn))
                        CrewmateModifiers.Add((typeof(Bait), CustomGameOptions.BaitOn));

                    if (Check(CustomGameOptions.AftermathOn))
                        CrewmateModifiers.Add((typeof(Aftermath), CustomGameOptions.AftermathOn));

                    if (Check(CustomGameOptions.MultitaskerOn))
                        CrewmateModifiers.Add((typeof(Multitasker), CustomGameOptions.MultitaskerOn));

                    if (Check(CustomGameOptions.FrostyOn))
                        CrewmateModifiers.Add((typeof(Frosty), CustomGameOptions.FrostyOn));

                    if (Check(CustomGameOptions.ImpostorAgentOn))
                        ObjectiveCrewmateModifiers.Add((typeof(ImpostorAgent), CustomGameOptions.ImpostorAgentOn));

                    if (Check(CustomGameOptions.FamousOn))
                        CrewmateModifiers.Add((typeof(Famous), CustomGameOptions.FamousOn));
                    #endregion
                    #region Global Modifiers
                    if (Check(CustomGameOptions.TiebreakerOn))
                        GlobalModifiers.Add((typeof(Tiebreaker), CustomGameOptions.TiebreakerOn));

                    if (Check(CustomGameOptions.FlashOn))
                        GlobalModifiers.Add((typeof(Flash), CustomGameOptions.FlashOn));

                    if (Check(CustomGameOptions.GiantOn))
                        GlobalModifiers.Add((typeof(Giant), CustomGameOptions.GiantOn));

                    if (Check(CustomGameOptions.ButtonBarryOn))
                        ButtonModifiers.Add((typeof(ButtonBarry), CustomGameOptions.ButtonBarryOn));

                    if (Check(CustomGameOptions.LoversOn))
                        ObjectiveGlobalModifiers.Add((typeof(Lover), CustomGameOptions.LoversOn));

                    if (Check(CustomGameOptions.SleuthOn))
                        GlobalModifiers.Add((typeof(Sleuth), CustomGameOptions.SleuthOn));

                    if (Check(CustomGameOptions.RadarOn))
                        GlobalModifiers.Add((typeof(Radar), CustomGameOptions.RadarOn));

                    if (Check(CustomGameOptions.DrunkOn))
                        GlobalModifiers.Add((typeof(Drunk), CustomGameOptions.DrunkOn));
                    #endregion
                    #region Impostor Modifiers
                    if (Check(CustomGameOptions.DisperserOn))
                        ImpostorModifiers.Add((typeof(Disperser), CustomGameOptions.DisperserOn));

                    if (Check(CustomGameOptions.DoubleShotOn))
                        AssassinModifiers.Add((typeof(DoubleShot), CustomGameOptions.DoubleShotOn));

                    if (CustomGameOptions.UnderdogOn > 0)
                        ImpostorModifiers.Add((typeof(Underdog), CustomGameOptions.UnderdogOn));

                    if (CustomGameOptions.TaskerOn > 0)
                        ImpostorModifiers.Add((typeof(Tasker), CustomGameOptions.TaskerOn));
                    #endregion
                    #region Assassin Ability
                    AssassinAbility.Add((typeof(Assassin), CustomRPC.SetAssassin, 100));
                    #endregion
                }

                if (CustomGameOptions.GameMode == GameMode.Horseman)
                {
                    #region Crewmate Roles
                    if (CustomGameOptions.MayorOn > 0)
                        CrewmatesRoles.Add((typeof(Mayor), CustomGameOptions.MayorOn, true));

                    if (CustomGameOptions.SheriffOn > 0)
                        CrewmatesRoles.Add((typeof(Sheriff), CustomGameOptions.SheriffOn, false));

                    if (CustomGameOptions.EngineerOn > 0)
                        CrewmatesRoles.Add((typeof(Engineer), CustomGameOptions.EngineerOn, false));

                    if (CustomGameOptions.SwapperOn > 0)
                        CrewmatesRoles.Add((typeof(Swapper), CustomGameOptions.SwapperOn, true));

                    if (CustomGameOptions.InvestigatorOn > 0)
                        CrewmatesRoles.Add((typeof(Investigator), CustomGameOptions.InvestigatorOn, false));

                    if (CustomGameOptions.MedicOn > 0)
                        CrewmatesRoles.Add((typeof(Medic), CustomGameOptions.MedicOn, true));

                    if (CustomGameOptions.SeerOn > 0)
                        CrewmatesRoles.Add((typeof(Seer), CustomGameOptions.SeerOn, false));

                    if (CustomGameOptions.SpyOn > 0)
                        CrewmatesRoles.Add((typeof(Spy), CustomGameOptions.SpyOn, false));

                    if (CustomGameOptions.SnitchOn > 0)
                        CrewmatesRoles.Add((typeof(Snitch), CustomGameOptions.SnitchOn, true));

                    if (CustomGameOptions.AltruistOn > 0)
                        CrewmatesRoles.Add((typeof(Altruist), CustomGameOptions.AltruistOn, true));

                    if (CustomGameOptions.VigilanteOn > 0)
                        CrewmatesRoles.Add((typeof(Vigilante), CustomGameOptions.VigilanteOn, false));

                    if (CustomGameOptions.VeteranOn > 0)
                        CrewmatesRoles.Add((typeof(Veteran), CustomGameOptions.VeteranOn, false));

                    if (CustomGameOptions.HunterOn > 0)
                        CrewmatesRoles.Add((typeof(Hunter), CustomGameOptions.HunterOn, false));

                    if (CustomGameOptions.TrackerOn > 0)
                        CrewmatesRoles.Add((typeof(Tracker), CustomGameOptions.TrackerOn, false));

                    if (CustomGameOptions.TransporterOn > 0)
                        CrewmatesRoles.Add((typeof(Transporter), CustomGameOptions.TransporterOn, false));

                    if (CustomGameOptions.MediumOn > 0)
                        CrewmatesRoles.Add((typeof(Medium), CustomGameOptions.MediumOn, false));

                    if (CustomGameOptions.MysticOn > 0)
                        CrewmatesRoles.Add((typeof(Mystic), CustomGameOptions.MysticOn, false));

                    if (CustomGameOptions.TrapperOn > 0)
                        CrewmatesRoles.Add((typeof(Trapper), CustomGameOptions.TrapperOn, false));

                    if (CustomGameOptions.DetectiveOn > 0)
                        CrewmatesRoles.Add((typeof(Detective), CustomGameOptions.DetectiveOn, false));

                    if (CustomGameOptions.ImitatorOn > 0)
                        CrewmatesRoles.Add((typeof(Imitator), CustomGameOptions.ImitatorOn, true));

                    if (CustomGameOptions.ProsecutorOn > 0)
                        CrewmatesRoles.Add((typeof(Prosecutor), CustomGameOptions.ProsecutorOn, true));

                    if (CustomGameOptions.OracleOn > 0)
                        CrewmatesRoles.Add((typeof(Oracle), CustomGameOptions.OracleOn, true));

                    if (CustomGameOptions.AurialOn > 0)
                        CrewmatesRoles.Add((typeof(Aurial), CustomGameOptions.AurialOn, false));

                    if (CustomGameOptions.InspectorOn > 0)
                        CrewmatesRoles.Add((typeof(Inspector), CustomGameOptions.InspectorOn, false));

                    if (CustomGameOptions.MonarchOn > 0)
                        CrewmatesRoles.Add((typeof(Monarch), CustomGameOptions.MonarchOn, true));

                    if (CustomGameOptions.TavernKeeperOn > 0)
                        CrewmatesRoles.Add((typeof(TavernKeeper), CustomGameOptions.TavernKeeperOn, false));

                    if (CustomGameOptions.UndercoverOn > 0)
                        CrewmatesRoles.Add((typeof(Undercover), CustomGameOptions.UndercoverOn, true));

                    if (CustomGameOptions.LookoutOn > 0)
                        CrewmatesRoles.Add((typeof(Lookout), CustomGameOptions.LookoutOn, false));
                    #endregion
                    #region Neutral Roles
                    if (CustomGameOptions.JesterOn > 0)
                        NeutralEvilRoles.Add((typeof(Jester), CustomGameOptions.JesterOn, false));

                    if (CustomGameOptions.AmnesiacOn > 0)
                        NeutralBenignRoles.Add((typeof(Amnesiac), CustomGameOptions.AmnesiacOn, false));

                    if (CustomGameOptions.CursedSoulOn > 0)
                        NeutralBenignRoles.Add((typeof(CursedSoul), CustomGameOptions.CursedSoulOn, false));

                    if (CustomGameOptions.ExecutionerOn > 0)
                        NeutralEvilRoles.Add((typeof(Executioner), CustomGameOptions.ExecutionerOn, false));

                    if (CustomGameOptions.DoomsayerOn > 0)
                        NeutralChaosRoles.Add((typeof(Doomsayer), CustomGameOptions.DoomsayerOn, false));

                    if (CustomGameOptions.PirateOn > 0)
                        NeutralChaosRoles.Add((typeof(Pirate), CustomGameOptions.PirateOn, true));

                    if (CustomGameOptions.InquisitorOn > 0)
                        NeutralChaosRoles.Add((typeof(Inquisitor), CustomGameOptions.InquisitorOn, true));

                    if (CustomGameOptions.WitchOn > 0)
                        NeutralEvilRoles.Add((typeof(Witch), CustomGameOptions.WitchOn, true));

                    if (CustomGameOptions.SurvivorOn > 0)
                        NeutralBenignRoles.Add((typeof(Survivor), CustomGameOptions.SurvivorOn, false));

                    if (CustomGameOptions.GuardianAngelOn > 0)
                        NeutralBenignRoles.Add((typeof(GuardianAngel), CustomGameOptions.GuardianAngelOn, false));

                    if (CustomGameOptions.GlitchOn > 0)
                        NeutralKillingRoles.Add((typeof(Glitch), CustomGameOptions.GlitchOn, true));

                    if (CustomGameOptions.ArsonistOn > 0)
                        NeutralKillingRoles.Add((typeof(Arsonist), CustomGameOptions.ArsonistOn, true));

                    if (CustomGameOptions.WerewolfOn > 0)
                        NeutralKillingRoles.Add((typeof(Werewolf), CustomGameOptions.WerewolfOn, true));

                    if (CustomGameOptions.VampireOn > 0)
                        NeutralKillingRoles.Add((typeof(Vampire), CustomGameOptions.VampireOn, true));

                    if (CustomGameOptions.SerialKillerOn > 0)
                        NeutralKillingRoles.Add((typeof(SerialKiller), CustomGameOptions.SerialKillerOn, true));
                    #endregion
                    #region Crewmate Modifiers
                    if (Check(CustomGameOptions.TorchOn))
                        CrewmateModifiers.Add((typeof(Torch), CustomGameOptions.TorchOn));

                    if (Check(CustomGameOptions.DiseasedOn))
                        CrewmateModifiers.Add((typeof(Diseased), CustomGameOptions.DiseasedOn));

                    if (Check(CustomGameOptions.BaitOn))
                        CrewmateModifiers.Add((typeof(Bait), CustomGameOptions.BaitOn));

                    if (Check(CustomGameOptions.AftermathOn))
                        CrewmateModifiers.Add((typeof(Aftermath), CustomGameOptions.AftermathOn));

                    if (Check(CustomGameOptions.MultitaskerOn))
                        CrewmateModifiers.Add((typeof(Multitasker), CustomGameOptions.MultitaskerOn));

                    if (Check(CustomGameOptions.FrostyOn))
                        CrewmateModifiers.Add((typeof(Frosty), CustomGameOptions.FrostyOn));

                    if (Check(CustomGameOptions.ApocalypseAgentOn))
                        ObjectiveCrewmateModifiers.Add((typeof(ApocalypseAgent), CustomGameOptions.ApocalypseAgentOn));

                    if (Check(CustomGameOptions.FamousOn))
                        CrewmateModifiers.Add((typeof(Famous), CustomGameOptions.FamousOn));
                    #endregion
                    #region Global Modifiers
                    if (Check(CustomGameOptions.TiebreakerOn))
                        GlobalModifiers.Add((typeof(Tiebreaker), CustomGameOptions.TiebreakerOn));

                    if (Check(CustomGameOptions.FlashOn))
                        GlobalModifiers.Add((typeof(Flash), CustomGameOptions.FlashOn));

                    if (Check(CustomGameOptions.GiantOn))
                        GlobalModifiers.Add((typeof(Giant), CustomGameOptions.GiantOn));

                    if (Check(CustomGameOptions.ButtonBarryOn))
                        ButtonModifiers.Add((typeof(ButtonBarry), CustomGameOptions.ButtonBarryOn));

                    if (Check(CustomGameOptions.LoversOn))
                        ObjectiveGlobalModifiers.Add((typeof(Lover), CustomGameOptions.LoversOn));

                    if (Check(CustomGameOptions.SleuthOn))
                        GlobalModifiers.Add((typeof(Sleuth), CustomGameOptions.SleuthOn));

                    if (Check(CustomGameOptions.RadarOn))
                        GlobalModifiers.Add((typeof(Radar), CustomGameOptions.RadarOn));

                    if (Check(CustomGameOptions.DrunkOn))
                        GlobalModifiers.Add((typeof(Drunk), CustomGameOptions.DrunkOn));
                    #endregion
                    #region Apocalypse Roles
                    //_Plaguebearer.Add((typeof(Plaguebearer), 100, true));
                    //_Baker.Add((typeof(Baker), 100, true));
                    //_Berserker.Add((typeof(Berserker), 100, true));
                    //_SoulCollector.Add((typeof(SoulCollector), 100, true));
                    if (Check(CustomGameOptions.PlaguebearerOn))
                        NeutralApocalypseRoles.Add((typeof(Plaguebearer), CustomGameOptions.PlaguebearerOn, true));
                    if (Check(CustomGameOptions.BakerOn)) 
                        NeutralApocalypseRoles.Add((typeof(Baker), CustomGameOptions.BakerOn, true));
                    if (Check(CustomGameOptions.BerserkerOn)) 
                        NeutralApocalypseRoles.Add((typeof(Berserker), CustomGameOptions.BerserkerOn, true));
                    if (Check(CustomGameOptions.SoulCollectorOn)) 
                        NeutralApocalypseRoles.Add((typeof(SoulCollector), CustomGameOptions.SoulCollectorOn, true));
                    #endregion
                    #region Assassin Ability
                    AssassinAbility.Add((typeof(Assassin), CustomRPC.SetAssassin, 100));
                    #endregion
                }

                if (CustomGameOptions.GameMode == GameMode.KillingOnly) GenEachRoleKilling(infected.ToList());
                else if (CustomGameOptions.GameMode == GameMode.Cultist) GenEachRoleCultist(infected.ToList());
                else if (CustomGameOptions.GameMode == GameMode.Teams) GenEachRoleTeams(infected.ToList());
                else if (CustomGameOptions.GameMode == GameMode.SoloKiller) GenEachRoleKiller(infected.ToList());
                else GenEachRole(infected.ToList());
            }
        }
    }
}
