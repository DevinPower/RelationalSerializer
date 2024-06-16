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
        public async Task UpdateField(string project, string objectid, string Field, object Value)
        {
            int arrayIndex = -1;
            if (Field.Contains('['))
            {
                arrayIndex = int.Parse(Field.Split('[')[1].Split(']')[0]);
                Field = Field.Split('[')[0];
            }
            int projectInteger;
            if (!Int32.TryParse(project, out projectInteger))
                return;
            Model.CustomObject obj = ProjectManager.projects[projectInteger].CustomObjects.First(x => x.GUID == objectid);

            CustomField field = obj.CustomFields
                .Where(x => x.Name == Field).First();
            if (!field.IsArray)
            {
                object oldValue = field.Value;
                field.Value = Value;

                foreach (Modifier modifier in field.Modifiers.Where(x => x is iValidator))
                {
                    ((iValidator)modifier).Validate(field, oldValue);
                }

                await Clients.All.SendAsync("updateFieldFromOther", Field, field.Value);
            }
            else
            {
                IEnumerable<object> valueArray = (IEnumerable<object>)field.Value;
                List<object> valueList = valueArray.ToList();
                while (valueList.Count <= arrayIndex)
                    valueList.Add(Value);

                valueList[arrayIndex] = Value;
                field.Value = valueList;

                await Clients.All.SendAsync("updateArrayFromOther", Field, Value, arrayIndex);
            }
            
            DBProjects.UpsertField(obj.CustomFields.First(x => x.Name == Field), obj.GUID);
        }

        public async Task RemoveFromArray(string project, string objectid, string Field, int index)
        {
            int projectInteger;
            if (!Int32.TryParse(project, out projectInteger))
                return;
            Model.CustomObject obj = ProjectManager.projects[projectInteger].CustomObjects.First(x => x.GUID == objectid);

            CustomField field = obj.CustomFields
                .Where(x => x.Name == Field).First();

            IEnumerable<object> valueArray = (IEnumerable<object>)field.Value;
            List<object> valueList = valueArray.ToList();
            valueList.RemoveAt(index);
            field.Value = valueList;

            await Clients.Others.SendAsync("removeFromArrayFromOther", Field, index);

            DBProjects.UpsertField(obj.CustomFields.First(x => x.Name == Field), obj.GUID);
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
