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

            Symbol = symbol;
            Production = production;
        }

        public T? GetAttributeValue<T>(SemanticAttribute<T> attribute)
        {
            /* --- À COMPLÉTER (gestion des erreurs seulement) --- */

            return (T?)_attributes[attribute];
        }

        public void SetAttributeValue<T>(SemanticAttribute<T> attribute, T value)
        {
            _attributes[attribute] = value;
        }

        public ParseNode GetBindedNode(IAttributeBinding binding)
        {
            /* --- À COMPLÉTER (logique et gestion des erreurs) --- */

            return this;
        }
    }
}
