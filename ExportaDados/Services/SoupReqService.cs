using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Xml.Linq;
using ExportaDados.Models;

namespace ExportaDados.Services
{
    public class SoupReqService
    {
        public List<SoupReq> listReq(XDocument soupXML)
        {
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

            //foreach (var i in reqList)
            //{
            //    System.Diagnostics.Debug.WriteLine($"\n====================== {i.name} =======================\n");
            //    System.Diagnostics.Debug.WriteLine(SoupReq.endpoint);
            //    System.Diagnostics.Debug.WriteLine(i.bodyXML);
            //}

                //List <SoupReq> a = new List <SoupReq>();
            return reqList;
        }

        static public async Task<XDocument> doRequest(SoupReq req)
        {
            var _client = new HttpClient();
            var content = new StringContent(req.bodyXML, Encoding.UTF8, "application/xml");
            var response = await _client.PostAsync(SoupReq.endpoint, content);  //sempre vai ser POST
            var xmlResponse = response.Content.ReadAsStringAsync().Result;

            //System.Diagnostics.Debug.WriteLine("===== RESPONSE 2 =====");
            //System.Diagnostics.Debug.WriteLine(xmlResponse);

            return XDocument.Parse(xmlResponse);
        }

        static public XDocument fillUploadArquivos(Dictionary<string, XDocument> response) {
            var uploadArqXML = XDocument.Load("wwwroot/Upload-Arquivos.xml");
            var root = uploadArqXML.Root.Descendants("arg0").FirstOrDefault();

            var pedidoExame = response["pedido de exames"].Root.Descendants("return").FirstOrDefault();
            System.Diagnostics.Debug.WriteLine(pedidoExame);


            //var codigoEmpresa = root.Element("codigoEmpresa");
            //var codigoFuncionario = root.Element("codigoFuncionario");
            //root.Element("codigoEmpresa").Value = "";
            //root.Element("codigoFuncionario");


            return new XDocument();
        }
    }
}
