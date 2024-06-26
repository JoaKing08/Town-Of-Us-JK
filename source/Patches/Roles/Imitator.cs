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
            TaskText = () => "Use dead roles to benefit the crew";
            Color = Patches.Colors.Imitator;
            RoleType = RoleEnum.Imitator;
            AddToRoleHistory(RoleType);
        }
    }
}