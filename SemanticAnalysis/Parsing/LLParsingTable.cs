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
                var production = rule.Key;
                var head = production.Head;

                // Initialiser l'entrée de la table pour ce non-terminal si nécessaire
                if (!_table.ContainsKey(head))
                {
                    _table[head] = new Dictionary<Symbol, Production?>();
                }

                var firstSet = scheme.FirstOfBody(production.Body);

                // Vérifier le conflit entre First et Follow
                // Un conflit First/Follow se produit lorsque :
                // 1. La production dérive epsilon (ε ∈ First(body))
                // 2. ET qu'il y a un terminal à la fois dans First(A) et Follow(A)
                if (firstSet.Contains(Symbol.EPSILON))
                {
                    var followSet = scheme.Follow[head];
                    var firstOfA = scheme.First[head];

                    // Vérifier s'il existe un terminal à la fois dans First(A) et Follow(A)
                    if (firstOfA.Intersect(followSet).Any())
                    {
                        throw new WhenFirstFollowConflictException(
                            head,
                            production,
                            firstOfA.Intersect(followSet).First()
                        );
                    }
                }

                // Ajouter les entrées à la table (cette partie s'exécute uniquement si aucun conflit n'est détecté)
                foreach (var terminal in firstSet.Where(t => t != Symbol.EPSILON))
                {
                    if (_table[head].ContainsKey(terminal))
                    {
                        throw new WhenFirstFirstConflictException(head, production, _table[head][terminal]);
                    }
                    _table[head][terminal] = production;
                }

                // Gérer les productions qui dérivent epsilon
                if (firstSet.Contains(Symbol.EPSILON))
                {
                    foreach (var terminal in scheme.Follow[head])
                    {
                        if (_table[head].ContainsKey(terminal))
                        {
                            throw new WhenFirstFirstConflictException(head, production, _table[head][terminal]);
                        }
                        _table[head][terminal] = production;
                    }
                }
            }
        }

        public Production? GetProduction(Symbol nonterminal, Symbol terminal)
        {
            // Vérifier d'abord les types de symboles avant de consulter le contenu de la table

            // 1. Vérifier que le paramètre "nonterminal" est bien un non-terminal
            if (nonterminal.Type == SymbolType.Terminal)
            {
                throw new WhenNonterminalIsTerminalException(nonterminal);
            }

            // 2. Vérifier que le paramètre "nonterminal" n'est pas un symbole spécial
            if (nonterminal.Type == SymbolType.Special)
            {
                throw new WhenNonterminalIsSpecialException(nonterminal);
            }

            // 3. Vérifier que le paramètre "terminal" est bien un terminal ou 'end'
            if (terminal.Type == SymbolType.Nonterminal)
            {
                throw new WhenTerminalIsNonterminalException(terminal);
            }

            // 4. Vérifier que s'il s'agit d'un symbole spécial, il doit être 'end'
            if (terminal.Type == SymbolType.Special && terminal != Symbol.END)
            {
                throw new WhenTerminalIsSpecialOtherThanEndException(terminal);
            }

            // Après la validation des types, vérifier le contenu de la table

            // 5. Vérifier que le non-terminal est présent dans la table
            if (!_table.ContainsKey(nonterminal))
            {
                throw new WhenNonterminalIsNotInTableException(nonterminal);
            }

            // Si le terminal n'est pas 'end', vérifier s'il existe dans une des lignes
            if (terminal != Symbol.END)
            {
                bool terminalFouded = false;
                foreach (var row in _table.Values)
                {
                    if (row.ContainsKey(terminal))
                    {
                        terminalFouded = true;
                        break;
                    }
                }

                if (!terminalFouded)
                {
                    throw new WhenTerminalIsNotInTableException(nonterminal, terminal);
                }
            }

            // 6. Vérifier que le terminal est présent dans la table pour ce non-terminal
            if (!_table[nonterminal].ContainsKey(terminal))
            {
                return null;
            }

            return _table[nonterminal][terminal];
        }
    }
}
