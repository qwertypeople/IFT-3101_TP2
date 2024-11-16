using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Parsing
{
    public interface ILLParsingTable
    {
        Symbol StartSymbol { get; }
        Production? GetProduction(Symbol nonTerminal, Symbol terminal);
    }
}
