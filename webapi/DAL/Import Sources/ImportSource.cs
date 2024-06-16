namespace webapi.DAL.ImportSources
{
    public class ImportSource
    {
        public virtual bool Authenticate()
        {
            throw new NotSupportedException("Base ImportSource not supported");
        }

        public virtual string GetData(string source)
        {
            throw new NotSupportedException("Base ImportSource not supported");
        }
    }
}
