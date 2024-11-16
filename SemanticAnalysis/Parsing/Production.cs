using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Parsing
{
    public class Production
    {
        public Symbol Head { get; }
        public List<Symbol> Body { get; }

        public Production(Symbol head, List<Symbol> body)
        {
            /* --- À COMPLÉTER (gestion des erreurs seulement) --- */
            if (!head.IsNonterminal())
            {
                throw new WhenHeadIsTerminalException(head.Name);
            }

            if (!head.IsSpecial())
            {
                throw new WhenHeadIsSpecialException(head.Name);
            }

            foreach (Symbol symbol in body)
            {
                if (symbol.IsEpsilon() && body.Count > 1)
                {
                    throw new WhenBodyWithMoreThanOneSymbolContainsEpsilonException();                    
                }
                else if (symbol.IsSpecial() && !symbol.IsEpsilon())
                {
                    throw new WhenBodyContainsSpecialOtherThanEpsilonException(symbol.Name);
                }
            }

            Head = head;
            Body = body;
        }
    }
}
