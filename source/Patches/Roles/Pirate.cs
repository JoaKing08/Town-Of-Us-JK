using UnityEngine;
using System;

namespace TownOfUs.Roles
{
    public class Pirate : Role
    {
        public Pirate(PlayerControl player) : base(player)
        {
            Name = "Pirate";
            ImpostorText = () => "Board And Loot The Treasures Of Crew";
            TaskText = () => "Prowl for plunder amongst those who hold riches";
            Color = Patches.Colors.Pirate;
            RoleType = RoleEnum.Pirate;
            AddToRoleHistory(RoleType);
            Faction = Faction.NeutralChaos;
        }
        public PlayerControl ClosestPlayer;
        public PlayerControl DueledPlayer = null;
        public int DuelsWon = 0;
        public bool WonByDuel = false;
        public DateTime LastDueled;

        protected override void IntroPrefix(IntroCutscene._ShowTeam_d__38 __instance)
        {
            var pirateTeam = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            pirateTeam.Add(PlayerControl.LocalPlayer);
            __instance.teamToShow = pirateTeam;
        }

        internal override bool NeutralWin(LogicGameFlowNormal __instance)
        {
            if (Player.Data.IsDead) return true;
            if (!CustomGameOptions.NeutralEvilWinEndsGame) return true;
            if (!WonByDuel) return true;
            Utils.EndGame();
            return false;
        }

        public float DuelTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDueled;
            var num = CustomGameOptions.DuelCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}