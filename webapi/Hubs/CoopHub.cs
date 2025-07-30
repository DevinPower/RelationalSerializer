using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using webapi.DAL;
using webapi.Model;
using webapi.Model.Modifiers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace webapi
{
    public class CoopHub : Hub
    {
        //TODO: Currently this updates for all clients, but we need to make it so it
        //      only updates for the clients that are viewing the same object
        public async Task UpdateField(string project, string objectid, string field, object value)
        {
            int arrayIndex = -1;
            if (field.Contains('['))
            {
                arrayIndex = int.Parse(field.Split('[')[1].Split(']')[0]);
                field = field.Split('[')[0];
            }
            int projectInteger;
            if (!Int32.TryParse(project, out projectInteger))
                return;
            Model.CustomObject obj = ProjectManager.projects[projectInteger].CustomObjects.First(x => x.GUID == objectid);

            CustomField fieldObject = obj.CustomFields
                .First(x => x.Name == field);
            if (!fieldObject.IsArray)
            {
                object oldValue = fieldObject.Value;
                fieldObject.Value = value;

                foreach (Modifier modifier in fieldObject.Modifiers.Where(x => x is IValidator))
                {
                    ((IValidator)modifier).Validate(fieldObject, oldValue);
                }

                await Clients.All.SendAsync("updateFieldFromOther", field, fieldObject.Value);
            }
            else
            {
                //TODO: Abstract array inserts
                IEnumerable<object> valueArray = (IEnumerable<object>)fieldObject.Value;
                List<object> valueList = valueArray.ToList();
                while (valueList.Count <= arrayIndex)
                    valueList.Add(value);

                valueList[arrayIndex] = value;
                fieldObject.Value = valueList;

                await Clients.All.SendAsync("updateArrayFromOther", field, value, arrayIndex);
            }
            
            await DBProjects.UpsertFieldAsync(obj.CustomFields.First(x => x.Name == field), obj.GUID);
        }

        public async Task RemoveFromArray(string project, string objectid, string field, int index)
        {
            int projectInteger;
            if (!Int32.TryParse(project, out projectInteger))
                return;
            Model.CustomObject obj = ProjectManager.projects[projectInteger].CustomObjects.First(x => x.GUID == objectid);

            CustomField fieldObject = obj.CustomFields
                .Where(x => x.Name == field).First();

            IEnumerable<object> valueArray = (IEnumerable<object>)fieldObject.Value;
            List<object> valueList = valueArray.ToList();
            valueList.RemoveAt(index);
            fieldObject.Value = valueList;

            await Clients.Others.SendAsync("removeFromArrayFromOther", field, index);

            await DBProjects.UpsertFieldAsync(obj.CustomFields.First(x => x.Name == field), obj.GUID);
        }

        public async Task InstantiateIntoField(string project, string guid, string field)
        {
            int projectInteger;
            if (!Int32.TryParse(project, out projectInteger))
                return;

            CustomObject customObject = ProjectManager.projects[projectInteger].CustomObjects.First(x => x.GUID == guid);
            CustomField propertyField = customObject.CustomFields.First(x => x.Name == field.Split('[')[0]);
            string referenceProjectName = propertyField.UnderlyingType;
            ProjectObject referenceProject = ProjectManager.GetProjectByName(referenceProjectName);

            CustomObject instantiated = referenceProject.Templates[0].Copy();
            referenceProject.CreateObject(instantiated, referenceProject.GUID);

            await UpdateField(project, guid, field, instantiated.GUID);
        }
    }
}
