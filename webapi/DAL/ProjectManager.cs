using webapi.Model;

namespace webapi.DAL
{
    public class ProjectManager
    {
        public static List<ProjectObject> projects;

        public static int AddProject(ProjectObject project)
        {
            projects.Add(project);
            return projects.Count - 1;
        }

        public static void DeleteProject(string GUID)
        {
            int index = projects.IndexOf(projects.Find(x => x.GUID == GUID));
            DBProjects.DeleteProject(GUID);
            projects.RemoveAt(index);
        }

        public static ProjectObject GetProjectByName(string name)
        {
            return projects.First(x=> x.Name == name);
        }

        public static int? GetProjectIndexByName(string name)
        {
            IEnumerable<ProjectObject> project = projects.Where(x => x.Name == name);
            if (project.Count() == 0)
                return null;

            return ProjectManager.projects.IndexOf(project.First());
        }

        public static CustomObject GetCustomObjectByGuid(string GUID)
        {
            foreach (ProjectObject project in projects)
            {
                foreach(CustomObject customObject in project.CustomObjects)
                {
                    if (customObject.GUID == GUID)
                        return customObject;
                }
            }

            return null;
        }
    }
}
