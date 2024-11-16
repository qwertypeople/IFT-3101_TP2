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

            Head = head;
            Body = body;
        }
    }
}
