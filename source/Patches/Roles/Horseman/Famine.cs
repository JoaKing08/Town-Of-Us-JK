using Hazel;
using System;
using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles.Horseman
{
    public class Famine : Role
    {
        public Famine(PlayerControl owner) : base(owner)
        {
            Name = "Famine";
            Color = Patches.Colors.Famine;
            LastStarved = DateTime.UtcNow;
            RoleType = RoleEnum.Famine;
            AddToRoleHistory(RoleType);
            ImpostorText = () => "";
            TaskText = () => "Starve everyone to death!\nFake Tasks:";
            Faction = Faction.NeutralApocalypse;
        }
        public bool Announced;

        public PlayerControl ClosestPlayer;
        public DateTime LastStarved { get; set; }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;
            var CKExists = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && (x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) && !x.Is(ModifierEnum.ApocalypseAgent))) > 0;
            var AliveCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(Faction.NeutralApocalypse) && !x.Is(ModifierEnum.ApocalypseAgent) && !x.Is(RoleEnum.Witch) && !(x.Is(RoleEnum.Undercover) && Utils.UndercoverIsApocalypse() && CustomGameOptions.OvertakeWin == OvertakeWin.Off)).ToList();
            var AliveApocs = PlayerControl.AllPlayerControls.ToArray().Where(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(Faction.NeutralApocalypse)).ToList();
            if (AliveCrew.Count <= (((!CKExists && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK) || CustomGameOptions.OvertakeWin == OvertakeWin.On) ? AliveApocs.Count : 0) &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Data.IsImpostor() || x.Is(Faction.NeutralKilling))) == 0 &&
                    PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected &&
                    (x.Is(RoleEnum.Plaguebearer) || x.Is(RoleEnum.Pestilence))) == 0)
            {
                Utils.Rpc(CustomRPC.ApocalypseWin3, Player.PlayerId);
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

        public float StarveTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastStarved;
            var num = CustomGameOptions.FamineCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}