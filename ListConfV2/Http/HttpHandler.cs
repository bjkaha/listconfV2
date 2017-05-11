using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ListConfV2.Http
{
    static class HttpHandler
    {
        public static string Token { get; set; } = null;
        public static string Db { get; set; } = null;
        public static Uri BaseUri { get; set; } = new Uri("https://dev.mxdeposit.net");
        public static string Email { get; set; } = null;
        public static string Password { get; set; } = null;
        /*
         * 
         *  Initiating HTTP Connection through login API 
         */
        public static async Task<bool> Init(Uri baseuri, string email, string pw)
        {        
            Dictionary<string, string> auth = new Dictionary<string, string>
            {
                { "email", email }, { "password", pw }
            };
            try
            {
                HttpResponseMessage res = await Request(HttpMethod.Post, "/login", JsonConvert.SerializeObject(auth));
                var response = JsonConvert.DeserializeObject<LoginResponse>(await res.Content.ReadAsStringAsync());
                Token = response.token;
                Db = (response.databases.Count == 0) ? response._id : response.databases[0];
                BaseUri = baseuri;
                Email = email;
                Password = pw;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
         * 
         *  General HTTP request method
         */
        public static async Task<HttpResponseMessage> Request(HttpMethod method, string subUrl, string json) 
        {
            using (HttpClient client = new HttpClient { BaseAddress = BaseUri })
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
                if (Token != null) client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", Token);
                if (Db != null) client.DefaultRequestHeaders.TryAddWithoutValidation("database", Db);
                try
                {
                    HttpRequestMessage msg = new HttpRequestMessage(method, subUrl);
                    if (json != null)
                        msg.Content = new StringContent(json, Encoding.UTF8, "application/json");
                    var res = await client.SendAsync(msg);
                    if (res.StatusCode != HttpStatusCode.OK && res.StatusCode != HttpStatusCode.Created)
                    {
                        throw new Exception(res.StatusCode.ToString() + " ( Http failed )");
                    }
                    return res;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        class LoginResponse
        {
            public string _id { get; set; }
            public List<string> databases { get; set; }
            public string token { get; set; }
        }
    }
}
