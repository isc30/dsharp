using System.Collections;
using System.Runtime.CompilerServices;
using NonStandard;

namespace System.NonStandard
{
    public sealed partial class Type
    {
        [ScriptField]
        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern Dictionary Prototype { get; }
    }
}
