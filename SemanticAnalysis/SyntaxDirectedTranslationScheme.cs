using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
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

            //bool isStartSymbolDefined = definition.Keys.Any(production => production.Head == startSymbol);
            bool isStartSymbolDefined = false;
            foreach (Production production in definition.Keys)
            {
                if (production.Head == startSymbol)
                {
                    isStartSymbolDefined = true;
                    break; 
                }
            }
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

            CheckForTerminalDefinition(definition);
            CheckForEquivalentProductions(definition.Keys);

            //ValidateLAttributedGrammar(definition);

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
            foreach (Production production in definition.Keys)
            {
                if (production == null)
                {
                    throw new ArgumentException("Une des productions dans le dictionnaire est nulle.");
                }

                // Ajoute la tête de la production aux non-terminaux
                Nonterminals.Add(production.Head);

                // Parcourt les symboles dans le corps de la production
                foreach (Symbol symbol in production.Body)
                {
                    if (symbol == null)
                    {
                        throw new WhenSymbolIsNullInProductionException(production);
                    }

                    switch (symbol.Type)
                    {
                        case SymbolType.Terminal:
                            Terminals.Add(symbol);
                            break;

                        case SymbolType.Nonterminal:                        
                            Nonterminals.Add(symbol);
                            break;

                        case SymbolType.Special:
                            break;

                        default:
                            throw new Exception("SymbolType non supporté.");
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
                               

                // Récupérer les symboles de la production
                List<Symbol> symbols = production.Body;

                Dictionary<Symbol, int> targetGroups = new Dictionary<Symbol, int>();
                // Ajouter les symboles avec leurs indices
                for (int i = 0; i < symbols.Count; i++)
                {
                    targetGroups[symbols[i]] = i;
                }
                //Ajouter la tête de la production à la fin
                targetGroups[production.Head] = symbols.Count;


                List<SemanticAction> orderedActions = new List<SemanticAction>(actions);
                // Trie les actions sémantiques en fonction des cibles, puis des sources
                orderedActions.Sort(new Comparison<SemanticAction>((action1, action2) =>
                {
                    int targetComparison = targetGroups[action1.Target.Symbol].CompareTo(targetGroups[action2.Target.Symbol]);
                    if (targetComparison != 0)
                    {
                        return targetComparison; // Trie d'abord par cible
                    }

                    // Si les cibles sont identiques, on trie par dépendance
                    int dependencyLevel1 = GetDependencyLevel(action1, targetGroups);
                    int dependencyLevel2 = GetDependencyLevel(action2, targetGroups);
                    return dependencyLevel1.CompareTo(dependencyLevel2);
                }));

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
            foreach (Symbol symbol in Nonterminals)
            {
                if (symbol.IsNonterminal())
                {
                    First[symbol] = new HashSet<Symbol>();
                }                
            } 

            bool updated; // Permet de suivre si des modifications sont apportées aux ensembles First

            do
            {
                updated = false;

                foreach (Production production in Rules.Keys)
                {
                    Symbol head = production.Head;
                    List<Symbol> body = production.Body;

                    // Calculer First pour cette production
                    // Calculer First pour cette production
                    foreach (Symbol symbol in body)
                    {
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
                            if (First.ContainsKey(symbol))
                            {
                                foreach (Symbol sym in First[symbol])
                                {
                                    if (sym != Symbol.EPSILON && First[head].Add(sym))
                                    {
                                        updated = true;
                                    }
                                }
                            }

                            // Si epsilon est dans First[symbol], continuer avec le prochain symbole
                            if (First.ContainsKey(symbol) && !First[symbol].Contains(Symbol.EPSILON))
                            {
                                break;
                            }
                        }
                    }
                    //foreach (Symbol symbol in body)
                    //{
                    //    if (symbol.IsTerminal())
                    //    {
                    //        // Ajouter directement le terminal à First[head]
                    //        if (First[head].Add(symbol))
                    //        {
                    //            updated = true;
                    //        }
                    //        break; // Arrêter car un terminal bloque la propagation
                    //    }
                    //    else
                    //    {
                    //        if (symbol != Symbol.EPSILON)
                    //        {
                    //            // Ajouter tous les éléments de First[symbol] à First[head], sauf epsilon
                    //            foreach (Symbol sym in First[symbol])
                    //            {
                    //                if (sym != Symbol.EPSILON && First[head].Add(sym))
                    //                {
                    //                    updated = true;
                    //                }
                    //            }

                    //            // Si epsilon est dans First[symbol], continuer avec le prochain symbole
                    //            if (!First[symbol].Contains(Symbol.EPSILON))
                    //            {
                    //                break;
                    //            }
                    //        }                         
                    //    }
                    //}

                    // Si tous les symboles du corps peuvent produire epsilon, ajouter epsilon à First[head]
                    bool allSymbolsCanProduceEpsilon = true;
                    foreach (Symbol symbol in body)
                    {
                        if (symbol.IsTerminal() || (First.ContainsKey(symbol) && !First[symbol].Contains(Symbol.EPSILON)))
                        {
                            allSymbolsCanProduceEpsilon = false;
                            break;
                        }
                    }

                    if (allSymbolsCanProduceEpsilon)
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
            // Initialize Follow sets
            Follow = new Dictionary<Symbol, HashSet<Symbol>>();

            foreach (Symbol nonterminal in Nonterminals)
            {
                Follow[nonterminal] = new HashSet<Symbol>();
            }

            // Add $ to Follow(StartSymbol)
            Follow[StartSymbol].Add(Symbol.END);

            bool updated;

            do
            {
                updated = false;

                foreach (Production production in Rules.Keys)
                {
                    Symbol head = production.Head;
                    List<Symbol> body = production.Body;

                    // Traverse the body of the production
                    for (int i = 0; i < body.Count; i++)
                    {
                        Symbol symbol = body[i];

                        if (symbol != Symbol.EPSILON && !symbol.IsTerminal())
                        {
                            // Step 1: Add First(β) \ {ε} to Follow(symbol)
                            for (int j = i + 1; j < body.Count; j++)
                            {
                                Symbol nextSymbol = body[j];

                                foreach (Symbol terminal in First[nextSymbol])
                                {
                                    if (terminal != Symbol.EPSILON && Follow[symbol].Add(terminal))
                                    {
                                        updated = true;
                                    }
                                }

                                // Stop if First[nextSymbol] does not contain ε
                                if (!First[nextSymbol].Contains(Symbol.EPSILON))
                                {
                                    break;
                                }
                            }

                            // Step 2: If β or all following symbols produce ε, add Follow(head) to Follow(symbol)
                            bool allFollowingProduceEpsilon = true;
                            for (int j = i + 1; j < body.Count; j++)
                            {
                                Symbol nextSymbol = body[j];
                                if (nextSymbol.IsTerminal() || !First[nextSymbol].Contains(Symbol.EPSILON))
                                {
                                    allFollowingProduceEpsilon = false;
                                    break;
                                }
                            }

                            if ((i == body.Count - 1 || allFollowingProduceEpsilon) && Follow.ContainsKey(head))
                            {
                                foreach (Symbol terminal in Follow[head])
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
            } while (updated); // Repeat until no updates are made
        }

        //private void ComputeFollowSets()
        //{
        //    // Initialize Follow sets
        //    Follow = new Dictionary<Symbol, HashSet<Symbol>>();

        //    foreach (Symbol nonterminal in Nonterminals)
        //    {
        //        Follow[nonterminal] = new HashSet<Symbol>();
        //    }

        //    // Add $ to Follow(StartSymbol)
        //    Follow[StartSymbol].Add(Symbol.END);

        //    bool updated;

        //    do
        //    {
        //        updated = false;

        //        foreach (Production production in Rules.Keys)
        //        {
        //            Symbol head = production.Head;
        //            List<Symbol> body = production.Body;

        //            // Traverse the body of the production
        //            for (int i = 0; i < body.Count; i++)
        //            {
        //                Symbol symbol = body[i];

        //                if (!symbol.IsTerminal())
        //                {
        //                    // Step 1: Add First(β) \ {ε} to Follow(symbol)
        //                    for (int j = i + 1; j < body.Count; j++)
        //                    {
        //                        Symbol nextSymbol = body[j];

        //                        foreach (Symbol terminal in First[nextSymbol])
        //                        {
        //                            if (terminal != Symbol.EPSILON && Follow[symbol].Add(terminal))
        //                            {
        //                                updated = true;
        //                            }
        //                        }

        //                        // Stop if First[nextSymbol] does not contain ε
        //                        if (!First[nextSymbol].Contains(Symbol.EPSILON))
        //                        {
        //                            break;
        //                        }
        //                    }

        //                    // Step 2: If β or all following symbols produce ε, add Follow(head) to Follow(symbol)
        //                    bool allFollowingProduceEpsilon = true;
        //                    for (int j = i + 1; j < body.Count; j++)
        //                    {
        //                        Symbol nextSymbol = body[j];
        //                        if (nextSymbol.IsTerminal() || !First[nextSymbol].Contains(Symbol.EPSILON))
        //                        {
        //                            allFollowingProduceEpsilon = false;
        //                            break;
        //                        }
        //                    }

        //                    if (i == body.Count - 1 || allFollowingProduceEpsilon)
        //                    {
        //                        foreach (Symbol terminal in Follow[head])
        //                        {
        //                            if (Follow[symbol].Add(terminal))
        //                            {
        //                                updated = true;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    } while (updated); // Repeat until no updates are made
        //}




        /// <summary>
        /// This method will check for the definition of the syntaxe.  
        /// If a NonTerminal is undefiened, it raise an error
        /// </summary>
        /// <param name="definition"></param>
        /// <exception cref="WhenNonterminalIsNotDefinedException"></exception>        
        private void CheckForTerminalDefinition(Dictionary<Production, HashSet<SemanticAction>> definition)
        {
            List<Symbol> undefinedNonTerminals = new List<Symbol>();
            foreach (Symbol nonTerminal in Nonterminals)
            {
                if (nonTerminal == Symbol.EPSILON) 
                { 
                    break; 
                }

                bool isDefined = false;
                foreach (Production production in definition.Keys)
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
        }


        private void CheckForEquivalentProductions(IEnumerable<Production> productions)
        {
            HashSet<(Symbol Head, List<Symbol> Body)> productionEquivalent = new HashSet<(Symbol Head, List<Symbol> Body)>();

            foreach (var production in productions)
            {
                (Symbol, List<Symbol>) current = (production.Head, production.Body);
                foreach ((Symbol, List<Symbol>) p in productionEquivalent)
                {
                    bool t1 = p.Item1 == current.Item1;
                    bool t2 = true;
                    for (int i = 0; i < current.Item2.Count; i++)
                    {
                        for (int j = 0; j < p.Item2.Count; j++)
                        {
                            if (current.Item2[i].Name != p.Item2[j].Name || current.Item2[i].Type != p.Item2[j].Type)
                            {
                                t2 = false;
                                break;
                            }
                        }
                        if (!t2)
                        {
                            break;
                        }
                    }
                           

                    if (t1 && t2)
                    {
                        throw new WhenProductionsAreEquivalentException(production.Head, production.Body);
                    }
                }

                productionEquivalent.Add(current);
            }
        }

        //private void ValidateLAttributedGrammar(Dictionary<Production, HashSet<SemanticAction>> definition)

        //private void ValidateLAttributedGrammar(Dictionary<Production, HashSet<SemanticAction>> definition)
        //{
        //    foreach (var entry in definition)
        //    {
        //        Production production = entry.Key;
        //        HashSet<SemanticAction> actions = entry.Value;

        //        // Récupérer la tête de la production et le corps
        //        Symbol head = production.Head;
        //        List<Symbol> body = production.Body;

        //        foreach (SemanticAction action in actions)
        //        {
                    
        //            // Identifier le symbole visé par l'action
        //            Symbol targetSymbol = action.Target.Symbol;

        //            // Récupérer les dépendances nécessaires pour calculer l'attribut
        //            HashSet<IAttributeBinding> dependencies = action.Sources;

        //            // Vérifier que chaque dépendance respecte les contraintes
        //            foreach (IAttributeBinding dependency in dependencies)
        //            {
        //                if (!ReferenceEquals(dependency, head)) // La tête est toujours une source valide
        //                {
        //                    // Si la dépendance est dans le corps, elle doit apparaître avant le symbole cible
        //                    int dependencyIndex = body.IndexOf(dependency.Symbol);
        //                    int targetIndex = body.IndexOf(targetSymbol);

        //                    if (dependencyIndex == -1 || dependencyIndex >= targetIndex)
        //                    {
        //                        throw new WhenDefinitionIsNotLAttributedException(production, action);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //}


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
