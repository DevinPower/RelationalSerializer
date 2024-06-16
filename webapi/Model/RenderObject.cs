using Microsoft.AspNetCore.Mvc.Diagnostics;
using webapi.Model.Modifiers;
using webapi.Utility;

namespace webapi.Model
{
    public class RenderField
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public bool IsArray { get; set; }
        public string RenderComponent { get; set; }
        public object AdditionalData { get; set; }

        public RenderField(string Name, object Value, string RenderComponent, bool IsArray)
        {
            this.Name = Name;
            this.Value = Value;
            this.RenderComponent = RenderComponent;
            this.IsArray = IsArray;
        }
    }

    public class RenderModifierField : RenderField
    {
        public List<Modifier> AvailableModifiers { get; set; }
        public List<Modifier> ActiveModifiers { get; set; }
        public RenderModifierField(string Name, object Value, string RenderComponent, bool IsArray, List<Modifier> availableModifiers, List<Modifier> activeModifiers) : base(Name, Value, RenderComponent, IsArray)
        {
            this.Name = Name;
            this.Value = Value;
            this.RenderComponent = RenderComponent;
            this.IsArray = IsArray;

            AvailableModifiers = availableModifiers;
            foreach(Modifier m in availableModifiers)
            {
                m.LoadUnderlyingObject(false);
            }
            ActiveModifiers = activeModifiers;
        }
    }

    public class RenderObject
    {
        public string DisplayName { get; set; }
        public string NameField { get; set; }
        public List<RenderField> Properties { get; set; }

        public RenderObject(CustomObject model) 
        {
            NameField = model.NameField;
            DisplayName = model.GetDisplayName();

            Properties = new List<RenderField>();

            foreach(CustomField field in model.CustomFields)
            {
                RenderField newField = new RenderField(field.Name, field.Value, field.EditorType, field.IsArray);
                if (field.Modifiers != null)
                {
                    foreach (Modifier modifier in field.Modifiers)
                    {
                        modifier.OnRender(newField);
                    }
                }

                if (field.IsReference)
                    newField.AdditionalData = new { ReferenceCategory = TypeUtilities.GetProjectFromName(field.UnderlyingType) };

                Properties.Add(newField);
            }
        }

        public RenderObject()
        {

        }
    }

    public class RenderModifierObject : RenderObject
    {
        public List<Modifier> AvailableModifiers { get; set; }
        public List<Modifier> ActiveModifiers { get; set; }

        public RenderModifierObject(CustomObject model) : base(model)
        {
            NameField = model.NameField;
            DisplayName = model.GetDisplayName();

            Properties = model.CustomFields.Select(x => new RenderModifierField(x.Name, x.Value, x.EditorType, x.IsArray,
                x.AvailableModifiers, x.Modifiers)).ToList<RenderField>();
        }

        public RenderModifierObject()
        {

        }


    }
}
