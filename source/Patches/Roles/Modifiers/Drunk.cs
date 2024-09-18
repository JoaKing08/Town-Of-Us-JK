using System;

namespace TownOfUs.Roles.Modifiers
{
    public class Drunk : Modifier
    {
        public int RoundsLeft;
        public Drunk(PlayerControl player) : base(player)
        {
            Name = "Drunk";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "I don't feel so good..." : "Nie czuje sie za dobrze...";
            Color = Patches.Colors.Drunk;
            ModifierType = ModifierEnum.Drunk;
            RoundsLeft = CustomGameOptions.DrunkDuration;
        }
    }
}