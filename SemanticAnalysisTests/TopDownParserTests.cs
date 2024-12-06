using SemanticAnalysis;
using SemanticAnalysis.Attributes;
using SemanticAnalysis.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TopDownParsingException;

namespace SemanticAnalysisTests
{
    //public class SimpleParsingTable : ILLParsingTable
    //{
    //    Symbol S = new Symbol("S", SymbolType.Nonterminal);
    //    public Symbol StartSymbol => S;

    //    //public Production? GetProduction(Symbol nonTerminal, Symbol lookahead)
    //    //{
         
    //    //    throw new NotImplementedException();
    //    //}
    //}

    public class TopDownParserTests
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void Parse_WhenEndTokenIsNotAtEndOfInput_ShouldThrowException()
        {
            // Arrange
            var symbol_S = new Symbol("S", SymbolType.Nonterminal);
            var terminal_a = new Symbol("a", SymbolType.Terminal);
            var special_epsilon = Symbol.EPSILON;
            var special_end = Symbol.END;

            // Create two productions:
            // S → a
            var production1 = new Production(symbol_S, new List<Symbol> { terminal_a });
            // S → ε (this will cause 'end' to be added from Follow set)
            var production2 = new Production(symbol_S, new List<Symbol> { Symbol.EPSILON });

            var scheme = new TestSyntaxDirectedTranslationScheme(
                startSymbol: symbol_S,
                terminals: new HashSet<Symbol> { terminal_a, special_end },  // Include 'end' in terminals
                nonterminals: new HashSet<Symbol> { symbol_S },
                rules: new Dictionary<Production, List<SemanticAction>>
                {
                    { production1, new List<SemanticAction>() },
                    { production2, new List<SemanticAction>() }
                },
                first: new Dictionary<Symbol, HashSet<Symbol>>
                {
                    { symbol_S, new HashSet<Symbol> { terminal_a, Symbol.EPSILON } },  // S can derive epsilon
                    { terminal_a, new HashSet<Symbol> { terminal_a } },
                    { special_end, new HashSet<Symbol> { special_end } },
                    { Symbol.EPSILON, new HashSet<Symbol> { Symbol.EPSILON } }  // Add First set for epsilon
                },
                follow: new Dictionary<Symbol, HashSet<Symbol>>
                {
                    { symbol_S, new HashSet<Symbol> { special_end } }  // Add 'end' to follow set
                }
            );

            // Arrange
            var parsingTable = new LLParsingTable(scheme);
            var parser = new TopDownParser(parsingTable);
            Symbol x = new Symbol("x", SymbolType.Terminal);
            Symbol plus = new Symbol("+", SymbolType.Terminal);
            Symbol y = new Symbol("y", SymbolType.Terminal);

            var tokens = new List<Token>
            {                
                new Token(x, "x"),
                new Token(plus, "+"),
                new Token(y, "y") 
            };


            // Act & Assert
            Assert.Throws<WhenEndTokenIsNotAtEndOfInputException>(() => parser.Parse(tokens));
        }

        [Test]
        public void Parse_WhenCannotMatchInput_ShouldThrowException()
        {
            // Arrange
            var symbol_S = new Symbol("S", SymbolType.Nonterminal);
            var terminal_a = new Symbol("a", SymbolType.Terminal);
            var special_epsilon = Symbol.EPSILON;
            var special_end = Symbol.END;

            // Create two productions:
            // S → a
            var production1 = new Production(symbol_S, new List<Symbol> { terminal_a });
            // S → ε (this will cause 'end' to be added from Follow set)
            var production2 = new Production(symbol_S, new List<Symbol> { Symbol.EPSILON });

            var scheme = new TestSyntaxDirectedTranslationScheme(
                startSymbol: symbol_S,
                terminals: new HashSet<Symbol> { terminal_a, special_end },  // Include 'end' in terminals
                nonterminals: new HashSet<Symbol> { symbol_S },
                rules: new Dictionary<Production, List<SemanticAction>>
                {
            { production1, new List<SemanticAction>() },
            { production2, new List<SemanticAction>() }
                },
                first: new Dictionary<Symbol, HashSet<Symbol>>
                {
            { symbol_S, new HashSet<Symbol> { terminal_a, Symbol.EPSILON } },  // S can derive epsilon
            { terminal_a, new HashSet<Symbol> { terminal_a } },
            { special_end, new HashSet<Symbol> { special_end } },
            { Symbol.EPSILON, new HashSet<Symbol> { Symbol.EPSILON } }  // Add First set for epsilon
                },
                follow: new Dictionary<Symbol, HashSet<Symbol>>
                {
            {symbol_S, new HashSet<Symbol> { special_end } }  // Add 'end' to follow set
                }
            );

            var parsingTable = new LLParsingTable(scheme);
            var parser = new TopDownParser(parsingTable);

            // Create a sequence of tokens where there's an issue between x and + or y
            var tokens = new List<Token>
    {
        new Token(terminal_a, "a"),  // This token is valid for the grammar
        new Token(Symbol.END, "$")   // End of input
    };

            // Act & Assert: This will trigger a mismatch error because the next token isn't expected by the parsing table
            Assert.Throws<WhenCannotMatchInputException>(() => parser.Parse(tokens));
        }

        [Test]
        public void Parse_WhenNoProductionDefined_ShouldThrowException()
        {
            Assert.Fail();
        }
    }
}
