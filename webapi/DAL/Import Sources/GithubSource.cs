using webapi.Model;

namespace webapi.DAL.ImportSources
{
    //TODO: Refactor to use octokit instead of httprequest
    public class GithubSource : ImportSource
    {
        public override bool Authenticate()
        {
            return true;
        }

        public override string GetData(string FilePath)
        {
            string Token = InstanceSettings.Singleton.GithubAPIKey;
            string Owner = InstanceSettings.Singleton.GithubRepository.Split('/')[0];
            string Repo = InstanceSettings.Singleton.GithubRepository.Split('/')[1];

            GithubManager ghm = new GithubManager(Token);
            return ghm.GetFileContent(Owner, Repo, FilePath).Result;
        }
    }
}