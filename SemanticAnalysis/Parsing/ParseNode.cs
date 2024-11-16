using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if (symbol.IsNonterminal() && production == null)
            {
                throw new WhenSymbolIsNonterminalAndProductionIsNullException();
            }

            if (production != null && symbol != production.Head)
            {
                throw new WhenSymbolIsNonterminalAndIsNotHeadOfProductionException();
            }

            if (symbol.IsNonterminal() && production != null)
            {
                throw new WhenSymbolIsTerminalAndProductionIsNotNullException();
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
            if (this.Symbol == binding.Symbol)
            {
                return this;
            }

            // Vérifier si l'un des enfants correspond au nom recherché
            foreach (ParseNode child in Children)
            {
                if (child.Symbol.Name == binding.Symbol.Name)
                {
                    return child;
                }
            }

            // Si le noeud n'est pas trouvé, lever une exception
            //TODO ajouté le nom du symbole de binding
            throw new WhenNodeCannotBeFoundException();
        }
    }
    }
}
