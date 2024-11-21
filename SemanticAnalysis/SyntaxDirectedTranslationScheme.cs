using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemanticAnalysis.Attributes;
using SemanticAnalysis.Parsing;
using SyntaxDirectedTranslationSchemeException;

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
            if (startSymbol.IsTerminal())
            {
                throw new WhenStartSymbolIsTerminalException();
            }

            if (startSymbol.IsSpecial())
            {
                throw new WhenStartSymbolIsSpecialException();
            }

            bool isStartSymbolDefined = definition.Keys.Any(production => production.Head == startSymbol);
            if (!isStartSymbolDefined)
            {
                throw new WhenStartSymbolIsNotDefinedException();
            }

            

            StartSymbol     = startSymbol;
            Terminals       = new HashSet<Symbol>();
            Nonterminals    = new HashSet<Symbol>();
            Rules           = new Dictionary<Production, List<SemanticAction>>();
            First           = new Dictionary<Symbol, HashSet<Symbol>>();
            Follow          = new Dictionary<Symbol, HashSet<Symbol>>();

            SetSymbols(definition);

            if (Terminals.Count == 0)
            {
                throw new WhenNoTerminalInProductionsException();
            }

            List<Symbol> undefinedNonTerminals = new List<Symbol>();
            foreach (var nonTerminal in Nonterminals)
            {
                bool isDefined = false;
                foreach (var production in definition.Keys)
                {
                    if (production.Head == nonTerminal)
                    {
                        isDefined = true;
                        break;
                    }
                }

                if (!isDefined)
                {
                    undefinedNonTerminals.Add(nonTerminal);
                }
            }            
            if (undefinedNonTerminals.Count > 0)
            {
                throw new WhenNonterminalIsNotDefinedException(undefinedNonTerminals);
            }

            CheckForEquivalentProductions(definition.Keys);

            ValidateLAttributedGrammar(definition);


            OrderRules(definition);
            ComputeFirstSets();
            ComputeFollowSets();
        }

        public HashSet<Symbol> FirstOfBody(List<Symbol> symbols)
        {
            /* --- À COMPLÉTER (logique et gestion des erreurs) --- */
            if (symbols.Count == 0)
            {
                throw new WhenInputIsEmptyException();
            }

            HashSet<Symbol> firstSet = new HashSet<Symbol>();

            // Vérification que chaque symbole de la liste fait partie de la grammaire
            foreach (Symbol symbol in symbols)
            {
                if (!Terminals.Contains(symbol) && !Nonterminals.Contains(symbol))
                {
                    throw new WhenInputSymbolIsNotInGrammarException(symbol.Name);
                }
            }

            return [];
        }

        private void SetSymbols(Dictionary<Production, HashSet<SemanticAction>> definition)
        {
            /* --- À COMPLÉTER (logique et gestion des erreurs) --- */
            foreach (var production in definition.Keys)
            {
                Nonterminals.Add(production.Head);             
                foreach (var symbol in production.Body)
                {
                    if (symbol.IsTerminal())
                    {
                        Terminals.Add(symbol);
                    }
                    else
                    {
                        Nonterminals.Add(symbol);
                    }
                }
            }
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


        ////////////////////////
        private void CheckForEquivalentProductions(IEnumerable<Production> productions)
        {
            HashSet<(Symbol Head, List<Symbol> Body)> productionEquivalent = new HashSet<(Symbol Head, List<Symbol> Body)>();

            foreach (var production in productions)
            {
                (Symbol, List<Symbol>) current = (production.Head, production.Body);
                if (productionEquivalent.Contains(current))
                {
                    throw new WhenProductionsAreEquivalentException(production.Head, production.Body);
                }

                productionEquivalent.Add(current);
            }
        }
                       
        private void ValidateLAttributedGrammar(Dictionary<Production, HashSet<SemanticAction>> definition)
        {
            foreach (KeyValuePair<Production, HashSet<SemanticAction>> kvp in definition)
            {
                Production production = kvp.Key;
                HashSet<SemanticAction> actions = kvp.Value;

                foreach (SemanticAction action in actions)
                {
                    if (!IsLAttributed(action, production))
                    {
                        throw new WhenDefinitionIsNotLAttributedException(production, action);
                    }
                }
            }
        }

        private bool IsLAttributed(SemanticAction action, Production production)
        {
            //TODO
            //// Pour chaque source d'attributs dans l'action
            //foreach (IAttributeBinding source in action.Sources)
            //{
            //    // Vérification de l'attribut hérité (ne doit dépendre que des symboles à gauche ou du parent)
            //    if (source.IsInherited)
            //    {
            //        if (!IsValidInheritedDependency(source, production))
            //        {
            //            return false;
            //        }
            //    }
            //    // Vérification de l'attribut synthétisé (ne doit dépendre que des symboles à droite ou des enfants)
            //    else if (source.IsSynthesized)
            //    {
            //        if (!IsValidSynthesizedDependency(source, production))
            //        {
            //            return false;
            //        }
            //    }
            //}
            return true;
        }
    }
}
