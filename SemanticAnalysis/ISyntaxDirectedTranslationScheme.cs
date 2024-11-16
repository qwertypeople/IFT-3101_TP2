using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemanticAnalysis.Attributes;
using SemanticAnalysis.Parsing;

namespace SemanticAnalysis
{
    public interface ISyntaxDirectedTranslationScheme
    {
        Symbol StartSymbol { get; }
        HashSet<Symbol> Terminals { get; }
        HashSet<Symbol> Nonterminals { get; }
        Dictionary<Production, List<SemanticAction>> Rules { get; }
        Dictionary<Symbol, HashSet<Symbol>> First { get; }
        Dictionary<Symbol, HashSet<Symbol>> Follow { get; }

        HashSet<Symbol> FirstOfBody(List<Symbol> symbols);
    }
}
