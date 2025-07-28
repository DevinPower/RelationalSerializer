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

    public RepositoryIndexModel(int id, string name)
    {
        this.Id = id;
        this.Name = name;
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
    public async Task<IActionResult> CheckAPIAccess([FromBody]string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey))
            return new BadRequestObjectResult($"APIKey cannot be empty.");

        try
        {
            GithubManager githubManager = new GithubManager(apiKey);
            var repositoryNames = await githubManager.GetRepositories();

            if (!repositoryNames.Any())
                return new BadRequestObjectResult("No repositories found.");
        }
        catch (AggregateException)
        {
            return new BadRequestObjectResult("Bad credentials provided.");
        }

        //TODO: Refactor this
        InstanceSettings.Singleton.GithubAPIKey = apiKey;

        ProjectObject SettingsProject = ProjectManager.projects.First(x => x.Name == "!Settings");
        CustomObject settingsObject = SettingsProject.CustomObjects[0];
        settingsObject.SetField("GithubAPIKey", apiKey);
        await DBProjects.UpsertObjectAsync(settingsObject, SettingsProject.GUID);

        return new OkResult();
    }

    [HttpGet, Route("/onboard/listrepositories")]
    public async Task<IActionResult> ListRepositories()
    {
        GithubManager githubManager = new GithubManager(InstanceSettings.Singleton.GithubAPIKey);
        int id = 1;
        return new OkObjectResult((await githubManager.GetRepositories())
            .Select(x =>
        {
            return new RepositoryIndexModel(id++, x);
        }));
    }

    [HttpPatch, Route("/onboard/repo")]
    public async Task<IActionResult> SetWorkingRepository([FromBody]string repoName)
    {
        //TODO: Note that repoName is {Owner}/{Repository}. We should break this up here before it reaches downstream.

        //TODO: Refactor this
        InstanceSettings.Singleton.GithubRepository = repoName;

        ProjectObject SettingsProject = ProjectManager.projects.First(x => x.Name == "!Settings");
        CustomObject settingsObject = SettingsProject.CustomObjects[0];
        settingsObject.SetField("GithubRepository", repoName);
        await DBProjects.UpsertObjectAsync(settingsObject, SettingsProject.GUID);

        return new OkResult();
    }

    [HttpGet, Route("/onboard")]
    public IActionResult NeedsOnboarding()
    {
        return new OkObjectResult(string.IsNullOrEmpty(InstanceSettings.Singleton.GithubRepository));
    }
}
