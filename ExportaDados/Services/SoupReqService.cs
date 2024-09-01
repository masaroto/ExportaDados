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

        public List<SoupReq> listReq(IFormFile file)
        {
            //var pathSoupXml = "exportaDados.xml";
            var tempPath = Path.GetTempFileName();
            using (var stream = new FileStream(tempPath, FileMode.Create))
            {
                file.CopyToAsync(stream);

            }

            var soupXML = XDocument.Load(tempPath);
            return _xmlUtils.loadSoupXml(soupXML.ToString());
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
        public List<string> fillUploadArquivos(Dictionary<string, string> response) {
            var listXml = new List<string>();

            //System.Diagnostics.Debug.WriteLine($"\n===== pedidoExameJson =====");
            var pedidoJsonList = _xmlUtils.getValue(response["pedido de exames"], "retorno");//pega o json de dentro do XML
            var gedJsonList = _xmlUtils.getValue(response["ged s+"], "retorno");//pega o json de dentro do XML
            var list = _jsonUtils.loadList(pedidoJsonList, "CODIGOEMPRESA", "CODIGOFUNCIONARIO", "SEQUENCIAFICHA");
            var pedidoExameList = list.GroupBy(dict => dict["SEQUENCIAFICHA"])
                .Select(group => group.First())
                .ToList();

            foreach (var pedidoExame in pedidoExameList)
            {
                //retorna json do ultimo funcionario
                var xml = _xmlUtils.load("wwwroot/Upload-Arquivos.xml").ToString();
                var jsonGed = _jsonUtils.getLast(gedJsonList, "CODIGO_FUNCIONARIO", pedidoExame["CODIGOFUNCIONARIO"]);



                if (jsonGed == null) continue;
                var codGed = _jsonUtils.getValue(jsonGed, "CODIGO_GED");//acessa json e retorna o valor no campo

                System.Diagnostics.Debug.WriteLine("\nANTES:");
                System.Diagnostics.Debug.WriteLine(xml);
                _xmlUtils.setValue(ref xml, "codigoEmpresa", pedidoExame["CODIGOEMPRESA"]);
                _xmlUtils.setValue(ref xml, "codigoFuncionario", pedidoExame["CODIGOFUNCIONARIO"]);
                _xmlUtils.setValue(ref xml, "codigoGed", codGed);
                _xmlUtils.setValue(ref xml, "codigoSequencialFicha", pedidoExame["SEQUENCIAFICHA"]);
                System.Diagnostics.Debug.WriteLine("\nDEPOIS:");
                System.Diagnostics.Debug.WriteLine(xml);

                listXml.Add(xml);
            }
            listXml.Add(_xmlUtils.load("wwwroot/Upload-Arquivos.xml").ToString());//adiciona XML padrao

            return listXml;
        }
    }
}
