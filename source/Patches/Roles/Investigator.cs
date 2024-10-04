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
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "You can see everyone's footprints, and check bodies" : "Mozesz widziec wszystkich slady i sprawdzac ciala";
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
                var message = Patches.TranslationPatches.CurrentLanguage == 0 ? $"You investigated that <b>{player.GetDefaultOutfit().PlayerName}</b>" : "Sprawdziles ze";
                var killer = Utils.PlayerById(match.KillerId);
                var killerPosition = killer.transform.position;
                var bodyPosition = target.transform.position;
                var reportType = Reports[target.ParentId].First();
                var killerRole = Role.GetRole(killer);
                Reports[target.ParentId].Remove(reportType);
                switch (reportType)
                {
                    case 0:
                        if (bodyTime.TotalSeconds < 10) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s wounds look very fresh <b>(Killed Less Than 10s Before Investigate)</b>" : $" rany <b>{player.GetDefaultOutfit().PlayerName}</b> wygladaja na bardzo swieze <b>(Zabito Mniej Niz 10s Przed Sprawdzeniem)</b>";
                        else if (bodyTime.TotalSeconds < 20) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s body lies here for a while <b>(Killed Less Than 20s Before Investigate)</b>" : $" <b>{player.GetDefaultOutfit().PlayerName}</b> lezy tu juz chwile <b>(Zabito Mniej Niz 20s Przed Sprawdzeniem)</b>";
                        else message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s body started to stink <b>(Killed More Than 20s Before Investigate)</b>" : $" <b>{player.GetDefaultOutfit().PlayerName}</b> zaczal smierdziec <b>(Zabito Wiecej Niz 20s Przed Sprawdzeniem)</b>";
                        break;
                    case 1:
                        if (player.Is(Faction.Impostors) && !player.Is((RoleEnum)254)) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s body had a knife next to it <b>(Player Was <color=#FF0000FF>Impostor</color>)</b>" : $" <b>{player.GetDefaultOutfit().PlayerName}</b> mial nóz przy sobie <b>(Graczem Byl <color=#FF0000FF>Impostor</color>)</b>";
                        else if (player.Is(Faction.NeutralApocalypse)) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s body radiates dark energy <b>(Player Was <color=#808080FF>Neutral</color> <color=#0000FFFF>Apocalypse</color>)</b>" : $" <b>{player.GetDefaultOutfit().PlayerName}</b> promieniuje mroczna energia <b>(Graczem Byl <color=#808080FF>Neutral</color> <color=#0000FFFF>Apocalypse</color>)</b>";
                        else if (player.Is(Faction.NeutralKilling)) message += Patches.TranslationPatches.CurrentLanguage == 0 ? " feels safe to be dead <b>(Player Was <color=#808080FF>Neutral</color> <color=#0000FFFF>Killing</color>)</b>" : $" czujesz sie bezpieczny z <b>{player.GetDefaultOutfit().PlayerName}</b> martwym <b>(Graczem Byl <color=#808080FF>Neutral</color> <color=#0000FFFF>Killing</color>)</b>";
                        else if (player.Is(Faction.NeutralEvil) || player.Is(Faction.NeutralChaos)) message += Patches.TranslationPatches.CurrentLanguage == 0 ? " is somewhat evil <b>(Player Was <color=#808080FF>Neutral</color> <color=#0000FFFF>Evil</color>/<color=#0000FFFF>Chaos</color>)</b>" : $" <b>{player.GetDefaultOutfit().PlayerName}</b> byl w jakims sesie zly <b>(Graczem Byl <color=#808080FF>Neutral</color> <color=#0000FFFF>Evil</color>/<color=#0000FFFF>Chaos</color>)</b>";
                        else if (player.Is(Faction.NeutralBenign)) message += Patches.TranslationPatches.CurrentLanguage == 0 ? " wasn't nor good nor bad <b>(Player Was <color=#808080FF>Neutral</color> <color=#0000FFFF>Benign</color>)</b>" : $" <b>{player.GetDefaultOutfit().PlayerName}</b> nie byl ani zly ani dobry <b>(Graczem Byl <color=#808080FF>Neutral</color> <color=#0000FFFF>Benign</color>)</b>";
                        else if (player.Is(Faction.Crewmates) || player.Is((RoleEnum)254)) message += Patches.TranslationPatches.CurrentLanguage == 0 ? " was just doing their tasks <b>(Player Was <color=#00FFFFFF>Crewmate</color>)</b>" : $" <b>{player.GetDefaultOutfit().PlayerName}</b> tylko robil zadania <b>(Graczem Byl <color=#00FFFFFF>Crewmate</color>)</b>";
                        break;
                    case 2:
                        if (match.KillerKillAbility) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s killers track was unable to identify <b>(Killed By Ability)</b>" : $" slady zabójcy <b>{player.GetDefaultOutfit().PlayerName}</b> byly niemozliwe do znalezienia <b>(Zabito Umiejetnoscia)</b>";
                        else if (match.KillerRunTo == KillerDirection.West) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s killer has walked to see sunset <b>(Killer Escaped West)</b>" : $" zabójca <b>{player.GetDefaultOutfit().PlayerName}</b> poszedl ogladac zachód <b>(Zabójca Uciekl Na Zachód)</b>";
                        else if (match.KillerRunTo == KillerDirection.East) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s killer has sailed to the east <b>(Killer Escaped East)</b>" : $" zabójca <b>{player.GetDefaultOutfit().PlayerName}</b> poplynal na wschód <b>(Zabójca Uciekl Na Wschód)</b>";
                        else if (match.KillerRunTo == KillerDirection.North) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s killer is wandering to the cold lands <b>(Killer Escaped North)</b>" : $" zabójca <b>{player.GetDefaultOutfit().PlayerName}</b> polazl do zimnych krain <b>(Zabójca Uciekl Na Pólnoc)</b>";
                        else if (match.KillerRunTo == KillerDirection.South) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s killer is going to the equator <b>(Killer Escaped South)</b>" : $" zabójca <b>{player.GetDefaultOutfit().PlayerName}</b> idzie na równik <b>(Zabójca Uciekl Na Poludnie)</b>";
                        else message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s killer is near! <b>(Killer Didn't Escape)</b>" : $" zabójca <b>{player.GetDefaultOutfit().PlayerName}</b> jest blisko! <b>(Zabójca Nie Uciekl)</b>";
                        break;
                    case 3:
                        var killerKills = killerRole.Kills + killerRole.CorrectAssassinKills + killerRole.IncorrectAssassinKills + killerRole.CorrectKills + killerRole.IncorrectKills;
                        if (killerKills > 5) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s killer is a mass murderer <b>(5+ Other Killer Kills)</b>" : $" zabójca <b>{player.GetDefaultOutfit().PlayerName}</b> jest masowym morderca <b>(5+ Innych Zabójstw Zabójcy)</b>";
                        else if (killerKills > 3) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s killer has large killing experience <b>(3+ Other Killer Kills)</b>" : $" zabójca <b>{player.GetDefaultOutfit().PlayerName}</b> ma duze doswiadczenie <b>(3+ Innych Zabójstw Zabójcy)</b>";
                        else if (killerKills > 1) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s killer did murder in the past <b>(1+ Other Killer Kills)</b>" : $" zabójca <b>{player.GetDefaultOutfit().PlayerName}</b> mordowal juz wczesniej <b>(1+ Innych Zabójstw Zabójcy)</b>";
                        else message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s killer is an amateur <b>(0 Other Killer Kills)</b>" : $" zabójca <b>{player.GetDefaultOutfit().PlayerName}</b> jest amatorem <b>(0 Innych Zabójstw Zabójcy)</b>";
                        break;
                    case 4:
                        if (match.KillerVented) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s body is near open vent <b>(Killer Vented)</b>" : $" <b>{player.GetDefaultOutfit().PlayerName}</b> lezy obok otwartego wenta <b>(Zabójca Wentowal)</b>";
                        else if (match.KillerEscapeAbility) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s killer vanished after kill <b>(Killer Used Ability)</b>" : $" zabójca <b>{player.GetDefaultOutfit().PlayerName}</b> zniknal po zabójstwie <b>(Zabójca Uzyl Umiejetnosci)</b>";
                        else message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s killer escaped normally <b>(Killer Walked Away)</b>" : $" zabójca <b>{player.GetDefaultOutfit().PlayerName}</b> uciekl normalnie <b>(Zabójca Odszedl Od Trupa)</b>";
                        break;
                    case 5:
                        if (match.KillerKillAbility) message += Patches.TranslationPatches.CurrentLanguage == 0 ? " was killed in uncommon way <b>(Killed By Ability)</b>" : $" <b>{player.GetDefaultOutfit().PlayerName}</b> zostal zabity w nienormalny sposób <b>(Zabójstwo Umiejetnoscia)</b>";
                        else message += Patches.TranslationPatches.CurrentLanguage == 0 ? " was killed normally, nothing revolutionary <b>(Normal Kill)</b>" : $" <b>{player.GetDefaultOutfit().PlayerName}</b> zostal zabity normalnie, nic rewolucyjnego <b>(Normalne Zabójstwo)</b>";
                        break;
                    case 6:
                        switch (killerRole.Faction)
                        {
                            case Faction.NeutralApocalypse:
                                if (killerRole.RoleType == RoleEnum.Pestilence || killerRole.RoleType == RoleEnum.Famine || killerRole.RoleType == RoleEnum.War || killerRole.RoleType == RoleEnum.Death) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s body has been decimated <b>(Killed By Transformed <color=#404040FF>Horseman</color>)</b>" : $" cialo <b>{player.GetDefaultOutfit().PlayerName}</b> zostalo unicestwione <b>(Zabity Przez Przemienionego <color=#404040FF>Jezdzca</color>)</b>";
                                else goto case Faction.NeutralKilling;
                                break;
                            case Faction.NeutralKilling:
                            case Faction.NeutralChaos:
                            case Faction.NeutralEvil:
                            case Faction.NeutralBenign:
                                message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s killer doesn't like anybody <b>(Killed By <color=#808080FF>Neutral</color>)</b>" : $" zabójca <b>{player.GetDefaultOutfit().PlayerName}</b> nie lubi nikogo <b>(Zabity Przez <color=#808080FF>Neutral</color>)</b>";
                                break;
                            case Faction.Crewmates:
                                message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s killer respects the law <b>(Killed By <color=#00FFFFFF>Crewmate</color>)</b>" : $" zabójca <b>{player.GetDefaultOutfit().PlayerName}</b> ma respekt do prawa <b>(Zabity Przez <color=#00FFFFFF>Crewmate</color>)</b>";
                                break;
                            case Faction.Impostors:
                                if (killerRole.RoleType != (RoleEnum)254) message += Patches.TranslationPatches.CurrentLanguage == 0 ? "'s body was covered in stab wounds <b>(Killed By <color=#FF0000FF>Impostor</color>)</b>" : $" <b>{player.GetDefaultOutfit().PlayerName}</b> byl pokryty w ranach cietych <b>(Zabity Przez <color=#FF0000FF>Impostor</color>)</b>";
                                else goto case Faction.Crewmates;
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