using System;
using UnityEngine;

namespace TownOfUs.CustomOption
{
    public class CustomPlayerOption : CustomOption
    {
        protected internal CustomPlayerOption(int id, MultiMenu menu, string name, bool allowNone) : base(id, menu, name,
            CustomOptionType.String,
            0)
        {
            Min = allowNone ? -1 : 0;
            Increment = 1;
        }

        protected float Min { get; set; }
        protected float Max => PlayerControl.AllPlayerControls.Count;
        protected float Increment { get; set; }

        protected internal float Get()
        {
            return (float) Value;
        }
       
        protected internal void Increase()
        {
            var increment = Increment > 5 && Input.GetKeyInt(KeyCode.LeftShift) ? 5 : Increment;

            if (Get() + increment > Max + 0.001f) // the slight increase is because of the stupid float rounding errors in the Giant speed
                Set(Min);
            else
                Set(Get() + increment);
        }

        protected internal void Decrease()
        {
            var increment = Increment > 5 && Input.GetKeyInt(KeyCode.LeftShift) ? 5 : Increment;

            if (Get() - increment < Min - 0.001f) // added it here to in case I missed something else
                Set(Max);
            else
                Set(Get() - increment);
        }

        public override void OptionCreated()
        {
            base.OptionCreated();
            var number = Setting.Cast<NumberOption>();

            number.TitleText.text = Name;
            number.ValidRange = new FloatRange(Min, Max);
            number.Increment = Increment;
            number.Value = number.oldValue = Get();
            number.ValueText.text = ToString();
        }
    }
}