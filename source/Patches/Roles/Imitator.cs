using System.Collections.Generic;
using UnityEngine;

namespace TownOfUs.Roles
{
    public class Imitator : Role
    {
        public readonly List<GameObject> Buttons = new List<GameObject>();

        public readonly List<bool> ListOfActives = new List<bool>();
        public PlayerControl ImitatePlayer = null;

        public List<RoleEnum> trappedPlayers = null;
        public List<string> Messages = null;
        public PlayerControl LastInspectedPlayer = null;
        public PlayerControl confessingPlayer = null;
        public byte SageFirst = byte.MaxValue;
        public byte SageSecond = byte.MaxValue;
        public byte VisionPlayer = byte.MaxValue;
        public List<byte> PlayersInteracted = null;
        public List<byte> InteractingPlayers = null;


        public Imitator(PlayerControl player) : base(player)
        {
            Name = "Imitator";
            ImpostorText = () => "Use The True-Hearted Dead To Benefit The Crew";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Use dead roles to benefit the crew" : "Uzyj ról martwych by wspomóc zaloge";
            Color = Patches.Colors.Imitator;
            RoleType = RoleEnum.Imitator;
            AddToRoleHistory(RoleType);
        }
    }
}