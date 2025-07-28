using Octokit;
using System.Text;

namespace webapi.DAL
{
    public class GithubItem
    {
        public string Name { get; }
        public string Type { get; }

        public GithubItem(string name, string type)
        {
            this.Name = name;
            this.Type = type;
        }
    }

    public class GithubManager
    {
        //TODO: Pull this out into a more generic place
        //      can reuse for other scenarios
        const string USER_AGENT = "RelSer";
        private string token;
        private GitHubClient _client;

        GitHubClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new GitHubClient(new ProductHeaderValue(USER_AGENT));
                    _client.Credentials = new Credentials(token);
                }

                return _client;
            }
        }

        public GithubManager(string token)
        {
            this.token = token;
        }

        public async Task<List<GithubItem>> GetRepoFolders(string owner, string repo, string path)
        {
            var items = new List<GithubItem>();
            try
            {
                var contents = await Client.Repository.Content.GetAllContents(owner, repo, path);
                foreach (var item in contents)
                {
                    items.Add(new GithubItem(item.Name, 
                        item.Type.ToString().ToLower()));
                }
            }
            catch (Octokit.NotFoundException)
            {
                return items;
            }
            return items;
        }

        public async Task<string> GetFileContent(string owner, string repo, string path)
        {
            return Encoding.UTF8.GetString(await Client.Repository.Content.GetRawContent(owner, repo, path));
        }

        public async Task<IEnumerable<string>> GetRepositories()
        {
            var repos = await Client.Repository.GetAllForCurrent();
            return repos.Select((x) => x.FullName );
        }
    }
}
