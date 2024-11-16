using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SemanticAnalysis.Parsing;

namespace SemanticAnalysis.Attributes
{
    public class SemanticAnalyzer(LexicalAnalyzer lexicalAnalyzer, ISyntaxDirectedTranslationScheme scheme)
    {
        private readonly LexicalAnalyzer _lexicalAnalyzer = lexicalAnalyzer;
        private readonly ISyntaxDirectedTranslationScheme _scheme = scheme;

        public void Annotate(ParseNode node)
        {
            /* --- À COMPLÉTER (logique seulement) --- */
        }
    }
}
