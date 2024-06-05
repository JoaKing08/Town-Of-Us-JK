using System;
using System.Collections.Generic;
using TMPro;

namespace TownOfUs.Roles
{
    public class Monarch : Role
    {
        public List<byte> Knights = new List<byte>();
        public List<byte> toKnight = new List<byte>();
        public bool CanKnight => Knights.Count < CustomGameOptions.MaxKnights && !(FirstRound && !CustomGameOptions.KnightFirstRound);
        public int UsesLeft => CustomGameOptions.MaxKnights - Knights.Count - toKnight.Count;
        public TextMeshPro UsesText;
        public bool FirstRound { get; set; }

        public Monarch(PlayerControl player) : base(player)
        {
            Name = "Monarch";
            ImpostorText = () => "Knight The Crew To Increse Voting Power";
            TaskText = () => "Knight the confirmed crew to increse their voting power";
            Color = Patches.Colors.Monarch;
            LastKnighted = DateTime.UtcNow;
            RoleType = RoleEnum.Monarch;
            Knights = new List<byte>();
            FirstRound = true;
            AddToRoleHistory(RoleType);
        }

        public PlayerControl ClosestPlayer;
        public DateTime LastKnighted { get; set; }

        public float KnightTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKnighted;
            var num = CustomGameOptions.KnightCooldown * 1000f;
            var flag2 = num - (float) timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float) timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}