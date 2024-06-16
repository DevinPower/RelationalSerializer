namespace webapi.Model
{
    public class ProjectModel
    {
        public string Name { get; set; }
        public string GUID { get; set; }

        public ProjectModel(ProjectObject project)
        {
            Name = project.Name;
            GUID = project.GUID;
        }
    }
}
