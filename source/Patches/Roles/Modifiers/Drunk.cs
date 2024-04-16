using TownOfUs.Extensions;

namespace TownOfUs.Roles.Modifiers
{
    public class Drunk : Modifier, IVisualAlteration
    {
        public int RoundsLeft;
        public Drunk(PlayerControl player) : base(player)
        {
            Name = "Drunk";
            TaskText = () => "I don't feel so good...";
            Color = Patches.Colors.Drunk;
            ModifierType = ModifierEnum.Drunk;
            RoundsLeft = CustomGameOptions.DrunkDuration;
        }

        public bool TryGetModifiedAppearance(out VisualAppearance appearance)
        {
            appearance = Player.GetDefaultAppearance();
            appearance.SpeedFactor = CustomGameOptions.DrunkWearsOff && RoundsLeft <= 0 ? 1f : -1f;
            return true;
        }
    }
}