using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Instructions
{
    public interface IInstruction
    {
        Label Label { get; }
    }
}
