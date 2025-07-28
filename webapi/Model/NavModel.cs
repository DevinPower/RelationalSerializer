namespace webapi
{
    public class NavModel
    {
        public string Name { get; set; }
        public string GUID { get; set; }
        public bool ExportExcluded { get; set; }

        public NavModel(string name, string guid, bool exportExcluded)
        {
            this.Name = name;
            this.GUID = guid;
            this.ExportExcluded = exportExcluded;
        }
    }
}
