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


        /* --- À COMPLÉTER (logique et gestion des erreurs) --- */
        public HashSet<Symbol> FirstOfBody(List<Symbol> symbols)
        {
            if (symbols.Count == 0)
            {
                throw new WhenInputIsEmptyException();
            }

            HashSet<Symbol> firstSet = new HashSet<Symbol>();

            // Vérification des symboles dans la grammaire
            foreach (Symbol symbol in symbols)
            {
                if (!Terminals.Contains(symbol) && !Nonterminals.Contains(symbol))
                {
                    throw new WhenInputSymbolIsNotInGrammarException(symbol.Name);
                }
            }

            // Parcours des symboles de la liste
            foreach (Symbol symbol in symbols)
            {
                if (symbol.IsTerminal())
                {
                    // Ajouter directement le terminal au premier ensemble
                    firstSet.Add(symbol);
                    break; // Arrêter le calcul après un terminal
                }

                // Ajouter tous les symboles de First(symbol) à firstSet
                foreach (var sym in First[symbol])
                {
                    if (sym != Symbol.EPSILON)
                    {
                        firstSet.Add(sym); // Ajouter les symboles terminaux sauf epsilon
                    }
                }

                // Vérifier si epsilon est présent dans First(symbol)
                if (!First[symbol].Contains(Symbol.EPSILON))
                {
                    break; // Arrêter si epsilon n'est pas présent
                }
            }

            // Si tous les symboles de la liste peuvent produire epsilon, ajouter epsilon
            if (symbols.All(s => !s.IsTerminal() && First[s].Contains(Symbol.EPSILON)))
            {
                firstSet.Add(Symbol.EPSILON);
            }

            return firstSet;
        }


        private void SetSymbols(Dictionary<Production, HashSet<SemanticAction>> definition)
        {
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition), "Le dictionnaire des productions ne peut pas être nul.");
            }

            Terminals = new HashSet<Symbol>();
            Nonterminals = new HashSet<Symbol>();


            /* --- À COMPLÉTER (logique et gestion des erreurs) --- */
            foreach (var production in definition.Keys)
            {
                if (production == null)
                {
                    throw new ArgumentException("Une des productions dans le dictionnaire est nulle.");
                }

                // Ajoute la tête de la production aux non-terminaux
                Nonterminals.Add(production.Head);

                // Parcourt les symboles dans le corps de la production
                foreach (var symbol in production.Body)
                {
                    if (symbol == null)
                    {
                        throw new ArgumentException($"Un symbole dans la production {production} est nul.");
                    }

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
            if (definition == null)
            {
                throw new ArgumentNullException(nameof(definition), "Le dictionnaire des productions ne peut pas être nul.");
            }

            Rules = new Dictionary<Production, List<SemanticAction>>();

            foreach (var entry in definition)
            {
                var production = entry.Key;
                var actions = entry.Value;

                if (production == null)
                {
                    throw new ArgumentException("Une des productions dans le dictionnaire est nulle.");
                }

                if (actions == null)
                {
                    throw new ArgumentException($"Les actions pour la production {production} ne peuvent pas être nulles.");
                }

                var orderedActions = new List<SemanticAction>();

                // Récupérer les symboles de la production
                var symbols = production.Body;
                var targetGroups = symbols
                    .Select((symbol, index) => new { Symbol = symbol, Index = index })
                    .Concat(new[] { new { Symbol = production.Head, Index = symbols.Count } }) // Ajout de la tête de la production
                    .ToDictionary(x => x.Symbol, x => x.Index);

                // Trie les actions sémantiques en fonction des cibles, puis des sources
                orderedActions = actions
                    .OrderBy(action => targetGroups[action.Target.Symbol]) // Par ordre des cibles
                    .ThenBy(action => GetDependencyLevel(action, targetGroups)) // En fonction des dépendances
                    .ToList();

                // Ajouter les actions triées dans le dictionnaire Rules
                Rules[production] = orderedActions;
            }
        }

        private void ComputeFirstSets()
        {
            /* --- À COMPLÉTER (logique seulement) --- */
            // Initialiser le dictionnaire des ensembles First
            First = new Dictionary<Symbol, HashSet<Symbol>>();

            // Initialisation : chaque symbole non-terminal commence avec un ensemble First vide
            foreach (var nonterminal in Nonterminals)
            {
                First[nonterminal] = new HashSet<Symbol>();
            }

            bool updated; // Permet de suivre si des modifications sont apportées aux ensembles First

            do
            {
                updated = false;

                foreach (var production in Rules.Keys)
                {
                    var head = production.Head;
                    var body = production.Body;

                    // Calculer First pour cette production
                    for (int i = 0; i < body.Count; i++)
                    {
                        var symbol = body[i];

                        if (symbol.IsTerminal())
                        {
                            // Ajouter directement le terminal à First[head]
                            if (First[head].Add(symbol))
                            {
                                updated = true;
                            }
                            break; // Arrêter car un terminal bloque la propagation
                        }
                        else
                        {
                            // Ajouter tous les éléments de First[symbol] à First[head], sauf epsilon
                            foreach (var sym in First[symbol])
                            {
                                if (sym != Symbol.EPSILON && First[head].Add(sym))
                                {
                                    updated = true;
                                }
                            }

                            // Si epsilon est dans First[symbol], continuer avec le prochain symbole
                            if (!First[symbol].Contains(Symbol.EPSILON))
                            {
                                break;
                            }
                        }
                    }

                    // Si tous les symboles du corps peuvent produire epsilon, ajouter epsilon à First[head]
                    if (body.All(s => !s.IsTerminal() && First[s].Contains(Symbol.EPSILON)))
                    {
                        if (First[head].Add(Symbol.EPSILON))
                        {
                            updated = true;
                        }
                    }
                }
            } while (updated); // Répéter tant que des modifications sont apportées
        }

        private void ComputeFollowSets()
        {
            /* --- À COMPLÉTER (logique seulement) --- */
            // Initialisation : chaque non-terminal commence avec un ensemble Follow vide
            Follow = new Dictionary<Symbol, HashSet<Symbol>>();

            foreach (var nonterminal in Nonterminals)
            {
                Follow[nonterminal] = new HashSet<Symbol>();
            }

            // Ajouter le symbole de fin ($) à l'ensemble Follow du symbole de départ
            Follow[StartSymbol].Add(Symbol.END);

            bool updated;

            do
            {
                updated = false;

                foreach (var production in Rules.Keys)
                {
                    var head = production.Head;
                    var body = production.Body;

                    // Parcourt les symboles du corps de la production
                    for (int i = 0; i < body.Count; i++)
                    {
                        var symbol = body[i];

                        if (!symbol.IsTerminal())
                        {
                            // Étape 1 : Ajouter First(β) \ {ε} à Follow(A)
                            for (int j = i + 1; j < body.Count; j++)
                            {
                                var nextSymbol = body[j];

                                // Ajouter First[nextSymbol] \ {ε} à Follow[symbol]
                                foreach (var terminal in First[nextSymbol])
                                {
                                    if (terminal != Symbol.EPSILON && Follow[symbol].Add(terminal))
                                    {
                                        updated = true;
                                    }
                                }

                                // Si First[nextSymbol] ne contient pas ε, arrêter
                                if (!First[nextSymbol].Contains(Symbol.EPSILON))
                                {
                                    break;
                                }
                            }

                            // Étape 2 : Si β ou les symboles suivants peuvent produire ε,
                            // ajouter Follow(head) à Follow(symbol)
                            if (i == body.Count - 1 || body.Skip(i + 1).All(s => !s.IsTerminal() && First[s].Contains(Symbol.EPSILON)))
                            {
                                foreach (var terminal in Follow[head])
                                {
                                    if (Follow[symbol].Add(terminal))
                                    {
                                        updated = true;
                                    }
                                }
                            }
                        }
                    }
                }
            } while (updated); // Répéter tant que des modifications sont apportées
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

        private int GetDependencyLevel(SemanticAction action, Dictionary<Symbol, int> targetGroups)
        {
            if (action.Sources == null || !action.Sources.Any())
            {
                return 0; // Aucun niveau de dépendance si aucune source
            }

            // Retourne le niveau maximal de dépendance parmi les sources
            return action.Sources.Max(source => targetGroups.ContainsKey(source.Symbol) ? targetGroups[source.Symbol] : int.MaxValue);
        }


    }
}
