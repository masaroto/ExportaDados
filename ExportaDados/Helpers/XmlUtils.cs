using System.Xml.Linq;
using ExportaDados.Interfaces;
using ExportaDados.Models;

namespace ExportaDados.Helpers
{
    public class XmlUtils : IXmlUtils
    {

        public XDocument load(string xml)
        {
            if (xml.EndsWith(".xml")) return XDocument.Load(xml);

            return XDocument.Parse(xml);
        }

        public List<SoupReq> loadSoupXml(string xml)
        {
            var soupXML = this.load(xml);
            XNamespace con = "http://eviware.com/soapui/config";
            var root = soupXML.Root.Descendants(con + "call");// <con:call

            SoupReq.endpoint = root.FirstOrDefault()
                                .Element(con + "endpoint").Value;

            var reqList = root.Select(call => new SoupReq
            {
                name = call.Attribute("name").Value,
                bodyXML = call.Element(con + "request").Value,
                method = "POST"
            }).ToList();

            return reqList;
        }
        public string getValue(string xml, string tag)
        {
            var root = this.load(xml);
            return root.Descendants(tag).FirstOrDefault().Value;

        }

        public void setValue(ref string xml, string tag, string value)
        {
            var root = this.load(xml);
            root.Descendants(tag).FirstOrDefault().Value = value;
            xml = root.ToString();
        }
    }
}
