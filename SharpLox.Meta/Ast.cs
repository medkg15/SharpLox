using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLox.Meta
{
    public class AbstractSyntaxTree
    {
        public AbstractSyntaxTree()
        {
            NodeTypes = new List<NodeType>();
        }

        public string Name { get; set; }

        public List<NodeType> NodeTypes { get; set; }
        
        public bool IsFirst { get; set; }

        public string NameForParameter()
        {
            return char.ToLower(Name[0]) + Name.Substring(1);
        }
    }
}
