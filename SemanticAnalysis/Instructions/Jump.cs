using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Instructions
{
    public abstract class Jump : Instruction
    {
        public Label? JumpLabel { get; set; } = null;

        public Jump()
        {
            IntermediateCodeManager.RegisterForBackpatching(this);
        }
    }
}
