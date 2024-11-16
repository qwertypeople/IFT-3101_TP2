using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemanticAnalysis.Parsing;

namespace SemanticAnalysis.Attributes
{
    public class AttributeBinding<T>(Symbol symbol, int subscript, SemanticAttribute<T> attribute) : IAttributeBinding
    {
        public Symbol Symbol { get; } = symbol;
        public int Subscript { get; } = subscript;
        public SemanticAttribute<T> Attribute { get; } = attribute;
    }
}
