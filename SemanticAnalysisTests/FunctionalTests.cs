using Newtonsoft.Json.Linq;
using SemanticAnalysis;
using SemanticAnalysis.Attributes;
using SemanticAnalysis.Instructions;
using SemanticAnalysis.Parsing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysisTests
{
    public class FunctionalTests
    {
        [Test]
        public void Exercises1D()
        {
            /* --- Exercice 1.d de la série sur le Top-Down parsing. --- */

            Symbol S = new("S", SymbolType.Nonterminal);
            Symbol A = new("A", SymbolType.Nonterminal);
            Symbol B = new("B", SymbolType.Nonterminal);
            Symbol C = new("C", SymbolType.Nonterminal);

            Symbol a = new("a", SymbolType.Terminal);
            Symbol b = new("b", SymbolType.Terminal);
            Symbol c = new("c", SymbolType.Terminal);

            Production production1 = new(S, [A, B, C]);
            Production production2 = new(A, [a, A]);
            Production production3 = new(A, [b]);
            Production production4 = new(B, [b, S, B]);
            Production production5 = new(B, [Symbol.EPSILON]);
            Production production6 = new(C, [c, C]);
            Production production7 = new(C, [a]);

            Dictionary<Production, HashSet<SemanticAction>> definition = new()
            {
                { production1, [] },
                { production2, [] },
                { production3, [] },
                { production4, [] },
                { production5, [] },
                { production6, [] },
                { production7, [] },
            };
            SyntaxDirectedTranslationScheme scheme = new(S, definition);

            HashSet<Symbol> expectedTerminals = [a, b, c];
            HashSet<Symbol> expectedNonterminals = [S, A, B, C];
            Assert.Multiple(() =>
            {
                Assert.That(scheme.Terminals, Is.EquivalentTo(expectedTerminals));
                Assert.That(scheme.Nonterminals, Is.EquivalentTo(expectedNonterminals));
            });

            Dictionary<Symbol, HashSet<Symbol>> expectedFirstSets = new()
            {
                { S, [a, b] },
                { A, [a, b] },
                { B, [b, Symbol.EPSILON] },
                { C, [a, c] },
            };
            Assert.Multiple(() =>
            {
                foreach (KeyValuePair<Symbol, HashSet<Symbol>> pair in expectedFirstSets)
                {
                    Assert.That(scheme.First[pair.Key], Is.EquivalentTo(pair.Value));
                }
            });

            Dictionary<Symbol, HashSet<Symbol>> expectedFollowSets = new()
            {
                { S, [a, b, c, Symbol.END] },
                { A, [a, b, c] },
                { B, [a, c] },
                { C, [a, b, c, Symbol.END] },
            };
            Assert.Multiple(() =>
            {
                foreach (KeyValuePair<Symbol, HashSet<Symbol>> pair in expectedFollowSets)
                {
                    Assert.That(scheme.Follow[pair.Key], Is.EquivalentTo(pair.Value));
                }
            });

            LLParsingTable table = new(scheme);

            Dictionary<(Symbol Nonterminal, Symbol Terminal), Production?> expectedProductions = new()
            {
                { (S, a), production1 },
                { (S, b), production1 },
                { (S, c), null },
                { (S, Symbol.END), null },
                { (A, a), production2 },
                { (A, b), production3 },
                { (A, c), null },
                { (A, Symbol.END), null },
                { (B, a), production5 },
                { (B, b), production4 },
                { (B, c), production5 },
                { (B, Symbol.END), null },
                { (C, a), production7 },
                { (C, b), null },
                { (C, c), production6 },
                { (C, Symbol.END), null },
            };
            Assert.Multiple(() =>
            {
                foreach (KeyValuePair<(Symbol Nonterminal, Symbol Terminal), Production?> pair in expectedProductions)
                {
                    Production? production = table.GetProduction(pair.Key.Nonterminal, pair.Key.Terminal);
                    Assert.That(production, Is.EqualTo(pair.Value));
                }
            });
        }

        [Test]
        public void SyntaxArithmeticGrammar()
        {
            /* --- Grammaire étudiée tout au long du module sur le Top-Down parsing. --- */

            Symbol E = new("E", SymbolType.Nonterminal);
            Symbol EPrime = new("E'", SymbolType.Nonterminal);
            Symbol T = new("T", SymbolType.Nonterminal);
            Symbol TPrime = new("T'", SymbolType.Nonterminal);
            Symbol F = new("F", SymbolType.Nonterminal);

            Symbol plus = new("+", SymbolType.Terminal);
            Symbol times = new("*", SymbolType.Terminal);
            Symbol leftParenthesis = new("(", SymbolType.Terminal);
            Symbol rightParenthesis = new(")", SymbolType.Terminal);
            Symbol id = new("id", SymbolType.Terminal);

            Production production1 = new(E, [T, EPrime]);
            Production production2 = new(EPrime, [plus, T, EPrime]);
            Production production3 = new(EPrime, [Symbol.EPSILON]);
            Production production4 = new(T, [F, TPrime]);
            Production production5 = new(TPrime, [times, F, TPrime]);
            Production production6 = new(TPrime, [Symbol.EPSILON]);
            Production production7 = new(F, [leftParenthesis, E, rightParenthesis]);
            Production production8 = new(F, [id]);

            Dictionary<Production, HashSet<SemanticAction>> definition = new()
            {
                { production1, [] },
                { production2, [] },
                { production3, [] },
                { production4, [] },
                { production5, [] },
                { production6, [] },
                { production7, [] },
                { production8, [] },
            };
            SyntaxDirectedTranslationScheme scheme = new(E, definition);

            HashSet<Symbol> expectedTerminals = [plus, times, leftParenthesis, rightParenthesis, id];
            HashSet<Symbol> expectedNonterminals = [E, EPrime, T, TPrime, F];
            Assert.Multiple(() =>
            {
                Assert.That(scheme.Terminals, Is.EquivalentTo(expectedTerminals));
                Assert.That(scheme.Nonterminals, Is.EquivalentTo(expectedNonterminals));
            });

            Dictionary<Symbol, HashSet<Symbol>> expectedFirstSets = new()
            {
                { E, [leftParenthesis, id] },
                { EPrime, [plus, Symbol.EPSILON] },
                { T, [leftParenthesis, id] },
                { TPrime, [times, Symbol.EPSILON] },
                { F, [leftParenthesis, id] },
            };
            Assert.Multiple(() =>
            {
                foreach (KeyValuePair<Symbol, HashSet<Symbol>> pair in expectedFirstSets)
                {
                    Assert.That(scheme.First[pair.Key], Is.EquivalentTo(pair.Value));
                }
            });

            Dictionary<Symbol, HashSet<Symbol>> expectedFollowSets = new()
            {
                { E, [rightParenthesis, Symbol.END] },
                { EPrime, [rightParenthesis, Symbol.END] },
                { T, [plus, rightParenthesis, Symbol.END] },
                { TPrime, [plus, rightParenthesis, Symbol.END] },
                { F, [plus, times, rightParenthesis, Symbol.END] },
            };
            Assert.Multiple(() =>
            {
                foreach (KeyValuePair<Symbol, HashSet<Symbol>> pair in expectedFollowSets)
                {
                    Assert.That(scheme.Follow[pair.Key], Is.EquivalentTo(pair.Value));
                }
            });

            LLParsingTable table = new(scheme);

            Dictionary<(Symbol Nonterminal, Symbol Terminal), Production?> expectedProductions = new()
            {
                { (E, id), production1 },
                { (E, plus), null },
                { (E, times), null },
                { (E, leftParenthesis), production1 },
                { (E, rightParenthesis), null },
                { (E, Symbol.END), null },
                { (EPrime, id), null },
                { (EPrime, plus), production2 },
                { (EPrime, times), null },
                { (EPrime, leftParenthesis), null },
                { (EPrime, rightParenthesis), production3 },
                { (EPrime, Symbol.END), production3 },
                { (T, id), production4 },
                { (T, plus), null },
                { (T, times), null },
                { (T, leftParenthesis), production4 },
                { (T, rightParenthesis), null },
                { (T, Symbol.END), null },
                { (TPrime, id), null },
                { (TPrime, plus), production6 },
                { (TPrime, times), production5 },
                { (TPrime, leftParenthesis), null },
                { (TPrime, rightParenthesis), production6 },
                { (TPrime, Symbol.END), production6 },
                { (F, id), production8 },
                { (F, plus), null },
                { (F, times), null },
                { (F, leftParenthesis), production7 },
                { (F, rightParenthesis), null },
                { (F, Symbol.END), null },
            };
            Assert.Multiple(() =>
            {
                foreach (KeyValuePair<(Symbol Nonterminal, Symbol Terminal), Production?> pair in expectedProductions)
                {
                    Production? production = table.GetProduction(pair.Key.Nonterminal, pair.Key.Terminal);
                    Assert.That(production, Is.EqualTo(pair.Value));
                }
            });

            TopDownParser parser = new(table);

            // Input : a + b * c $
            Token token1 = new(id, "a");
            Token token2 = new(plus, "+");
            Token token3 = new(id, "b");
            Token token4 = new(times, "*");
            Token token5 = new(id, "c");
            Token token6 = new(Symbol.END, null);
            List<Token> tokens = [token1, token2, token3, token4, token5, token6];
            ParseNode root = parser.Parse(tokens);

            ParseNode node1 = new(Symbol.EPSILON, null);
            ParseNode node2 = new(EPrime, production3);
            node2.Children.Add(node1);
            ParseNode node3 = new(Symbol.EPSILON, null);
            ParseNode node4 = new(TPrime, production6);
            node4.Children.Add(node3);
            ParseNode node5 = new(id, null);
            ParseNode node6 = new(F, production8);
            node6.Children.Add(node5);
            ParseNode node7 = new(times, null);
            ParseNode node8 = new(TPrime, production5);
            node8.Children.Add(node7);
            node8.Children.Add(node6);
            node8.Children.Add(node4);
            ParseNode node9 = new(id, null);
            ParseNode node10 = new(F, production8);
            node10.Children.Add(node9);
            ParseNode node11 = new(T, production4);
            node11.Children.Add(node10);
            node11.Children.Add(node8);
            ParseNode node12 = new(plus, null);
            ParseNode node13 = new(EPrime, production2);
            node13.Children.Add(node12);
            node13.Children.Add(node11);
            node13.Children.Add(node2);
            ParseNode node14 = new(Symbol.EPSILON, null);
            ParseNode node15 = new(TPrime, production6);
            node15.Children.Add(node14);
            ParseNode node16 = new(id, null);
            ParseNode node17 = new(F, production8);
            node17.Children.Add(node16);
            ParseNode node18 = new(T, production4);
            node18.Children.Add(node17);
            node18.Children.Add(node15);
            ParseNode expectedRoot = new(E, production1);
            expectedRoot.Children.Add(node18);
            expectedRoot.Children.Add(node13);
            Assert.Multiple(() =>
            {
                AssertParseTreesAreEqual(expectedRoot, root);
            });
        }

        [Test]
        public void SemanticArithmeticGrammar()
        {
            /* --- Grammaire L-attribuée étudiée pendant le module sur la traduction. --- */

            SemanticAttribute<int> inh = new("inh", AttributeType.Inherited);
            SemanticAttribute<int> syn = new("syn", AttributeType.Synthesized);
            SemanticAttribute<int> val = new("val", AttributeType.Synthesized);

            Symbol T = new("T", SymbolType.Nonterminal);
            Symbol TPrime = new("T'", SymbolType.Nonterminal);
            Symbol F = new("F", SymbolType.Nonterminal);

            Symbol times = new("*", SymbolType.Terminal);
            Symbol id = new("id", SymbolType.Terminal);

            Production production1 = new(T, [F, TPrime]);
            Production production2 = new(TPrime, [times, F, TPrime]);
            Production production3 = new(TPrime, [Symbol.EPSILON]);
            Production production4 = new(F, [id]);

            // T'.inh = F.val
            AttributeBinding<int> binding1 = new(TPrime, 0, inh);
            AttributeBinding<int> binding2 = new(F, 0, val);
            SemanticAction action1 = new(
                binding1,
                [binding2],
                (ParseNode node) =>
                {
                    int value = GetAttributeValue(node, binding2);
                    SetAttributeValue(node, binding1, value);
                }
            );
            // T.val = T'.syn
            AttributeBinding<int> binding3 = new(T, 0, val);
            AttributeBinding<int> binding4 = new(TPrime, 0, syn);
            SemanticAction action2 = new(
                binding3,
                [binding4],
                (ParseNode node) =>
                {
                    int value = GetAttributeValue(node, binding4);
                    SetAttributeValue(node, binding3, value);
                }
            );
            // T'_1.inh = T'.inh * F.val
            AttributeBinding<int> binding5 = new(TPrime, 1, inh);
            AttributeBinding<int> binding6 = new(TPrime, 0, inh);
            AttributeBinding<int> binding7 = new(F, 0, val);
            SemanticAction action3 = new(
                binding5,
                [binding6, binding7],
                (ParseNode node) =>
                {
                    int value0 = GetAttributeValue(node, binding6);
                    int value1 = GetAttributeValue(node, binding7);
                    SetAttributeValue(node, binding5, value0 * value1);
                }
            );
            // T'.syn = T'_1.syn
            AttributeBinding<int> binding8 = new(TPrime, 0, syn);
            AttributeBinding<int> binding9 = new(TPrime, 1, syn);
            SemanticAction action4 = new(
                binding8,
                [binding9],
                (ParseNode node) =>
                {
                    int value = GetAttributeValue(node, binding9);
                    SetAttributeValue(node, binding8, value);
                }
            );
            // T'.syn = T'inh
            AttributeBinding<int> binding10 = new(TPrime, 0, syn);
            AttributeBinding<int> binding11 = new(TPrime, 0, inh);
            SemanticAction action5 = new(
                binding10,
                [binding11],
                (ParseNode node) =>
                {
                    int value = GetAttributeValue(node, binding11);
                    SetAttributeValue(node, binding10, value);
                }
            );
            // F.val = (int)id.lexval
            AttributeBinding<int> binding12 = new(F, 0, val);
            AttributeBinding<string> binding13 = new(id, 0, SemanticAttribute.LEXICAL_VALUE);
            SemanticAction action6 = new(
                binding12,
                [binding13],
                (ParseNode node) =>
                {
                    string value = GetAttributeValue(node, binding13);
                    SetAttributeValue(node, binding12, int.Parse(value));
                }
            );

            Dictionary<Production, HashSet<SemanticAction>> definition = new()
            {
                { production1, [action1, action2] },
                { production2, [action3, action4] },
                { production3, [action5] },
                { production4, [action6] },
            };
            SyntaxDirectedTranslationScheme scheme = new(T, definition);

            Dictionary<Production, List<SemanticAction>> expectedRules = new()
            {
                { production1, [action1, action2] },
                { production2, [action3, action4] },
                { production3, [action5] },
                { production4, [action6] },
            };
            Assert.Multiple(() =>
            {
                foreach (KeyValuePair<Production, List<SemanticAction>> pair in expectedRules)
                {
                    Assert.That(scheme.Rules[pair.Key], Is.EquivalentTo(pair.Value));
                }
            });

            LLParsingTable table = new(scheme);
            TopDownParser parser = new(table);

            // Input : 3 * 5 $
            Token token1 = new(id, "3");
            Token token2 = new(times, null);
            Token token3 = new(id, "5");
            Token token4 = new(Symbol.END, null);
            List<Token> tokens = [token1, token2, token3, token4];
            ParseNode root = parser.Parse(tokens);

            ParseNode node1 = new(Symbol.EPSILON, null);
            ParseNode node2 = new(TPrime, production3);
            node2.Children.Add(node1);
            ParseNode node3 = new(id, null);
            ParseNode node4 = new(F, production4);
            node4.Children.Add(node3);
            ParseNode node5 = new(times, null);
            ParseNode node6 = new(TPrime, production2);
            node6.Children.Add(node5);
            node6.Children.Add(node4);
            node6.Children.Add(node2);
            ParseNode node7 = new(id, null);
            ParseNode node8 = new(F, production4);
            node8.Children.Add(node7);
            ParseNode node9 = new(T, production1);
            node9.Children.Add(node8);
            node9.Children.Add(node6);
            Assert.Multiple(() =>
            {
                AssertParseTreesAreEqual(node9, root);
            });

            LexicalAnalyzer lexicalAnalyzer = new(tokens);
            SemanticAnalyzer semanticAnalyzer = new(lexicalAnalyzer, scheme);
            semanticAnalyzer.Annotate(node9);
            Assert.Multiple(() =>
            {
                // id.lexval
                Assert.That(GetAttributeValue(node7, binding13), Is.EqualTo("3"));
                // F.val
                Assert.That(GetAttributeValue(node8, binding12), Is.EqualTo(3));
                Assert.That(GetAttributeValue(node9, binding2), Is.EqualTo(3));
                // T'.inh
                Assert.That(GetAttributeValue(node9, binding1), Is.EqualTo(3));
                Assert.That(GetAttributeValue(node6, binding6), Is.EqualTo(3));
                // id.lexval
                Assert.That(GetAttributeValue(node3, binding13), Is.EqualTo("5"));
                // F.val
                Assert.That(GetAttributeValue(node4, binding12), Is.EqualTo(5));
                Assert.That(GetAttributeValue(node6, binding7), Is.EqualTo(5));
                // T'.inh
                Assert.That(GetAttributeValue(node6, binding5), Is.EqualTo(15));
                Assert.That(GetAttributeValue(node2, binding11), Is.EqualTo(15));
                // T'.syn
                Assert.That(GetAttributeValue(node2, binding10), Is.EqualTo(15));
                Assert.That(GetAttributeValue(node6, binding9), Is.EqualTo(15));
                // T'.syn
                Assert.That(GetAttributeValue(node6, binding8), Is.EqualTo(15));
                Assert.That(GetAttributeValue(node9, binding4), Is.EqualTo(15));
                // T.val
                Assert.That(GetAttributeValue(node9, binding3), Is.EqualTo(15));
            });
        }

        [Test]
        public void ControlFlowBackpatchingGrammar()
        {
            /* --- Grammaire étudiée pendant le module sur le backpatching.
             * Simplifiée et adaptée pour en faire une grammare LL(1) L-attribuée. --- */

            SemanticAttribute<HashSet<int>> inhTrueList = new("itl", AttributeType.Inherited);
            SemanticAttribute<HashSet<int>> inhFalseList = new("ifl", AttributeType.Inherited);
            SemanticAttribute<HashSet<int>> trueList = new("tl", AttributeType.Synthesized);
            SemanticAttribute<HashSet<int>> falseList = new("fl", AttributeType.Synthesized);
            SemanticAttribute<int> labelId = new("id", AttributeType.Synthesized);
            SemanticAttribute<List<IInstruction>> code = new("code", AttributeType.Synthesized);

            Symbol S = new("S", SymbolType.Nonterminal);
            Symbol SPrime = new("S'", SymbolType.Nonterminal);
            Symbol B = new("B", SymbolType.Nonterminal);
            Symbol BPrime = new("B'", SymbolType.Nonterminal);
            Symbol R = new("R", SymbolType.Nonterminal);
            Symbol M = new("M", SymbolType.Nonterminal);

            Symbol _if = new("if", SymbolType.Terminal);
            Symbol _else = new("else", SymbolType.Terminal);
            Symbol or = new("or", SymbolType.Terminal);
            Symbol and = new("and", SymbolType.Terminal);
            Symbol id = new("id", SymbolType.Terminal);
            Symbol op = new("op", SymbolType.Terminal);
            Symbol affect = new("affect", SymbolType.Terminal);
            Symbol nb = new("nb", SymbolType.Terminal);
            Symbol leftParenthesis = new("(", SymbolType.Terminal);
            Symbol rightParenthesis = new(")", SymbolType.Terminal);

            Production production1 = new(S, [_if, leftParenthesis, B, rightParenthesis, M, SPrime, _else, M, SPrime]);
            Production production2 = new(B, [BPrime, R]);
            Production production3 = new(BPrime, [id, op, id]);
            Production production4 = new(R, [or, M, B]);
            Production production5 = new(R, [and, M, B]);
            Production production6 = new(R, [Symbol.EPSILON]);
            Production production7 = new(M, [Symbol.EPSILON]);
            Production production8 = new(SPrime, [id, affect, nb]);

            #region SemanticActions

            // Backpatch(B.tl, M_0.id)
            AttributeBinding<List<IInstruction>> binding00 = new(S, 0, code);
            AttributeBinding<HashSet<int>> binding1 = new(B, 0, trueList);
            AttributeBinding<int> binding2 = new(M, 0, labelId);
            SemanticAction action1 = new(
                binding00,
                [binding1, binding2],
                (ParseNode node) =>
                {
                    HashSet<int> list = GetAttributeValue(node, binding1);
                    int value = GetAttributeValue(node, binding2);
                    IntermediateCodeManager.Backpatch(list, value);
                }
            );
            // Backpatch(B.fl, M_1.id)
            AttributeBinding<List<IInstruction>> binding01 = new(S, 0, code);
            AttributeBinding<HashSet<int>> binding3 = new(B, 0, falseList);
            AttributeBinding<int> binding4 = new(M, 1, labelId);
            SemanticAction action2 = new(
                binding01,
                [binding3, binding4],
                (ParseNode node) =>
                {
                    HashSet<int> list = GetAttributeValue(node, binding3);
                    int value = GetAttributeValue(node, binding4);
                    IntermediateCodeManager.Backpatch(list, value);
                }
            );
            // S.code = B.code + S'_0.code + S'_1.code
            AttributeBinding<List<IInstruction>> binding5 = new(S, 0, code);
            AttributeBinding<List<IInstruction>> binding6 = new(B, 0, code);
            AttributeBinding<List<IInstruction>> binding7 = new(SPrime, 0, code);
            AttributeBinding<List<IInstruction>> binding8 = new(SPrime, 1, code);
            SemanticAction action3 = new(
                binding5,
                [binding6, binding7, binding8],
                (ParseNode node) =>
                {
                    List<IInstruction> valueB = GetAttributeValue(node, binding6);
                    List<IInstruction> valueSPrime0 = GetAttributeValue(node, binding7);
                    List<IInstruction> valueSPrime1 = GetAttributeValue(node, binding8);
                    List<IInstruction> value = [.. valueB, .. valueSPrime0, .. valueSPrime1];
                    SetAttributeValue(node, binding5, value);
                }
            );
            // B.tl = R.tl
            AttributeBinding<HashSet<int>> binding9 = new(B, 0, trueList);
            AttributeBinding<HashSet<int>> binding10 = new(R, 0, trueList);
            SemanticAction action4 = new(
                binding9,
                [binding10],
                (ParseNode node) =>
                {
                    HashSet<int> value = GetAttributeValue(node, binding10);
                    SetAttributeValue(node, binding9, value);
                }
            );
            // B.fl = R.fl
            AttributeBinding<HashSet<int>> binding11 = new(B, 0, falseList);
            AttributeBinding<HashSet<int>> binding12 = new(R, 0, falseList);
            SemanticAction action5 = new(
                binding11,
                [binding12],
                (ParseNode node) =>
                {
                    HashSet<int> value = GetAttributeValue(node, binding12);
                    SetAttributeValue(node, binding11, value);
                }
            );
            // R.itl = B'.tl
            AttributeBinding<HashSet<int>> binding13 = new(R, 0, inhTrueList);
            AttributeBinding<HashSet<int>> binding14 = new(BPrime, 0, trueList);
            SemanticAction action6 = new(
                binding13,
                [binding14],
                (ParseNode node) =>
                {
                    HashSet<int> value = GetAttributeValue(node, binding14);
                    SetAttributeValue(node, binding13, value);
                }
            );
            // R.ifl = B'.fl
            AttributeBinding<HashSet<int>> binding15 = new(R, 0, inhFalseList);
            AttributeBinding<HashSet<int>> binding16 = new(BPrime, 0, falseList);
            SemanticAction action7 = new(
                binding15,
                [binding16],
                (ParseNode node) =>
                {
                    HashSet<int> value = GetAttributeValue(node, binding16);
                    SetAttributeValue(node, binding15, value);
                }
            );
            // B.code = B'.code + R.code
            AttributeBinding<List<IInstruction>> binding17 = new(B, 0, code);
            AttributeBinding<List<IInstruction>> binding18 = new(BPrime, 0, code);
            AttributeBinding<List<IInstruction>> binding19 = new(R, 0, code);
            SemanticAction action8 = new(
                binding17,
                [binding18, binding19],
                (ParseNode node) =>
                {
                    List<IInstruction> valueBPrime = GetAttributeValue(node, binding18);
                    List<IInstruction> valueR = GetAttributeValue(node, binding19);
                    List<IInstruction> value = [.. valueBPrime, .. valueR];
                    SetAttributeValue(node, binding17, value);
                }
            );
            // B'.tl = MakeList(nextId)
            AttributeBinding<HashSet<int>> binding20 = new(BPrime, 0, trueList);
            SemanticAction action9 = new(
                binding20,
                [],
                (ParseNode node) =>
                {
                    HashSet<int> value = [IntermediateCodeManager.NextId];
                    SetAttributeValue(node, binding20, value);
                }
            );
            // B'.fl = MakeList(nextId + 1)
            AttributeBinding<HashSet<int>> binding21 = new(BPrime, 0, falseList);
            AttributeBinding<HashSet<int>> binding04 = new(BPrime, 0, trueList);
            SemanticAction action10 = new(
                binding21,
                [binding04],
                (ParseNode node) =>
                {
                    HashSet<int> value = [IntermediateCodeManager.NextId + 1];
                    SetAttributeValue(node, binding21, value);
                }
            );
            // B'.code = "if id_0.val op.val id_1.val goto _" + "goto _"
            AttributeBinding<List<IInstruction>> binding22 = new(BPrime, 0, code);
            AttributeBinding<HashSet<int>> binding05 = new(BPrime, 0, falseList);
            AttributeBinding<string> binding23 = new(id, 0, SemanticAttribute.LEXICAL_VALUE);
            AttributeBinding<string> binding24 = new(op, 0, SemanticAttribute.LEXICAL_VALUE);
            AttributeBinding<string> binding25 = new(id, 1, SemanticAttribute.LEXICAL_VALUE);
            SemanticAction action11 = new(
                binding22,
                [binding05, binding23, binding24, binding25],
                (ParseNode node) =>
                {
                    string id0Val = GetAttributeValue(node, binding23);
                    string opVal = GetAttributeValue(node, binding24);
                    string id1Val = GetAttributeValue(node, binding25);
                    ConditionalJump conditionalJump = new(id0Val, opVal, id1Val);
                    UnconditionalJump unconditionalJump = new();
                    List<IInstruction> value = [conditionalJump, unconditionalJump];
                    SetAttributeValue(node, binding22, value);
                }
            );
            // R.tl = Merge(R.itl, B.tl)
            AttributeBinding<HashSet<int>> binding26 = new(R, 0, trueList);
            AttributeBinding<HashSet<int>> binding27 = new(R, 0, inhTrueList);
            AttributeBinding<HashSet<int>> binding28 = new(B, 0, trueList);
            SemanticAction action12 = new(
                binding26,
                [binding27, binding28],
                (ParseNode node) =>
                {
                    HashSet<int> valueR = GetAttributeValue(node, binding27);
                    HashSet<int> valueB = GetAttributeValue(node, binding28);
                    HashSet<int> value = [.. valueR, .. valueB];
                    SetAttributeValue(node, binding26, value);
                }
            );
            // R.fl = B.fl
            AttributeBinding<HashSet<int>> binding29 = new(R, 0, falseList);
            AttributeBinding<HashSet<int>> binding30 = new(B, 0, falseList);
            SemanticAction action13 = new(
                binding29,
                [binding30],
                (ParseNode node) =>
                {
                    HashSet<int> value = GetAttributeValue(node, binding30);
                    SetAttributeValue(node, binding29, value);
                }
            );
            // Backpatch(R.ifl, M.id)
            AttributeBinding<List<IInstruction>> binding02 = new(R, 0, code);
            AttributeBinding<HashSet<int>> binding31 = new(R, 0, inhFalseList);
            AttributeBinding<int> binding32 = new(M, 0, labelId);
            SemanticAction action14 = new(
                binding02,
                [binding31, binding32],
                (ParseNode node) =>
                {
                    HashSet<int> list = GetAttributeValue(node, binding31);
                    int value = GetAttributeValue(node, binding32);
                    IntermediateCodeManager.Backpatch(list, value);
                }
            );
            // R.code = B.code
            AttributeBinding<List<IInstruction>> binding33 = new(R, 0, code);
            AttributeBinding<List<IInstruction>> binding34 = new(B, 0, code);
            SemanticAction action15 = new(
                binding33,
                [binding34],
                (ParseNode node) =>
                {
                    List<IInstruction> value = GetAttributeValue(node, binding34);
                    SetAttributeValue(node, binding33, value);
                }
            );
            // R.tl = B.tl
            AttributeBinding<HashSet<int>> binding35 = new(R, 0, trueList);
            AttributeBinding<HashSet<int>> binding36 = new(B, 0, trueList);
            SemanticAction action16 = new(
                binding35,
                [binding36],
                (ParseNode node) =>
                {
                    HashSet<int> value = GetAttributeValue(node, binding36);
                    SetAttributeValue(node, binding35, value);
                }
            );
            // R.fl = Merge(R.ifl, B.fl)
            AttributeBinding<HashSet<int>> binding37 = new(R, 0, falseList);
            AttributeBinding<HashSet<int>> binding38 = new(R, 0, inhFalseList);
            AttributeBinding<HashSet<int>> binding39 = new(B, 0, falseList);
            SemanticAction action17 = new(
                binding37,
                [binding38, binding39],
                (ParseNode node) =>
                {
                    HashSet<int> valueR = GetAttributeValue(node, binding38);
                    HashSet<int> valueB = GetAttributeValue(node, binding39);
                    HashSet<int> value = [.. valueR, .. valueB];
                    SetAttributeValue(node, binding37, value);
                }
            );
            // Backpatch(R.itl, M.id)
            AttributeBinding<List<IInstruction>> binding03 = new(R, 0, code);
            AttributeBinding<HashSet<int>> binding40 = new(R, 0, inhTrueList);
            AttributeBinding<int> binding41 = new(M, 0, labelId);
            SemanticAction action18 = new(
                binding03,
                [binding40, binding41],
                (ParseNode node) =>
                {
                    HashSet<int> list = GetAttributeValue(node, binding40);
                    int value = GetAttributeValue(node, binding41);
                    IntermediateCodeManager.Backpatch(list, value);
                }
            );
            // R.code = B.code
            AttributeBinding<List<IInstruction>> binding42 = new(R, 0, code);
            AttributeBinding<List<IInstruction>> binding43 = new(B, 0, code);
            SemanticAction action19 = new(
                binding42,
                [binding43],
                (ParseNode node) =>
                {
                    List<IInstruction> value = GetAttributeValue(node, binding43);
                    SetAttributeValue(node, binding42, value);
                }
            );
            // R.tl = R.itl
            AttributeBinding<HashSet<int>> binding44 = new(R, 0, trueList);
            AttributeBinding<HashSet<int>> binding45 = new(R, 0, inhTrueList);
            SemanticAction action20 = new(
                binding44,
                [binding45],
                (ParseNode node) =>
                {
                    HashSet<int> value = GetAttributeValue(node, binding45);
                    SetAttributeValue(node, binding44, value);
                }
            );
            // R.fl = R.ifl
            AttributeBinding<HashSet<int>> binding46 = new(R, 0, falseList);
            AttributeBinding<HashSet<int>> binding47 = new(R, 0, inhFalseList);
            SemanticAction action21 = new(
                binding46,
                [binding47],
                (ParseNode node) =>
                {
                    HashSet<int> value = GetAttributeValue(node, binding47);
                    SetAttributeValue(node, binding46, value);
                }
            );
            // R.code = ""
            AttributeBinding<List<IInstruction>> binding48 = new(R, 0, code);
            SemanticAction action22 = new(
                binding48,
                [],
                (ParseNode node) =>
                {
                    SetAttributeValue(node, binding48, []);
                }
            );
            // M.id = nextId
            AttributeBinding<int> binding49 = new(M, 0, labelId);
            SemanticAction action23 = new(
                binding49,
                [],
                (ParseNode node) =>
                {
                    SetAttributeValue(node, binding49, IntermediateCodeManager.NextId);
                }
            );
            // S'.code = "id.val = nb.val"
            AttributeBinding<List<IInstruction>> binding50 = new(SPrime, 0, code);
            AttributeBinding<string> binding51 = new(id, 0, SemanticAttribute.LEXICAL_VALUE);
            AttributeBinding<string> binding52 = new(nb, 0, SemanticAttribute.LEXICAL_VALUE);
            SemanticAction action24 = new(
                binding50,
                [binding51, binding52],
                (ParseNode node) =>
                {
                    string idVal = GetAttributeValue(node, binding51);
                    string nbVal = GetAttributeValue(node, binding52);
                    Assignment assignment = new(idVal, nbVal);
                    SetAttributeValue(node, binding50, [assignment]);
                }
            );

            #endregion SemanticActions

            Dictionary<Production, HashSet<SemanticAction>> definition = new()
            {
                { production1, [action1, action2, action3] },
                { production2, [action4, action5, action6, action7, action8] },
                { production3, [action9, action10, action11] },
                { production4, [action12, action13, action14, action15] },
                { production5, [action16, action17, action18, action19] },
                { production6, [action20, action21, action22] },
                { production7, [action23] },
                { production8, [action24] },
            };
            SyntaxDirectedTranslationScheme scheme = new(S, definition);

            LLParsingTable table = new(scheme);
            TopDownParser parser = new(table);

            // Input : if ( x < 100 || x > 200 && x != y ) a = 4 else a = 8
            Token token1 = new(_if, null);
            Token token2 = new(leftParenthesis, null);
            Token token3 = new(id, "x");
            Token token4 = new(op, "<");
            Token token5 = new(id, "100");
            Token token6 = new(or, null);
            Token token7 = new(id, "x");
            Token token8 = new(op, ">");
            Token token9 = new(id, "200");
            Token token10 = new(and, null);
            Token token11 = new(id, "x");
            Token token12 = new(op, "!=");
            Token token13 = new(id, "y");
            Token token14 = new(rightParenthesis, null);
            Token token15 = new(id, "a");
            Token token16 = new(affect, null);
            Token token17 = new(nb, "4");
            Token token18 = new(_else, null);
            Token token19 = new(id, "a");
            Token token20 = new(affect, null);
            Token token21 = new(nb, "8");
            Token token22 = new(Symbol.END, null);
            List<Token> tokens = [token1, token2, token3, token4, token5, token6, token7, token8, token9, token10, token11, token12, token13, token14, token15, token16, token17, token18, token19, token20, token21, token22];
            ParseNode root = parser.Parse(tokens);

            LexicalAnalyzer lexicalAnalyzer = new(tokens);
            SemanticAnalyzer semanticAnalyzer = new(lexicalAnalyzer, scheme);
            semanticAnalyzer.Annotate(root);

            List<IInstruction>? rootCode = root.GetAttributeValue(code);
            Assert.Multiple(() =>
            {
                Assert.That(rootCode, Is.Not.Null);

                Assert.That(rootCode[0], Is.TypeOf(typeof(ConditionalJump)));
                ConditionalJump instruction100 = (ConditionalJump)rootCode[0];
                Assert.That(rootCode[1], Is.TypeOf(typeof(UnconditionalJump)));
                UnconditionalJump instruction101 = (UnconditionalJump)rootCode[1];
                Assert.That(rootCode[2], Is.TypeOf(typeof(ConditionalJump)));
                ConditionalJump instruction102 = (ConditionalJump)rootCode[2];
                Assert.That(rootCode[3], Is.TypeOf(typeof(UnconditionalJump)));
                UnconditionalJump instruction103 = (UnconditionalJump)rootCode[3];
                Assert.That(rootCode[4], Is.TypeOf(typeof(ConditionalJump)));
                ConditionalJump instruction104 = (ConditionalJump)rootCode[4];
                Assert.That(rootCode[5], Is.TypeOf(typeof(UnconditionalJump)));
                UnconditionalJump instruction105 = (UnconditionalJump)rootCode[5];
                Assert.That(rootCode[6], Is.TypeOf(typeof(Assignment)));
                Assignment instruction106 = (Assignment)rootCode[6];
                Assert.That(rootCode[7], Is.TypeOf(typeof(Assignment)));
                Assignment instruction107 = (Assignment)rootCode[7];

                // 100
                Assert.That(instruction100.Label.Id, Is.EqualTo(100));
                Assert.That(instruction100.JumpLabel, Is.EqualTo(instruction106.Label));
                // 101
                Assert.That(instruction101.Label.Id, Is.EqualTo(101));
                Assert.That(instruction101.JumpLabel, Is.EqualTo(instruction102.Label));
                // 102
                Assert.That(instruction102.Label.Id, Is.EqualTo(102));
                Assert.That(instruction102.JumpLabel, Is.EqualTo(instruction104.Label));
                // 103
                Assert.That(instruction103.Label.Id, Is.EqualTo(103));
                Assert.That(instruction103.JumpLabel, Is.EqualTo(instruction107.Label));
                // 104
                Assert.That(instruction104.Label.Id, Is.EqualTo(104));
                Assert.That(instruction104.JumpLabel, Is.EqualTo(instruction106.Label));
                // 105
                Assert.That(instruction105.Label.Id, Is.EqualTo(105));
                Assert.That(instruction105.JumpLabel, Is.EqualTo(instruction107.Label));
                // 106
                Assert.That(instruction106.Label.Id, Is.EqualTo(106));
                // 107
                Assert.That(instruction107.Label.Id, Is.EqualTo(107));
            });
        }

        private static void AssertParseTreesAreEqual(ParseNode expected, ParseNode actual)
        {
            Assert.Multiple(() =>
            {
                Assert.That(expected.Symbol, Is.EqualTo(actual.Symbol));
                Assert.That(expected.Production, Is.EqualTo(actual.Production));
            });
            for (int i = 0; i < expected.Children.Count; i++)
            {
                AssertParseTreesAreEqual(expected.Children[i], actual.Children[i]);
            }
        }

        private static T GetAttributeValue<T>(ParseNode node, AttributeBinding<T> binding)
        {
            return node.GetBindedNode(binding).GetAttributeValue(binding.Attribute)
                ?? throw new Exception("Unexpected null value.");
        }

        private static void SetAttributeValue<T>(ParseNode node, AttributeBinding<T> binding, T value)
        {
            node.GetBindedNode(binding).SetAttributeValue(binding.Attribute, value);
        }
    }
}
