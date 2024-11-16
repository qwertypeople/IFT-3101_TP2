using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemanticAnalysis.Attributes;
using SemanticAnalysis.Parsing;

namespace SemanticAnalysis
{
    public class SyntaxDirectedTranslationScheme : ISyntaxDirectedTranslationScheme
    {
        public Symbol StartSymbol { get; private set; }
        public HashSet<Symbol> Terminals { get; private set; }
        public HashSet<Symbol> Nonterminals { get; private set; }
        public Dictionary<Production, List<SemanticAction>> Rules { get; private set; }
        public Dictionary<Symbol, HashSet<Symbol>> First { get; private set; }
        public Dictionary<Symbol, HashSet<Symbol>> Follow { get; private set; }

        public SyntaxDirectedTranslationScheme(Symbol startSymbol, Dictionary<Production, HashSet<SemanticAction>> definition)
        {
            /* --- À COMPLÉTER (gestion des erreurs seulement) --- */

            StartSymbol = startSymbol;
            Terminals = [];
            Nonterminals = [];
            Rules = [];
            First = [];
            Follow = [];

            SetSymbols(definition);
            OrderRules(definition);
            ComputeFirstSets();
            ComputeFollowSets();
        }

        public HashSet<Symbol> FirstOfBody(List<Symbol> symbols)
        {
            /* --- À COMPLÉTER (logique et gestion des erreurs) --- */

            return [];
        }

        private void SetSymbols(Dictionary<Production, HashSet<SemanticAction>> definition)
        {
            /* --- À COMPLÉTER (logique et gestion des erreurs) --- */
        }

        private void OrderRules(Dictionary<Production, HashSet<SemanticAction>> definition)
        {
            /* --- À COMPLÉTER (logique et gestion des erreurs) --- */
        }

        private void ComputeFirstSets()
        {
            /* --- À COMPLÉTER (logique seulement) --- */
        }

        private void ComputeFollowSets()
        {
            /* --- À COMPLÉTER (logique seulement) --- */
        }
    }
}
