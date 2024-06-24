using UnityEngine;
using System;
using TownOfUs.ImpostorRoles.BomberMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches;
using System.Collections.Generic;

namespace TownOfUs.Roles
{
    public class Occultist : Role

    {
        public KillButton _markButton;
        public DateTime LastMark { get; set; }
        public PlayerControl ClosestPlayer;
        public List<byte> MarkedPlayers;

        public Occultist(PlayerControl player) : base(player)
        {
            Name = "Occultist";
            ImpostorText = () => "Sacrifice The Crew To The Greater Cause";
            TaskText = () => "Mark everyone who oposes you, to perform the ritual.";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Occultist;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            LastMark = DateTime.UtcNow;
            MarkedPlayers = new();
        }
        public KillButton MarkButton
        {
            get => _markButton;
            set
            {
                _markButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float MarkTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastMark;
            var num = (CustomGameOptions.MarkCooldown + (CustomGameOptions.MarkCooldownIncrease * MarkedPlayers.Count)) * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}