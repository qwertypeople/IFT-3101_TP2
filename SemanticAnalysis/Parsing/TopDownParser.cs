using LLParsingTableException;
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
            // Vérifier que l'entrée contient au moins le symbole END
            if (input == null || input.Count == 0 || input.Last().Symbol != Symbol.END)
                throw new WhenEndTokenIsNotAtEndOfInputException();

            // Initialiser la pile avec le symbole de fin et le symbole de départ
            Stack<Symbol> stack = new Stack<Symbol>(new[] { Symbol.END, _parsingTable.StartSymbol });
            int inputIndex = 0;

            while (stack.Count > 0)
            {
                Symbol top = stack.Pop();

                if (top.Type == SymbolType.Terminal)
                {
                    // Vérifier la correspondance avec le prochain symbole de l'entrée
                    if (inputIndex >= input.Count)
                        throw new WhenCannotMatchInputException();
                    if (top != input[inputIndex].Symbol)
                        throw new WhenCannotMatchInputException();

                    // Avancer dans l'entrée
                    inputIndex++;
                }
                else if (top.Type == SymbolType.Nonterminal)
                {
                    // Récupérer une production à partir de la table d'analyse
                    Symbol lookahead = input[inputIndex].Symbol;
                    Production? production;
                    try
                    {
                        production = _parsingTable.GetProduction(top, lookahead);
                        if (production == null)
                            throw new WhenNoProductionDefinedException();
                    }
                    catch (WhenTerminalIsNotInTableException)
                    {
                        throw new WhenNoProductionDefinedException();
                    }

                    // Ajouter les symboles de la production à la pile (dans l'ordre inverse)
                    var bodyReversed = production.Body;
                    bodyReversed.Reverse();
                    foreach (var symbol in bodyReversed)
                        stack.Push(symbol);
                }
            }

            // Retourner l'arbre d'analyse
            return new ParseNode(_parsingTable.StartSymbol, null);
        }
    }
}
