using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Parsing
{
    public class LLParsingTable : ILLParsingTable
    {
        public Symbol StartSymbol { get; }

        private readonly Dictionary<Symbol, Dictionary<Symbol, Production?>> _table;

        public LLParsingTable(ISyntaxDirectedTranslationScheme scheme)
        {
            StartSymbol = scheme.StartSymbol;
            _table = [];
            
            /* --- À COMPLÉTER (logique et gestion des erreurs) --- */
        }

        public Production? GetProduction(Symbol nonterminal, Symbol terminal)
        {
            /* --- À COMPLÉTER (gestion des erreurs seulement) --- */

            return _table[nonterminal][terminal];
        }


    }
}
