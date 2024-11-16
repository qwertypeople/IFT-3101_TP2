using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Parsing
{
    public enum SymbolType
    {
        Nonterminal,
        Terminal,
        Special,
    }

    public class Symbol(string name, SymbolType type)
    {
        public static readonly Symbol EPSILON = new("epsilon", SymbolType.Special);
        public static readonly Symbol END = new("$", SymbolType.Special);

        public string Name { get; } = name;
        public SymbolType Type { get; } = type;

        public bool IsNonterminal()
        {
            return Type == SymbolType.Nonterminal; 
        }

        public bool IsTerminal()
        {
            return Type == SymbolType.Terminal;
        }

        public bool IsSpecial() 
        { 
            return Type == SymbolType.Special;
        }

        public bool IsEpsilon()      
        {
            return IsSpecial() && Name == EPSILON.Name;
        }

        public bool IsEnd()
        {
            return IsSpecial() && Name == END.Name;
        }


    }
}
