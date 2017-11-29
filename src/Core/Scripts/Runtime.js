/*! Script# Runtime
 * Designed and licensed for use and distribution with Script#-generated scripts.
 * Copyright (c) 2012, Nikhil Kothari, and the Script# Project.
 * More information at http://scriptsharp.com
 */

"use strict"; 

#include "Runtime\Polyfills\Object.js"

(function (global) {
        function _ss() {
            var versionNumber = "1.1.0";

        #include "System\Primitives.js"
        #include "System\Delegate.js"
        #include "System\Guid.js"
        #include "System\Interfaces.js"
        #include "System\String.js"
        #include "System\Random.js"
        #include "System\EventArgs.js"

        #include "System\ComponentModel\Interfaces.js"
        #include "System\ComponentModel\CancelEventArgs.js"

        #include "System\Collections\Generic\Dictionary.js"
        #include "System\Collections\Generic\Queue.js"
        #include "System\Collections\Generic\Stack.js"
        #include "System\Collections\Generic\Interfaces.js"

        #include "System\Reflection\Assembly.js"

        #include "System\Runtime\CompilerServices\HashCode.js"

        #include "System\Text\StringBuilder.js"

        #include "Runtime\Collections.js"
        #include "Runtime\Culture.js"
        #include "Runtime\Format.js"
        #include "Runtime\Misc.js"
        #include "Runtime\TypeSystem.js"

        var ns_System = "System";
        var ns_System$Reflection = "System.Reflection";
        var ns_System$Collections$Generic = "System.Collections.Generic";
        var ns_System$ComponentModel = "System.ComponentModel";
        var ns_System$Runtime$CompilerServices = "System.Runtime.CompilerServices";
        var ns_System$Text = "System.Text";

        var exports = {
            Assembly: defineClass(Assembly, Assembly$, [], null, [], ns_System$Reflection),
            AssemblyName: defineClass(AssemblyName, AssemblyName$, [], null, [], ns_System$Reflection),
            //TODO: Move to System.ComponentModel folder
            CancelEventArgs: defineClass(CancelEventArgs, {}, [], null, ns_System$ComponentModel),
            Dictionary: defineClass(Dictionary, Dictionary$, [], null, [IReadOnlyDictionary, IDictionary], ns_System$Collections$Generic),
            EventArgs: defineClass(EventArgs, {}, [], null, ns_System),
            Guid: defineClass(Guid, Guid$, [], null, [IEquatable], ns_System),
            ICollection: defineInterface(ICollection, null, ns_System$Collections$Generic),
            IContainer: defineInterface(IContainer, null, ns_System$ComponentModel),
            IDictionary: defineInterface(IDictionary, [ICollection], ns_System$Collections$Generic),
            IDisposable: defineInterface(IDisposable, null, ns_System),
            IEnumerable: defineInterface(IEnumerable, null, ns_System),
            IEnumerator: defineInterface(IEnumerator, null, ns_System),
            IEquatable: defineInterface(IEquatable, null, ns_System),
            IEqualityComparer: defineInterface(IEqualityComparer, null, ns_System$Collections$Generic),
            IList: defineInterface(IList, [IReadOnlyList, ICollection], ns_System$Collections$Generic),
            IObserver: defineInterface(IObserver, null, ns_System),
            IReadOnlyCollection: defineInterface(IReadOnlyCollection, [IEnumerable], ns_System$Collections$Generic),
            IReadOnlyDictionary: defineInterface(IReadOnlyDictionary, [IReadOnlyCollection], ns_System$Collections$Generic),
            IReadOnlyList: defineInterface(IReadOnlyList, [IReadOnlyCollection], ns_System$Collections$Generic),
            Queue: defineClass(Queue, Queue$, [], null, [ICollection], ns_System$Collections$Generic),
            Stack: defineClass(Stack, Stack$, [], null, [ICollection], ns_System$Collections$Generic),
            StringBuilder: defineClass(StringBuilder, StringBuilder$, [], null, ns_System$Text),
            Random: defineClass(Random, Random$, [], null, [], ns_System),
            Version: defineClass(Version, Version$, [], null, [], ns_System)
        };

        var extensions = {
            version: versionNumber,

            isValue: isValue,
            value: value,
            extend: extend,
            keys: keys,
            keyCount: keyCount,
            keyExists: keyExists,
            clearKeys: clearKeys,
            enumerate: enumerate,
            array: toArray,
            remove: removeItem,
            boolean: parseBoolean,
            regexp: parseRegExp,
            number: parseNumber,
            date: parseDate,
            truncate: truncate,
            now: now,
            today: today,
            compareDates: compareDates,
            string: string,
            emptyString: emptyString,
            whitespace: whitespace,
            format: format,
            compareStrings: compareStrings,
            startsWith: startsWith,
            endsWith: endsWith,
            padLeft: padLeft,
            padRight: padRight,
            trim: trim,
            trimStart: trimStart,
            trimEnd: trimEnd,
            insertString: insertString,
            removeString: removeString,
            replaceString: replaceString,
            bind: bind,
            bindAdd: bindAdd,
            bindSub: bindSub,
            bindExport: bindExport,
            paramsGenerator: paramsGenerator,
            createPropertyGet: createPropertyGet,
            createPropertySet: createPropertySet,

            module: module,
            modules: _modules,

            isClass: isClass,
            isInterface: isInterface,
            typeOf: typeOf,
            type: type,
            typeName: typeName,
            canCast: canCast,
            safeCast: safeCast,
            canAssign: canAssign,
            instanceOf: instanceOf,
            baseProperty: baseProperty,
            defineClass: defineClass,
            defineInterface: defineInterface,
            getConstructorParams: getConstructorParams,
            createInstance: paramsGenerator(1, createInstance),

            hash: hashString,
            getHashCode: hashObject,

            culture: {
                neutral: neutralCulture,
                current: currentCulture
            },

            fail: fail
        }

        return extend(module('ss', versionNumber, null, exports), extensions);
    }


    function _export() {
        var ss = _ss();
        typeof exports == 'object' ? ss.extend(exports, ss) : global.ss = ss;
    }

    global.define ? global.define('ss', [], _ss) : _export();
})(this);
