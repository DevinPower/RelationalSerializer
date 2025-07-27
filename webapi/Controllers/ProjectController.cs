using Microsoft.AspNetCore.Mvc;
using System.Text;
using webapi.DAL;
using webapi.DAL.ImportSources;
using webapi.Model;
using webapi.Model.Modifiers;
using webapi.Utility;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class ProjectController : ControllerBase
{
    private readonly ILogger<ProjectController> _logger;

    public ProjectController(ILogger<ProjectController> logger)
    {
        _logger = logger;
    }

    [HttpGet, Route("/project/{id:int}/objects")]
    public IActionResult GetObjects(int id)
    {
        if (id >= ProjectManager.projects.Count)
            return BadRequest("Project out of index");

        return Ok(ProjectManager.projects[id].CustomObjects
            .Where(x=>!x.HiddenFromNav)
            .Select(x => new NavModel(x.GetDisplayName(), x.GUID, x.ExcludeExport)));
    }

    [HttpGet, Route("/project/{id:int}/name")]
    public IActionResult GetProjectName(int id)
    {
        if (id >= ProjectManager.projects.Count)
            return BadRequest("Project out of index");

        return Ok(ProjectManager.projects[id].Name);
    }

    [HttpGet, Route("/project/count")]
    public IActionResult GetProjectsCount()
    {
        return Ok(ProjectManager.projects.Count);
    }

    [HttpGet, Route("/project")]
    public IActionResult GetProjects()
    {
        return Ok(ProjectManager.projects.Select(x=>new ProjectModel(x)));
    }

    [HttpPut, Route("/project/create")]
    public IActionResult CreateProject([FromBody] string source)
    {
        CSharpClassParser parser = new CSharpClassParser();
        List<ParsedClass> classes = parser.GetTemplateClasses(source);

        List<string> createdProjects = new List<string>();

        foreach (ParsedClass NewClass in classes)
        {
            if (ProjectManager.projects.Count(x => x.Name == NewClass.Name) > 0)
            {
                NewClass.LoadIntoExistingProject(ProjectManager.projects.First(x => x.Name == NewClass.Name));
            }
            else
            {
                NewClass.AddAsNewProject();
                createdProjects.Add(NewClass.Name);
            }
        }

        return Ok(createdProjects);
    }

    [HttpPut, Route("/project/{id:int}/objects")]
    public IActionResult CreateObject(int id)
    {
        CustomObject newObject = ProjectManager.projects[id].Templates[0].Copy();

        ProjectManager.projects[id].CreateObject(newObject, ProjectManager.projects[id].GUID);

        return Ok();
    }

    [HttpPut, Route("/project/import")]
    public IActionResult ImportProject(string url)
    {
        ImportSource source = new GithubSource();

        source.Authenticate();
        IActionResult returnedValue = CreateProject(source.GetData(url));

        List<string> newProjects = ((OkObjectResult)returnedValue).Value as List<string>;
        foreach(string project in newProjects)
            DBProjects.InsertSource(project, "Github", url);

        return returnedValue;
    }

    [HttpPut, Route("/project/{projectName}/reimport")]
    public IActionResult Reimport(string projectName)
    {
        string sourceURL = DBProjects.GetSourceByName(projectName);
        ImportProject(sourceURL);
        return Ok();
    }

    [HttpPost, Route("/project/ALL/reimport")]
    public IActionResult ReimportAll()
    {
        foreach (var project in DBProjects.GetProjects())
            ImportProject(DBProjects.GetSourceByName(project.Name));

        return Ok();
    }

    [HttpDelete, Route("/project/{guid}/delete")]
    public IActionResult DeleteProject(string guid)
    {
        ProjectManager.DeleteProject(guid);
        return Ok();
    }

    [HttpGet, Route("/export")]
    public IActionResult Export()
    {
        List<string> resolvedGUIDs = new List<string>();
        StringBuilder export = new StringBuilder();
        export.Append("{");
        foreach (ProjectObject project in ProjectManager.projects)
        {
            if (project.Name == "!Settings")
                continue;

            project.Export(export, resolvedGUIDs);
            if (project != ProjectManager.projects.Last())
                export.Append(',');
        }
        export.Append("}");

        return Ok(export.ToString());
    }

    [HttpGet, Route("/project/importable")]
    public IActionResult GetImportable(string? Path)
    {
        string Token = InstanceSettings.Singleton.GithubAPIKey;
        string Owner = InstanceSettings.Singleton.GithubRepository.Split('/')[0];
        string Repo = InstanceSettings.Singleton.GithubRepository.Split('/')[1];

        return new OkObjectResult(new GithubManager(Token).GetRepoFolders(Owner, Repo, Path).Result);
    }
}
