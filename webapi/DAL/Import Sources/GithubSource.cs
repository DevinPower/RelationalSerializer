using webapi.Model;

namespace webapi.DAL.ImportSources
{
    public class GithubSource : ImportSource
    {
        string _token;
        public override bool Authenticate()
        {
            _token = InstanceSettings.Singleton.GithubAPIKey;
            return true;
        }

        public override string GetData(string source)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), source))
                {
                    request.Headers.TryAddWithoutValidation("Authorization", "token " + _token);
                    var response = httpClient.SendAsync(request).Result;

                    return response.Content.ReadAsStringAsync().Result;
                }
            }
        }
    }
}