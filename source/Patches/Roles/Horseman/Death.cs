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
            TaskText = () => "Cast an apocalipse upon crewmates!\nFake Tasks:";
            Faction = Faction.NeutralApocalypse;
        }
        public bool Announced;
        public DateTime LastApocalypse { get; set; }

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