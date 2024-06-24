using UnityEngine;
using System;
using TownOfUs.ImpostorRoles.BomberMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches;
using System.Collections.Generic;
using TMPro;

namespace TownOfUs.Roles
{
    public class Demagogue : Role

    {
        public int Charges { get; set; }
        public int ExtraVotes { get; set; }
        public List<byte> Convinced { get; set; }
        public KillButton _convinceButton;
        public TextMeshPro CostText;
        public DateTime LastConvince { get; set; }
        public PlayerControl ClosestPlayer;
        public PlayerVoteArea ExtraVote { get; set; }
        public Dictionary<byte, (GameObject button, TextMeshPro text)> MeetingKillButtons = new();
        public Demagogue(PlayerControl player) : base(player)
        {
            Name = "Demagogue";
            ImpostorText = () => "Rule The Meetings From The Shadows";
            TaskText = () => $"Use crew mistakes for your own advantage.\nCharges left: {Charges}";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Demagogue;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
            Charges = CustomGameOptions.StartingCharges;
            ExtraVotes = 0;
            Convinced = new();
        }
        public KillButton ConvinceButton
        {
            get => _convinceButton;
            set
            {
                _convinceButton = value;
                ExtraButtons.Clear();
                ExtraButtons.Add(value);
            }
        }
        public float ConvinceTimer()
        {
            var utcNow = DateTime.UtcNow;
            var timeSpan = utcNow - LastConvince;
            var num = CustomGameOptions.ConvinceCooldown * 1000f;
            var flag2 = num - (float)timeSpan.TotalMilliseconds < 0f;
            if (flag2) return 0;
            return (num - (float)timeSpan.TotalMilliseconds) / 1000f;
        }
    }
}