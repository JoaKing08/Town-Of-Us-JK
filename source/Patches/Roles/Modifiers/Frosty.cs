using System;

namespace TownOfUs.Roles.Modifiers
{
    public class Frosty : Modifier
    {
        public PlayerControl Chilled;
        public DateTime LastChilled { get; set; }
        public bool IsChilled = false;

        public Frosty(PlayerControl player) : base(player)
        {
            Name = "Frosty";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Leave behind an icy surprise" : "Zostaw za soba lodowata niespodzianke";
            Color = Patches.Colors.Frosty;
            ModifierType = ModifierEnum.Frosty;
        }
    }
}