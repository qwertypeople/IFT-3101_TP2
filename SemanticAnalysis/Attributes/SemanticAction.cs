using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemanticAnalysis.Parsing;

namespace SemanticAnalysis.Attributes
{
    public class SemanticAction
    {
        public IAttributeBinding Target { get; }
        public HashSet<IAttributeBinding> Sources { get; }

        private readonly Action<ParseNode> _action;

        public SemanticAction(IAttributeBinding target, HashSet<IAttributeBinding> sources, Action<ParseNode> action)
        {
            /* --- À COMPLÉTER (gestion des erreurs seulement) --- */

            Target = target;
            Sources = sources;
            _action = action;
        }

        public void Execute(ParseNode node)
        {
            /* --- À COMPLÉTER (gestion des erreurs seulement) --- */

            _action(node);
        }
    }
}
