using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using webapi.Model;
using webapi.DAL;

namespace webapi.Utility
{
    public class ParsedClass
    {
        public string Name { get; private set; }
        public List<CustomField> CustomFields { get; private set; }

        public ParsedClass(string Name, List<CustomField> CustomFields)
        {
            this.Name = Name;
            this.CustomFields = CustomFields;
        }

        CustomObject GetObjectTemplate()
        {
            CustomObject objectTemplate = new CustomObject()
            {
                NameField = "name",
                CustomFields = this.CustomFields
            };

            objectTemplate.CustomFields.ForEach(x =>
            {
                x.LoadDefaultEditor();
                x.AvailableModifiers.AddRange(AvailableModifierResolver.GetPotentialModifiers(objectTemplate, x));
                foreach (var modifier in x.Modifiers)
                {
                    modifier.LoadUnderlyingObject(true);
                }
            });

            return objectTemplate;
        }

        public async Task AddAsNewProject()
        {
            CustomObject objectTemplate = GetObjectTemplate();

            List<CustomObject> templates = new List<CustomObject>() { objectTemplate };

            ProjectObject newProject = new ProjectObject(this.Name, templates);
            await DBProjects.CreateProjectAsync(newProject);

            ProjectManager.AddProject(newProject);
            await DBProjects.UpsertModsAsync(objectTemplate);
        }

        public void LoadIntoExistingProject(ProjectObject existingProject)
        {
            CustomObject objectTemplate = GetObjectTemplate();
            CustomObject originalTemplate = existingProject.Templates[0];

            new CSharpClassParser().UpdateObjectFields(existingProject, originalTemplate, objectTemplate);
        }
    }

    public class CSharpClassParser : iClassParser
    {
        public List<ParsedClass> GetTemplateClasses(string sourceCode)
        {
            List<ParsedClass> readClasses = new List<ParsedClass>();
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);

            var classes = syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (ClassDeclarationSyntax readClass in classes)
            {
                List<CustomField> parsedFields = new List<CustomField>();

                EnumDeclarationSyntax[] enums = readClass.DescendantNodes().OfType<EnumDeclarationSyntax>().ToArray();

                foreach (EnumDeclarationSyntax readEnum in enums)
                {
                    CustomEnum customEnum = new CustomEnum(readEnum.Identifier.ToString());
                    int i = 0;
                    foreach (EnumMemberDeclarationSyntax ms in readEnum.Members)
                    {
                        if (ms.EqualsValue != null)
                        {
                            i = (int.Parse(ms.EqualsValue.Value.ToString()));
                            customEnum.AddValue(ms.Identifier.ToString(), i);
                            i++;
                        }
                        else
                        {
                            customEnum.AddValue(ms.Identifier.ToString(), i++);
                        }
                    }
                    DBProjects.InsertEnumAsync(customEnum);
                }

                PropertyDeclarationSyntax[] properties = readClass.DescendantNodes()
                    .OfType<PropertyDeclarationSyntax>()
                    .Where(property => property.AccessorList != null &&
                       property.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.GetAccessorDeclaration) &&
                       property.AccessorList.Accessors.Any(a => a.Kind() == SyntaxKind.SetAccessorDeclaration))
                    .ToArray();

                if (readClass.BaseList != null)
                {
                    string clean = readClass.BaseList.ToString();
                    clean = clean.Replace(" ", "");
                    clean = clean.Replace(":", "");

                    //cc.dependencies.Add(clean);
                }

                foreach (PropertyDeclarationSyntax property in properties)
                {
                    TypeSyntax propertyType = property.Type;
                    string fieldType = propertyType.ToString();
                    string propertyName = property.Identifier.ToString();

                    bool isArray = false;
                    if (propertyType.GetFirstToken().ToString() == "List")
                    {
                        fieldType = propertyType.GetFirstToken().GetNextToken().GetNextToken().ToString();
                        isArray = true;
                    }

                    if (propertyType.GetLastToken().ToString() == "]")
                    {
                        fieldType = propertyType.GetFirstToken().ToString();
                        isArray = true;
                    }

                    CustomField newField;

                    if (TypeUtilities.IsEnum(fieldType).Result)
                    {
                        newField = new CustomField(propertyName, "enum", isArray);
                        newField.Modifiers.Add(new Model.Modifiers.ChoiceModifier() { EnumName = fieldType });
                    }
                    else
                    {
                        newField = new CustomField(propertyName, fieldType, isArray);
                    }

                    parsedFields.Add(newField);
                }

                readClasses.Add(new ParsedClass(readClass.Identifier.ToString(), parsedFields));
            }

            return readClasses;
        }

        /// <summary>
        /// Updates oldObject to match newObject's fields
        /// </summary>
        /// <param name="oldObject">This should be the object we have stored in the database</param>
        /// <param name="newObject">This should be what was parsed from an external source</param>
        public void UpdateObjectFields(ProjectObject project, CustomObject oldObject, CustomObject newObject)
        {
            List<CustomField> removedFields = oldObject.CustomFields
                .Where(x => newObject.CustomFields.Count(y => y.Name == x.Name) == 0)
                .ToList();

            List<CustomField> addedFields = newObject.CustomFields
                .Where(x => oldObject.CustomFields.Count(y => y.Name == x.Name) == 0)
                .ToList();

            List<CustomField> changedFields = oldObject.CustomFields
                .Where(x => newObject.CustomFields.Count(y => y.Name == x.Name && y.UnderlyingType != x.UnderlyingType) > 0)
                .ToList();

            foreach (CustomField field in removedFields)
                project.RemoveField(field.Name);

            foreach (CustomField field in addedFields)
                project.AddField(field);

            foreach (CustomField field in changedFields)
                project.ModifyType(field.Name, newObject.CustomFields.First(x => x.Name == field.Name).UnderlyingType);

            //probably need to update whether or not it's an array as well
        }
    }
}
