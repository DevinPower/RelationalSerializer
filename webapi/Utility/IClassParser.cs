using webapi.Model;

namespace webapi.Utility
{
    public interface IClassParser
    {
        List<ParsedClass> GetTemplateClasses(string sourceCode);
    }
}
