// Delegate.cs
// Script#/Libraries/CoreLib
// This source code is subject to terms and conditions of the Apache License, Version 2.0.
//

using System.Runtime.CompilerServices;

namespace System {

    [ScriptImport]
    public abstract class Delegate {

        protected Delegate(object target, string method) {
        }

        protected Delegate(Type target, string method) {
        }

        [ScriptAlias("ss.bindAdd")]
        public static Delegate Combine(Delegate a, Delegate b) {
            return null;
        }

        [ScriptAlias("ss.bind")]
        public static Delegate Create(Function f, object instance) {
            return null;
        }


        [ScriptAlias("ss.bindSub")]
        public static Delegate Remove(Delegate source, Delegate value) {
            return null;
        }
    }
}
