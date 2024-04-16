namespace TownOfUs.Roles.Modifiers
{
    public class Famous : Modifier
    {
        public Famous(PlayerControl player) : base(player)
        {
            Name = "Famous";
            TaskText = () => "Notice everyone that you died";
            Color = Patches.Colors.Famous;
            ModifierType = ModifierEnum.Famous;
        }
    }
}