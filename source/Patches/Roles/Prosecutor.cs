namespace TownOfUs.Roles
{
    public class Prosecutor : Role
    {
        public Prosecutor(PlayerControl player) : base(player)
        {
            Name = "Prosecutor";
            ImpostorText = () => "Exile One Person Of Your Choosing";
            TaskText = () => "Choose to exile anyone you want";
            Color = Patches.Colors.Prosecutor;
            RoleType = RoleEnum.Prosecutor;
            AddToRoleHistory(RoleType);
            StartProsecute = false;
            Revealed = false;
            ProsecuteThisMeeting = false;
            ProsecutionsLeft = CustomGameOptions.MaxProsecutions;
        }
        public bool ProsecuteThisMeeting { get; set; }
        public bool Revealed { get; set; }
        public int ProsecutionsLeft { get; set; }
        public bool StartProsecute { get; set; }
        public PlayerVoteArea Prosecute { get; set; }

        internal override bool Criteria()
        {
            return Revealed && CustomGameOptions.RevealProsecutor && !Player.Data.IsDead || base.Criteria();
        }

        internal override bool RoleCriteria()
        {
            if (!Player.Data.IsDead && CustomGameOptions.RevealProsecutor) return Revealed || base.RoleCriteria();
            return false || base.RoleCriteria();
        }
    }
}
