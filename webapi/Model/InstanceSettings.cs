namespace webapi.Model
{
    public class InstanceSettings
    {
        public static InstanceSettings Singleton;

        public string GithubAPIKey { get; set; }
        public string CodeImportLanguage { get; set; }
        public string CodeEditorTheme { get; set; }
        public bool EnableLiveUpdates { get; set; }
    }
}
