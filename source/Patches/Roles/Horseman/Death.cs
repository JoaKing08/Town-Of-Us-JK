using Hazel;
using System;
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
            var CKExists = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) && !x.Is(ObjectiveEnum.ApocalypseAgent))) > 0;
            var AliveCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.NeutralApocalypse) && !x.Is(ObjectiveEnum.ApocalypseAgent) && !x.Is(RoleEnum.Witch) && !(x.Is(RoleEnum.Undercover) && Utils.UndercoverIsApocalypse() && CustomGameOptions.OvertakeWin == OvertakeWin.Off)).ToList();
            var AliveApocs = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.NeutralApocalypse)).ToList();
            if (AliveCrew.Count <= (((!CKExists && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK) || CustomGameOptions.OvertakeWin == OvertakeWin.On) ? AliveApocs.Count : 0) &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling)) && !x.Is(ObjectiveEnum.ApocalypseAgent)) == 0 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && 
                    (x.Is(RoleEnum.Plaguebearer) || x.Is(RoleEnum.Pestilence) || x.Is(RoleEnum.Baker) || x.Is(RoleEnum.Famine) || 
                    x.Is(RoleEnum.Berserker) || x.Is(RoleEnum.War))) == 0 && !AliveApocs.Any(x => !x.Is(FactionOverride.None)))
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