using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using webapi.DAL;
using webapi.Model;
using webapi.Model.Modifiers;

namespace webapi.Controllers;

[ApiController]
[Route("[controller]")]
public class ObjectController : ControllerBase
{
    private readonly ILogger<ProjectController> _logger;

    public ObjectController(ILogger<ProjectController> logger)
    {
        _logger = logger;
    }

    [HttpGet, Route("/object/{project:int}/{id}")]
    public IActionResult GetObject(int project, string id)
    {
        RenderObject renderObject = new RenderObject(ProjectManager.projects[project].CustomObjects
            .Where(x => x.GUID == id).First());

        return Ok(renderObject);
    }

    [HttpPut, Route("/object/{project:int}/{guid}/duplicate")]
    public IActionResult CopyObject(int project, string guid)
    {
        CustomObject customObject = ProjectManager.projects[project].CustomObjects
            .Where(x => x.GUID == guid).First().Copy();

        ProjectManager.projects[project].CustomObjects.Add(customObject);
        DBProjects.UpsertObject(customObject, ProjectManager.projects[project].GUID);
        return Ok();
    }

    [HttpDelete, Route("/object/{project:int}/{guid}/delete")]
    public IActionResult DeleteObject(int project, string guid)
    {
        ProjectManager.projects[project].DeleteObject(guid);
        return Ok();
    }

    [HttpPatch, Route("/object/{project:int}/{guid}/exporttoggle")]
    public IActionResult SetExportSetting(int project, string guid)
    {
        CustomObject customObject = ProjectManager.projects[project].CustomObjects.Where(x => x.GUID == guid).First();
        customObject.ExcludeExport = !customObject.ExcludeExport;
        DBProjects.SetExportExclude(guid, customObject.ExcludeExport);
        return Ok();
    }

    [HttpGet, Route("/object/{project:int}/template")]
    public IActionResult GetTemplate(int project)
    {
        return Ok(new RenderModifierObject(ProjectManager.projects[project].Templates[0]));
    }

    [HttpPatch, Route("/object/{project:int}/template")]
    public IActionResult SetTemplate(int project, int templateNumber, [FromBody] List<JObject> modifierDynamic)
    {
        CustomObject customObject = ProjectManager.projects[project].Templates[0];
        foreach (CustomField customField in customObject.CustomFields)
        {
            JToken modifierField = modifierDynamic.Cast<JToken>().Where(x => x["name"].Value<string>() == customField.Name).First();
            JArray activeModifiers = modifierField["activeModifiers"].ToObject<JArray>();
            JArray inactiveModifiers = modifierField["availableModifiers"].ToObject<JArray>();

            if (customField.Modifiers == null)
                customField.Modifiers = new List<Modifier>();

            //Remove modifiers that are no longer active
            for (int i = 0; i < customField.Modifiers.Count; i++)
            {
                Modifier internalActive = customField.Modifiers[i];
                if (activeModifiers.Count(x => ((JObject)x)["name"].Value<string>() == internalActive.Name) > 0)
                {
                    internalActive.OnRemove(customObject, customField);
                    customField.Modifiers.Remove(internalActive);
                    customField.AvailableModifiers.Add(internalActive);
                    i--;
                }
                else
                {
                    var dictionary = activeModifiers[internalActive.Name].ToObject<Dictionary<string, object>>();
                    //internalActive.BuildModifierFromUnderlyingObject(
                    //    activeModifiers[internalActive.Name].UnderlyingObject);
                }
            }

            if (customField.AvailableModifiers == null)
                customField.AvailableModifiers = new List<Modifier>();

            //Add modifiers that are now active
            for (int i = 0; i < customField.AvailableModifiers.Count; i++)
            {
                if (activeModifiers == null)
                    break;

                Modifier internalInactive = customField.AvailableModifiers[i];
                if (activeModifiers.Count(x => ((JObject)x)["name"].Value<string>() == internalInactive.Name) > 0)
                {
                    var modifier = activeModifiers
                        .Select(x => (JObject)x)
                        .First(jObj => jObj["name"].Value<string>() == internalInactive.Name);

                    var properties = modifier["underlyingObject"]["properties"]
                        .Children<JObject>()
                        .ToDictionary(prop => prop["name"].ToString(), prop => prop["value"].ToObject<object>());

                    internalInactive.BuildModifierFromUnderlyingObject(properties);

                    internalInactive.OnApply(customObject, customField);
                    customField.AvailableModifiers.Remove(internalInactive);
                    customField.Modifiers.Add(internalInactive);
                    i--;
                    if (customField.AvailableModifiers.Count == 0)
                        break;
                }
                else
                {
                    //internalActive.BuildModifierFromUnderlyingObject(
                    //    activeModifiers[internalActive.Name].UnderlyingObject);
                }
            }
        }

        DBProjects.UpsertMods(customObject);

        return Ok();
    }
}
