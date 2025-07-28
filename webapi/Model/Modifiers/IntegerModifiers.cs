namespace webapi.Model.Modifiers
{
    public class MinimumModifier : Modifier, IValidator
    {
        public int Minimum { get; set; }

        public override bool CanTargetProperty(CustomObject owner, CustomField field)
        {
            //TODO: Type utility for better abstraction
            return field.UnderlyingType.ToLower() == "int" ||
                field.UnderlyingType.ToLower() == "int32";
        }

        public override void OnApply(CustomObject owner, CustomField field)
        {
        }

        public override void OnRemove(CustomObject owner, CustomField field)
        {
        }

        public override void OnRender(RenderField field)
        {
        }

        public bool Validate(CustomField field, object oldValue)
        {
            int newValue;
            if (!int.TryParse(field.Value.ToString(), out newValue))
            {
                field.Value = oldValue;
                return false;
            }

            if (newValue < Minimum)
            {
                field.Value = oldValue;
                return false;
            }

            return true;
        }
    }

    public class MaximumModifier : Modifier, IValidator
    {
        public int Maximum { get; set; }

        public override bool CanTargetProperty(CustomObject owner, CustomField field)
        {
            //TODO: Type utility for better abstraction
            return field.UnderlyingType.ToLower() == "int" ||
                field.UnderlyingType.ToLower() == "int32";
        }

        public override void OnApply(CustomObject owner, CustomField field)
        {
        }

        public override void OnRemove(CustomObject owner, CustomField field)
        {
        }

        public override void OnRender(RenderField field)
        {
        }

        public bool Validate(CustomField field, object oldValue)
        {
            int newValue;
            if (!int.TryParse(field.Value.ToString(), out newValue))
            {
                field.Value = oldValue;
                return false;
            }

            if (newValue > Maximum)
            {
                field.Value = oldValue;
                return false;
            }

            return true;
        }
    }

    public class SliderModifier : Modifier, IValidator
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }

        public override bool CanTargetProperty(CustomObject owner, CustomField field)
        {
            return field.UnderlyingType.ToLower() == "int" ||
                field.UnderlyingType.ToLower() == "int32";
        }

        public override void OnApply(CustomObject owner, CustomField field)
        {
        }

        public override void OnRemove(CustomObject owner, CustomField field)
        {
        }

        public override void OnRender(RenderField field)
        {
            field.RenderComponent = "FC_Slider";
            field.AdditionalData = new { Minimum = this.Minimum, Maximum = this.Maximum };

        }

        public bool Validate(CustomField field, object oldValue)
        {
            int newValue;
            if (!int.TryParse(field.Value.ToString(), out newValue))
            {
                field.Value = oldValue;
                return false;
            }

            if (newValue > Maximum || newValue < Minimum)
            {
                field.Value = oldValue;
                return false;
            }

            return true;
        }
    }
}
