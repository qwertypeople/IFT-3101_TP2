using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Parsing
{
    public class Token
    {
        public Symbol Symbol { get; }
        public string? Value { get; }

        public Token(Symbol symbol, string? value)
        {
            /* --- À COMPLÉTER (gestion des erreurs seulement) --- */
            if (symbol.Type == SymbolType.Nonterminal)
            {
                throw new WhenSymbolIsNonterminalException(symbol.Name);                
            }

            if (symbol.Type == SymbolType.Special && symbol.Name != Symbol.END.Name)
            {
                throw new WhenSymbolIsSpecialOtherThanEndException(symbol.Name);
            }

            Symbol = symbol;
            Value = value;
        }
    }
}
