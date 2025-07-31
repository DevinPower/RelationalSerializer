using System.Reflection;

namespace webapi.Model.Modifiers
{
    public abstract class Modifier
    {
        public string Name 
        { 
            get
            {
                return GetType().Name;
            }
        }

        public virtual bool SystemModifier
        {
            get
            {
                return false;
            }
        }

        public abstract bool CanTargetProperty(CustomObject owner, CustomField field);
        public abstract void OnApply(CustomObject owner, CustomField field);
        public abstract void OnRemove(CustomObject owner, CustomField field);
        public abstract void OnRender(RenderField field);

        public RenderObject UnderlyingObject { get; set; }

        public void LoadUnderlyingObject(bool loadValues)
        {
            UnderlyingObject = StripTypesFromBaseType(new CustomObject(this), loadValues);
        }

        RenderObject StripTypesFromBaseType(CustomObject CustomObject, bool loadValues)
        {
            string[] ModifierBaseProperties = typeof(Modifier).GetProperties().Select(x => x.Name).ToArray();
            for(int i = 0; i < CustomObject.CustomFields.Count; i++)
            {
                CustomField field = CustomObject.CustomFields[i];
                if (ModifierBaseProperties.Contains(field.Name))
                {
                    CustomObject.CustomFields.RemoveAt(i);
                    i--;
                }
                else
                {
                    string propertyName = field.Name;
                    PropertyInfo property = GetType().GetProperty(propertyName);
                    field.Value = property.GetValue(this);
                }
            }
            return new RenderObject(CustomObject);
        }

        public void BuildModifierFromUnderlyingObject(Dictionary<string, object> fromObject)
        {
            foreach(string field in fromObject.Keys)
            {
                PropertyInfo property = GetType().GetProperty(field);
                if (property == null)
                    continue;

                Type propertyType = property.PropertyType;
                object convertedObject = Convert.ChangeType(fromObject[field], propertyType);
                property.SetValue(this, convertedObject);

                UnderlyingObject.Properties.First(x => x.Name == field).Value = convertedObject;
            }
        }
    }
}
