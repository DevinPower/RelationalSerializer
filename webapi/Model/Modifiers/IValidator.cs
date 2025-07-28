namespace webapi.Model.Modifiers
{
    public interface IValidator
    {
        public bool Validate(CustomField field, object oldValue);
    }
}
