using Microsoft.AspNetCore.Mvc;
using System.Text;
using Octokit;
using webapi.DAL;
using webapi.DAL.ImportSources;
using webapi.Model;
using webapi.Model.Modifiers;
using webapi.Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.DataProtection.KeyManagement;

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

    [HttpPost, Route("/onboard/validate")]
    public IActionResult CheckAPIAccess([FromBody]string APIKey)
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

    [HttpPatch, Route("/onboard/repo")]
    public IActionResult SetWorkingRepository([FromBody]string RepoName)
    {
        //TODO: Note that RepoName is {Owner}/{Repository}. We should break this up here before it reaches downstream.

        //TODO: Refactor this
        InstanceSettings.Singleton.GithubRepository = RepoName;

        ProjectObject SettingsProject = ProjectManager.projects.Where(x => x.Name == "!Settings").First();
        CustomObject settingsObject = SettingsProject.CustomObjects[0];
        settingsObject.SetField("GithubRepository", RepoName);
        DBProjects.UpsertObject(settingsObject, SettingsProject.GUID);

        return new OkResult();
    }

    [HttpGet, Route("/onboard")]
    public IActionResult NeedsOnboarding()
    {
        return new OkObjectResult(string.IsNullOrEmpty(InstanceSettings.Singleton.GithubRepository));
    }
}
