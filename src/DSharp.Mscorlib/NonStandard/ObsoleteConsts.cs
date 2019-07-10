namespace NonStandard
{
    internal static class ObsoleteConsts
    {
        public const string MESSAGE_ON_OBSOLETE = "Non CLR Compliant method";

#if net40
        public const bool ERROR_ON_OBSOLETE = false;
#else
        public const bool ERROR_ON_OBSOLETE = true;
#endif
    }
}
