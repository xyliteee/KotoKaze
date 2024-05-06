using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanContent
{
    public class CleanStruct
    {
        public string Type { get; set; }
        public string Path { get; set; }
        public string Key { get; set; }
        public CleanStruct(string type,string path,string key) 
        {
            Type = type;
            Path = path;
            Key = key;
        }
        
    }
}
