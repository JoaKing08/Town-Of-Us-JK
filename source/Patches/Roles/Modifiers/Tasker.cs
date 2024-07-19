using TownOfUs.Modifiers.UnderdogMod;

namespace TownOfUs.Roles.Modifiers
{
    public class Tasker : Modifier
    {
        public Tasker(PlayerControl player) : base(player)
        {
            Name = "Tasker";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Fake tasks like nobody else can" : "Falszuj zadania jak nikt inny";
            Color = Patches.Colors.Impostor;
            ModifierType = ModifierEnum.Tasker;
        }
    }
}
