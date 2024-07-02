    using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.Patches;

namespace TownOfUs.Roles
{
    public class Arsonist : Role
    {
        private KillButton _igniteButton;
        public bool ArsonistWins;
        public PlayerControl ClosestPlayerDouse;
        public PlayerControl ClosestPlayerIgnite;
        public List<byte> DousedPlayers = new List<byte>();
        public DateTime LastDoused;
        public bool LastKiller = false;

        public int DousedAlive => DousedPlayers.Count(x => Utils.PlayerById(x) != null && Utils.PlayerById(x).Data != null && !Utils.PlayerById(x).Data.IsDead && !Utils.PlayerById(x).Data.Disconnected);


        public Arsonist(PlayerControl player) : base(player)
        {
            Name = "Arsonist";
            ImpostorText = () => "Douse Players And Ignite The Light";
            TaskText = () => "Douse players and ignite to kill all douses\nFake Tasks:";
            Color = Patches.Colors.Arsonist;
            LastDoused = DateTime.UtcNow;
            RoleType = RoleEnum.Arsonist;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralKilling;
        }

        public KillButton IgniteButton
        {
            get => _igniteButton;
            set
            {
                _igniteButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;
            if (FactionOverride != FactionOverride.None || Player.Is(ObjectiveEnum.ImpostorAgent) || Player.Is(ObjectiveEnum.ApocalypseAgent)) return false;
            var AlivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.PlayerId != Player.PlayerId);
            var KillingAlives = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.PlayerId != Player.PlayerId && ((x.Data.IsImpostor() || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.NeutralKilling)) || ((x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Hunter)) && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK)));
            var ga = new Dictionary<byte, bool>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var i = false;
                if (player.Is(RoleEnum.GuardianAngel)) i = GetRole<GuardianAngel>(player).target.PlayerId == Player.PlayerId;
                ga.Add(player.PlayerId, i);
            }
            var onlyNonstopping = !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && !(x.Is(RoleEnum.GuardianAngel) && ga[x.PlayerId]) && !x.Is(RoleEnum.Survivor) && !x.Is(RoleEnum.Witch) && x.PlayerId != Player.PlayerId);

            if ((1 >= AlivePlayers && KillingAlives == 0 && CustomGameOptions.OvertakeWin != OvertakeWin.Off) || (1 > 0 && AlivePlayers == 0) || onlyNonstopping)
            {
                Utils.Rpc(CustomRPC.ArsonistWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return false;
            }

            return false;
        }


        public void Wins()
        {
            ArsonistWins = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var arsonistTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            arsonistTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = arsonistTeam;
        }

        public float DouseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDoused;
            var num = CustomGameOptions.DouseCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public void Ignite()
        {
            foreach (var playerId in DousedPlayers)
            {
                var player = Utils.PlayerById(playerId);
                if (player.IsBugged()) Utils.Rpc(CustomRPC.BugMessage, playerId, (byte)RoleEnum.Arsonist, (byte)1);
                if (!player.Is(RoleEnum.Pestilence) && !player.Is(RoleEnum.Famine) && !player.Is(RoleEnum.War) && !player.Is(RoleEnum.Death) && !player.IsShielded() && !player.IsProtected() && player != ShowRoundOneShield.FirstRoundShielded)
                {
                    Utils.RpcMultiMurderPlayer(Player, player);
                    Utils.Rpc(CustomRPC.KillAbilityUsed, playerId);
                }
                else if (player.IsShielded())
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    Utils.Rpc(CustomRPC.AttemptSound, medic, player.PlayerId);
                    StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
            }
            DousedPlayers.Clear();
        }
    }
}
