using UnityEngine;
using System;
using TownOfUs.ImpostorRoles.BomberMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Patches;

namespace TownOfUs.Roles
{
    public class Mafioso : Role

    {
        public Mafioso(PlayerControl player) : base(player)
        {
            Name = "Mafioso";
            ImpostorText = () => "";
            TaskText = () => "Kill off the crew and make Godfather pleased!";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Mafioso;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }
    }
}