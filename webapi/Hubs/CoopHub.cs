using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using webapi.DAL;
using webapi.Model;
using webapi.Model.Modifiers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace webapi
{
    public class CoopHub : Hub
    {
        // Debouncing mechanism for database writes
        private static readonly ConcurrentDictionary<string, Timer> _fieldUpdateTimers = new ConcurrentDictionary<string, Timer>();
        private static readonly ConcurrentDictionary<string, (CustomField field, string guid)> _pendingUpdates = new ConcurrentDictionary<string, (CustomField, string)>();
        private const int DEBOUNCE_DELAY_MS = 1000;
		
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
            
            // Use debounced database update instead of immediate write
            DebouncedUpsertField(obj.CustomFields.First(x => x.Name == field), obj.GUID, field);
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

            // Use debounced database update instead of immediate write
            DebouncedUpsertField(obj.CustomFields.First(x => x.Name == field), obj.GUID, field);
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

        /// <summary>
        /// Debounces database updates to reduce database hammering during rapid field edits
        /// </summary>
        private static void DebouncedUpsertField(CustomField field, string guid, string fieldName)
        {
            string key = $"{guid}_{fieldName}";
            
            // Store the latest update for this field
            _pendingUpdates[key] = (field, guid);
            
            // Cancel existing timer if present
            if (_fieldUpdateTimers.TryGetValue(key, out Timer? existingTimer))
            {
                existingTimer?.Dispose();
            }
            
            // Create new timer that will execute the database update after delay
            Timer newTimer = new Timer(async _ =>
            {
                if (_pendingUpdates.TryRemove(key, out (CustomField field, string guid) pendingUpdate))
                {
                    try
                    {
                        await DBProjects.UpsertFieldAsync(pendingUpdate.field, pendingUpdate.guid);
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't crash the application
                        Console.WriteLine($"Error updating field {fieldName} for object {pendingUpdate.guid}: {ex.Message}");
                    }
                }
                
                // Clean up timer
                if (_fieldUpdateTimers.TryRemove(key, out Timer? timer))
                {
                    timer?.Dispose();
                }
            }, null, DEBOUNCE_DELAY_MS, Timeout.Infinite);
            
            _fieldUpdateTimers[key] = newTimer;
        }
    }
}
