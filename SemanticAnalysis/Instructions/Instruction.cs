using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Instructions
{
    public abstract class Instruction : IInstruction
    {
        public Label Label { get; } = IntermediateCodeManager.EmitLabel();
    }
}
