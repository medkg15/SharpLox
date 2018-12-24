using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpLox.Meta
{
    public class FieldName
    {
        private readonly string _name;

        public FieldName(string name)
        {
            _name = name;
        }

        public string ForProperty()
        {
            return char.ToUpper(_name[0]) + _name.Substring(1);
        }

        public string ForConstructorParameter()
        {
            // fix reserved words
            if (_name == "operator")
            {
                return "@operator";
            }
            else
            {
                return _name;
            }
        }
    }
}
