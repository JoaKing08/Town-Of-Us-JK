using System.Collections.Generic;
using TownOfUs.CrewmateRoles.InvestigatorMod;
using TownOfUs.CrewmateRoles.MedicMod;
using System;
using System.Linq;
using TownOfUs.Extensions;
using AmongUs.GameOptions;
using TMPro;

namespace TownOfUs.Roles
{
    public class Investigator : Role
    {
        public readonly List<Footprint> AllPrints = new List<Footprint>();
        public List<string> Messages = new List<string>();
        public Dictionary<byte, List<byte>> Reports;
        public DateTime LastInvestigate { get; set; }
        public DeadBody CurrentTarget;

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;

        public Investigator(PlayerControl player) : base(player)
        {
            Name = "Investigator";
            ImpostorText = () => "Find All Impostors By Examining Footprints";
            TaskText = () => "You can see everyone's footprints, and check bodies";
            Color = Patches.Colors.Investigator;
            RoleType = RoleEnum.Investigator;
            AddToRoleHistory(RoleType);
            Scale = 1.4f;
            Reports = new Dictionary<byte, List<byte>>();
            foreach (var playerControl in PlayerControl.AllPlayerControls)
            {
                var report = new List<byte>() { 0, 1, 2, 3, 4, 5, 6 };
                report.Shuffle();
                Reports.Add(playerControl.PlayerId, report);
            }
            UsesLeft = CustomGameOptions.MaxInvestigates;
        }

        public float InvestigateTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInvestigate;
            var num = CustomGameOptions.InvestigateCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
        public void GenerateMessage(DeadBody target)
        {
            var matches = Murder.KilledPlayers.Where(x => x.PlayerId == target.ParentId);
            if (matches.Count() > 0)
            {
                var match = matches.First();
                var player = Utils.PlayerById(target.ParentId);
                var bodyTime = (DateTime.UtcNow - match.KillTime);
                var message = $"You investigated that {player.GetDefaultOutfit().PlayerName}";
                var killer = Utils.PlayerById(match.KillerId);
                var killerPosition = killer.transform.position;
                var bodyPosition = target.transform.position;
                var reportType = Reports[target.ParentId].First();
                var killerRole = Role.GetRole(killer);
                Reports[target.ParentId].Remove(reportType);
                switch (reportType)
                {
                    case 0:
                        if (bodyTime.TotalSeconds < 10) message += "'s wounds look very fresh (Killed Less 10s Before Investigate)";
                        else if (bodyTime.TotalSeconds < 20) message += "'s body lies here for a while (Killed Less 20s Before Investigate)";
                        else message += "'s body started to stink (Killed Less 20s Before Investigate)";
                        break;
                    case 1:
                        if (player.Is(Faction.Impostors)) message += "'s body had a knife next to it (Player Was Impostor)";
                        else if (player.Is(Faction.NeutralApocalypse)) message += "'s body radiates dark energy (Player Was Neutral Apocalypse)";
                        else if (player.Is(Faction.NeutralKilling)) message += " feels safe to be dead (Player Was Neutral Killing)";
                        else if (player.Is(Faction.NeutralEvil) || player.Is(Faction.NeutralChaos)) message += " is somewhat evil (Player Was Neutral Evil/Chaos)";
                        else if (player.Is(Faction.NeutralBenign)) message += " wasn't nor good nor bad (Player Was Neutral Benign)";
                        else if (player.Is(Faction.Crewmates)) message += " was just doing their tasks (Player Was Crewmate)";
                        break;
                    case 2:
                        if (match.KillerRunTo == KillerDirection.West) message += "'s killer has walked to see sunset (Killer Escaped West)";
                        else if (match.KillerRunTo == KillerDirection.East) message += "'s killer has sailed to the east (Killer Escaped East)";
                        else if (match.KillerRunTo == KillerDirection.North) message += "'s killer is wandering to the cold lands (Killer Escaped North)";
                        else if (match.KillerRunTo == KillerDirection.South) message += "'s killer is going to the equator (Killer Escaped South)";
                        else if (match.KillerKillAbility) message += "'s killers track was unable to identify (Killed By Ability)";
                        else message += "'s killer is near! (Killer Didn't Escape)";
                        break;
                    case 3:
                        var killerKills = killerRole.Kills + killerRole.CorrectAssassinKills + killerRole.IncorrectAssassinKills + killerRole.CorrectKills + killerRole.IncorrectAssassinKills;
                        if (killerKills > 5) message += "'s killer is a mass murderer (5+ Other Killer Kills)";
                        else if (killerKills > 3) message += "'s killer has large killing experience (3+ Other Killer Kills)";
                        else if (killerKills > 1) message += "'s killer did murder in the past (1+ Other Killer Kills)";
                        else message += "'s killer is an amateur (0 Other Killer Kills)";
                        break;
                    case 4:
                        if (match.KillerVented) message += "'s body is near open vent (Killer Vented)";
                        else if (match.KillerEscapeAbility) message += "'s killer vanished after kill (Killer Used Ability)";
                        else message += "'s killer escaped normally (Killer Walked Away)";
                        break;
                    case 5:
                        if (match.KillerKillAbility) message += " was killed in uncommon way (Killed By Ability)";
                        else message += " was killed normally, nothing revolutionary (Normal Kill)";
                        break;
                    case 6:
                        switch (killerRole.Faction)
                        {
                            case Faction.NeutralApocalypse:
                                if (killerRole.RoleType == RoleEnum.Pestilence || killerRole.RoleType == RoleEnum.Famine || killerRole.RoleType == RoleEnum.War || killerRole.RoleType == RoleEnum.Death) message += "'s body has been decimated (Killed By Transformed Horseman)";
                                else goto case Faction.NeutralKilling;
                                break;
                            case Faction.NeutralKilling:
                            case Faction.NeutralChaos:
                            case Faction.NeutralEvil:
                            case Faction.NeutralBenign:
                                message += "'s killer doesn't like anybody (Killed By Neutral)";
                                break;
                            case Faction.Crewmates:
                                message += "'s killer respects the law (Killed By Crewmate)";
                                break;
                            case Faction.Impostors:
                                message += "'s body was covered in stab wounds (Killed By Impostor)";
                                break;
                        }
                        break;
                }
                Messages.Add(message);
            }
            else Messages.Add("Error");
        }
    }
}