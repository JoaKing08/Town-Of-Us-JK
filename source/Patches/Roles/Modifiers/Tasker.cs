using TownOfUs.Modifiers.UnderdogMod;

namespace TownOfUs.Roles.Modifiers
{
    public class Tasker : Modifier
    {
        public Tasker(PlayerControl player) : base(player)
        {
            Name = "Tasker";
            TaskText = () => "Fake tasks like nobody else can";
            Color = Patches.Colors.Impostor;
            ModifierType = ModifierEnum.Tasker;
        }
    }
}
