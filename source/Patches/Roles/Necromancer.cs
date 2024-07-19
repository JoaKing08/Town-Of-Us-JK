using System;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using Reactor.Utilities.Extensions;
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
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Revive Crewmates to turn them into Undead\nFake Tasks:" : "Wskrzes crewmate'ów by zmienic ich w nieumarlych\nFake Tasks:";
            Color = Patches.Colors.Necromancer;
            LastRevived = DateTime.UtcNow;
            LastKill = DateTime.UtcNow;
            RoleType = RoleEnum.JKNecromancer;
            Faction = Faction.NeutralKilling;
            FactionOverride = FactionOverride.Undead;
            ReviveCount = 0;
            AddToRoleHistory(RoleType);
        }
        private KillButton _killButton;
        public DeadBody CurrentTarget;
        public DateTime LastRevived;
        public DateTime LastKill;
        public int ReviveCount;
        public PlayerControl ClosestPlayer;
        public int NecroKills = 0;
        public int UsesLeft => CustomGameOptions.MaxNumberOfUndead - ReviveCount;
        public bool NecromancerWin { get; set; }
        public TextMeshPro UsesText;
        public bool LastKiller => !PlayerControl.AllPlayerControls.ToArray().Any(x => (x.Is(Faction.NeutralApocalypse) || x.Data.IsImpostor() || (x.Is(Faction.NeutralKilling) && !x.Is(RoleEnum.JKNecromancer))) && !x.Data.IsDead && !x.Data.Disconnected);

        public float ReviveTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastRevived;
            var num = (CustomGameOptions.NecromancerReviveCooldown + (CustomGameOptions.ReviveCooldownIncrease * ReviveCount)) * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = (CustomGameOptions.RitualKillCooldown + (CustomGameOptions.RitualKillCooldownIncrease * NecroKills)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead || Player.Data.Disconnected) return true;
            var Undead = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(FactionOverride.Undead));
            var AlivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(FactionOverride.Undead));
            var KillingAlives = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(FactionOverride.Undead) && ((x.Data.IsImpostor() || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.NeutralKilling)) || ((x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Hunter)) && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK)));
            var ga = new Dictionary<byte, bool>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var i = false;
                if (player.Is(RoleEnum.GuardianAngel)) i = GetRole<GuardianAngel>(player).target.Is(FactionOverride.Undead);
                ga.Add(player.PlayerId, i);
            }
            var onlyNonstopping = !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && !(x.Is(RoleEnum.GuardianAngel) && ga[x.PlayerId]) && !x.Is(RoleEnum.Survivor) && !x.Is(RoleEnum.Witch) && !x.Is(FactionOverride.Undead));

            if ((Undead >= AlivePlayers && KillingAlives == 0 && CustomGameOptions.OvertakeWin != OvertakeWin.Off) || (Undead > 0 && AlivePlayers == 0) || onlyNonstopping)
            {
                Utils.Rpc(CustomRPC.NecromancerWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return false;
            }
            return false;
        }

        public void Wins()
        {
            NecromancerWin = true;
        }

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var necroTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            necroTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = necroTeam;
        }

        public KillButton KillButton
        {
            get => _killButton;
            set
            {
                _killButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
    }
}