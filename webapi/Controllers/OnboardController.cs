using Microsoft.AspNetCore.Mvc;
using System.Text;
using Octokit;
using webapi.DAL;
using webapi.DAL.ImportSources;
using webapi.Model;
using webapi.Model.Modifiers;
using webapi.Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace webapi.Controllers;

public class RepositoryIndexModel
{
    public int Id { get; set; }
    public string Name { get; set; }

    public RepositoryIndexModel(int Id, string Name)
    {
        this.Id = Id;
        this.Name = Name;
    }
}

[ApiController]
[Route("[controller]")]
public class OnboardController : ControllerBase
{
    private readonly ILogger<OnboardController> _logger;

    public OnboardController(ILogger<OnboardController> logger)
    {
        _logger = logger;
    }

    [HttpPost, Route("/onboard/repo")]
    public IActionResult GetObjects([FromBody]string APIKey)
    {
        if (string.IsNullOrEmpty(APIKey))
            return new BadRequestObjectResult($"APIKey cannot be empty.");

        try
        {
            GithubManager githubManager = new GithubManager(APIKey);
            var repositoryNames = githubManager.GetRepositories().Result;

            if (repositoryNames.Count() == 0)
                return new BadRequestObjectResult("No repositories found.");
        }
        catch (AggregateException)
        {
            return new BadRequestObjectResult("Bad credentials provided.");
        }

        //TODO: Refactor this
        InstanceSettings.Singleton.GithubAPIKey = APIKey;

        ProjectObject SettingsProject = ProjectManager.projects.Where(x => x.Name == "!Settings").First();
        CustomObject settingsObject = SettingsProject.CustomObjects[0];
        settingsObject.SetField("GithubAPIKey", APIKey);
        DBProjects.UpsertObject(settingsObject, SettingsProject.GUID);

        return new OkResult();
    }

    [HttpGet, Route("/onboard/listrepositories")]
    public IActionResult ListRepositories()
    {
        GithubManager githubManager = new GithubManager(InstanceSettings.Singleton.GithubAPIKey);
        int id = 1;
        return new OkObjectResult(githubManager.GetRepositories().Result.Select(x =>
        {
            return new RepositoryIndexModel(id++, x);
        }));
    }
}
