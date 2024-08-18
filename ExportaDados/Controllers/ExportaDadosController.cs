using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ExportaDados.Models;
using ExportaDados.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace ExportaDados.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExportaDados : ControllerBase
    {
        private readonly HttpClient _client;// = new HttpClient();

        public ExportaDados(HttpClient httpClient)
        {
            _client = httpClient;
        }

        [HttpPost]
        public string listar()
        {

            var reqNameList = new List<string> { "01.02.04.03 - Pedido de exames", "01.03 - GED S+", "01.07 - Tipo SOCGED" };
            var response = new Dictionary<string, XDocument>();

            var soupXML = XDocument.Load("exportaDados.xml");
            var listReq = new SoupReqService().listReq(soupXML) //somente as req que precisam ser feitas
                .Where(req => reqNameList.Contains(req.name))
                .ToList();

            foreach (var req in listReq)
            {
                var xmlResponse = SoupReqService.doRequest(req).Result;
                var responseName = Regex.Match(req.name, @"- (.+)").Groups[1].Value.ToLower();
                response[responseName] = xmlResponse; //ex: ["ged s+","pedido de exames","tipo socged"]
                
                
                System.Diagnostics.Debug.WriteLine($"\n===== RESPONSE {responseName} =====");
                System.Diagnostics.Debug.WriteLine(req.bodyXML);
                //System.Diagnostics.Debug.WriteLine(xmlResponse);

            }

            SoupReqService.fillUploadArquivos(response);
            //var requests = XDocument.Parse(xmlString)
            //response[]["ged s+","pedido de exames","tipo socged"]
            return "Deu certo";
        }
    }
}
