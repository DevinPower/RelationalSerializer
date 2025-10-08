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

        public RenderField(string name, object value, string renderComponent, bool isArray)
        {
            this.Name = name;
            this.Value = value;
            this.RenderComponent = renderComponent;
            this.IsArray = isArray;
        }
    }

    public class RenderModifierField : RenderField
    {
        public List<Modifier> AvailableModifiers { get; set; }
        public List<Modifier> ActiveModifiers { get; set; }
        public RenderModifierField(string name, object value, string renderComponent, bool isArray, List<Modifier> availableModifiers, List<Modifier> activeModifiers) : base(name, value, renderComponent, isArray)
        {
            this.Name = name;
            this.Value = value;
            this.RenderComponent = renderComponent;
            this.IsArray = isArray;

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
                bool dontAdd = false;
                RenderField newField = new RenderField(field.Name, field.Value, field.EditorType, field.IsArray);
                if (field.Modifiers != null)
                {
                    foreach (Modifier modifier in field.Modifiers)
                    {
                        if (modifier is HiddenModifier)
                        {
                            dontAdd = true;
                            break;
                        }

                        modifier.OnRender(newField);
                    }
                }

                if (field.IsReference)
                    newField.AdditionalData = new { ReferenceCategory = TypeUtilities.GetProjectFromName(field.UnderlyingType) };

                if (!dontAdd)
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
