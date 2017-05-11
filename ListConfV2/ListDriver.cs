using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ListConfV2.Http;
using System.Collections;

namespace ListConfV2
{
    public class ListDriver
    {
        public void Run(Uri baseUri, string email, string pw, List<ListInput> inputs)
        {
            List<ListObj> listObjs = new List<ListObj>();
            try
            {
                // Initializing Helper Classes
                var init = HttpHandler.Init(baseUri, email, pw).Result;
                init = Presence.Init().Result;

                // For each list
                for (int i= 0; i<inputs.Count; i++)
                {
                    if (inputs[i].IsIncluded)
                    {
                        // make list
                        listObjs.Add(new ListObj(inputs[i].Name, inputs[i].Desc, inputs[i].CateogoryName));
                       
                        // make columns
                        foreach (var col in inputs[i].Cols)
                        {
                            if (col.Value.IsIncluded)
                            {
                                if (col.Value.IsPrimary)
                                {
                                    listObjs.Last().AddValueColumn();
                                }
                                else if (col.Value.IsDesc)
                                {
                                    listObjs.Last().AddDescColumn();                                
                                }
                                else
                                {
                                    listObjs.Last().AddColumn(col.Key);
                                }
                            }
                        }

                        foreach (var row in inputs[i].Rows)
                        {
                            if (row.IsIncluded)
                            {
                                ArrayList r = new ArrayList();
                                foreach (var col in inputs[i].Cols)
                                {
                                    if (col.Value.IsIncluded)
                                    {

                                    }
                                }
                            }
                        }                        

                        // make rows
                        foreach (var entry in inputs[i].Entries)
                        {
                            for (int j = 0; j < entry.Count; j++)
                            {
                                ArrayList row = new ArrayList();
                                if (inputs[i].ColConfigs[j].IsIncluded)
                                {
                                    row.Add(entry[j]);
                                }
                                listObjs.Last().AddRow(row);
                            }
                        }
                        // publish list
                        listObjs.Last().Publish();
                    }
                }
            }
            catch (Exception ex)
            {
                // Clean-up (rollback) changes in case of failure
                try
                {
                    foreach (var obj in listObjs)
                    {
                        obj.DeleteSelf(3);
                    }
                }
                catch
                {
                    throw new Exception(
                        ex.Message + " -> Failed to rollback (clean up) changes...");
                }
                throw ex;
            }
        }
    }
}
