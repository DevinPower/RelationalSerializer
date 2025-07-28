using webapi.DAL;
using webapi.Model;
using webapi.Utility;

namespace webapi
{
    public class Bootup
    {
        public static void Run(ConfigurationManager configuration)
        {
            ProjectManager.projects = new List<ProjectObject>();
            Settings.ConnectionString = configuration["ConnectionString"];

            LoadProjects();
            List<string> templateGUIDs = LoadTemplates();
            LoadObjects(templateGUIDs);
            ApplyTemplateModifiersToObjects();
            LoadSettings();
        }

        static void LoadProjects()
        {
            List<(string guid, string name)> projects = DBProjects.GetProjects();
            foreach((string guid, string name) project in projects)
            {
                ProjectObject projectObject = new ProjectObject(project.name, project.guid);
                ProjectManager.projects.Add(projectObject);
            }
        }

        static List<string> LoadTemplates()
        {
            List<(string ProjectGUID, string ObjectGuid)> templates = DBProjects.GetTemplates();
            foreach((string ProjectGUID, string ObjectGuid) template in templates)
            {
                CustomObject customObject = DBProjects.GetTemplateModObjects(template.ObjectGuid);

                foreach(CustomField field in customObject.CustomFields)
                {
                    field.AvailableModifiers = AvailableModifierResolver.GetPotentialModifiers(customObject, field);
                    foreach(string name in field.Modifiers.Select(x => x.Name))
                    {
                        field.AvailableModifiers.RemoveAll(x => x.Name == name);
                    }
                }

                ProjectManager.projects.Where(x => x.GUID == template.ProjectGUID).First().Templates.Add(customObject);
            }

            return templates.Select(x => x.ObjectGuid).ToList();
        }

        static void LoadObjects(List<string> filter)
        {
            List<CustomObjectMeta> values = DBProjects.GetObjectGUIDsByProject();
            foreach(var meta in values)
            {
                if (filter.Contains(meta.GUID))
                    continue;
                CustomObject customObject = DBProjects.GetObject(meta.GUID);
                customObject.HiddenFromNav = meta.IsHidden;
                customObject.ExcludeExport = meta.ExcludeExport;

                ProjectManager.projects.Where(x => x.GUID == meta.Project).First().CustomObjects.Add(customObject);
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
                                                       .Where(x => x.Name == field.Name).First().Modifiers);
                        field.Modifiers.ForEach(x => x.OnApply(customObject, field));
                    }
                }
            }
        }

        static void LoadSettings()
        {
            if (ProjectManager.projects.Where(x => x.Name == "!Settings").Count() == 0)
                CreateSettings();

            ProjectObject settingsProject = ProjectManager.projects.Where(x => x.Name == "!Settings").First();

            if (settingsProject.CustomObjects.Count == 0)
                CreateSettingsObject(settingsProject);

            InstanceSettings.Singleton = settingsProject.CustomObjects.First().ReflectIntoClass<InstanceSettings>();
        }

        static void CreateSettings()
        {
            CustomObject settingsObject = new CustomObject(new InstanceSettings());
            ProjectObject settingsProject = new ProjectObject("!Settings", new List<CustomObject>() { settingsObject });
            ProjectManager.AddProject(settingsProject);
            DBProjects.CreateProject(settingsProject);
        }

        static void CreateSettingsObject(ProjectObject SettingsProject)
        {
            CustomObject settingsObject = SettingsProject.Templates.First().Copy();
            SettingsProject.AddObject(settingsObject);
            DBProjects.UpsertObject(settingsObject.Copy(), SettingsProject.GUID);
        }
    }
}
