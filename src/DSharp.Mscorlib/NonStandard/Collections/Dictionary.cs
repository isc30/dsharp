using NonStandard;

namespace System.Collections
{
    public sealed partial class Dictionary
    {
        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern int Count { get; }

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern static Dictionary GetDictionary(object o);
    }
}
