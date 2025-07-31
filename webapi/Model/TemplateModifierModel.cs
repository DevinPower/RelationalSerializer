using webapi.Model.Modifiers;

namespace webapi.Model
{
    public class TemplateModifierModel
    {
        public string FieldName { get; set; }
        public List<RenderObject> ActiveModifiers { get; set; }
        public List<RenderObject> AvailableModifiers { get; set; }

        public TemplateModifierModel(string fieldName, List<Modifier> activeModifiers, List<Modifier> availableModifiers)
        {
            this.FieldName = fieldName;
            if (activeModifiers == null)
            {
                this.ActiveModifiers = new List<RenderObject>();
            }
            else
            {
                this.ActiveModifiers = activeModifiers.Select(x =>
                {
                    RenderObject JITObject = new RenderObject(new CustomObject(x));
                    JITObject.DisplayName = x.GetType().Name;
                    return JITObject;
                }).ToList();
            }

            this.AvailableModifiers = availableModifiers.Select(x => {
                    RenderObject JITObject = new RenderObject(new CustomObject(x));
                    JITObject.DisplayName = x.GetType().Name;
                    return JITObject;
                }).ToList();
        }

        public List<Modifier> GetActiveModifiers()
        {
            List<Modifier> toReturn = new List<Modifier>();
            foreach(RenderObject renderObject in ActiveModifiers)
            {
                Type type = Type.GetType(renderObject.DisplayName);

                if (!(type is Modifier))
                    throw new Exception($"Type of {type.Name} is not a modifier type");

                Modifier underlyingModifier = (Modifier)Activator.CreateInstance(type);
                foreach (var property in type.GetProperties())
                {
                    var value = renderObject.Properties.First(x => x.Name == property.Name).Value;
                    property.SetValue(underlyingModifier, value);
                }

                toReturn.Add(underlyingModifier);
            }

            return toReturn;
        }
    }
}
