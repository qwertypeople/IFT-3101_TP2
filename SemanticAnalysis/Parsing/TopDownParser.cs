using ParseNodeExecption;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDownParsingException;

namespace SemanticAnalysis.Parsing
{
    public class TopDownParser(ILLParsingTable parsingTable)
    {
        private readonly ILLParsingTable _parsingTable = parsingTable;

        public ParseNode Parse(List<Token> input)
        {
            if (input == null || input.Count == 0 || input.Last().Symbol != Symbol.END)
            {
                throw new WhenEndTokenIsNotAtEndOfInputException();
            }

            
            // Obtenir la production pour le symbole de départ
            Production? startProduction = _parsingTable.GetProduction(_parsingTable.StartSymbol, input[0].Symbol);
            if (startProduction == null)
            {
                throw new WhenNoProductionDefinedException();
            }

            // Créer le nœud racine avec la production initiale
            ParseNode root = new(_parsingTable.StartSymbol, startProduction);

            // Initialiser la pile avec le premier symbole du corps
            Stack<(Symbol Symbol, ParseNode Parent)> stack = new();
            
            // Empiler les symboles de la production initiale
            for (int i = startProduction.Body.Count - 1; i >= 0; i--)
            {
                stack.Push((startProduction.Body[i], root));
            }

            int pos = 0;
            while (stack.Count > 0)
            {
                var (symbol, parent) = stack.Pop();
                Symbol currentInput = input[pos].Symbol;

                if (symbol.IsEnd())
                {
                    if (!currentInput.IsEnd())
                    {
                        throw new WhenCannotMatchInputException();
                    }
                    pos++;
                }
                else if (symbol.IsTerminal())
                {
                    if (symbol != currentInput)
                    {
                        throw new WhenCannotMatchInputException();
                    }
                    parent.Children.Add(new ParseNode(symbol, null));
                    pos++;
                }
                else if (symbol.IsEpsilon())
                {
                    parent.Children.Add(new ParseNode(Symbol.EPSILON, null));
                }
                else if (symbol.IsNonterminal())
                {
                    // Obtenir la production depuis la table d'analyse
                    Production? production = _parsingTable.GetProduction(symbol, currentInput);
                    if (production == null)
                    {
                        throw new WhenNoProductionDefinedException();
                    }

                    // Créer un nouveau nœud pour ce non-terminal
                    var newNode = new ParseNode(symbol, production);
                    parent.Children.Add(newNode);

                    // Empiler les symboles du corps en ordre inverse
                    for (int i = production.Body.Count - 1; i >= 0; i--)
                    {
                        stack.Push((production.Body[i], newNode));
                    }
                }
            }

            if (pos < input.Count - 1)
            {
                throw new WhenCannotMatchInputException();
            }

            return root;
        }
    }
}
