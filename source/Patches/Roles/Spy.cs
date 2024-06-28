using Reactor.Utilities.Extensions;
using System;
using System.Collections.Generic;
using TMPro;

namespace TownOfUs.Roles
{
    public class Spy : Role
    {
        public DateTime LastBugged;
        public PlayerControl ClosestPlayer;
        public List<byte> BuggedPlayers;
        public List<string> Messages;
        public int BugsLeft;
        public TextMeshPro UsesText;
        public bool ButtonUsable => BugsLeft != 0;
        public Spy(PlayerControl player) : base(player)
        {
            Name = "Spy";
            ImpostorText = () => "Snoop Around And Find Stuff Out";
            TaskText = () => "Gain extra information on the Admin Table, and give\nbugs to players to know what happend to them";
            Color = Patches.Colors.Spy;
            RoleType = RoleEnum.Spy;
            BugsLeft = CustomGameOptions.BugsPerGame;
            LastBugged = DateTime.UtcNow;
            BuggedPlayers = new List<byte>();
            Messages = new List<string>();
            AddToRoleHistory(RoleType);
        }

        public float BugTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBugged;
            var num = CustomGameOptions.BugCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        internal void GenerateMessage(PlayerControl interacted, RoleEnum interactorRole, byte abilityId)
        {
            var message = $"Your bug on <b>{interacted.Data.PlayerName}</b> says that he was";
            switch (abilityId)
            {
                case 0:
                    switch (interactorRole)
                    {
                        case RoleEnum.Altruist:
                            message += $" revived by <b><color=#{Patches.Colors.Altruist.ToHtmlStringRGBA()}>Altruist</color></b>";
                            break;
                        case RoleEnum.Aurial:
                            message += $" radiated by <b><color=#{Patches.Colors.Aurial.ToHtmlStringRGBA()}>Aurial</color></b>";
                            break;
                        case RoleEnum.Detective:
                            message += $" inspected by <b><color=#{Patches.Colors.Detective.ToHtmlStringRGBA()}>Detective</color></b>";
                            break;
                        case RoleEnum.Inspector:
                            message += $" inspected by <b><color=#{Patches.Colors.Inspector.ToHtmlStringRGBA()}>Inspector</color></b>";
                            break;
                        case RoleEnum.Medic:
                            message += $" shielded by <b><color=#{Patches.Colors.Medic.ToHtmlStringRGBA()}>Medic</color></b>";
                            break;
                        case RoleEnum.Medium:
                            message += $" meditated by <b><color=#{Patches.Colors.Medium.ToHtmlStringRGBA()}>Medium</color></b>";
                            break;
                        case RoleEnum.Monarch:
                            message += $" knighted by <b><color=#{Patches.Colors.Monarch.ToHtmlStringRGBA()}>Monarch</color></b>";
                            break;
                        case RoleEnum.Oracle:
                            message += $" confessed by <b><color=#{Patches.Colors.Oracle.ToHtmlStringRGBA()}>Oracle</color></b>";
                            break;
                        case RoleEnum.Seer:
                            message += $" revealed by <b><color=#{Patches.Colors.Seer.ToHtmlStringRGBA()}>Seer</color></b>";
                            break;
                        case RoleEnum.Sheriff:
                            message += $" attacked by <b><color=#{Patches.Colors.Sheriff.ToHtmlStringRGBA()}>Sheriff</color></b>";
                            break;
                        case RoleEnum.Spy:
                            message += $" bugged by <b><color=#{Patches.Colors.Spy.ToHtmlStringRGBA()}>Spy</color></b>";
                            break;
                        case RoleEnum.TavernKeeper:
                            message += $" drunk by <b><color=#{Patches.Colors.TavernKeeper.ToHtmlStringRGBA()}>Tavern Keeper</color></b>";
                            break;
                        case RoleEnum.Tracker:
                            message += $" tracked by <b><color=#{Patches.Colors.Tracker.ToHtmlStringRGBA()}>Tracker</color></b>";
                            break;
                        case RoleEnum.Transporter:
                            message += $" transported by <b><color=#{Patches.Colors.Transporter.ToHtmlStringRGBA()}>Transporter</color></b>";
                            break;
                        case RoleEnum.Trapper:
                            message += $" trapped by <b><color=#{Patches.Colors.Trapper.ToHtmlStringRGBA()}>Trapper</color></b>";
                            break;
                        case RoleEnum.VampireHunter:
                            message += $" checked by <b><color=#{Patches.Colors.VampireHunter.ToHtmlStringRGBA()}>Vampire Hunter</color></b>";
                            break;
                        case RoleEnum.Amnesiac:
                            message += $" remembered by <b><color=#{Patches.Colors.Amnesiac.ToHtmlStringRGBA()}>Amnesiac</color></b>";
                            break;
                        case RoleEnum.Arsonist:
                            message += $" doused by <b><color=#{Patches.Colors.Arsonist.ToHtmlStringRGBA()}>Arsonist</color></b>";
                            break;
                        case RoleEnum.Doomsayer:
                            message += $" observed by <b><color=#{Patches.Colors.Doomsayer.ToHtmlStringRGBA()}>Doomsayer</color></b>";
                            break;
                        case RoleEnum.Glitch:
                            message += $" attacked by <b><color=#{Patches.Colors.Glitch.ToHtmlStringRGBA()}>The Glitch</color></b>";
                            break;
                        case RoleEnum.GuardianAngel:
                            message += $" protected by <b><color=#{Patches.Colors.GuardianAngel.ToHtmlStringRGBA()}>Guardian Angel</color></b>";
                            break;
                        case RoleEnum.Inquisitor:
                            message += $" inquired by <b><color=#{Patches.Colors.Inquisitor.ToHtmlStringRGBA()}>Inquisitor</color></b>";
                            break;
                        case RoleEnum.Witch:
                            message += $" controled by <b><color=#{Patches.Colors.Witch.ToHtmlStringRGBA()}>Witch</color></b>";
                            break;
                        case RoleEnum.Juggernaut:
                            message += $" attacked by <b><color=#{Patches.Colors.Juggernaut.ToHtmlStringRGBA()}>Juggernaut</color></b>";
                            break;
                        case RoleEnum.Pirate:
                            message += $" dueled by <b><color=#{Patches.Colors.Pirate.ToHtmlStringRGBA()}>Pirate</color></b>";
                            break;
                        case RoleEnum.SerialKiller:
                            message += $" attacked by <b><color=#{Patches.Colors.SerialKiller.ToHtmlStringRGBA()}>Serial Killer</color></b>";
                            break;
                        case RoleEnum.Vampire:
                            message += $" bitted by <b><color=#{Patches.Colors.Vampire.ToHtmlStringRGBA()}>Vampire</color></b>";
                            break;
                        case RoleEnum.Werewolf:
                            message += $" attacked by <b><color=#{Patches.Colors.Werewolf.ToHtmlStringRGBA()}>Werewolf</color></b>";
                            break;
                        case RoleEnum.Baker:
                            message += $" breaded by <b><color=#{Patches.Colors.Baker.ToHtmlStringRGBA()}>Baker</color></b>";
                            break;
                        case RoleEnum.Berserker:
                            message += $" attacked by <b><color=#{Patches.Colors.Berserker.ToHtmlStringRGBA()}>Berserker</color></b>";
                            break;
                        case RoleEnum.Famine:
                            message += $" starved directly by <b><color=#{Patches.Colors.Famine.ToHtmlStringRGBA()}>Famine</color></b>";
                            break;
                        case RoleEnum.Pestilence:
                            message += $" attacked by <b><color=#{Patches.Colors.Pestilence.ToHtmlStringRGBA()}>Pestilence</color></b>";
                            break;
                        case RoleEnum.Plaguebearer:
                            message += $" plagued by <b><color=#{Patches.Colors.Plaguebearer.ToHtmlStringRGBA()}>Plaguebearer</color></b>";
                            break;
                        case RoleEnum.SoulCollector:
                            message += $" reaped by <b><color=#{Patches.Colors.SoulCollector.ToHtmlStringRGBA()}>Soul Collector</color></b>";
                            break;
                        case RoleEnum.War:
                            message += $" attacked by <b><color=#{Patches.Colors.War.ToHtmlStringRGBA()}>War</color></b>";
                            break;
                        case RoleEnum.Impostor:
                            message += $" attacked by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Impostor</color></b>";
                            break;
                        case RoleEnum.Blackmailer:
                            message += $" blackmailed by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Blackmailer</color></b>";
                            break;
                        case RoleEnum.Bomber:
                            message += $" blown up by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Bomber</color></b>";
                            break;
                        case RoleEnum.Grenadier:
                            message += $" blinded by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Grenadier</color></b>";
                            break;
                        case RoleEnum.Janitor:
                            message += $" cleaned by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Janitor</color></b>";
                            break;
                        case RoleEnum.Morphling:
                            message += $" sampled by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Morphling</color></b>";
                            break;
                        case RoleEnum.Poisoner:
                            message += $" poisoned by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Poisoner</color></b>";
                            break;
                        case RoleEnum.Sniper:
                            message += $" aimed by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Sniper</color></b>";
                            break;
                        case RoleEnum.Undertaker:
                            message += $" dragged by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Undertaker</color></b>";
                            break;
                        case RoleEnum.CursedSoul:
                            message += $" attempted to <b>Soul Swap</b> directly by <b><color=#{Patches.Colors.CursedSoul.ToHtmlStringRGBA()}>Cursed Soul</color></b>";
                            break;
                        case RoleEnum.Hunter:
                            message += $" stalked by <b><color=#{Patches.Colors.Hunter.ToHtmlStringRGBA()}>Hunter</color></b>";
                            break;
                        case RoleEnum.JKNecromancer:
                            message += $" revived by <b><color=#{Patches.Colors.Necromancer.ToHtmlStringRGBA()}>Necromancer</color></b>";
                            break;
                        case RoleEnum.Jackal:
                            message += $" attacked by <b><color=#{Patches.Colors.Jackal.ToHtmlStringRGBA()}>Jackal</color></b>";
                            break;
                        case RoleEnum.Bodyguard:
                            message += $" guarded by <b><color=#{Patches.Colors.Bodyguard.ToHtmlStringRGBA()}>Bodyguard</color></b>";
                            break;
                        case RoleEnum.Crusader:
                            message += $" fortified by <b><color=#{Patches.Colors.Crusader.ToHtmlStringRGBA()}>Crusader</color></b>";
                            break;
                        case RoleEnum.Cleric:
                            message += $" barriered by <b><color=#{Patches.Colors.Cleric.ToHtmlStringRGBA()}>Cleric</color></b>";
                            break;
                        case RoleEnum.Deputy:
                            message += $" aimed by <b><color=#{Patches.Colors.Deputy.ToHtmlStringRGBA()}>Deputy</color></b>";
                            break;
                        case RoleEnum.Mystic:
                            message += $" visioned by <b><color=#{Patches.Colors.Mystic.ToHtmlStringRGBA()}>Mystic</color></b>";
                            break;
                        case RoleEnum.Sage:
                            message += $" compared by <b><color=#{Patches.Colors.Sage.ToHtmlStringRGBA()}>Sage</color></b>";
                            break;
                        case RoleEnum.Demagogue:
                            message += $" convinced by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Demagogue</color></b>";
                            break;
                        case RoleEnum.Godfather:
                            message += $" recruited by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Godfather</color></b>";
                            break;
                        case RoleEnum.Occultist:
                            message += $" marked by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Occultist</color></b>";
                            break;
                    }
                    break;
                case 1:
                    switch (interactorRole)
                    {
                        case RoleEnum.Detective:
                            message += $" examined by <b><color=#{Patches.Colors.Detective.ToHtmlStringRGBA()}>Detective</color></b>";
                            break;
                        case RoleEnum.VampireHunter:
                            message += $" attacked by <b><color=#{Patches.Colors.VampireHunter.ToHtmlStringRGBA()}>Vampire Hunter</color></b>";
                            break;
                        case RoleEnum.Arsonist:
                            message += $" ignited by <b><color=#{Patches.Colors.Arsonist.ToHtmlStringRGBA()}>Arsonist</color></b>";
                            break;
                        case RoleEnum.Glitch:
                            message += $" hacked by <b><color=#{Patches.Colors.Glitch.ToHtmlStringRGBA()}>The Glitch</color></b>";
                            break;
                        case RoleEnum.Inquisitor:
                            message += $" attacked by <b><color=#{Patches.Colors.Inquisitor.ToHtmlStringRGBA()}>Inquisitor</color></b>";
                            break;
                        case RoleEnum.Witch:
                            message += $" ordered by <b><color=#{Patches.Colors.Witch.ToHtmlStringRGBA()}>Witch</color></b>";
                            break;
                        case RoleEnum.Famine:
                            message += $" starved indirectly by <b><color=#{Patches.Colors.Famine.ToHtmlStringRGBA()}>Famine</color></b>";
                            break;
                        case RoleEnum.Sniper:
                            message += $" shot by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Sniper</color></b>";
                            break;
                        case RoleEnum.CursedSoul:
                            message += $" attempted to <b>Soul Swap</b> indirectly by <b><color=#{Patches.Colors.CursedSoul.ToHtmlStringRGBA()}>Cursed Soul</color></b>";
                            break;
                        case RoleEnum.Hunter:
                            message += $" attacked by <b><color=#{Patches.Colors.Hunter.ToHtmlStringRGBA()}>Hunter</color></b>";
                            break;
                        case RoleEnum.JKNecromancer:
                            message += $" attacked by <b><color=#{Patches.Colors.Necromancer.ToHtmlStringRGBA()}>Necromancer</color></b>";
                            break;
                        case RoleEnum.Bodyguard:
                            message += $" attacked by <b><color=#{Patches.Colors.Bodyguard.ToHtmlStringRGBA()}>Bodyguard</color></b>";
                            break;
                        case RoleEnum.Crusader:
                            message += $" attacked by <b><color=#{Patches.Colors.Crusader.ToHtmlStringRGBA()}>Crusader</color></b>";
                            break;
                    }
                    break;
                case 2:
                    switch (interactorRole)
                    {
                        case RoleEnum.CursedSoul:
                            message += $" <b>Soul Swapped</b> using <b><color=#{Patches.Colors.CursedSoul.ToHtmlStringRGBA()}>Cursed Soul</color></b>";
                            break;
                    }
                    break;
            }
            Messages.Add(message);
        }
    }
}