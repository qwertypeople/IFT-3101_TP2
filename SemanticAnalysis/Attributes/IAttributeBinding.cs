using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemanticAnalysis.Parsing;

namespace SemanticAnalysis.Attributes
{
    public interface IAttributeBinding
    {
        Symbol Symbol { get; }
        int Subscript { get; }
    }
}
