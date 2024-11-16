using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Instructions
{
    public class Assignment(string leftId, string rightId) : Instruction
    {
        public string LeftIf { get; } = leftId;
        public string RightIf { get; } = rightId;
    }
}
