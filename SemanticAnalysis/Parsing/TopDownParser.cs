using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Parsing
{
    public class TopDownParser(ILLParsingTable parsingTable)
    {
        private readonly ILLParsingTable _parsingTable = parsingTable;

        public ParseNode Parse(List<Token> input)
        {
            Stack<Symbol> stack = new([Symbol.END, _parsingTable.StartSymbol]);

            /* --- À COMPLÉTER (logique et gestion des erreurs) --- */

            return new(_parsingTable.StartSymbol, null);
        }
    }
}
