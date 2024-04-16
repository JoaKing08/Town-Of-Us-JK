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
            var message = $"Your bug on {interacted.Data.PlayerName} says that he was";
            switch (abilityId)
            {
                case 0:
                    switch (interactorRole)
                    {
                        case RoleEnum.Altruist:
                            message += " revived by Altruist";
                            break;
                        case RoleEnum.Aurial:
                            message += " radiated by Aurial";
                            break;
                        case RoleEnum.Detective:
                            message += " inspected by Detective";
                            break;
                        case RoleEnum.Inspector:
                            message += " inspected by Inspector";
                            break;
                        case RoleEnum.Medic:
                            message += " shielded by Medic";
                            break;
                        case RoleEnum.Medium:
                            message += " meditated by Medium";
                            break;
                        case RoleEnum.Monarch:
                            message += " knighted by Monarch";
                            break;
                        case RoleEnum.Oracle:
                            message += " confessed by Oracle";
                            break;
                        case RoleEnum.Seer:
                            message += " revealed by Seer";
                            break;
                        case RoleEnum.Sheriff:
                            message += " attacked by Sheriff";
                            break;
                        case RoleEnum.Spy:
                            message += " bugged by Spy";
                            break;
                        case RoleEnum.TavernKeeper:
                            message += " drunk by Tavern Keeper";
                            break;
                        case RoleEnum.Tracker:
                            message += " tracked by Tracker";
                            break;
                        case RoleEnum.Transporter:
                            message += " transported by Transporter";
                            break;
                        case RoleEnum.Trapper:
                            message += " trapped by Trapper";
                            break;
                        case RoleEnum.VampireHunter:
                            message += " checked by Vampire Hunter";
                            break;
                        case RoleEnum.Amnesiac:
                            message += " remembered by Amnesiac";
                            break;
                        case RoleEnum.Arsonist:
                            message += " doused by Arsonist";
                            break;
                        case RoleEnum.Doomsayer:
                            message += " observed by Doomsayer";
                            break;
                        case RoleEnum.Glitch:
                            message += " attacked by The Glitch";
                            break;
                        case RoleEnum.GuardianAngel:
                            message += " protected by Guardian Angel";
                            break;
                        case RoleEnum.Inquisitor:
                            message += " inquired by Inquisitor";
                            break;
                        case RoleEnum.Witch:
                            message += " controled by Witch";
                            break;
                        case RoleEnum.Juggernaut:
                            message += " attacked by Juggernaut";
                            break;
                        case RoleEnum.Pirate:
                            message += " dueled by Pirate";
                            break;
                        case RoleEnum.SerialKiller:
                            message += " attacked by Serial Killer";
                            break;
                        case RoleEnum.Vampire:
                            message += " bitted by Vampire";
                            break;
                        case RoleEnum.Werewolf:
                            message += " attacked by Werewolf";
                            break;
                        case RoleEnum.Baker:
                            message += " breaded by Baker";
                            break;
                        case RoleEnum.Berserker:
                            message += " attacked by Berserker";
                            break;
                        case RoleEnum.Famine:
                            message += " starved directly by Famine";
                            break;
                        case RoleEnum.Pestilence:
                            message += " attacked by Pestilence";
                            break;
                        case RoleEnum.Plaguebearer:
                            message += " plagued by Plaguebearer";
                            break;
                        case RoleEnum.SoulCollector:
                            message += " reaped by Soul Collector";
                            break;
                        case RoleEnum.War:
                            message += " attacked by War";
                            break;
                        case RoleEnum.Impostor:
                            message += " attacked by Impostor";
                            break;
                        case RoleEnum.Blackmailer:
                            message += " blackmailed by Blackmailer";
                            break;
                        case RoleEnum.Bomber:
                            message += " blown up by Bomber";
                            break;
                        case RoleEnum.Grenadier:
                            message += " blinded by Grenadier";
                            break;
                        case RoleEnum.Janitor:
                            message += " cleaned by Janitor";
                            break;
                        case RoleEnum.Morphling:
                            message += " sampled by Morphling";
                            break;
                        case RoleEnum.Poisoner:
                            message += " poisoned by Poisoner";
                            break;
                        case RoleEnum.Sniper:
                            message += " aimed by Sniper";
                            break;
                        case RoleEnum.Undertaker:
                            message += " dragged by Undertaker";
                            break;
                    }
                    break;
                case 1:
                    switch (interactorRole)
                    {
                        case RoleEnum.Detective:
                            message += " examined by Detective";
                            break;
                        case RoleEnum.VampireHunter:
                            message += " attacked by Vampire Hunter";
                            break;
                        case RoleEnum.Arsonist:
                            message += " ignited by Arsonist";
                            break;
                        case RoleEnum.Glitch:
                            message += " hacked by The Glitch";
                            break;
                        case RoleEnum.Inquisitor:
                            message += " attacked by Inquisitor";
                            break;
                        case RoleEnum.Witch:
                            message += " ordered by Witch";
                            break;
                        case RoleEnum.Famine:
                            message += " starved indirectly by Famine";
                            break;
                        case RoleEnum.Sniper:
                            message += " shot by Sniper";
                            break;
                    }
                    break;
            }
            Messages.Add(message);
        }
    }
}