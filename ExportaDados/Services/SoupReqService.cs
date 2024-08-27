using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using ExportaDados.Helpers;
using ExportaDados.Interfaces;
using ExportaDados.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExportaDados.Services
{
    public class SoupReqService
    {
        private readonly HttpClient _client;
        private readonly IJsonUtils _jsonUtils;
        private readonly IXmlUtils _xmlUtils;

        public SoupReqService(HttpClient client, IJsonUtils json, IXmlUtils xmlUtils) {  
            _client = client;
            _jsonUtils = json;
            _xmlUtils = xmlUtils;
        }

        public List<SoupReq> listReq(XDocument soupXML)
        {
            var pathSoupXml = "exportaDados.xml";
            
            //foreach (var i in reqList)
            //{
            //    System.Diagnostics.Debug.WriteLine($"\n====================== {i.name} =======================\n");
            //    System.Diagnostics.Debug.WriteLine(SoupReq.endpoint);
            //    System.Diagnostics.Debug.WriteLine(i.bodyXML);
            //}
            return _xmlUtils.loadSoupXml(pathSoupXml);
        }

        public async Task<string> doRequest(SoupReq req)
        {
            var content = new StringContent(req.bodyXML, Encoding.UTF8, "application/xml");
            var response = await _client.PostAsync(SoupReq.endpoint, content);  //sempre vai ser POST
            var xmlResponse = response.Content.ReadAsStringAsync().Result;

            //System.Diagnostics.Debug.WriteLine("===== RESPONSE =====");
            //System.Diagnostics.Debug.WriteLine(xmlResponse);

            return xmlResponse;
        }
        
        //preenche XML de resposta
        public XDocument fillUploadArquivos(Dictionary<string, string> response) {
            //var uploadArqXML = XDocument.Load("wwwroot/Upload-Arquivos.xml");
            //var root = uploadArqXML.Root.Descendants("arg0").FirstOrDefault();

            var pedidoExame = response["pedido de exames"];
            var gedMais = response["ged s+"];
            var socGed = response["tipo socged"];

            System.Diagnostics.Debug.WriteLine($"\n===== pedidoExameJson =====");
            var pedidoJsonList = _xmlUtils.getValue(response["pedido de exames"], "retorno");//pega o json de dentro do XML
            var list = _jsonUtils.loadList(pedidoJsonList, "CODIGOEMPRESA", "CODIGOFUNCIONARIO", "SEQUENCIAFICHA");
            var linqList = list.GroupBy(dict => dict["SEQUENCIAFICHA"])
                .Select(group => group.First())
                .ToList();

            //System.Diagnostics.Debug.WriteLine(a.Count());
            //System.Diagnostics.Debug.WriteLine(x);


            //foreach (var i in a)
            //{
            //    //var i = k.FirstOrDefault();
            //    System.Diagnostics.Debug.WriteLine($"Empresa:{i["CODIGOEMPRESA"]}, CODIGOFUNCIONARIO: {i["CODIGOFUNCIONARIO"]}, SEQUENCIAFICHA: {i["SEQUENCIAFICHA"]}");
            //}

            //var x = a.GroupBy(i => i["SEQUENCIAFICHA"])
            //    .Any(i => i.Count() > 1);

            //var a = _jsonUtils.getLast(pedidoExameJson, "CODIGOFUNCIONARIO", "352084");
            //a = _jsonUtils.getValue(a, "SEQUENCIAFICHA");



            //var pedidoExameJson = this.getJsonList(pedidoExame, "retorno");
            //var gedMaisJson = this.getJsonList(gedMais, "retorno", "CODIGO_GED");
            //var socGedJson = this.getJsonList(socGed, "retorno", "CODIGO");
            //if (x == null) return null;


            //var codigoEmpresa = root.Element("codigoEmpresa");
            //var codigoFuncionario = root.Element("codigoFuncionario");
            //System.Diagnostics.Debug.WriteLine(codigoEmpresa);
            //System.Diagnostics.Debug.WriteLine(codigoFuncionario);
            //root.Element("codigoEmpresa").Value = "";
            //root.Element("codigoFuncionario");


            return new XDocument();
        }
    }
}
