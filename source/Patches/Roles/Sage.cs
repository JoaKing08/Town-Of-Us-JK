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
    public class Sage : Role
    {
        public DateTime LastCompared;
        public PlayerControl ClosestPlayer;
        public byte FirstPlayer;
        public byte SecondPlayer;

        public bool ButtonUsable => FirstPlayer == byte.MaxValue || Utils.PlayerById(FirstPlayer).Data.IsDead || Utils.PlayerById(FirstPlayer).Data.Disconnected || SecondPlayer == byte.MaxValue || Utils.PlayerById(SecondPlayer).Data.IsDead || Utils.PlayerById(SecondPlayer).Data.Disconnected;

        public Sage(PlayerControl player) : base(player)
        {
            Name = "Sage";
            ImpostorText = () => "Learn Which Players Are Enemies";
            TaskText = () => "Compare win conditions of players to know evildoers.";
            Color = Patches.Colors.Sage;
            RoleType = RoleEnum.Sage;
            FirstPlayer = byte.MaxValue;
            SecondPlayer = byte.MaxValue;
            LastCompared = DateTime.UtcNow;
            AddToRoleHistory(RoleType);
        }

        public float CompareTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastCompared;
            var num = CustomGameOptions.CompareCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
