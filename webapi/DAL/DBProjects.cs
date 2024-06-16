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

        public static void CreateProject(ProjectObject project)
        {
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("InsertProject", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@guid", project.GUID));
                cmd.Parameters.Add(new SqlParameter("@name", project.Name));
                cmd.ExecuteNonQuery();
            }

            foreach (CustomObject customObject in project.Templates)
            {
                UpsertObject(customObject, project.GUID);
                InsertTemplateMeta(customObject.GUID, project.GUID);
            }
        }

        public static void UpsertObject(CustomObject customObject, string Project)
        {
            using (SqlConnection conn = OpenConnection())
            {
                foreach(CustomField field in customObject.CustomFields)
                {
                    UpsertField(field, customObject.GUID);
                }

                SqlCommand objCmd = new SqlCommand("UpsertObjectMeta", conn);
                objCmd.CommandType = CommandType.StoredProcedure;
                objCmd.Parameters.Add(new SqlParameter("@guid", customObject.GUID));
                objCmd.Parameters.Add(new SqlParameter("@project", Project));
                objCmd.ExecuteNonQuery();
            }
        }

        public static void UpsertMods(CustomObject customObject)
        {
            using (SqlConnection conn = OpenConnection())
            {
                foreach (CustomField field in customObject.CustomFields)
                {
                    SqlCommand cmd = new SqlCommand("UpsertModifiers", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@guid", customObject.GUID));
                    cmd.Parameters.Add(new SqlParameter("@field", field.Name));

                    string serializedValue = JsonConvert.SerializeObject(field.Modifiers);

                    cmd.Parameters.Add(new SqlParameter("@value", serializedValue));
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void UpsertField(CustomField field, string GUID)
        {
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("UpsertField", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@guid", GUID));
                cmd.Parameters.Add(new SqlParameter("@field", field.Name));
                cmd.Parameters.Add(new SqlParameter("@type", field.UnderlyingType));
                cmd.Parameters.Add(new SqlParameter("@isArray", field.IsArray));

                string serializedValue = JsonConvert.SerializeObject(field.Value);
                cmd.Parameters.Add(new SqlParameter("@value", serializedValue));

                cmd.ExecuteNonQuery();
            }
        }

        public static void InsertTemplateMeta(string ObjectGUID, string ProjectGUID)
        {
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("InsertTemplateMeta", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@projectGUID", ProjectGUID));
                cmd.Parameters.Add(new SqlParameter("@objectGUID", ObjectGUID));
                cmd.ExecuteNonQuery();
            }
        }

        public static List<(string ProjectGuid, string ObjectGuid)> GetTemplates()
        {
            List<(string projectGUID, string objectGUID)> templates = new List<(string projectGUID, string objectGUID)>();
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("GetTemplates", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    templates.Add((reader["PROJECT_GUID"].ToString(), reader["OBJECT_GUID"].ToString()));
                }
            }

            return templates;
        }

        public static CustomObject GetTemplateModObjects(string guid)
        {
            using (SqlConnection conn = OpenConnection())
            {
                CustomObject completedObject = GetObject(guid);
                SqlCommand cmd = new SqlCommand("GetTemplateMods", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@GUID", guid));

                SqlDataReader reader = cmd.ExecuteReader();

                while(reader.Read())
                {
                    string field = reader["FIELD"].ToString();
                    string value = reader["VALUE"].ToString();

                    List<Modifier> modifiers = ModifierCaster.CastIntoModifiers(value);

                    completedObject.CustomFields.Where(x=>x.Name == field).First().Modifiers = modifiers;
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

        public static CustomObject GetObject(string guid)
        {
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("GetObjectFields", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@GUID", guid));

                SqlDataReader reader = cmd.ExecuteReader();

                CustomObject readObject = new CustomObject();
                readObject.GUID = guid;
                while (reader.Read())
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

        public static List<(string GUID, string Name)> GetProjects()
        {
            List<(string, string)> projects = new List<(string, string)>();
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("GetProjects", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string guid = reader["GUID"].ToString();
                    string name = reader["NAME"].ToString();

                    projects.Add((guid, name));
                }

                return projects;
            }
        }

        public static List<CustomObjectMeta> GetObjectGUIDsByProject()
        {
            List<CustomObjectMeta> objects = new List<CustomObjectMeta>();
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("GetAllObjectGuids", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
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

        public static void InsertEnum(CustomEnum customEnum)
        {
            DataTable myTable = new DataTable();
            myTable.Columns.Add("Enum", typeof(string));
            myTable.Columns.Add("Label", typeof(string));
            myTable.Columns.Add("Value", typeof(int));

            foreach(ValuePairs pair in customEnum.Values)
                myTable.Rows.Add(customEnum.Name, pair.Name, pair.Value);

            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("SetEnum", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter tvpParam = cmd.Parameters.AddWithValue("@InsertValues", myTable);
                tvpParam.SqlDbType = SqlDbType.Structured;

                cmd.ExecuteNonQuery();
            }
        }

        public static CustomEnum? GetEnum(string name)
        {
            using (SqlConnection conn = OpenConnection())
            {
                CustomEnum toReturn = new CustomEnum(name);
                SqlCommand cmd = new SqlCommand("GetEnumValues", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@name", name));

                SqlDataReader reader = cmd.ExecuteReader();

                if (!reader.HasRows)
                    return null;

                while (reader.Read())
                {
                    string label = reader["label"].ToString();
                    int value = (int)reader["value"];

                    toReturn.AddValue(label, value);
                }

                return toReturn;
            }
        }

        public static List<string> GetEnumTypes()
        {
            using (SqlConnection conn = OpenConnection())
            {
                List<string> toReturn = new List<string>();
                SqlCommand cmd = new SqlCommand("GetEnumTypes", conn);
                cmd.CommandType = CommandType.StoredProcedure;

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string name = reader["enum"].ToString();
                    toReturn.Add(name);
                }

                return toReturn;
            }
        }

        public static void DeleteProject(string guid)
        {
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("DeleteProject", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@target", guid));

                cmd.ExecuteNonQuery();
            }
        }

        public static void SetExportExclude(string guid, bool value)
        {
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("SetExportExclude", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@GUID", guid));
                cmd.Parameters.Add(new SqlParameter("@newValue", value));

                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteField(string fieldName, string guid)
        {
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("DeleteField", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@property", fieldName));
                cmd.Parameters.Add(new SqlParameter("@GUID", guid));

                cmd.ExecuteNonQuery();
            }
        }

        public static void DeleteObject(string guid)
        {
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("DeleteObject", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@GUID", guid));

                cmd.ExecuteNonQuery();
            }
        }

        public static void InsertSource(string projectName, string sourceType, string source)
        {
            using (SqlConnection conn = OpenConnection())
            {
                SqlCommand cmd = new SqlCommand("InsertSource", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@name", projectName));
                cmd.Parameters.Add(new SqlParameter("@source_type", sourceType));
                cmd.Parameters.Add(new SqlParameter("@source", source));

                cmd.ExecuteNonQuery();
            }
        }

        public static string GetSourceByName(string name)
        {
            using (SqlConnection conn = OpenConnection())
            {
                List<string> toReturn = new List<string>();
                SqlCommand cmd = new SqlCommand("GetSourceByName", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@name", name));

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string source = reader["source"].ToString();
                    return source;
                }

                return null;
            }
        }
    }
}
