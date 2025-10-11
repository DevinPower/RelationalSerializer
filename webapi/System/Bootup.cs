using System.Diagnostics;
using webapi.DAL;
using webapi.Model;
using webapi.Utility;

namespace webapi
{
    public class Bootup
    {
        public static async Task Run(ConfigurationManager configuration)
        {
            ProjectManager.projects = new List<ProjectObject>();
            Settings.ConnectionString = configuration["ConnectionString"];

            Stopwatch projectTimer = new Stopwatch();
            projectTimer.Start();
            await LoadProjects();
            projectTimer.Stop();

            Stopwatch templateTimer = new Stopwatch();
            templateTimer.Start();
            List<string> templateGUIDs = await LoadTemplates();
            templateTimer.Stop();

            Stopwatch objectTimer = new Stopwatch();
            objectTimer.Start();
            await LoadObjects(templateGUIDs);
            objectTimer.Stop();

            ApplyTemplateModifiersToObjects();
            LoadSettings();

            string bootStats = string.Format("Load duration:\n\tProjects={0}\n\tTemplates={1}\n\tObjects={2}",
                projectTimer.Elapsed.TotalSeconds.ToString(),
                templateTimer.Elapsed.TotalSeconds.ToString(),
                objectTimer.Elapsed.TotalSeconds.ToString());

            Console.WriteLine(bootStats);

            Console.WriteLine("Established connection with database.");
        }

        static async Task LoadProjects()
        {
            List<(string guid, string name)> projects = await DBProjects.GetProjectsAsync();
            foreach((string guid, string name) project in projects)
            {
                ProjectObject projectObject = new ProjectObject(project.name, project.guid);
                ProjectManager.projects.Add(projectObject);
            }
        }

        static async Task<List<string>> LoadTemplates()
        {
            List<(string ProjectGUID, string ObjectGuid)> templates = await DBProjects.GetTemplatesAsync();
            foreach((string ProjectGUID, string ObjectGuid) template in templates)
            {
                CustomObject customObject = await DBProjects.GetTemplateModObjectsAsync(template.ObjectGuid);

                foreach(CustomField field in customObject.CustomFields)
                {
                    field.AvailableModifiers = AvailableModifierResolver.GetPotentialModifiers(customObject, field);
                    foreach(string name in field.Modifiers.Select(x => x.Name))
                    {
                        field.AvailableModifiers.RemoveAll(x => x.Name == name);
                    }
                }

                ProjectManager.projects.First(x => x.GUID == template.ProjectGUID).Templates.Add(customObject);
            }

            return templates.Select(x => x.ObjectGuid).ToList();
        }

        static async Task LoadObjects(List<string> filter)
        {
            List<CustomObjectMeta> values = await DBProjects.GetObjectGUIDsByProjectAsync();
            foreach(var meta in values)
            {
                if (filter.Contains(meta.GUID))
                    continue;
                CustomObject customObject = await DBProjects.GetObjectAsync(meta.GUID);
                customObject.HiddenFromNav = meta.IsHidden;
                customObject.ExcludeExport = meta.ExcludeExport;

                ProjectManager.projects.First(x => x.GUID == meta.Project).CustomObjects.Add(customObject);
            }
        }

        static void ApplyTemplateModifiersToObjects()
        {
            foreach(ProjectObject project in ProjectManager.projects)
            {
                CustomObject templateObject = project.Templates.First();

                foreach(CustomObject customObject in project.CustomObjects)
                {
                    foreach(CustomField field in customObject.CustomFields)
                    {
                        field.Modifiers = new List<Model.Modifiers.Modifier>();
                        field.Modifiers.AddRange(templateObject.CustomFields
                                                       .First(x => x.Name == field.Name).Modifiers);
                        field.Modifiers.ForEach(x => x.OnApply(customObject, field));
                    }
                }
            }
        }

        static void LoadSettings()
        {
            if (ProjectManager.projects.Where(x => x.Name == "!Settings").Count() == 0)
                CreateSettings();

            ProjectObject settingsProject = ProjectManager.projects.First(x => x.Name == "!Settings");

            if (settingsProject.CustomObjects.Count == 0)
                CreateSettingsObject(settingsProject);

            InstanceSettings.Singleton = settingsProject.CustomObjects.Last().ReflectIntoClass<InstanceSettings>();
        }

        static void CreateSettings()
        {
            CustomObject settingsObject = new CustomObject(new InstanceSettings());
            ProjectObject settingsProject = new ProjectObject("!Settings", new List<CustomObject>() { settingsObject });
            ProjectManager.AddProject(settingsProject);
            DBProjects.CreateProjectAsync(settingsProject);
        }

        static void CreateSettingsObject(ProjectObject SettingsProject)
        {
            CustomObject settingsObject = SettingsProject.Templates.First().Copy();
            SettingsProject.AddObject(settingsObject);
            DBProjects.UpsertObjectAsync(settingsObject.Copy(), SettingsProject.GUID);
        }
    }
}
