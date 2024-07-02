using Hazel;
using System;
using System.Collections.Generic;
using System.Linq;
using TownOfUs.Extensions;

namespace TownOfUs.Roles.Horseman
{
    public class War : Role
    {
        public War(PlayerControl owner) : base(owner)
        {
            Name = "War";
            Color = Patches.Colors.War;
            LastKill = DateTime.UtcNow;
            RoleType = RoleEnum.War;
            AddToRoleHistory(RoleType);
            ImpostorText = () => "";
            TaskText = () => "Kill everyone with your unstoppable attacks!\nFake Tasks:";
            Faction = Faction.NeutralApocalypse;
        }
        public bool Announced;

        public PlayerControl ClosestPlayer;
        public DateTime LastKill { get; set; }
        public DateTime StartUseTime;
        public bool UsingCharge;

        public float KillTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastKill;
            var num = CustomGameOptions.WarCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float UseTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - StartUseTime;
            var num = CustomGameOptions.WarRampage * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}