using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TextTemplating;

namespace SharpLox.Meta
{
    public class AstDefinitionProvider
    {
        private readonly string _file;
        private readonly ITextTemplatingEngineHost _host;

        public AstDefinitionProvider(ITextTemplatingEngineHost host, string file)
        {
            _file = file;
            _host = host;
        }

        public AstDefinition GetDefinition()
        {
            var fullPath = _host.ResolvePath(_file);

            var lines = System.IO.File.ReadLines(fullPath);

            var definition = new AstDefinition();

            AbstractSyntaxTree currentAst = null; // the root AST node type, which is generated as an abstract class, and all other node types inherit from.

            foreach (var line in lines)
            {
                // check if the line we're dealing with is an AST definition, or an AST node-type definition.
                if (!line.Contains(":"))
                {
                    var isFirst = currentAst == null;
                    currentAst = new AbstractSyntaxTree();
                    var name = line.Trim();
                    if (line.EndsWith("<>"))
                    {
                        currentAst.Name = line.Substring(0, line.Length - 2);
                        currentAst.HasVisitResult = true;
                    }
                    else
                    {
                        currentAst.Name = line.Trim();
                    }
                    currentAst.IsFirst = isFirst;
                    definition.Trees.Add(currentAst);
                }
                else
                {
                    // extract node type information: name and fields (type and name of each).
                    var segments = line.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    var className = segments[0].Trim();
                    var fields = segments[1].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(fieldsSegment =>
                        {
                            var fieldSegments = fieldsSegment.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            // key: field name, value: field type
                            return new KeyValuePair<FieldName, string>(new FieldName(fieldSegments[1].Trim()), fieldSegments[0].Trim());
                        })
                        .ToList();

                    currentAst.NodeTypes.Add(new NodeType
                    {
                        Name = className,
                        Fields = fields
                    });
                }
            }

            return definition;
        }
    }
}
