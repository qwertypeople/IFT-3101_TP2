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
            // Si la production n'est pas nulle et que le symbole est un non-terminal,
            // ajouter les enfants du corps de la production
            if (Production != null && Symbol.IsNonterminal())
            {
                foreach (Symbol bodySymbol in Production.Body)
                {
                    // Créer un nœud pour chaque symbole du corps de la production
                    // Vous pouvez adapter selon que vous avez des sous-productions spécifiques ou non
                    ParseNode childNode = new ParseNode(bodySymbol, null); // Ici production est à null pour l'enfant
                    Children.Add(childNode); // Ajouter le nœud enfant à la liste des enfants
                }
            }
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
