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
        public void Parse_WhenNoProductionDefined_ShouldThrowException()
        {
            // Arrange
            var symbol_E = new Symbol("E", SymbolType.Nonterminal);
            var symbol_T = new Symbol("T", SymbolType.Nonterminal);
            var terminal_plus = new Symbol("+", SymbolType.Terminal);
            var terminal_a = new Symbol("a", SymbolType.Terminal);
            var special_end = Symbol.END;

            // Productions:
            // E → T + a     (E can start with whatever T starts with)
            // T → a         (T can only start with 'a')
            var production1 = new Production(
                symbol_E,
                new List<Symbol> { symbol_T, terminal_plus, terminal_a }
            );
            var production2 = new Production(
                symbol_T,
                new List<Symbol> { terminal_a }
            );

            var scheme = new TestSyntaxDirectedTranslationScheme(
                startSymbol: symbol_E,
                terminals: new HashSet<Symbol> { terminal_a, terminal_plus, special_end },
                nonterminals: new HashSet<Symbol> { symbol_E, symbol_T },
                rules: new Dictionary<Production, List<SemanticAction>>
                {
                    { production1, new List<SemanticAction>() },
                    { production2, new List<SemanticAction>() }
                },
                first: new Dictionary<Symbol, HashSet<Symbol>>
                {
                    { symbol_E, new HashSet<Symbol> { terminal_a } },        // E starts with what T starts with (a)
                    { symbol_T, new HashSet<Symbol> { terminal_a } },        // T starts with a
                    { terminal_a, new HashSet<Symbol> { terminal_a } },
                    { terminal_plus, new HashSet<Symbol> { terminal_plus } },
                    { special_end, new HashSet<Symbol> { special_end } }
                },
                follow: new Dictionary<Symbol, HashSet<Symbol>>
                {
                    { symbol_E, new HashSet<Symbol> { special_end } },
                    { symbol_T, new HashSet<Symbol> { terminal_plus } }      // T can be followed by +
                }
            );

            var parsingTable = new LLParsingTable(scheme);
            var parser = new TopDownParser(parsingTable);

            // Try to parse "+ a end" - there's no production for E that starts with '+'
            // but '+' is a valid terminal in the grammar (it appears in production1)
            var tokens = new List<Token>
            {
                new Token(terminal_plus, "+"),   // No production for E starts with '+'
                new Token(terminal_a, "a"),
                new Token(special_end, "end")
            };

            // Act & Assert
            Assert.Throws<WhenNoProductionDefinedException>(() => parser.Parse(tokens));
        }

        [Test]
        public void Parse_WhenCannotMatchInput_ShouldThrowException()
        {
            // Arrange
            // Grammar that accepts only "a + a"
            var symbol_E = new Symbol("E", SymbolType.Nonterminal);  // Expression
            var terminal_a = new Symbol("a", SymbolType.Terminal);
            var terminal_plus = new Symbol("+", SymbolType.Terminal);
            var special_end = Symbol.END;

            // Production: E → a + a
            var production = new Production(
                symbol_E, 
                new List<Symbol> { terminal_a, terminal_plus, terminal_a }
            );

            var scheme = new TestSyntaxDirectedTranslationScheme(
                startSymbol: symbol_E,
                terminals: new HashSet<Symbol> { terminal_a, terminal_plus, special_end },
                nonterminals: new HashSet<Symbol> { symbol_E },
                rules: new Dictionary<Production, List<SemanticAction>>
                {
                    { production, new List<SemanticAction>() }
                },
                first: new Dictionary<Symbol, HashSet<Symbol>>
                {
                    { symbol_E, new HashSet<Symbol> { terminal_a } },
                    { terminal_a, new HashSet<Symbol> { terminal_a } },
                    { terminal_plus, new HashSet<Symbol> { terminal_plus } },
                    { special_end, new HashSet<Symbol> { special_end } }
                },
                follow: new Dictionary<Symbol, HashSet<Symbol>>
                {
                    { symbol_E, new HashSet<Symbol> { special_end } }
                }
            );

            var parsingTable = new LLParsingTable(scheme);
            var parser = new TopDownParser(parsingTable);

            // Try to parse "a + b" when grammar only accepts "a + a"
            var tokens = new List<Token>
            {
                new Token(terminal_a, "a"),      // First 'a' matches
                new Token(terminal_plus, "+"),    // '+' matches
                new Token(terminal_plus, "+"),    // Should fail here - expecting 'a' but got '+'
                new Token(special_end, "end")
            };

            // Act & Assert
            Assert.Throws<WhenCannotMatchInputException>(() => parser.Parse(tokens));
        }
    }
}
