using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ParseNodeExecption;
using SemanticAnalysis.Attributes;

namespace SemanticAnalysis.Parsing
{
    public class ParseNode
    {
        public Symbol Symbol { get; }
        public Production? Production { get; set; }
        public List<ParseNode> Children { get; } = [];

        private readonly Dictionary<ISemanticAttribute, object?> _attributes = [];

        public ParseNode(Symbol symbol, Production? production)
        {
            /* --- À COMPLÉTER (gestion des erreurs seulement) --- */
            if (symbol.IsTerminal() && production != null)
            {
                throw new WhenSymbolIsTerminalAndProductionIsNotNullException();
            }

            if (symbol.IsNonterminal() && production == null)
            {
                throw new WhenSymbolIsNonterminalAndProductionIsNullException();
            }

            if (production != null && symbol != production.Head)
            {
                throw new WhenSymbolIsNonterminalAndIsNotHeadOfProductionException();
            }

            if (symbol.IsEpsilon() && production != null)
            {
                throw new WhenSymbolIsEpsilonAndProductionIsNotNullException();
            }

            if (symbol.IsSpecial() && !symbol.IsEpsilon())
            {
                throw new WhenSymbolIsSpecialOtherThanEpsilonException();
            }

            Symbol = symbol;
            Production = production;
            Children = new List<ParseNode>();

        }

        public T? GetAttributeValue<T>(SemanticAttribute<T> attribute)
        {
            /* --- À COMPLÉTER (gestion des erreurs seulement) --- */
            if (!_attributes.ContainsKey(attribute))
            {                
                throw new WhenNoValueForAttributeException();
            }

            return (T?)_attributes[attribute];
        }

        public void SetAttributeValue<T>(SemanticAttribute<T> attribute, T value)
        {
            _attributes[attribute] = value;
        }

        public ParseNode GetBindedNode(IAttributeBinding binding)
        {
            /* --- À COMPLÉTER (logique et gestion des erreurs) --- */
            if (binding == null)
            {
                throw new ArgumentNullException(nameof(binding), "L'AttributeBinding ne peut pas être nul.");
            }

            int occurrenceCount = 0;

            // Vérifie si le noeud actuel correspond au symbole spécifié
            if (this.Symbol == binding.Symbol)
            {
                if (binding.Subscript == 0) // Si c'est la première occurrence
                {
                    return this;
                }
                occurrenceCount++; // Compte cette occurrence
            }

            // Parcourir les enfants pour rechercher l'occurrence spécifiée
            foreach (ParseNode child in Children)
            {
                if (child.Symbol == binding.Symbol)
                {
                    // Vérifie si c'est l'occurrence recherchée
                    if (occurrenceCount == binding.Subscript)
                    {
                        return child;
                    }
                    occurrenceCount++;
                }
            }

            // Si le noeud n'est pas trouvé, lever une exception
            throw new WhenNodeCannotBeFoundException();
        }
    }
    
}
