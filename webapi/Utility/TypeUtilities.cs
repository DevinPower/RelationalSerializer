using webapi.Controllers;
using webapi.DAL;

namespace webapi.Utility
{
    public class TypeUtilities
    {
        public static string GetGenericsEditor(string type)
        {
            if (IsReferenceType(type))
                return "FC_Reference";

            switch (type.ToLower())
            {
                case "string":
                    return "FC_Default";
                case "int32":
                case "int":
                    return "FC_Number";
                case "boolean":
                case "bool":
                    return "FC_Toggle";
            }

            return "FC_Default";
        }

        public static bool IsReferenceType(string type)
        {
            return ProjectManager.projects.Count(x => x.Name == type) > 0;
        }

        public static int GetProjectFromName(string name)
        {
            return ProjectManager.projects.FindIndex(x => x.Name == name);
        }

        public static bool IsEnum(string type)
        {
            return DBProjects.GetEnumTypes().Contains(type);
        }
    }
}
