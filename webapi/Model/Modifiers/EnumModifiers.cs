using webapi.DAL;

namespace webapi.Model.Modifiers
{
    public class ChoiceModifier : Modifier
    {
        public string EnumName { get; set; }
        CustomEnum _Enum;

        public override bool SystemModifier
        {
            get
            {
                return true;
            }
        }

        public override bool CanTargetProperty(CustomObject owner, CustomField field)
        {
            return false;
        }

        public override void OnApply(CustomObject owner, CustomField field)
        {
        }

        public override void OnRemove(CustomObject owner, CustomField field)
        {
        }

        public override void OnRender(RenderField field)
        {
            if (GetEnum() == null)
            {
                field.RenderComponent = "error";
                return;
            }    

            field.RenderComponent = "FC_Choice";
            field.AdditionalData = new { Enum = _Enum };
        }

        async Task<CustomEnum> GetEnum()
        {
            if (_Enum != null)
                return _Enum;
            _Enum = await DBProjects.GetEnumAsync(EnumName);
            return _Enum;
        }
    }
}
