//Source from: https://github.com/garycourt/murmurhash-js/blob/master/murmurhash2_gc.js
function murmurhash2_32_gc(str, seed) {
    var l = str.length,
        h = (seed || 0) ^ l,
        i = 0,
        k;

    while (l >= 4) {
        k =
            ((str.charCodeAt(i) & 0xff)) |
            ((str.charCodeAt(++i) & 0xff) << 8) |
            ((str.charCodeAt(++i) & 0xff) << 16) |
            ((str.charCodeAt(++i) & 0xff) << 24);

        k = (((k & 0xffff) * 0x5bd1e995) + ((((k >>> 16) * 0x5bd1e995) & 0xffff) << 16));
        k ^= k >>> 24;
        k = (((k & 0xffff) * 0x5bd1e995) + ((((k >>> 16) * 0x5bd1e995) & 0xffff) << 16));

        h = (((h & 0xffff) * 0x5bd1e995) + ((((h >>> 16) * 0x5bd1e995) & 0xffff) << 16)) ^ k;

        l -= 4;
        ++i;
    }

    switch (l) {
        case 3: h ^= (str.charCodeAt(i + 2) & 0xff) << 16;
        case 2: h ^= (str.charCodeAt(i + 1) & 0xff) << 8;
        case 1: h ^= (str.charCodeAt(i) & 0xff);
            h = (((h & 0xffff) * 0x5bd1e995) + ((((h >>> 16) * 0x5bd1e995) & 0xffff) << 16));
    }

    h ^= h >>> 13;
    h = (((h & 0xffff) * 0x5bd1e995) + ((((h >>> 16) * 0x5bd1e995) & 0xffff) << 16));
    h ^= h >>> 15;

    return h >>> 0;
}

var hashString = function (str) {
    return murmurhash2_32_gc(str);
}

var hashObject = function (obj) {
    return resolveHashCodeFunction(obj)(obj);
}

function defaultHashCodeFunction() {
    return 0;
}

function hashObjectToString(obj) {
    return hashString(obj.toString());
}

function resolveHashCodeFunction(obj) {
    if (obj == undefined) {
        return defaultHashCodeFunction;
    }

    var name = typeName(obj);

    var primitiveHashCodeFunction = primitiveHashCodeLookup[name];
    if (primitiveHashCodeFunction) {
        return primitiveHashCodeFunction;
    }

    var objGetHashCode = obj["getHashCode"];
    if (objGetHashCode) {
        return function (obj) {
            return obj.getHashCode();
        };
    }

    return defaultHashCodeFunction;
}

var primitiveHashCodeLookup = {
    Number: hashObjectToString,
    Boolean: hashObjectToString,
    Function: hashObjectToString,
    RegExp: hashObjectToString,
    String: function (str) {
        return hashString(str);
    },
    Object: function (obj) {
        var serialisedObject = JSON.stringify(obj) || "";
        return hashString(serialisedObject);
    },
    Date: function (date) {
        var jsonDate = date.toJSON();
        return hashString(jsonDate);
    },
    Array: function (arr) {
        //BUG: Currently returns negative hash codes. Need to fix!
        function combineHashCodes(hash1, hash2) {
            var hash = 5381;
            hash = ((hash << 5) + hash) ^ hash1;
            hash = ((hash << 5) + hash) ^ hash2;
            return hash >>> 0;
        }

        var hashCode = 0;
        for (var i = 0, ln = arr.length; i < ln; i++) {
            var element = arr[i];
            var getElementHashCode = resolveHashCodeFunction(element);
            hashCode = combineHashCodes(hashCode, getElementHashCode(element));
        }

        return hashCode;
    }
};