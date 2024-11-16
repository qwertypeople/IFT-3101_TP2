using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Parsing
{
    public class LexicalAnalyzer(List<Token> tokens)
    {
        private readonly List<Token> _tokens = tokens;
        private int _index = 0;

        public string? GetNextValue(Symbol symbol)
        {
            Token token = _tokens[_index];
            if (token.Symbol != symbol)
            {
                throw new Exception("Unexpected error : The symbol provided does not match with the next token.");
            }
            _index++;
            return token.Value;
        }
    }
}
