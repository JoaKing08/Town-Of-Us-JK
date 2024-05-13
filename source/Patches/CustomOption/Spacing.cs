namespace TownOfUs.CustomOption
{
    public class CustomSpacingOption : CustomOption
    {
        protected internal CustomSpacingOption(int id, MultiMenu menu) : base(id, menu, "", CustomOptionType.Spacing, 0)
        {
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            Setting.Cast<ToggleOption>().TitleText.text = "";
        }
    }
}