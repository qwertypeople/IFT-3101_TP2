using LLParsingTableException;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Parsing
{
    public class LLParsingTable : ILLParsingTable
    {
        public Symbol StartSymbol { get; }

        private readonly Dictionary<Symbol, Dictionary<Symbol, Production?>> _table;

        public LLParsingTable(ISyntaxDirectedTranslationScheme scheme)
        {
            StartSymbol = scheme.StartSymbol;
            _table = new Dictionary<Symbol, Dictionary<Symbol, Production?>>();


            foreach (var rule in scheme.Rules)
            {
                Production production = rule.Key;

                //Symbol head = rule.production.Head;
                Symbol head = production.Head;

                // Initialiser la table pour ce non-terminal
                if (!_table.ContainsKey(head))
                {
                    _table[head] = new Dictionary<Symbol, Production?>();
                }

                // Calculer First(body) pour chaque production
                HashSet<Symbol> firstSet = scheme.FirstOfBody(production.Body);

                foreach (Symbol terminal in firstSet)
                {
                    if (terminal == Symbol.EPSILON) continue; // Ne pas inclure epsilon dans les conflits

                    // Vérifier les conflits First/First
                    if (_table[head].ContainsKey(terminal))
                    {
                        Production? existingProduction = _table[head][terminal];
                        throw new WhenFirstFirstConflictException(head, production, existingProduction);
                    }

                    // Ajouter la production à la table
                    _table[head][terminal] = production;
                }

                // Gestion de epsilon (transférer au Follow(head))
                if (firstSet.Contains(Symbol.EPSILON))
                {
                    HashSet<Symbol> followSet = scheme.Follow[head];
                    foreach (Symbol followSymbol in followSet)
                    {
                        if (_table[head].ContainsKey(followSymbol))
                        {
                            Production? existingProduction = _table[head][followSymbol];
                            throw new WhenFirstFirstConflictException(head, production, existingProduction);
                        }

                        _table[head][followSymbol] = production;
                    }
                }
            }


            /* --- À COMPLÉTER (logique et gestion des erreurs) --- */
        }


        public Production? GetProduction(Symbol nonterminal, Symbol terminal)
        {
            /* --- À COMPLÉTER (gestion des erreurs seulement) --- */

            // Vérification que le non-terminal existe dans la table
            if (!_table.ContainsKey(nonterminal))
            {
                return null; // Retourner null si le non-terminal n'existe pas
            }

            // Vérification que le terminal existe dans le dictionnaire associé au non-terminal
            if (!_table[nonterminal].ContainsKey(terminal))
            {
                return null; // Retourner null si le terminal n'existe pas pour ce non-terminal
            }

            return _table[nonterminal][terminal]; 
        }

        //var tt = _table[nonterminal];
        //var ss = tt[terminal];

    }
}
