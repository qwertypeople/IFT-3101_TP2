using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemanticAnalysis.Parsing;

namespace SemanticAnalysis.Attributes
{
    public class SemanticAnalyzer(LexicalAnalyzer lexicalAnalyzer, ISyntaxDirectedTranslationScheme scheme)
    {
        private readonly LexicalAnalyzer _lexicalAnalyzer = lexicalAnalyzer;
        private readonly ISyntaxDirectedTranslationScheme _scheme = scheme;

        public void Annotate(ParseNode node)
        {
            // Pour les nœuds terminaux, gérer uniquement la valeur lexicale
            if (node.Symbol.IsTerminal() && !node.Symbol.IsEpsilon())
            {
                try
                {
                    var value = _lexicalAnalyzer.GetNextValue(node.Symbol);
                    node.SetAttributeValue(SemanticAttribute.LEXICAL_VALUE, value);
                }
                catch (Exception)
                {
                    // Si nous ne pouvons pas obtenir la valeur, continuer simplement
                }
                return;
            }

            // Obtenir les actions sémantiques pour la production de ce nœud
            var actions = node.Production != null ? _scheme.Rules[node.Production] : new List<SemanticAction>();

            // D'abord traiter tous les enfants terminaux
            foreach (var child in node.Children.Where(c => c.Symbol.IsTerminal() && !c.Symbol.IsEpsilon()))
            {
                Annotate(child);
            }

            // Ensuite traiter les enfants non-terminaux dans l'ordre
            for (int i = 0; i < node.Children.Count; i++)
            {
                var child = node.Children[i];
                if (child.Symbol.IsTerminal() && !child.Symbol.IsEpsilon())
                {
                    continue; // Déjà traité ci-dessus
                }

                // Calculer les attributs hérités pour cet enfant
                foreach (var action in actions.Where(a => 
                    a.Target is AttributeBinding<object> binding && 
                    binding.Attribute.Type == AttributeType.Inherited &&
                    node.GetBindedNode(binding) == child))
                {
                    action.Execute(node);
                }

                // Annoter récursivement l'enfant
                Annotate(child);
            }

            // Calculer les attributs synthétisés pour ce nœud
            foreach (var action in actions.Where(a => 
                a.Target is AttributeBinding<object> binding && 
                binding.Attribute.Type == AttributeType.Synthesized &&
                node.GetBindedNode(binding) == node))
            {
                action.Execute(node);
            }
        }
    }
}
