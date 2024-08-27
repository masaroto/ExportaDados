using System.Reflection.Metadata;
using System.Text.Json.Nodes;
using System.Xml.Linq;
using ExportaDados.Interfaces;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ExportaDados.Helpers
{
    public class JsonUtils : IJsonUtils
    {
        private JArray _jsonList;
        private JObject _jsonObj;
        //public JObject _json;

        //retorna ultimo valor
        string IJsonUtils.getValue(string json, string attr)
        {
            _jsonObj = JObject.Parse(json);

            //checa se o campo esta presentes no json ou se o json é null
            if (_jsonObj == null || !_jsonObj.ContainsKey(attr))
            {
                //System.Diagnostics.Debug.WriteLine("Campo não está presente no JSON");
                return null; 
            }
            return _jsonObj[attr].ToString();
        }

        //Em uma lista de Json, retorna o ultimo json os parametros passados
        string IJsonUtils.getLast(string json, string attr, string value)
        {
            _jsonList = JArray.Parse(json);
            if (_jsonList == null)
            {
                return null;
            }

            return _jsonList.Children<JObject>()
                    .Where(json => (string)json[attr] == value)
                    .LastOrDefault()
                    .ToString();
        }

        public List<Dictionary<string, string>> loadList(string json, params string[] attrs)
        {
            var jarray = JArray.Parse(json);
            var list = new List<Dictionary<string, string>>();

            foreach ( var jsonItem in jarray)
            {
                var dict = new Dictionary<string, string>();
                foreach ( var attr in attrs )
                {
                    dict[attr] = jsonItem[attr].ToString();
                    //System.Diagnostics.Debug.WriteLine($"dict[{attr}] = {dict[attr]}");
                }
                list.Add(dict);
            }
            return list;
        }
    }
}
