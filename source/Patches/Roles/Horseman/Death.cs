using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles.Horseman
{
    public class Death : Role
    {
        public Death(PlayerControl owner) : base(owner)
        {
            Name = "Death";
            Color = Patches.Colors.Death;
            RoleType = RoleEnum.Death;
            LastApocalypse = DateTime.UtcNow;
            AddToRoleHistory(RoleType);
            ImpostorText = () => "";
            TaskText = () => "Cast a apocalipse upon crewmates!\nFake Tasks:";
            Faction = Faction.NeutralApocalypse;
        }
        public bool Announced;
        public DateTime LastApocalypse { get; set; }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;
            var Apocalypse = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.NeutralApocalypse));
            var AlivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.NeutralApocalypse));
            var KillingAlives = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !(x.Is(FactionOverride.None) && (x.Is(Faction.NeutralApocalypse) || x.Is(ObjectiveEnum.ApocalypseAgent))) && ((x.Data.IsImpostor() || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.NeutralKilling)) || ((x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Hunter)) && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK)));
            var ga = new Dictionary<byte, bool>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var i = false;
                if (player.Is(RoleEnum.GuardianAngel)) i = GetRole<GuardianAngel>(player).target.Is(Faction.NeutralApocalypse) || GetRole<GuardianAngel>(player).target.Is(ObjectiveEnum.ApocalypseAgent);
                ga.Add(player.PlayerId, i);
            }
            var onlyNonstopping = !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(ObjectiveEnum.ApocalypseAgent) && !(x.Is(RoleEnum.GuardianAngel) && ga[x.PlayerId]) && !x.Is(RoleEnum.Survivor) && !x.Is(RoleEnum.Witch) && !x.Is(FactionOverride.Undead));

            if ((Apocalypse >= AlivePlayers && KillingAlives == 0 && CustomGameOptions.OvertakeWin != OvertakeWin.Off) || (Apocalypse > 0 && AlivePlayers == 0) || onlyNonstopping)
            {
                Utils.Rpc(CustomRPC.ApocalypseWin7, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return false;
            }

            return false;
        }

        public void Wins()
        {
            ApocalypseWins = true;
        }

        public float ApocalypseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastApocalypse;
            var num = CustomGameOptions.DeathCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}