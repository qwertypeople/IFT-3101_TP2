using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemanticActionExecption;
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
            if (sources.Contains(target))
            {
                throw new WhenTargetIsInSourcesException();
            }

            Target = target;
            Sources = sources;
            _action = action;

        }

        public void Execute(ParseNode node)
        {
            /* --- À COMPLÉTER (gestion des erreurs seulement) --- */
            bool hasMatchingChildForTarget = false;
            foreach (var child in node.Children)
            {
                if (child.Symbol == Target.Symbol)
                {
                    hasMatchingChildForTarget = true;
                    break;
                }
            }

            bool targetExists = (node.Symbol == Target.Symbol) || hasMatchingChildForTarget;

            if (!targetExists)
            {
                throw new WhenNodeSymbolIsNonterminalAndTargetIsNotInNodeException(Target.Symbol.Name, node.Symbol.Name);                  
            } 

            foreach (var source in Sources)
            {
                bool hasMatchingChildForSource = false;
                foreach (var child in node.Children)
                {
                    if (child.Symbol == source.Symbol)
                    {
                        hasMatchingChildForSource = true;
                        break;
                    }
                }

                bool sourceExists = (node.Symbol == source.Symbol) || hasMatchingChildForSource;
                if (!sourceExists)
                {
                    throw new WhenNodeSymbolIsNonterminalAndSourceIsNotInNodeException(source.Symbol.Name, node.Symbol.Name);
                }                      
            }

            if (node.Symbol.IsTerminal())
            {
                throw new WhenNodeSymbolIsTerminalException(node.Symbol.Name);
            }

            if (node.Symbol.IsSpecial())
            {
                throw new WhenNodeSymbolIsSpecialException(node.Symbol.Name);
            }
            _action(node);
        }
    }
}
