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

            Symbol = symbol;
            Value = value;
        }
    }
}
