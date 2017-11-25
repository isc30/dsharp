// Guid.cs
// Script#/Libraries/CoreLib
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Runtime.CompilerServices;

namespace System
{
    [ScriptImport]
    [ScriptName("Guid")]
    public struct Guid : IEquatable<Guid>
    {
        [ScriptName(PreserveCase = true)]
        public extern static Guid NewGuid();

        [ScriptSkip]
        public extern override string ToString();

        public extern bool Equals(Guid other);
    }
}
