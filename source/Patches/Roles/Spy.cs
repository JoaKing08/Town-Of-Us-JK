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
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Gain extra information on the Admin Table, and give\nbugs to players to know what happend to them" : "Zdobywaj dodatkowe informacje na Admin Table, i podkladaj\npluskwy graczom by wiedziec co im sie stalo";
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
            var message = Patches.TranslationPatches.CurrentLanguage == 0 ? $"Your bug on <b>{interacted.Data.PlayerName}</b> says that he was" : $"Twoja pluskwa na <b>{interacted.Data.PlayerName}</b> wykryla ze zostal on";
            switch (abilityId)
            {
                case 0:
                    switch (interactorRole)
                    {
                        case RoleEnum.Altruist:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" revived by <b><color=#{Patches.Colors.Altruist.ToHtmlStringRGBA()}>Altruist</color></b>" : $" wskrzeszony przez <b><color=#{Patches.Colors.Altruist.ToHtmlStringRGBA()}>Altruist</color></b>";
                            break;
                        case RoleEnum.Aurial:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" radiated by <b><color=#{Patches.Colors.Aurial.ToHtmlStringRGBA()}>Aurial</color></b>" : $" napromieniowany przez <b><color=#{Patches.Colors.Aurial.ToHtmlStringRGBA()}>Aurial</color></b>";
                            break;
                        case RoleEnum.Detective:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" inspected by <b><color=#{Patches.Colors.Detective.ToHtmlStringRGBA()}>Detective</color></b>" : $" zinspektowany przez <b><color=#{Patches.Colors.Detective.ToHtmlStringRGBA()}>Detective</color></b>";
                            break;
                        case RoleEnum.Inspector:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" inspected by <b><color=#{Patches.Colors.Inspector.ToHtmlStringRGBA()}>Inspector</color></b>" : $" zinspektowany przez <b><color=#{Patches.Colors.Inspector.ToHtmlStringRGBA()}>Inspector</color></b>";
                            break;
                        case RoleEnum.Medic:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" shielded by <b><color=#{Patches.Colors.Medic.ToHtmlStringRGBA()}>Medic</color></b>" : $" ochroniony przez <b><color=#{Patches.Colors.Medic.ToHtmlStringRGBA()}>Medic</color></b>";
                            break;
                        case RoleEnum.Medium:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" meditated by <b><color=#{Patches.Colors.Medium.ToHtmlStringRGBA()}>Medium</color></b>" : $" wykryty przez <b><color=#{Patches.Colors.Medium.ToHtmlStringRGBA()}>Medium</color></b>";
                            break;
                        case RoleEnum.Monarch:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" knighted by <b><color=#{Patches.Colors.Monarch.ToHtmlStringRGBA()}>Monarch</color></b>" : $" mianowany przez <b><color=#{Patches.Colors.Monarch.ToHtmlStringRGBA()}>Monarch</color></b>";
                            break;
                        case RoleEnum.Oracle:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" confessed by <b><color=#{Patches.Colors.Oracle.ToHtmlStringRGBA()}>Oracle</color></b>" : $" spowiadany przez <b><color=#{Patches.Colors.Oracle.ToHtmlStringRGBA()}>Oracle</color></b>";
                            break;
                        case RoleEnum.Seer:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" revealed by <b><color=#{Patches.Colors.Seer.ToHtmlStringRGBA()}>Seer</color></b>" : $" ujawniony przez <b><color=#{Patches.Colors.Seer.ToHtmlStringRGBA()}>Seer</color></b>";
                            break;
                        case RoleEnum.Sheriff:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.Sheriff.ToHtmlStringRGBA()}>Sheriff</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.Sheriff.ToHtmlStringRGBA()}>Sheriff</color></b>";
                            break;
                        case RoleEnum.Spy:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" bugged by <b><color=#{Patches.Colors.Spy.ToHtmlStringRGBA()}>Spy</color></b>" : $" obserwowany przez <b><color=#{Patches.Colors.Spy.ToHtmlStringRGBA()}>Spy</color></b>";
                            break;
                        case RoleEnum.TavernKeeper:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" drunk by <b><color=#{Patches.Colors.TavernKeeper.ToHtmlStringRGBA()}>Tavern Keeper</color></b>" : $" upity przez <b><color=#{Patches.Colors.TavernKeeper.ToHtmlStringRGBA()}>Tavern Keeper</color></b>";
                            break;
                        case RoleEnum.Tracker:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" tracked by <b><color=#{Patches.Colors.Tracker.ToHtmlStringRGBA()}>Tracker</color></b>" : $" sledzony przez <b><color=#{Patches.Colors.Tracker.ToHtmlStringRGBA()}>Tracker</color></b>";
                            break;
                        case RoleEnum.Transporter:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" transported by <b><color=#{Patches.Colors.Transporter.ToHtmlStringRGBA()}>Transporter</color></b>" : $" przeniesiony przez <b><color=#{Patches.Colors.Transporter.ToHtmlStringRGBA()}>Transporter</color></b>";
                            break;
                        case RoleEnum.Trapper:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" trapped by <b><color=#{Patches.Colors.Trapper.ToHtmlStringRGBA()}>Trapper</color></b>" : $" zpulapkowany przez <b><color=#{Patches.Colors.Trapper.ToHtmlStringRGBA()}>Trapper</color></b>";
                            break;
                        case RoleEnum.VampireHunter:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" checked by <b><color=#{Patches.Colors.VampireHunter.ToHtmlStringRGBA()}>Vampire Hunter</color></b>" : $" sprawdzony przez <b><color=#{Patches.Colors.VampireHunter.ToHtmlStringRGBA()}>Vampire Hunter</color></b>";
                            break;
                        case RoleEnum.Amnesiac:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" remembered by <b><color=#{Patches.Colors.Amnesiac.ToHtmlStringRGBA()}>Amnesiac</color></b>" : $" zapamietany przez <b><color=#{Patches.Colors.Amnesiac.ToHtmlStringRGBA()}>Amnesiac</color></b>";
                            break;
                        case RoleEnum.Arsonist:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" doused by <b><color=#{Patches.Colors.Arsonist.ToHtmlStringRGBA()}>Arsonist</color></b>" : $" polany przez <b><color=#{Patches.Colors.Arsonist.ToHtmlStringRGBA()}>Arsonist</color></b>";
                            break;
                        case RoleEnum.Doomsayer:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" observed by <b><color=#{Patches.Colors.Doomsayer.ToHtmlStringRGBA()}>Doomsayer</color></b>" : $" obserwowany przez <b><color=#{Patches.Colors.Doomsayer.ToHtmlStringRGBA()}>Doomsayer</color></b>";
                            break;
                        case RoleEnum.Glitch:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.Glitch.ToHtmlStringRGBA()}>The Glitch</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.Glitch.ToHtmlStringRGBA()}>The Glitch</color></b>";
                            break;
                        case RoleEnum.GuardianAngel:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" protected by <b><color=#{Patches.Colors.GuardianAngel.ToHtmlStringRGBA()}>Guardian Angel</color></b>" : $" chroniony przez <b><color=#{Patches.Colors.GuardianAngel.ToHtmlStringRGBA()}>Guardian Angel</color></b>";
                            break;
                        case RoleEnum.Inquisitor:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" inquired by <b><color=#{Patches.Colors.Inquisitor.ToHtmlStringRGBA()}>Inquisitor</color></b>" : $" przepytany przez <b><color=#{Patches.Colors.Inquisitor.ToHtmlStringRGBA()}>Inquisitor</color></b>";
                            break;
                        case RoleEnum.Witch:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" controled by <b><color=#{Patches.Colors.Witch.ToHtmlStringRGBA()}>Witch</color></b>" : $" kontrolowany przez <b><color=#{Patches.Colors.Witch.ToHtmlStringRGBA()}>Witch</color></b>";
                            break;
                        case RoleEnum.Juggernaut:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.Juggernaut.ToHtmlStringRGBA()}>Juggernaut</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.Juggernaut.ToHtmlStringRGBA()}>Juggernaut</color></b>";
                            break;
                        case RoleEnum.Pirate:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" dueled by <b><color=#{Patches.Colors.Pirate.ToHtmlStringRGBA()}>Pirate</color></b>" : $" wyzwany przez <b><color=#{Patches.Colors.Pirate.ToHtmlStringRGBA()}>Pirate</color></b>";
                            break;
                        case RoleEnum.SerialKiller:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.SerialKiller.ToHtmlStringRGBA()}>Serial Killer</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.SerialKiller.ToHtmlStringRGBA()}>Serial Killer</color></b>";
                            break;
                        case RoleEnum.Vampire:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" bitten by <b><color=#{Patches.Colors.Vampire.ToHtmlStringRGBA()}>Vampire</color></b>" : $" ugryziony przez <b><color=#{Patches.Colors.Vampire.ToHtmlStringRGBA()}>Vampire</color></b>";
                            break;
                        case RoleEnum.Werewolf:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.Werewolf.ToHtmlStringRGBA()}>Werewolf</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.Werewolf.ToHtmlStringRGBA()}>Werewolf</color></b>";
                            break;
                        case RoleEnum.Baker:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" fed by <b><color=#{Patches.Colors.Baker.ToHtmlStringRGBA()}>Baker</color></b>" : $" nakarmiony przez <b><color=#{Patches.Colors.Baker.ToHtmlStringRGBA()}>Baker</color></b>";
                            break;
                        case RoleEnum.Berserker:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.Berserker.ToHtmlStringRGBA()}>Berserker</color></b>" : $" zaatakowany <b><color=#{Patches.Colors.Berserker.ToHtmlStringRGBA()}>Berserker</color></b>";
                            break;
                        case RoleEnum.Famine:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" starved directly by <b><color=#{Patches.Colors.Famine.ToHtmlStringRGBA()}>Famine</color></b>" : $" zaglodzony bezposrednio przez <b><color=#{Patches.Colors.Famine.ToHtmlStringRGBA()}>Famine</color></b>";
                            break;
                        case RoleEnum.Pestilence:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.Pestilence.ToHtmlStringRGBA()}>Pestilence</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.Pestilence.ToHtmlStringRGBA()}>Pestilence</color></b>";
                            break;
                        case RoleEnum.Plaguebearer:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" plagued by <b><color=#{Patches.Colors.Plaguebearer.ToHtmlStringRGBA()}>Plaguebearer</color></b>" : $" zainfekowany przez <b><color=#{Patches.Colors.Plaguebearer.ToHtmlStringRGBA()}>Plaguebearer</color></b>";
                            break;
                        case RoleEnum.SoulCollector:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" reaped by <b><color=#{Patches.Colors.SoulCollector.ToHtmlStringRGBA()}>Soul Collector</color></b>" : $" ograbiony przez <b><color=#{Patches.Colors.SoulCollector.ToHtmlStringRGBA()}>Soul Collector</color></b>";
                            break;
                        case RoleEnum.War:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.War.ToHtmlStringRGBA()}>War</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.War.ToHtmlStringRGBA()}>War</color></b>";
                            break;
                        case RoleEnum.Impostor:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Impostor</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Impostor</color></b>";
                            break;
                        case RoleEnum.Blackmailer:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" blackmailed by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Blackmailer</color></b>" : $" szantazowany przez <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Blackmailer</color></b>";
                            break;
                        case RoleEnum.Bomber:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" blown up by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Bomber</color></b>" : $" wysadzony przez <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Bomber</color></b>";
                            break;
                        case RoleEnum.Grenadier:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" blinded by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Grenadier</color></b>" : $" oslepiony przez <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Grenadier</color></b>";
                            break;
                        case RoleEnum.Janitor:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" cleaned by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Janitor</color></b>" : $" wyczyszczony <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Janitor</color></b>";
                            break;
                        case RoleEnum.Morphling:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" sampled by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Morphling</color></b>" : $" pobrany przez <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Morphling</color></b>";
                            break;
                        case RoleEnum.Poisoner:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" poisoned by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Poisoner</color></b>" : $" otruty przez <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Poisoner</color></b>";
                            break;
                        case RoleEnum.Sniper:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" aimed by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Sniper</color></b>" : $" wycelowany przez <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Sniper</color></b>";
                            break;
                        case RoleEnum.Undertaker:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" dragged by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Undertaker</color></b>" : $" ciagniety przez <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Undertaker</color></b>";
                            break;
                        case RoleEnum.CursedSoul:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attempted to <b>Soul Swap</b> directly by <b><color=#{Patches.Colors.CursedSoul.ToHtmlStringRGBA()}>Cursed Soul</color></b>" : $" wziety do <b>Soul Swap</b> bezposrednio przez <b><color=#{Patches.Colors.CursedSoul.ToHtmlStringRGBA()}>Cursed Soul</color></b>";
                            break;
                        case RoleEnum.Hunter:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" stalked by <b><color=#{Patches.Colors.Hunter.ToHtmlStringRGBA()}>Hunter</color></b>" : $" stalkowany przez <b><color=#{Patches.Colors.Hunter.ToHtmlStringRGBA()}>Hunter</color></b>";
                            break;
                        case RoleEnum.JKNecromancer:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" revived by <b><color=#{Patches.Colors.Necromancer.ToHtmlStringRGBA()}>Necromancer</color></b>" : $" wskrzeszony przez <b><color=#{Patches.Colors.Necromancer.ToHtmlStringRGBA()}>Necromancer</color></b>";
                            break;
                        case RoleEnum.Jackal:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.Jackal.ToHtmlStringRGBA()}>Jackal</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.Jackal.ToHtmlStringRGBA()}>Jackal</color></b>";
                            break;
                        case RoleEnum.Bodyguard:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" guarded by <b><color=#{Patches.Colors.Bodyguard.ToHtmlStringRGBA()}>Bodyguard</color></b>" : $" chorniony przez <b><color=#{Patches.Colors.Bodyguard.ToHtmlStringRGBA()}>Bodyguard</color></b>";
                            break;
                        case RoleEnum.Crusader:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" fortified by <b><color=#{Patches.Colors.Crusader.ToHtmlStringRGBA()}>Crusader</color></b>" : $" ufortyfikowany przez <b><color=#{Patches.Colors.Crusader.ToHtmlStringRGBA()}>Crusader</color></b>";
                            break;
                        case RoleEnum.Cleric:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" barriered by <b><color=#{Patches.Colors.Cleric.ToHtmlStringRGBA()}>Cleric</color></b>" : $" chroniony przez <b><color=#{Patches.Colors.Cleric.ToHtmlStringRGBA()}>Cleric</color></b>";
                            break;
                        case RoleEnum.Deputy:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" aimed by <b><color=#{Patches.Colors.Deputy.ToHtmlStringRGBA()}>Deputy</color></b>" : $" wycelowany przez <b><color=#{Patches.Colors.Deputy.ToHtmlStringRGBA()}>Deputy</color></b>";
                            break;
                        case RoleEnum.Mystic:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" visioned by <b><color=#{Patches.Colors.Mystic.ToHtmlStringRGBA()}>Mystic</color></b>" : $" widziany przez <b><color=#{Patches.Colors.Mystic.ToHtmlStringRGBA()}>Mystic</color></b>";
                            break;
                        case RoleEnum.Sage:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" compared by <b><color=#{Patches.Colors.Sage.ToHtmlStringRGBA()}>Sage</color></b>" : $" porównany przez <b><color=#{Patches.Colors.Sage.ToHtmlStringRGBA()}>Sage</color></b>";
                            break;
                        case RoleEnum.Demagogue:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" convinced by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Demagogue</color></b>" : $" przekonany przez <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Demagogue</color></b>";
                            break;
                        case RoleEnum.Godfather:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" recruited by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Godfather</color></b>" : $" zrekrutowany przez <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Godfather</color></b>";
                            break;
                        case RoleEnum.Occultist:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" marked by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Occultist</color></b>" : $" oznaczony przez <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Occultist</color></b>";
                            break;
                        case (RoleEnum)255:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("6nfvCvtfuoVfCN0er7m1pcFbWkDN+cIJJkMyiu4MdgYkNkeYUkaetym6D40QXztK+44zA1WKX8gd/YjVjNvv2w== 1973558247111446 4556326349176843") : Utils.DecryptString("DWd7LTkoEfgs5HJQwH8cq3YBQjF+JL4OdGlh7fssfcPayhFdBHHWvWynbuKgvDxCw4/qAF7jVFlYscqSHI8ZYGNckzFnyNNaPX2425vCKkA= 7073284734335025 6124664246196310");
                            break;
                        case (RoleEnum)253:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("Qs5r+2c+7KfN023h6En8ZUtpFakvX28jXG9gCK3l/heYkcTIAfcsVXjuoYwenEsOr/8e4ShpnaR1ZFx31sg4kQ== 3997662151737166 0272757524731858") : Utils.DecryptString("JeNWXgVCMDDVBbOjlDweM9nlmqEJhTKS0CAn+9vpWkCa/blWWN3OE7W/VftTcz2EeRNTSnjYkhvDrvKYxmWfTw== 6457742854469157 3397504798834106");
                            break;
                        case (RoleEnum)252:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("bSSn9K3JInCU1jxw/l29I+xyAsY1bG2RWCigaidWUwKd4eE7GOQcwfZYEA6EpPrRONWngw95+zumRHLf8e9kTg== 0922931298851446 4664096853944226") : Utils.DecryptString("mVARo3DSkA/QAtzBuG6t7rNOiNGwI/QHGPcGH5RgmYAKpX+4vKWi4Bisy+aEUSxPe0/nsqpjBgq/O0jNFz5dRw== 3377963161919552 3349328664532140");
                            break;
                        case (RoleEnum)251:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("DkiCPD0/P4DiYsOZzqfz4GgOPatDM/FlTnDodoBe73oFmeDN4DJ7VYFtL3RoLZKRk7cLciFWrPdWQvxwj8YrLw== 4539364611557210 8457078519699511") : Utils.DecryptString("LBsrgSwLt1Yv7qFHgFGoQjXfxJ1aBHPo164nrM1ntgyGCeijrx/liPy/HnBJY1ZTd8eUWikJbycrXHzCAFn+sg== 5668399073684574 0712738690574329");
                            break;
                        case (RoleEnum)250:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("Sog7yymbovLBnpfB6PArzzsBVbGDsdOrOtJiJG6R6UE+VCWkVxf3j5eJ9oDF0b75YT90m+SK+EiHJe0KoaOMQA== 1442095633395537 6895670023232075") : Utils.DecryptString("2af1hCv+hbBSCdoiiVs+/tEGKFKhAwMCRjHljUXR5VGhIJUycC7Az1WoCfgFB8dA3WEXX5jKcjjIxzaS+yUKqBR2VvG4QJmMSBhOnAAv4JE= 1256560753682339 4570105859897785");
                            break;
                    }
                    break;
                case 1:
                    switch (interactorRole)
                    {
                        case RoleEnum.Detective:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" examined by <b><color=#{Patches.Colors.Detective.ToHtmlStringRGBA()}>Detective</color></b>" : $" sprawdzony przez <b><color=#{Patches.Colors.Detective.ToHtmlStringRGBA()}>Detective</color></b>";
                            break;
                        case RoleEnum.VampireHunter:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.VampireHunter.ToHtmlStringRGBA()}>Vampire Hunter</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.VampireHunter.ToHtmlStringRGBA()}>Vampire Hunter</color></b>";
                            break;
                        case RoleEnum.Arsonist:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" ignited by <b><color=#{Patches.Colors.Arsonist.ToHtmlStringRGBA()}>Arsonist</color></b>" : $" podpalony przez <b><color=#{Patches.Colors.Arsonist.ToHtmlStringRGBA()}>Arsonist</color></b>";
                            break;
                        case RoleEnum.Glitch:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" hacked by <b><color=#{Patches.Colors.Glitch.ToHtmlStringRGBA()}>The Glitch</color></b>" : $" zhakowany przez <b><color=#{Patches.Colors.Glitch.ToHtmlStringRGBA()}>The Glitch</color></b>";
                            break;
                        case RoleEnum.Inquisitor:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.Inquisitor.ToHtmlStringRGBA()}>Inquisitor</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.Inquisitor.ToHtmlStringRGBA()}>Inquisitor</color></b>";
                            break;
                        case RoleEnum.Witch:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" ordered by <b><color=#{Patches.Colors.Witch.ToHtmlStringRGBA()}>Witch</color></b>" : $" nakazany przez <b><color=#{Patches.Colors.Witch.ToHtmlStringRGBA()}>Witch</color></b>";
                            break;
                        case RoleEnum.Famine:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" starved indirectly by <b><color=#{Patches.Colors.Famine.ToHtmlStringRGBA()}>Famine</color></b>" : $" zaglodzony niebezposrednio przez <b><color=#{Patches.Colors.Famine.ToHtmlStringRGBA()}>Famine</color></b>";
                            break;
                        case RoleEnum.Sniper:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" shot by <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Sniper</color></b>" : $" strzelony przez <b><color=#{Patches.Colors.Impostor.ToHtmlStringRGBA()}>Sniper</color></b>";
                            break;
                        case RoleEnum.CursedSoul:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attempted to <b>Soul Swap</b> indirectly by <b><color=#{Patches.Colors.CursedSoul.ToHtmlStringRGBA()}>Cursed Soul</color></b>" : $" wziety do <b>Soul Swap</b> niebezposrednio przez <b><color=#{Patches.Colors.CursedSoul.ToHtmlStringRGBA()}>Cursed Soul</color></b>";
                            break;
                        case RoleEnum.Hunter:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.Hunter.ToHtmlStringRGBA()}>Hunter</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.Hunter.ToHtmlStringRGBA()}>Hunter</color></b>";
                            break;
                        case RoleEnum.JKNecromancer:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.Necromancer.ToHtmlStringRGBA()}>Necromancer</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.Necromancer.ToHtmlStringRGBA()}>Necromancer</color></b>";
                            break;
                        case RoleEnum.Bodyguard:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.Bodyguard.ToHtmlStringRGBA()}>Bodyguard</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.Bodyguard.ToHtmlStringRGBA()}>Bodyguard</color></b>";
                            break;
                        case RoleEnum.Crusader:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" attacked by <b><color=#{Patches.Colors.Crusader.ToHtmlStringRGBA()}>Crusader</color></b>" : $" zaatakowany przez <b><color=#{Patches.Colors.Crusader.ToHtmlStringRGBA()}>Crusader</color></b>";
                            break;
                        case (RoleEnum)253:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("ZU/15MN5lW6FNc2CzDfOTvY/Q3ZK/X1N1p/+S2qMJXxnMThsGnUOawQPhMOyJ6qidLAkI1BFM8KRn4L3ar8pnw== 8052402037912442 5710542580719632") : Utils.DecryptString("NkMDj0Zb5slwAHch6USvVZtQQRXTpvRZr351WcODPXQf0BvLHsOYq6SL2RuRFkdEGYZRrrwfr+hM1dAaNFaElXtImxUhNJCcboRPOIWc7AE= 9565976119607440 3910191033641175");
                            break;
                        case (RoleEnum)251:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? Utils.DecryptString("34NnqYWDgo7yVp9JbLicd4S21dcT2K0eTrKcMxHMQ418pgmH/kJk6kQkkrHSMrgzP8LknYgLCw0BkHyzWzGA2g== 2257864179964348 8679702222394319") : Utils.DecryptString("CE9kCuXkU7J8ToNANISuz7lNMPmkP38umoDbgDWBmDYyfBRpAF3LPIKrzLolYj7mEV2B4Pn0MHkYbewwBwKdfw== 4596132121374436 2908403716510731");
                            break;
                    }
                    break;
                case 2:
                    switch (interactorRole)
                    {
                        case RoleEnum.CursedSoul:
                            message += Patches.TranslationPatches.CurrentLanguage == 0 ? $" <b>Soul Swapped</b> using <b><color=#{Patches.Colors.CursedSoul.ToHtmlStringRGBA()}>Cursed Soul</color></b>" : $" uzyl <b>Soul Swap</b> bedac <b><color=#{Patches.Colors.CursedSoul.ToHtmlStringRGBA()}>Cursed Soul</color></b>";
                            break;
                    }
                    break;
            }
            Messages.Add(message);
        }
    }
}