using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLox.Meta
{
    public class AstDefinition
    {
        public AstDefinition()
        {
            Trees = new List<AbstractSyntaxTree>();
        }

        public List<AbstractSyntaxTree> Trees { get; set; }
    }
}
