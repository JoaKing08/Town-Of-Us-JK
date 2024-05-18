using System;

namespace TownOfUs.CustomOption
{
    public class CustomHeaderOption : CustomOption
    {
        protected internal CustomHeaderOption(int id, MultiMenu menu, string name, Func<bool> hideWhen = null) : base(id, menu, name, CustomOptionType.Header, 0, null, hideWhen)
        {
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            Setting.Cast<ToggleOption>().TitleText.text = Name;
        }
    }
}