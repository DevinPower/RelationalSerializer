using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System;
using System.Data;
using webapi.Model;
using webapi.Model.Modifiers;
using webapi.Utility;

namespace webapi.DAL
{
    public class DBProjects
    {
        static SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(Settings.ConnectionString);
            connection.Open();

            return connection;
        }

        public static async Task CreateProjectAsync(ProjectObject project)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("InsertProject", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@guid", project.GUID));
                cmd.Parameters.Add(new SqlParameter("@name", project.Name));
                await cmd.ExecuteNonQueryAsync();
            }

            foreach (CustomObject customObject in project.Templates)
            {
                await UpsertObjectAsync(customObject, project.GUID);
                await InsertTemplateMetaAsync(customObject.GUID, project.GUID);
            }
        }

        public static async Task UpsertObjectAsync(CustomObject customObject, string Project)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                
                // Process all fields using the same connection for better performance
                foreach(CustomField field in customObject.CustomFields)
                {
                    await UpsertFieldWithConnectionAsync(field, customObject.GUID, conn);
                }

                SqlCommand objCmd = new SqlCommand("UpsertObjectMeta", conn);
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add(new SqlParameter("@guid", customObject.GUID));
                objCmd.Parameters.Add(new SqlParameter("@project", Project));
                await objCmd.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Helper method to upsert field using an existing connection for better performance
        /// </summary>
        private static async Task UpsertFieldWithConnectionAsync(CustomField field, string GUID, SqlConnection conn)
        {
            SqlCommand cmd = new SqlCommand("UpsertField", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@guid", GUID));
            cmd.Parameters.Add(new SqlParameter("@field", field.Name));
            cmd.Parameters.Add(new SqlParameter("@type", field.UnderlyingType));
            cmd.Parameters.Add(new SqlParameter("@isArray", field.IsArray));

            string serializedValue = JsonConvert.SerializeObject(field.Value);
            cmd.Parameters.Add(new SqlParameter("@value", serializedValue));

            await cmd.ExecuteNonQueryAsync();
        }

        public static async Task UpsertModsAsync(CustomObject customObject)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                foreach (CustomField field in customObject.CustomFields)
                {
                    SqlCommand cmd = new SqlCommand("UpsertModifiers", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@guid", customObject.GUID));
                    cmd.Parameters.Add(new SqlParameter("@field", field.Name));

                    string serializedValue = JsonConvert.SerializeObject(field.Modifiers);

                    cmd.Parameters.Add(new SqlParameter("@value", serializedValue));
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public static async Task UpsertFieldAsync(CustomField field, string GUID)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("UpsertField", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@guid", GUID));
                cmd.Parameters.Add(new SqlParameter("@field", field.Name));
                cmd.Parameters.Add(new SqlParameter("@type", field.UnderlyingType));
                cmd.Parameters.Add(new SqlParameter("@isArray", field.IsArray));

                string serializedValue = JsonConvert.SerializeObject(field.Value);
                cmd.Parameters.Add(new SqlParameter("@value", serializedValue));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task InsertTemplateMetaAsync(string ObjectGUID, string ProjectGUID)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("InsertTemplateMeta", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@projectGUID", ProjectGUID));
                cmd.Parameters.Add(new SqlParameter("@objectGUID", ObjectGUID));
                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task<List<(string ProjectGuid, string ObjectGuid)>> GetTemplatesAsync()
        {
            List<(string projectGUID, string objectGUID)> templates = new List<(string projectGUID, string objectGUID)>();
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("GetTemplates", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    templates.Add((reader["PROJECT_GUID"].ToString(), reader["OBJECT_GUID"].ToString()));
                }
            }

            return templates;
        }

        public static async Task<CustomObject> GetTemplateModObjectsAsync(string guid)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                CustomObject completedObject = await GetObjectAsync(guid);
                SqlCommand cmd = new SqlCommand("GetTemplateMods", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@GUID", guid));

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while(await reader.ReadAsync())
                {
                    string field = reader["FIELD"].ToString();
                    string value = reader["VALUE"].ToString();

                    List<Modifier> modifiers = ModifierCaster.CastIntoModifiers(value);

                    var matches = completedObject.CustomFields.Where(x => x.Name == field);
                    if (matches.Count() > 0)
                        matches.First().Modifiers = modifiers;
                }

                foreach(CustomField field in completedObject.CustomFields)
                {
                    if (field.Modifiers == null)
                        field.Modifiers = new List<Modifier>();
                    if (field.AvailableModifiers == null)
                        field.AvailableModifiers = new List<Modifier>();

                    foreach(Modifier modifier in field.Modifiers)
                    {
                        modifier.OnApply(completedObject, field);
                    }
                }

                return completedObject;
            }
        }

        public static async Task<CustomObject> GetObjectAsync(string guid)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("GetObjectFields", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@GUID", guid));

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                CustomObject readObject = new CustomObject();
                readObject.GUID = guid;
                while (await reader.ReadAsync())
                {
                    string field = reader["PROPERTY"].ToString();
                    string value = reader["PROPERTY_VALUE"].ToString();
                    string type = reader["TYPE"].ToString();
                    bool isArray = (bool)reader["ISARRAY"];

                    CustomField newField = new CustomField(field, type, isArray)
                    {
                        Value = JsonConvert.DeserializeObject(value)
                    };

                    newField.LoadDefaultEditor();

                    readObject.CustomFields.Add(newField);
                }

                return readObject;
            }
        }

        public static async Task<List<(string GUID, string Name)>> GetProjectsAsync()
        {
            List<(string, string)> projects = new List<(string, string)>();
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("GetProjects", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    string guid = reader["GUID"].ToString();
                    string name = reader["NAME"].ToString();

                    projects.Add((guid, name));
                }

                return projects;
            }
        }

        public static async Task<List<CustomObjectMeta>> GetObjectGUIDsByProjectAsync()
        {
            List<CustomObjectMeta> objects = new List<CustomObjectMeta>();
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("GetAllObjectGuids", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    string guid = reader["GUID"].ToString();
                    string project = reader["PROJECT"].ToString();
                    bool isHidden = reader["NAV_HIDDEN"] is DBNull ? false : (bool)reader["NAV_HIDDEN"];
                    bool excludeExport = reader["EXPORT_EXCLUDE"] is DBNull ? false : (bool)reader["EXPORT_EXCLUDE"];

                    objects.Add(new CustomObjectMeta(guid, project, isHidden, excludeExport));
                }

                return objects;
            }
        }

        public static async Task InsertEnumAsync(CustomEnum customEnum)
        {
            DataTable myTable = new DataTable();
            myTable.Columns.Add("Enum", typeof(string));
            myTable.Columns.Add("Label", typeof(string));
            myTable.Columns.Add("Value", typeof(int));

            foreach(ValuePairs pair in customEnum.Values)
                myTable.Rows.Add(customEnum.Name, pair.Name, pair.Value);

            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SetEnum", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter tvpParam = cmd.Parameters.AddWithValue("@InsertValues", myTable);
                tvpParam.SqlDbType = SqlDbType.Structured;

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task<CustomEnum?> GetEnumAsync(string name)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                CustomEnum toReturn = new CustomEnum(name);
                SqlCommand cmd = new SqlCommand("GetEnumValues", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@name", name));

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                if (!reader.HasRows)
                    return null;

                while (await reader.ReadAsync())
                {
                    string label = reader["label"].ToString();
                    int value = (int)reader["value"];

                    toReturn.AddValue(label, value);
                }

                return toReturn;
            }
        }

        public static async Task<List<string>> GetEnumTypesAsync()
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                List<string> toReturn = new List<string>();
                SqlCommand cmd = new SqlCommand("GetEnumTypes", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    string name = reader["enum"].ToString();
                    toReturn.Add(name);
                }

                return toReturn;
            }
        }

        public static async Task DeleteProjectAsync(string guid)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("DeleteProject", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@target", guid));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task SetExportExcludeAsync(string guid, bool value)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("SetExportExclude", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@GUID", guid));
                cmd.Parameters.Add(new SqlParameter("@newValue", value));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task DeleteFieldAsync(string fieldName, string guid)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("DeleteField", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@property", fieldName));
                cmd.Parameters.Add(new SqlParameter("@GUID", guid));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task DeleteObjectAsync(string guid)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("DeleteObject", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@GUID", guid));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task InsertSourceAsync(string projectName, string sourceType, string source)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("InsertSource", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@name", projectName));
                cmd.Parameters.Add(new SqlParameter("@source_type", sourceType));
                cmd.Parameters.Add(new SqlParameter("@source", source));

                await cmd.ExecuteNonQueryAsync();
            }
        }

        public static async Task<string> GetSourceByNameAsync(string name)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                await conn.OpenAsync();
                SqlCommand cmd = new SqlCommand("GetSourceByName", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@name", name));

                SqlDataReader reader = await cmd.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    string source = reader["source"].ToString();
                    return source;
                }

                return null;
            }
        }
    }
}
