namespace webapi.Model.Modifiers
{
    public interface iValidator
    {
        public bool Validate(CustomField field, object oldValue);
    }
}
