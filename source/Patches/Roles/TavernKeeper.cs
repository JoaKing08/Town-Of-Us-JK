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
    public class TavernKeeper : Role
    {
        public DateTime LastDrink;
        public PlayerControl ClosestPlayer;
        public List<PlayerControl> DrunkPlayers = new List<PlayerControl>();
        public int UsesLeft => CustomGameOptions.DrinksPerRound - DrunkPlayers.Count;
        public TextMeshPro UsesText;
        public bool CanDrink => UsesLeft != 0;

        public TavernKeeper(PlayerControl player) : base(player)
        {
            Name = "Tavern Keeper";
            ImpostorText = () => "Block Abilities Of Suspicious Players!";
            TaskText = () => "Block abilities of players you don't trust";
            Color = Patches.Colors.TavernKeeper;
            RoleType = RoleEnum.TavernKeeper;
            LastDrink = DateTime.UtcNow;
            AddToRoleHistory(RoleType);
        }

        public float DrinkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastDrink;
            var num = CustomGameOptions.DrinkCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
