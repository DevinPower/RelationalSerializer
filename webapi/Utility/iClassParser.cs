using webapi.Model;

namespace webapi.Utility
{
    public interface iClassParser
    {
        List<ParsedClass> GetTemplateClasses(string sourceCode);
    }
}
