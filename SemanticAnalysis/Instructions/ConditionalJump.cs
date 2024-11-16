using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Instructions
{
    public class ConditionalJump(string leftId, string op, string rightId) : Jump
    {
        public string LeftIf { get; } = leftId;
        public string Operator { get; } = op;
        public string RightIf { get; } = rightId;
    }
}
