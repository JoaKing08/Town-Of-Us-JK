using Il2CppSystem.Collections.Generic;
using Reactor.Utilities.Extensions;
using System;
using System.Linq;

namespace TownOfUs.Roles
{
    public class Inquisitor : Role
    {
        public List<byte> heretics;
        public bool HereticsDead;
        public KillButton _vanquishButton;
        public bool CanVanquish = true;

        public Inquisitor(PlayerControl player) : base(player)
        {
            Name = "Inquisitor";
            ImpostorText = () => "Hunt Down The Heretics";
            TaskText = () => CustomGameOptions.HereticsInfo == HereticsInfo.Nothing ? $"Hunt down the heretics.\nFake Tasks:" : $"Hunt down the heretics.\nHeretics Left: {GetHereticList()}\nFake Tasks:";
            Color = Patches.Colors.Inquisitor;
            RoleType = RoleEnum.Inquisitor;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralChaos;
            LastAbility = DateTime.UtcNow;
            heretics = new List<byte>();
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__36 __instance)
        {
            var inqTeam = new List<PlayerControl>();
            inqTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = inqTeam;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead) return true;
            if (!CustomGameOptions.NeutralEvilWinEndsGame) return true;
            if (!HereticsDead || heretics.ToArray().Count(x => !Utils.PlayerById(x).Data.IsDead) != 0) return true;
            Utils.EndGame();
            return false;
        }

        public void Wins()
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return;
            HereticsDead = true;
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastAbility { get; set; }

        public float AbilityTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastAbility;
            var num = CustomGameOptions.InquisitorCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public KillButton VanquishButton
        {
            get => _vanquishButton;
            set
            {
                _vanquishButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public string GetHereticList()
        {
            var hereticInfo = "";
            var _heretics = heretics.ToArray().Where(x => !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected).ToList();
            if (CustomGameOptions.HereticsInfo == HereticsInfo.Count)
            {
                hereticInfo = _heretics.Count.ToString();
            }
            else if (CustomGameOptions.HereticsInfo == HereticsInfo.Faction)
            {
                for (int i = 0; i < _heretics.Count; i++)
                {

                    var faction = "Null";
                    if (Utils.PlayerById(_heretics[i]).Is(Faction.Crewmates))
                        faction = "<color=#CFFFFFFF>Crewmate</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(Faction.NeutralBenign) || Utils.PlayerById(_heretics[i]).Is(Faction.NeutralEvil) || Utils.PlayerById(_heretics[i]).Is(Faction.NeutralKilling) || Utils.PlayerById(_heretics[i]).Is(Faction.NeutralChaos))
                        faction = "<color=#808080FF>Neutral</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(Faction.NeutralApocalypse))
                        faction = "<color=#404040FF>Apocalypse</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(Faction.Impostors))
                        faction = "<color=#FF0000FF>Impostor</color>";
                    hereticInfo += faction;
                    if (i == _heretics.Count - 2) hereticInfo += " and ";
                    else if (i < _heretics.Count - 2) hereticInfo += ", ";
                }
            }
            else if (CustomGameOptions.HereticsInfo == HereticsInfo.Aligment)
            {
                for (int i = 0; i < _heretics.Count; i++)
                {
                    
                    var aligment = "Null";
                    if (Utils.PlayerById(_heretics[i]).Is(RoleEnum.Aurial) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Detective) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Investigator) ||
                        Utils.PlayerById(_heretics[i]).Is(RoleEnum.Mystic) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Seer) ||
                        Utils.PlayerById(_heretics[i]).Is(RoleEnum.Snitch) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Spy) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Tracker) ||
                        Utils.PlayerById(_heretics[i]).Is(RoleEnum.Trapper) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Inspector))
                        aligment = "<color=#CFFFFFFF>Crew Investigative</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(RoleEnum.Altruist) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Medic))
                        aligment = "<color=#CFFFFFFF>Crew Protective</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(RoleEnum.Sheriff) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.VampireHunter) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Veteran) ||
                        Utils.PlayerById(_heretics[i]).Is(RoleEnum.Vigilante))
                        aligment = "<color=#CFFFFFFF>Crew Killing</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(RoleEnum.Engineer) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Imitator) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.TavernKeeper) ||
                        Utils.PlayerById(_heretics[i]).Is(RoleEnum.Medium) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Transporter) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Undercover))
                        aligment = "<color=#CFFFFFFF>Crew Support</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(RoleEnum.Mayor) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Oracle) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Prosecutor) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Swapper) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Monarch))
                        aligment = "<color=#CFFFFFFF>Crew Power</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(RoleEnum.Crewmate))
                        aligment = "<color=#CFFFFFFF>Crew</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(Faction.NeutralBenign))
                        aligment = "<color=#808080FF>Neutral Benign</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(Faction.NeutralEvil))
                        aligment = "<color=#808080FF>Neutral Evil</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(Faction.NeutralChaos))
                        aligment = "<color=#808080FF>Neutral Chaos</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(Faction.NeutralKilling))
                        aligment = "<color=#808080FF>Neutral Killing</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(Faction.NeutralApocalypse))
                        aligment = "<color=#808080FF>Neutral Apocalypse</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(RoleEnum.Escapist) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Morphling) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Swooper) ||
                        Utils.PlayerById(_heretics[i]).Is(RoleEnum.Grenadier) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Venerer))
                        aligment = "<color=#FF0000FF>Impostor Concealing</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(RoleEnum.Bomber) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Traitor) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Warlock) ||
                        Utils.PlayerById(_heretics[i]).Is(RoleEnum.Poisoner) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Sniper))
                        aligment = "<color=#FF0000FF>Impostor Killing</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(RoleEnum.Blackmailer) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Janitor) || Utils.PlayerById(_heretics[i]).Is(RoleEnum.Miner) ||
                        Utils.PlayerById(_heretics[i]).Is(RoleEnum.Undertaker))
                        aligment = "<color=#FF0000FF>Impostor Support</color>";
                    else if (Utils.PlayerById(_heretics[i]).Is(RoleEnum.Impostor))
                        aligment = "<color=#FF0000FF>Impostor</color>";
                    hereticInfo += aligment;
                    if (i == _heretics.Count - 2) hereticInfo += " and ";
                    else if (i < _heretics.Count - 2) hereticInfo += ", ";
                }
            }
            else if (CustomGameOptions.HereticsInfo == HereticsInfo.Role)
            {
                for (int i = 0; i < _heretics.Count; i++)
                {
                    var role = $"<color=#{GetRole(Utils.PlayerById(_heretics[i])).Color.ToHtmlStringRGBA()}>{GetRole(Utils.PlayerById(_heretics[i])).Name}</color>";
                    hereticInfo += role;
                    if (i == _heretics.Count - 2) hereticInfo += " and ";
                    else if (i < _heretics.Count - 2) hereticInfo += ", ";
                }
            }
            if (hereticInfo == "") hereticInfo = "Error";
            return hereticInfo;
        }
    }
}