using System.Collections;
using NonStandard;

namespace System.Collections
{
    public sealed partial class ArrayList
    {
        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern ArrayList Concat(params object[] objects);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern bool Every(ArrayFilterCallback filterCallback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern bool Every(ArrayItemFilterCallback itemFilterCallback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern Array Filter(ArrayFilterCallback filterCallback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern Array Filter(ArrayItemFilterCallback itemFilterCallback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern void ForEach(ArrayCallback callback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern void ForEach(ArrayItemCallback itemCallback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern Array GetRange(int index);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern Array GetRange(int index, int count);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern int IndexOf(object item, int startIndex);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern void Insert(int index, object item);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        // ICollection c instead of params object[] itmes
        public extern void InsertRange(int index, params object[] items);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern string Join();

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern string Join(string delimiter);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern int LastIndexOf(object item);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern int LastIndexOf(object item, int fromIndex);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern Array Map(ArrayMapCallback mapCallback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern Array Map(ArrayItemMapCallback mapItemCallback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern static ArrayList Parse(string s);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern object Reduce(ArrayReduceCallback callback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern object Reduce(ArrayReduceCallback callback, object initialValue);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern object Reduce(ArrayItemReduceCallback callback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern object Reduce(ArrayItemReduceCallback callback, object initialValue);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern object ReduceRight(ArrayReduceCallback callback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern object ReduceRight(ArrayReduceCallback callback, object initialValue);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern object ReduceRight(ArrayItemReduceCallback callback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern object ReduceRight(ArrayItemReduceCallback callback, object initialValue);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern Array RemoveRange(int index, int count);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern void Reverse();

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern object Shift();

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern Array Slice(int start);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern Array Slice(int start, int end);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern bool Some(ArrayFilterCallback filterCallback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern bool Some(ArrayItemFilterCallback itemFilterCallback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern void Sort();

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern void Sort(CompareCallback compareCallback);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern void Splice(int start, int deleteCount);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern void Splice(int start, int deleteCount, params object[] itemsToInsert);

        [Obsolete(ObsoleteConsts.MESSAGE_ON_OBSOLETE, ObsoleteConsts.ERROR_ON_OBSOLETE)]
        public extern void Unshift(params object[] items);
    }
}
