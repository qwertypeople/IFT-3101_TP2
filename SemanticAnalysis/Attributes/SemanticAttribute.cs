using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Attributes
{
    public enum AttributeType
    {
        Synthesized,
        Inherited,
    }

    public abstract class SemanticAttribute : ISemanticAttribute
    {
        public static readonly SemanticAttribute<string> LEXICAL_VALUE = new("lexval", AttributeType.Synthesized);

        public abstract string Name { get; }
    }

    public class SemanticAttribute<T>(string name, AttributeType type) : SemanticAttribute
    {
        public override string Name { get; } = name;
        public AttributeType Type { get; } = type;
    }
}
