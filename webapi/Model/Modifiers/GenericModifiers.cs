namespace webapi.Model.Modifiers
{
    public class ReadOnlyModifier : Modifier
    {
        public override bool CanTargetProperty(CustomObject owner, CustomField field)
        {
            return true;
        }

        public override void OnApply(CustomObject owner, CustomField field)
        {
            throw new NotImplementedException("ReadOnly modifier not implemented");
        }

        public override void OnRemove(CustomObject owner, CustomField field)
        {
        }

        public override void OnRender(RenderField field)
        {
        }
    }

    public class JSONIgnoreModifier : Modifier
    {
        public override bool CanTargetProperty(CustomObject owner, CustomField field)
        {
            return true;
        }

        public override void OnApply(CustomObject owner, CustomField field)
        {
            throw new NotImplementedException("JSONIgnore modifier not implemented");
        }

        public override void OnRemove(CustomObject owner, CustomField field)
        {
        }

        public override void OnRender(RenderField field)
        {
        }
    }
}
