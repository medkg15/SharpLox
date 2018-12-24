using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLox.Meta
{
    public class NodeType
    {
        public string Name { get; set; }

        public List<KeyValuePair<FieldName, string>> Fields { get; set; }
    }
}
