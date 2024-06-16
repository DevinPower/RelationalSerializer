namespace webapi.Model
{
    public class CustomObjectMeta
    {
        public string GUID { get; private set; }
        public string Project { get; private set; }
        public bool IsHidden { get; private set; }
        public bool ExcludeExport { get; private set; }

        public CustomObjectMeta(string GUID, string Project, bool IsHidden, bool ExcludeExport) 
        {
            this.GUID = GUID;
            this.Project = Project;
            this.IsHidden = IsHidden;
            this.ExcludeExport = ExcludeExport;
        }
    }
}
