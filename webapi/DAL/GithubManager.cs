namespace webapi.DAL
{
    public class GithubManager
    {
        //TODO: Pull this out into a more generic place
        //      can reuse for other scenarios
        const string USER_AGENT = "RelSer";
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

        public static async Task<List<GithubItem>> GetRepoFolders(string Owner, string Repo, string Token, string Path = "")
        {
            var client = new Octokit.GitHubClient(new Octokit.ProductHeaderValue(USER_AGENT));
            if (!string.IsNullOrEmpty(Token))
            {
                client.Credentials = new Octokit.Credentials(Token);
            }

            var items = new List<GithubItem>();
            try
            {
                var contents = await client.Repository.Content.GetAllContents(Owner, Repo, Path ?? "");
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
    }
}
