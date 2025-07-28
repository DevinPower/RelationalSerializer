namespace webapi.Model
{
    public class CustomObjectMeta
    {
        public string GUID { get; private set; }
        public string Project { get; private set; }
        public bool IsHidden { get; private set; }
        public bool ExcludeExport { get; private set; }

        public CustomObjectMeta(string guid, string project, bool isHidden, bool excludeExport) 
        {
            this.GUID = guid;
            this.Project = project;
            this.IsHidden = isHidden;
            this.ExcludeExport = excludeExport;
        }
    }
}
