using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Text;
using webapi.Controllers;
using webapi.DAL;
using webapi.Model.Modifiers;

namespace webapi.Model
{
    public class CustomObject
    {
        public string NameField { get; set; }
        public string GUID { get; set; }
        public List<CustomField> CustomFields { get; set; }
        public bool HiddenFromNav { get; set; }
        public bool ExcludeExport { get; set; }

        public CustomObject()
        {
            GUID = Guid.NewGuid().ToString();
            CustomFields = new List<CustomField>();
        }

        public CustomObject(object baseObject)
        {
            GUID = Guid.NewGuid().ToString();
            CustomFields = new List<CustomField>();

            PropertyInfo[] properties = baseObject.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                CustomField customField = new CustomField(property.Name, property.PropertyType.Name, false);
                customField.Value = property.GetValue(baseObject);
                customField.LoadDefaultEditor();
                CustomFields.Add(customField);
            }
        }

        public void SetField(string fieldName, object value)
        {
            CustomField customField = CustomFields.First(x => x.Name == fieldName);
            customField.Value = value;
        }

        public object GetField(string fieldName)
        {
            return CustomFields.First(x => x.Name == fieldName).Value;
        }

        public CustomObject Copy()
        {
            CustomObject copy = new CustomObject();
            copy.NameField = NameField;
            foreach(CustomField field in CustomFields)
            {
                CustomField newField = field.Copy();
                copy.CustomFields.Add(newField);
                foreach(Modifier modifier in newField.Modifiers)
                {
                    modifier.OnApply(copy, newField);
                }
            }
            return copy;
        }

        public string GetDisplayName()
        {
            IEnumerable<CustomField> NameFieldObject = CustomFields.Where(x => x.Name == NameField);
            if (NameFieldObject.Count() > 0 && NameFieldObject.First().Value != null)
                return NameFieldObject.First().Value.ToString();

            return "new object";
        }

        public string ToJson(bool resolveReferences)
        {
            if (resolveReferences)
                throw new NotImplementedException();

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{");

            stringBuilder.AppendLine($"\"DisplayName\" : \"{NameField}\",");

            stringBuilder.AppendLine("\"Properties\" : [");
            foreach (CustomField customField in CustomFields)
            {
                stringBuilder.AppendLine($"{{\"{customField.Name}\" : \"{customField.Value}\"}}");
                if (customField != CustomFields.Last())
                    stringBuilder.Append(',');
            }

            stringBuilder.Append("]}");

            return stringBuilder.ToString();
        }

        public StringBuilder Export(StringBuilder builder, List<string> resolvedGUIDs)
        {
            builder.Append("{");

            if (resolvedGUIDs.Contains(GUID))
            {
                builder.Append($"\"$ref\": \"{GUID}\"");
            }
            else
            {
                builder.Append($"\"$id\": \"{GUID}\",");
                resolvedGUIDs.Add(GUID);

                foreach (CustomField customField in CustomFields)
                {
                    string serialValue = "";
                    if (customField.IsArray)
                    {
                        if (customField.IsReference)
                        {
                             List<string> referenceStrings = (customField.Value as IEnumerable<object>)?.Select(x => x?.ToString()).ToList();

                            serialValue = "[";
                            if (referenceStrings != null)
                            {
                                foreach (string referenceString in referenceStrings)
                                {
                                    StringBuilder referenceObject = new StringBuilder();

                                    CustomObject referenceCustomObject = ProjectManager.GetCustomObjectByGuid(referenceString);
                                    if (referenceCustomObject != null)
                                    {
                                        referenceCustomObject.Export(referenceObject, resolvedGUIDs);
                                    }
                                    else
                                    {
                                        referenceObject.Append("");
                                    }
                                    serialValue += referenceObject.ToString();
                                    if (referenceString != referenceStrings.Last())
                                        serialValue += ",";
                                }
                            }
                            serialValue += "]";
                        }
                        else
                        {
                            serialValue = JsonConvert.SerializeObject(customField.Value);
                        }
                    }
                    else if (customField.IsReference)
                    {
                        StringBuilder referenceObject = new StringBuilder();

                        if (customField.Value != null)
                        {
                            ProjectManager.GetCustomObjectByGuid(customField.Value.ToString()).Export(referenceObject, resolvedGUIDs);
                            serialValue = referenceObject.ToString();
                        }
                        else
                        {
                            serialValue = "null";
                        }

                    }
                    else
                    {
                        serialValue = JsonConvert.SerializeObject(customField.Value);
                    }

                    builder.AppendLine($"\"{customField.Name}\" : {serialValue}");
                    if (customField != CustomFields.Last())
                        builder.Append(',');
                }
            }
            builder.Append("}");

            return builder;
        }

        public T ReflectIntoClass<T>()
        {
            T newObject = Activator.CreateInstance<T>();
            PropertyInfo[] properties = newObject.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                CustomField customField = CustomFields.First(x => x.Name == property.Name);
                property.SetValue(newObject, customField.Value);
            }
            return newObject;
        }
    }
}
