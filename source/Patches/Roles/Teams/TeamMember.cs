using System;
using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles.Teams
{
    public class TeamMember : Role
    {
        public TeamMember(PlayerControl player) : base(player) { }
        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        public bool TeamWins { get; set; }

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = ((CustomGameOptions.GameMode == GameMode.Teams ? CustomGameOptions.TeamsKCd : CustomGameOptions.SoloKillerKCd) == 0 ? (CustomGameOptions.GameMode == GameMode.Teams ? CustomGameOptions.TeamsKCd : CustomGameOptions.SoloKillerKCd) + 0.001f : (CustomGameOptions.GameMode == GameMode.Teams ? CustomGameOptions.TeamsKCd : CustomGameOptions.SoloKillerKCd)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}