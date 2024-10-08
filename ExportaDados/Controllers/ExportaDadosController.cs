using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ExportaDados.Models;
using ExportaDados.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExportaDados.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExportaDados : ControllerBase
    {
        private readonly SoupReqService _soupReq;

        public ExportaDados(SoupReqService soupReq)
        {
            //_client = httpClient;
            _soupReq = soupReq;
        }

        [HttpPost]
        //[Consumes("application/xml")]
        public IActionResult listaExames(IFormFile file)
        {
            
            var reqNameList = new List<string> { "01.02.04.03 - Pedido de exames", "01.03 - GED S+", "01.07 - Tipo SOCGED" };
            var response = new Dictionary<string, string>();//reponse["pedido exame"] = xml



            var listReq = _soupReq.listReq(file) //somente as req que precisam ser feitas
                .Where(req => reqNameList.Contains(req.name))
                .ToList();

            foreach (var req in listReq)
            {
                var xmlResponse = _soupReq.doRequest(req).Result;
                //regex: 01.02.04.03 - Pedido de exames -> pedido de exames
                var responseName = Regex.Match(req.name, @"- (.+)").Groups[1].Value.ToLower();
                response[responseName] = xmlResponse; //["ged s+"],["pedido de exames"],["tipo socged"]
                //_soupReq.getJson(req.bodyXML, "retorno", "CODIGOEMPRESA", "CODIGOFUNCIONARIO", "SEQUENCIAFICHA");
                System.Diagnostics.Debug.WriteLine($"\n===== RESPONSE {responseName} =====");
                System.Diagnostics.Debug.WriteLine(req.bodyXML);
            }

            var ret = _soupReq.fillUploadArquivos(response);
            if (ret == null) return BadRequest("Verifique XML de entrada.");

            return Ok(ret);
        }
    }
}
