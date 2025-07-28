using System.Collections;
using System.Linq;

namespace webapi.Model.Modifiers
{
    public class LengthModifier : Modifier, iValidator
    {
        public int Length { get; set; }

        public override bool CanTargetProperty(CustomObject owner, CustomField field)
        {
            return field.UnderlyingType.ToLower() == "string" || field.IsArray;
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
            bool isValid = true;
            object newValue = field.Value;
            if (!field.IsArray)
                isValid = newValue.ToString().Length <= Length;
            else
                isValid = ((IEnumerable<object>)newValue).Count() <= Length;

            if (!isValid)
                field.Value = oldValue;

            return isValid;
        }
    }

    public class TypeSwapCode : Modifier
    {
        public string SyntaxLanguage { get; set; }
        public override bool CanTargetProperty(CustomObject owner, CustomField field)
        {
            return field.UnderlyingType == "string" && field.EditorType == "FC_Default";
        }

        public override void OnApply(CustomObject owner, CustomField field)
        {
        }

        public override void OnRemove(CustomObject owner, CustomField field)
        {
        }

        public override void OnRender(RenderField field)
        {
            field.RenderComponent = "FC_Code";
            field.AdditionalData = new { Language = SyntaxLanguage };
        }
    }

    public class TypeSwapTextArea : Modifier
    {
        public override bool CanTargetProperty(CustomObject owner, CustomField field)
        {
            return field.UnderlyingType == "string" && field.EditorType == "FC_Default";
        }

        public override void OnApply(CustomObject owner, CustomField field)
        {
        }

        public override void OnRemove(CustomObject owner, CustomField field)
        {
        }

        public override void OnRender(RenderField field)
        {
            field.RenderComponent = "FC_TextArea";
        }
    }

    public class NameField : Modifier
    {
        public override bool CanTargetProperty(CustomObject owner, CustomField field)
        {
            return field.UnderlyingType == "string" && !field.IsArray;
        }

        public override void OnApply(CustomObject owner, CustomField field)
        {
            owner.NameField = field.Name;
        }

        public override void OnRemove(CustomObject owner, CustomField field)
        {
            owner.NameField = "name";
        }

        public override void OnRender(RenderField field)
        {
        }
    }
}
