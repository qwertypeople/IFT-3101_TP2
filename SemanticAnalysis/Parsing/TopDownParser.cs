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

            // Obtenir la production initiale pour le symbole de départ
            Symbol startSymbol = _parsingTable.StartSymbol;
            Symbol firstLookahead = input[0].Symbol;
            Production? startProduction;
            try
            {
                startProduction = _parsingTable.GetProduction(startSymbol, firstLookahead);
                if (startProduction == null)
                    throw new WhenNoProductionDefinedException();
            }
            catch (WhenTerminalIsNotInTableException)
            {
                throw new WhenNoProductionDefinedException();
            }
            
            // Créer le nœud racine avec le symbole de départ et sa production
            var root = new ParseNode(startSymbol, startProduction);
            
            // Initialiser la pile avec le symbole de fin et les symboles de la production initiale
            var stack = new Stack<(Symbol Symbol, ParseNode? Node)>();
            stack.Push((Symbol.END, null));  // On met juste le symbole END, pas de nœud

            // Créer les nœuds enfants pour la production initiale
            foreach (var bodySymbol in startProduction.Body)
            {
                var childNode = new ParseNode(bodySymbol, bodySymbol.IsTerminal() ? null : startProduction);
                root.Children.Add(childNode);
            }

            // Ajouter les nœuds à la pile dans l'ordre inverse
            for (int i = root.Children.Count - 1; i >= 0; i--)
            {
                stack.Push((root.Children[i].Symbol, root.Children[i]));
            }
            
            int inputIndex = 0;

            while (stack.Count > 0)
            {
                var (symbol, node) = stack.Pop();

                if (symbol.Type == SymbolType.Special)  // Pour END
                {
                    if (symbol != Symbol.END || inputIndex >= input.Count || input[inputIndex].Symbol != Symbol.END)
                        throw new WhenEndTokenIsNotAtEndOfInputException();
                    inputIndex++;
                }
                else if (symbol.Type == SymbolType.Terminal)
                {
                    // Vérifier la correspondance avec le prochain symbole de l'entrée
                    if (inputIndex >= input.Count)
                        throw new WhenCannotMatchInputException();
                    if (symbol != input[inputIndex].Symbol)
                        throw new WhenCannotMatchInputException();

                    // Le nœud terminal est déjà créé, avancer dans l'entrée
                    inputIndex++;
                }
                else if (symbol.Type == SymbolType.Nonterminal && node != null)
                {
                    // Récupérer une production à partir de la table d'analyse
                    Symbol lookahead = input[inputIndex].Symbol;
                    Production? production;
                    try
                    {
                        production = _parsingTable.GetProduction(symbol, lookahead);
                        if (production == null)
                            throw new WhenNoProductionDefinedException();
                    }
                    catch (WhenTerminalIsNotInTableException)
                    {
                        throw new WhenNoProductionDefinedException();
                    }

                    // Créer les nœuds enfants pour chaque symbole dans la production
                    foreach (var bodySymbol in production.Body)
                    {
                        var childNode = new ParseNode(bodySymbol, bodySymbol.IsTerminal() ? null : production);
                        node.Children.Add(childNode);
                    }

                    // Ajouter les nœuds à la pile dans l'ordre inverse
                    for (int i = node.Children.Count - 1; i >= 0; i--)
                    {
                        stack.Push((node.Children[i].Symbol, node.Children[i]));
                    }
                }
            }

            return root;
        }
    }
}
