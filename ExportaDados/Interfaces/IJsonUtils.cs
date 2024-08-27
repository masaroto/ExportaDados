using Newtonsoft.Json.Linq;

namespace ExportaDados.Interfaces
{
    public interface IJsonUtils
    {
        string getValue(string json, string attr);
        string getLast(string json, string attr, string value);
        List<Dictionary<string, string>> loadList(string json, params string[] attrs);
    }
}
