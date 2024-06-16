using webapi.DAL;

namespace webapi.Model.Modifiers
{
    public class InlineModifier : Modifier
    {
        public string ProjectName { get; set; }

        public override bool CanTargetProperty(CustomObject owner, CustomField field)
        {
            return field.IsReference;
        }

        public override void OnApply(CustomObject owner, CustomField field)
        {
        }

        public override void OnRemove(CustomObject owner, CustomField field)
        {
            //TODO: delete the object(s) tied to inline guid
        }

        public override void OnRender(RenderField field)
        {
            int? project = ProjectManager.GetProjectIndexByName(ProjectName);
            if (project == null)
            {
                field.RenderComponent = "error";
                return;
            }    

            field.RenderComponent = "FC_InlineReference";
            field.AdditionalData = new { InlineProject =  project.Value };
        }
    }
}
