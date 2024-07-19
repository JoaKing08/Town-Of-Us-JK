namespace TownOfUs.Roles.Cultist
{
    public class CultistSnitch : Role
    {
        public bool CompletedTasks;
        public PlayerControl RevealedPlayer;
        public CultistSnitch(PlayerControl player) : base(player)
        {
            Name = "Snitch";
            ImpostorText = () => "Complete All Your Tasks To Reveal An Impostor";
            TaskText = () => Patches.TranslationPatches.CurrentLanguage == 0 ? "Complete all your tasks to reveal an Impostor!" : "Skoncz swoje zadania by ujawnic Impostora!";
            Color = Patches.Colors.Snitch;
            RoleType = RoleEnum.CultistSnitch;
            AddToRoleHistory(RoleType);
        }
    }
}