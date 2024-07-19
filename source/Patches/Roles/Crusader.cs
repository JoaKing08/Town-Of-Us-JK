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
    public class Crusader : Role
    {
        public DateTime LastFortified;
        public PlayerControl ClosestPlayer;
        public List<byte> FortifiedPlayers;

        public int UsesLeft;
        public TextMeshPro UsesText;

        public bool ButtonUsable => UsesLeft != 0;

        public Crusader(PlayerControl player) : base(player)
        {
            Name = "Crusader";
            ImpostorText = () => "Protect Crew By Killing!";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Protect crewmates to kill interactors" : "Bron crewmate'ów by zabic interaktorów";
            Color = Patches.Colors.Crusader;
            RoleType = RoleEnum.Crusader;
            LastFortified = DateTime.UtcNow;
            FortifiedPlayers = new List<byte>();
            UsesLeft = CustomGameOptions.MaxFortify;
            AddToRoleHistory(RoleType);
        }

        public float FortifyTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastFortified;
            var num = CustomGameOptions.FortifyCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}
