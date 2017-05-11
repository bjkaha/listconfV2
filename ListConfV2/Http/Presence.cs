using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ListConfV2.Http
{
    static class Presence
    {
        public static Dictionary<string, string> Categories = new Dictionary<string, string>();
        public static Dictionary<string, string> Lists = new Dictionary<string, string>();

        public static string GetCategory(string name)
        {
            if (!Categories.Keys.Contains(name)) return Categories[name];
            else return null;
        }

        public static string GetList(string name)
        {
            if (Lists.Keys.Contains(name)) return Lists[name];
            else return null;
        }

        public static void AddCategory(string name, string id)
        {
            Categories.Add(name, id);
        }

        public static void AddList(string name, string id)
        {
            Lists.Add(name, id);
        }

        public async static Task<bool> Init()
        {
            try
            {
                var res = await HttpHandler.Request(HttpMethod.Get, "/list-categories/", null);
                string json = await res.Content.ReadAsStringAsync();
                JObject obj = JObject.Parse(json);
                foreach (var cat in obj["ListCategory"].Children())
                {
                    Categories.Add((string)cat["name"], (string)cat["_id"]);
                }
                res = await HttpHandler.Request(HttpMethod.Get, "/lists/", null);
                json = await res.Content.ReadAsStringAsync();
                obj = JObject.Parse(json);
                foreach (var lst in obj["List"].Children())
                {
                    Lists.Add((string)lst["name"], (string)lst["_id"]);
                }
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
