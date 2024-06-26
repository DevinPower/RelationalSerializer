﻿using webapi.Model.Modifiers;

namespace webapi.Model
{
    public class TemplateModifierModel
    {
        public string FieldName { get; set; }
        public List<RenderObject> ActiveModifiers { get; set; }
        public List<RenderObject> AvailableModifiers { get; set; }

        public TemplateModifierModel(string FieldName, List<Modifier> ActiveModifiers, List<Modifier> AvailableModifiers)
        {
            this.FieldName = FieldName;
            if (ActiveModifiers == null)
            {
                ActiveModifiers = new List<Modifier>();
            }
            else
            {
                this.ActiveModifiers = ActiveModifiers.Select(x =>
                {
                    RenderObject JITObject = new RenderObject(new CustomObject(x));
                    JITObject.DisplayName = x.GetType().Name;
                    return JITObject;
                }).ToList();
            }

            this.AvailableModifiers = AvailableModifiers.Select(x => {
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
                    var value = renderObject.Properties.Where(x => x.Name == property.Name).First().Value;
                    property.SetValue(underlyingModifier, value);
                }

                toReturn.Add(underlyingModifier);
            }

            return toReturn;
        }
    }
}
