using Octokit;

namespace webapi.DAL
{
    public class GithubItem
    {
        public string Name { get; }
        public string Type { get; }

        public GithubItem(string Name, string Type)
        {
            this.Name = Name;
            this.Type = Type;
        }
    }

    public class GithubManager
    {
        //TODO: Pull this out into a more generic place
        //      can reuse for other scenarios
        const string USER_AGENT = "RelSer";
        private string Token;
        private GitHubClient _client;

        GitHubClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new GitHubClient(new ProductHeaderValue(USER_AGENT));
                    _client.Credentials = new Credentials(Token);
                }

                return _client;
            }
        }

        public GithubManager(string Token)
        {
            this.Token = Token;
        }

        public async Task<List<GithubItem>> GetRepoFolders(string Owner, string Repo, string Path)
        {
            var items = new List<GithubItem>();
            try
            {
                var contents = await Client.Repository.Content.GetAllContents(Owner, Repo, Path);
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

        public async Task<IEnumerable<string>> GetRepositories()
        {
            var repos = await Client.Repository.GetAllForCurrent();
            return repos.Select((x) => x.FullName );
        }
    }
}
