using webapi.DAL;

namespace webapi.Model
{
    public class InstanceSettings
    {
        public static InstanceSettings Singleton;

        public string GithubAPIKey { get; set; }
        public string GithubRepository { get; set; }
        public string JiraAPIKey { get; set; }
        public bool RegisterWebhooks { get; set; }
    }
}
