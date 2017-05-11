using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ListConfV2
{
    public class ListParser
    {
        /*
        public List<ListInput> ParseCsv()
        {

        }*/

        // public List<ListInput> ParseFromSql() 
    }


    public class ListInput
    {
        public ListInput(string name, string desc, string categoryname, bool isincluded)
        {
            Name = name;
            Desc = desc;
            CateogoryName = categoryname;
            IsIncluded = isincluded;
        }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string CateogoryName { get; set; }
        public bool IsIncluded { get; set; } = true;
        public Dictionary<string, ColumnConfig> Cols { get; set; }
        public List<RowEntry> Rows { get; set; }
    }

    public struct ColumnConfig
    {
        public ColumnConfig(string name, bool isincluded, bool isprimary, bool isdesc)
        {
            Name = name;
            IsIncluded = isincluded;
            IsPrimary = isprimary;
            IsDesc = isdesc;
        }
        public string Name { get; set; }
        public bool IsIncluded { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsDesc { get; set; }
    }

    public struct RowEntry
    {
        public RowEntry (bool isIncluded, Dictionary<string, dynamic> entries)
        {
            IsIncluded = isIncluded;
            Entries = entries;
        }
        public bool IsIncluded {get;set;}
        public Dictionary<string, dynamic> Entries { get; set; }
    }
}
