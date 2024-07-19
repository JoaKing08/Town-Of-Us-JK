using System;
using System.Linq;
using Il2CppSystem.Collections.Generic;
using TownOfUs.Extensions;

namespace TownOfUs.Roles
{
    public class Jackal : Role
    {
        public Jackal(PlayerControl player) : base(player)
        {
            Name = "Jackal";
            ImpostorText = () => "Kill Everyone With Your Recruits";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? (RecruitsAlive ? "Survive and wait until recruits make a kingdom of yours\nFake Tasks:" : "Your recruits died, now take down the crew yourself\nFake Tasks:") : (RecruitsAlive ? "Przetrwaj i czekaj az rekruci zrobia twoj raj\nFake Tasks:" : "Twoi rekruci umarli, teraz zdejmij zaloge samodzielnie\nFake Tasks:");
            Color = Patches.Colors.Jackal;
            LastKill = DateTime.UtcNow;
            RoleType = RoleEnum.Jackal;
            Faction = Faction.NeutralKilling;
            FactionOverride = FactionOverride.Recruit;
            RecruitsAlive = true;
            AddToRoleHistory(RoleType);
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        public bool RecruitsAlive { get; set; }
        public bool JackalWin { get; set; }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = CustomGameOptions.JackalKCd * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            var Recruits = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && x.Is(FactionOverride.Recruit));
            if (Recruits == 0) return true;
            var AlivePlayers = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(FactionOverride.Recruit));
            var KillingAlives = PlayerControl.AllPlayerControls.ToArray().Count(x => !x.Data.IsDead && !x.Data.Disconnected && !x.Is(FactionOverride.Recruit) && ((x.Data.IsImpostor() || x.Is(Faction.NeutralApocalypse) || x.Is(Faction.NeutralKilling)) || ((x.Is(RoleEnum.Sheriff) || x.Is(RoleEnum.Vigilante) || x.Is(RoleEnum.Veteran) || x.Is(RoleEnum.VampireHunter) || x.Is(RoleEnum.Hunter)) && CustomGameOptions.OvertakeWin == OvertakeWin.WithoutCK)));
            var ga = new Dictionary<byte, bool>();
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                var i = false;
                if (player.Is(RoleEnum.GuardianAngel)) i = GetRole<GuardianAngel>(player).target.Is(FactionOverride.Recruit);
                ga.Add(player.PlayerId, i);
            }
            var onlyNonstopping = !PlayerControl.AllPlayerControls.ToArray().Any(x => !x.Data.IsDead && !x.Data.Disconnected && !(x.Is(RoleEnum.GuardianAngel) && ga[x.PlayerId]) && !x.Is(RoleEnum.Survivor) && !x.Is(RoleEnum.Witch) && !x.Is(FactionOverride.Recruit));

            if ((Recruits >= AlivePlayers && KillingAlives == 0 && CustomGameOptions.OvertakeWin != OvertakeWin.Off) || (Recruits > 0 && AlivePlayers == 0) || onlyNonstopping)
            {
                Utils.Rpc(CustomRPC.JackalWin, Player.PlayerId);
                Wins();
                Utils.EndGame();
                return false;
            }
            return false;
        }

        public void Wins()
        {
            JackalWin = true;
        }
    }
}