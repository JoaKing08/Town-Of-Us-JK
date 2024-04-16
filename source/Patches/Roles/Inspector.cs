using System.Collections.Generic;
using System.Linq;
using TMPro;
using TownOfUs.Patches;
using UnityEngine;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.GuardianAngelMod;
using System;
using TownOfUs.CrewmateRoles.VampireHunterMod;

namespace TownOfUs.Roles
{
    public class Inspector : Role
    {
        public DateTime LastInspected;
        public PlayerControl ClosestPlayer;
        public PlayerControl LastInspectedPlayer;

        public Inspector(PlayerControl player) : base(player)
        {
            Name = "Inspector";
            ImpostorText = () => "Check The Roles Of The Crew!";
            TaskText = () => "Check the roles of the crew to find fakes";
            Color = Patches.Colors.Inspector;
            RoleType = RoleEnum.Inspector;
            LastInspected = DateTime.UtcNow;
            AddToRoleHistory(RoleType);
        }

        public float InspectTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastInspected;
            var num = CustomGameOptions.InspectCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }

        public float BloodTimer(DateTime LastBlood)
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastBlood;
            var num = CustomGameOptions.BloodDuration * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
