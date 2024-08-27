using System.Xml.Linq;
using ExportaDados.Models;

namespace ExportaDados.Interfaces
{
    public interface IXmlUtils
    {
        List<SoupReq> loadSoupXml(string xml);
        string getValue(string xml, string tag);

        XDocument load(string xml);

        void setValue(ref string xml, string tag, string value);
        //string getJson(string xml, string tag);
    }
}
