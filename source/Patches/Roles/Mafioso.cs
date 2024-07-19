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
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Kill off the crew and make Godfather pleased!" : "Wybij zaloge i spraw by Godfather byl zadowolony!";
            Color = Palette.ImpostorRed;
            RoleType = RoleEnum.Mafioso;
            AddToRoleHistory(RoleType);
            Faction = Faction.Impostors;
        }
    }
}