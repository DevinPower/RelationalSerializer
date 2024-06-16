namespace webapi
{
    public class NavModel
    {
        public string Name { get; set; }
        public string GUID { get; set; }
        public bool ExportExcluded { get; set; }

        public NavModel(string Name, string GUID, bool ExportExcluded)
        {
            this.Name = Name;
            this.GUID = GUID;
            this.ExportExcluded = ExportExcluded;
        }
    }
}
