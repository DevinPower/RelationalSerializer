using webapi.Model.Modifiers;
using webapi.Utility;

namespace webapi.Model
{
    public class CustomField
    {
        public string Name { get; set; }
        public string UnderlyingType { get; set; }
        public string EditorType { get; set; }
        public object Value { get; set; }
        public bool IsArray { get; set; }
        public bool IsReference
        {
            get
            {
                return TypeUtilities.IsReferenceType(UnderlyingType);
            }
        }

        public List<Modifier> Modifiers { get; set; }
        public List<Modifier> AvailableModifiers { get; set; }

        public CustomField(string name, string underlyingType, bool isArray) 
        {
            this.Name = name;
            this.UnderlyingType = underlyingType;
            this.IsArray = isArray;

            Modifiers = new List<Modifier>();
            AvailableModifiers = new List<Modifier>();

            if (isArray)
            {
                Value = new List<object>();
            }
        }

        public CustomField()
        {

        }

        public CustomField Copy()
        {
            CustomField copy = new CustomField(Name, UnderlyingType, IsArray);

            copy.EditorType = EditorType;
            copy.Value = Value;
            copy.Modifiers = Modifiers;

            return copy;
        }

        public void LoadDefaultEditor()
        {
            EditorType = TypeUtilities.GetGenericsEditor(UnderlyingType);
        }

        public void Validate()
        {

        }
    }
}
