using System;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using TMPro;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Necromancer : Role
    {
        public Necromancer(PlayerControl player) : base(player)
        {
            Name = "Necromancer";
            ImpostorText = () => "Revive The Dead To Do Your Dirty Work";
            TaskText = () => "Revive Crewmates to turn them into Undead\nFake Tasks:";
            Color = Patches.Colors.Necromancer;
            LastRevived = DateTime.UtcNow;
            RoleType = RoleEnum.JKNecromancer;
            Faction = Faction.NeutralKilling;
            FactionOverride = FactionOverride.Undead;
            ReviveCount = 0;
            AddToRoleHistory(RoleType);
        }
        public DeadBody CurrentTarget;
        public DateTime LastRevived;
        public int ReviveCount;
        public int UsesLeft => CustomGameOptions.MaxNumberOfUndead - ReviveCount;
        public bool NecromancerWin { get; set; }
        public TextMeshPro UsesText;

        public float ReviveTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastRevived;
            var num = (CustomGameOptions.NecromancerReviveCooldown + (CustomGameOptions.ReviveCooldownIncrease * ReviveCount)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            var Undead = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(FactionOverride.Undead));
            var AlivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(FactionOverride.Undead));
            var KillingAlives = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(FactionOverride.Undead) && ((x.Data.IsImpostor() || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.NeutralKilling)) || ((x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Hunter)) && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK)));

            if ((Undead >= AlivePlayers && !(KillingAlives > 0 || CustomGameOptions.OvertakeWin == OvertakeWin.Off)) || (Undead > 0 && AlivePlayers == 0))
            {
                Utils.Rpc(CustomRPC.NecromancerWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
            }
            return false;
        }

        public void Wins()
        {
            NecromancerWin = true;
        }
    }
}