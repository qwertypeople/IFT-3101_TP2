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
                
                // Initialize table entry for this non-terminal if needed
                if (!_table.ContainsKey(head))
                {
                    _table[head] = new Dictionary<Symbol, Production?>();
                }

                var firstSet = scheme.FirstOfBody(production.Body);

                // Check for First/Follow conflict
                // A First/Follow conflict occurs when:
                // 1. The production derives epsilon (ε ∈ First(body))
                // 2. AND there's a terminal in both First(A) and Follow(A)
                if (firstSet.Contains(Symbol.EPSILON))
                {
                    var followSet = scheme.Follow[head];
                    var firstOfA = scheme.First[head];
                    
                    // Check if there's any terminal that's in both First(A) and Follow(A)
                    if (firstOfA.Intersect(followSet).Any())
                    {
                        throw new WhenFirstFollowConflictException(
                            head,
                            production,
                            firstOfA.Intersect(followSet).First()
                        );
                    }
                }

                // Add entries to the table (this part would only execute if no conflict was found)
                foreach (var terminal in firstSet.Where(t => t != Symbol.EPSILON))
                {
                    if (_table[head].ContainsKey(terminal))
                    {
                        throw new WhenFirstFirstConflictException(head, production, _table[head][terminal]);
                    }
                    _table[head][terminal] = production;
                }

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
            // First verify symbol types before checking table contents
            
            // 1. Verify that the "nonterminal" parameter is actually a non-terminal
            if (nonterminal.Type == SymbolType.Terminal)
            {
                throw new WhenNonterminalIsTerminalException(nonterminal);
            }

            // 2. Verify that the "nonterminal" parameter is not a special symbol
            if (nonterminal.Type == SymbolType.Special)
            {
                throw new WhenNonterminalIsSpecialException(nonterminal);
            }

            // 3. Verify that the "terminal" parameter is actually a terminal or 'end'
            if (terminal.Type == SymbolType.Nonterminal)
            {
                throw new WhenTerminalIsNonterminalException(terminal);
            }

            // 4. Verify that if it's a special symbol, it must be 'end'
            if (terminal.Type == SymbolType.Special && terminal != Symbol.END)
            {
                throw new WhenTerminalIsSpecialOtherThanEndException(terminal);
            }

            // After type validation, check table contents

            // 5. Verify that the nonterminal is in the table
            if (!_table.ContainsKey(nonterminal))
            {
                throw new WhenNonterminalIsNotInTableException(nonterminal);
            }

            if (terminal != Symbol.END)
            {
                bool terminalFouded = false;
                foreach (var row in _table.Values)
                {
                    if(row.ContainsKey(terminal))
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

            // 6. Verify that the terminal is in the table for this nonterminal
            if (!_table[nonterminal].ContainsKey(terminal))
            {                
                return null;
            }

            return _table[nonterminal][terminal];
        }
    }
}
