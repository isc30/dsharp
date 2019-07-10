using NonStandard;

namespace System.Collections.Generic
{
    public sealed partial class Dictionary<TKey, TValue>
    {
        [Obsolete("This is only for use by the c# compiler, and cannot be used for generating script.", /* error */ true)]
        public extern bool TryGetValue(TKey key, out TValue value);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern static Dictionary<TKey, TValue> GetDictionary(object o);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator();

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        extern IEnumerator IEnumerable.GetEnumerator();

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern int Count { get; }
    }
}
