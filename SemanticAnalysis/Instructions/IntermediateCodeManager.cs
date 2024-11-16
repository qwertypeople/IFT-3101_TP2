using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemanticAnalysis.Instructions
{
    public static class IntermediateCodeManager
    {
        public const int FIRST_ID = 100;

        public static int NextId => _nextId;

        private static int _nextId = FIRST_ID;
        /* --- À COMPLÉTER (logique seulement) --- */

        public static Label EmitLabel()
        {
            /* --- À COMPLÉTER (logique seulement) --- */

            return new(0);
        }

        public static void RegisterForBackpatching(Jump instruction)
        {
            /* --- À COMPLÉTER (logique et gestion des erreurs) --- */
        }

        public static void Backpatch(HashSet<int> list, int labelId)
        {
            /* --- À COMPLÉTER (logique et gestion des erreurs) --- */
        }
    }
}
