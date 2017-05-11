using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ListConfV2.Http
{
    class ListObj
    {
        public Boolean IsEmpty { get; set; } = true;
        public string Name { get; set; }
        public string Id { get; set; }
        public string PrimaryId { get; set; }
        public string DescId { get; set; }
        public string InUseId { get; set; }
        public string FirstRow { get; set; }
        public ArrayList Columns = new ArrayList();

        /*
         * Constructing new list
         */
        public ListObj(string listname, string desc, string categoryname)
        {
            string name = listname;
            Dictionary<string, string> body;
            HttpResponseMessage res;
            try
            {
                // name and category name are required
                if (name == null || categoryname == null)
                {
                    throw new Exception("Name or Category cannot be null");
                }

                // if list name exists, use a different name
                int iter = 1;
                while (true)
                {
                    if (Presence.GetList(name) != null)
                    {
                        name = name + " (" + iter.ToString() + ")";
                        iter += 1;
                    }
                    else break;
                }

                // if category doesn't already exist, create one
                string category_id = Presence.GetCategory(categoryname);
                if (category_id == null)
                {
                    body = new Dictionary<string, string> { { "name", categoryname } };
                    res = HttpHandler.Request(HttpMethod.Post, "/list-categories/", JsonConvert.SerializeObject(body)).Result;
                    JObject cat = JObject.Parse(res.Content.ReadAsStringAsync().Result);
                    category_id = (string)cat["_id"];
                    Presence.AddCategory(categoryname, category_id);
                }

                // create new list
                body = new Dictionary<string, string> { { "name", name }, { "category", category_id } };
                if (desc != null) body.Add("description", desc);
                res = HttpHandler.Request(HttpMethod.Post, "/lists/", JsonConvert.SerializeObject(body)).Result;
                ListResponse lst = JsonConvert.DeserializeObject<ListResponse>(res.Content.ReadAsStringAsync().Result);
                this.Name = name;
                this.Id = lst._id;
                this.PrimaryId = lst.primary_column;
                this.InUseId = lst.inuse_column;
                foreach (var entry in lst.columns)
                {
                    if (entry.Key != this.PrimaryId && entry.Key != this.InUseId)
                    {
                        this.DescId = entry.Key;
                    }
                }
                this.FirstRow = lst.rows.Keys.First();
                Presence.AddList(name, Id);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
         * Adding new column
         */
        public void AddColumn(string name)
        {
            try
            {
                if (Id == null || name == null)
                {
                    throw new Exception("List not initiated yet or provided column name is null");
                }
                var body = new Dictionary<string, dynamic>
                {
                    {"name", name}, {"type", "text"}, {"length", 255}
                };
                var res = HttpHandler.Request(HttpMethod.Post, "/lists/" + Id + "/columns", JsonConvert.SerializeObject(body)).Result;
                JObject obj = JObject.Parse(res.Content.ReadAsStringAsync().Result);
                Columns.Add((string)obj["_id"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
         * Adding value column 
         */
        public void AddValueColumn()
        {          
            if (Id == null || PrimaryId == null)
            {
                throw new Exception("List not initiated yet");
            }
            Columns.Add(PrimaryId);
        }

        /*
         * Adding description column
         */
        public void AddDescColumn()
        {
            if (Id == null || DescId == null)
            {
                throw new Exception("List not initiated yet");
            }
            Columns.Add(DescId);
        }

        /*
         * Adding new row 
         */
        public void AddRow(ArrayList values)
        {
            HttpResponseMessage res;
            try
            {
                if (values.Count > Columns.Count)
                {
                    throw new Exception("More number of row fields than the number of columns");
                }
                var body = new Dictionary<string, Dictionary<string, dynamic>>();
                body.Add("values", new Dictionary<string, dynamic>());
                for (int i=0; i<values.Count; i++)
                {
                    body["values"].Add((string)Columns[i], values[i]);
                }
                body["values"].Add(InUseId, true);
                if (IsEmpty)
                {
                    res = HttpHandler.Request(HttpMethod.Put, "/lists/" + Id + "/rows/" + FirstRow, JsonConvert.SerializeObject(body)).Result;
                    IsEmpty = false;
                }
                else
                {
                    res = HttpHandler.Request(HttpMethod.Post, "/lists/" + Id + "/rows", JsonConvert.SerializeObject(body)).Result;
                }                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
         * Publish
         */
        public string Publish()
        {
            try
            {
                var body = new Dictionary<string, bool>
                {
                    { "published", true }
                };
                var res = HttpHandler.Request(HttpMethod.Put, "/lists/" + Id, JsonConvert.SerializeObject(body)).Result;
                JObject obj = JObject.Parse(res.Content.ReadAsStringAsync().Result);
                return (string) obj["_id"];
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /*
         * Delete self
         */
        public string DeleteSelf(int trials)
        {
            if (Id == null) return null;
            try
            {
                var res = HttpHandler.Request(HttpMethod.Delete, "/lists/" + Id, null);
                return Id;
            }
            catch (Exception ex)
            {
                try
                {
                    var init = HttpHandler.Init(HttpHandler.BaseUri, HttpHandler.Email, HttpHandler.Password).Result;
                }
                catch
                {
                    throw ex;
                }
                if (trials == 0) throw ex;
                else return DeleteSelf(trials - 1);
            }
        }
    }

    class ListResponse
    {
        public string _id { get; set; }
        public string primary_column { get; set; }
        public string inuse_column { get; set; }
        public Dictionary<string, dynamic> columns { get; set; }
        public Dictionary<string, dynamic> rows { get; set; }
    }
}
