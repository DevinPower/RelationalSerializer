using Azure.Core.GeoJson;
using Newtonsoft.Json.Linq;
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

        public async Task CreateObject(CustomObject customObject, string projectGUID)
        {
            AddObject(customObject);
            await DBProjects.UpsertObjectAsync(customObject, projectGUID);
        }

        public void AddObject(CustomObject customObject)
        {
            CustomObjects.Add(customObject);
        }

        public async Task RemoveField(string FieldName)
        {
            List<Task> tasks = new List<Task>();

            CustomObjects.ForEach(x => {
                x.CustomFields.RemoveAll(y => y.Name == FieldName);
                tasks.Add(DBProjects.DeleteFieldAsync(FieldName, x.GUID));
            });

            Templates.ForEach(x => {
                x.CustomFields.RemoveAll(y => y.Name == FieldName);
                tasks.Add(DBProjects.DeleteFieldAsync(FieldName, x.GUID));
            });

            await Task.WhenAll(tasks);
        }

        public async Task AddField(CustomField NewField)
        {
            List<Task> tasks = new List<Task>();

            CustomObjects.ForEach(x => {
                x.CustomFields.Add(NewField.Copy());
                tasks.Add(DBProjects.UpsertFieldAsync(NewField, x.GUID));
            });

            Templates.ForEach(x =>
            {
                x.CustomFields.Add(NewField.Copy());
                tasks.Add(DBProjects.UpsertFieldAsync(NewField, x.GUID));
            });

            await Task.WhenAll(tasks);
        }

        public async Task ModifyType(string FieldName, string NewType, bool isArray)
        {
            List<Task> tasks = new List<Task>();

            CustomObjects.ForEach(x => {
                CustomField field = x.CustomFields.First(y => y.Name == FieldName);
                field.UnderlyingType = NewType;
                object oldValue = field.Value;
                field.IsArray = isArray;

                //TODO: Abstract array inserts
                IEnumerable<object> valueArray = (IEnumerable<object>)field.Value;
                List<object> valueList = valueArray.ToList();
                valueList.Add(oldValue);
                field.Value = valueList;

                tasks.Add(DBProjects.UpsertFieldAsync(field, x.GUID));
            });

            Templates.ForEach(x =>
            {
                CustomField field = x.CustomFields.First(y => y.Name == FieldName);
                field.UnderlyingType = NewType;
                field.IsArray = isArray;
                tasks.Add(DBProjects.UpsertFieldAsync(field, x.GUID));
            });

            await Task.WhenAll(tasks);
        }

        public async Task DeleteObject(string guid)
        {
            CustomObjects.RemoveAll(x => x.GUID == guid);
            await DBProjects.DeleteObjectAsync(guid);
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
