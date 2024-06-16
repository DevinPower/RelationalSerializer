using Azure.Core.GeoJson;
using System.Text;
using webapi.DAL;

namespace webapi.Model
{
    public class ProjectObject
    {
        public string Name { get; set; }
        public string GUID { get; set; }
        public List<CustomObject> CustomObjects { get; set; }
        public List<CustomObject> Templates { get; set; }

        public ProjectObject(string name, List<CustomObject> templates)
        {
            GUID = System.Guid.NewGuid().ToString();
            CustomObjects = new List<CustomObject>();
            Templates = templates;
            Name = name;
        }

        public ProjectObject(string name, string guid)
        {
            GUID = guid;
            CustomObjects = new List<CustomObject>();
            Templates = new List<CustomObject>();
            Name = name;
        }

        public void CreateObject(CustomObject customObject, string projectGUID)
        {
            AddObject(customObject);
            DBProjects.UpsertObject(customObject, projectGUID);
        }

        public void AddObject(CustomObject customObject)
        {
            CustomObjects.Add(customObject);
        }

        public void RemoveField(string FieldName)
        {
            CustomObjects.ForEach(x => {
                x.CustomFields.RemoveAll(y => y.Name == FieldName);
                DBProjects.DeleteField(FieldName, x.GUID);
            });

            Templates.ForEach(x => {
                x.CustomFields.RemoveAll(y => y.Name == FieldName);
                DBProjects.DeleteField(FieldName, x.GUID);
            });

        }

        public void AddField(CustomField NewField)
        {
            CustomObjects.ForEach(x => {
                x.CustomFields.Add(NewField.Copy());
                DBProjects.UpsertField(NewField, x.GUID);
            });

            Templates.ForEach(x =>
            {
                x.CustomFields.Add(NewField.Copy());
                DBProjects.UpsertField(NewField, x.GUID);
            });
        }

        public void ModifyType(string FieldName, string NewType)
        {
            CustomObjects.ForEach(x => {
                CustomField field = x.CustomFields.Where(y => y.Name == FieldName).First();
                field.UnderlyingType = NewType;
                DBProjects.UpsertField(field, x.GUID);
            });

            Templates.ForEach(x =>
            {
                CustomField field = x.CustomFields.Where(y => y.Name == FieldName).First();
                field.UnderlyingType = NewType;
                DBProjects.UpsertField(field, x.GUID);
            });
        }

        public void DeleteObject(string guid)
        {
            CustomObjects.RemoveAll(x => x.GUID == guid);
            DBProjects.DeleteObject(guid);
        }

        public StringBuilder Export(StringBuilder builder, List<string> resolvedGUIDs)
        {
            builder.Append($"\"{Name}\" : [");

            foreach(CustomObject customObject in CustomObjects)
            {
                customObject.Export(builder, resolvedGUIDs);
                if (customObject != CustomObjects.Last())
                    builder.Append(',');
            }

            builder.Append("]");
            return builder;
        }
    }
}
