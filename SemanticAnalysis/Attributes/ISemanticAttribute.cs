using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SemanticAnalysis.Attributes
{
    public interface ISemanticAttribute
    {
        string Name { get; }
    }
}
