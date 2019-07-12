function toArray(obj) {
    return obj
        ? typeof obj == "string"
            ? JSON.parse("(" + obj + ")")
            : Array.prototype.slice.call(obj)
        : null;
}
function removeItem(a, item) {
    var index = a.indexOf(item);
    return index >= 0
        ? (a.splice(index, 1), true)
        : false;
}
function clearKeys(obj) {
    for (var key in obj) {
        delete obj[key];
    }
}
function keyExists(obj, key) {
    return obj[key] !== undefined;
}
function keyValueExists(obj, keyValue) {
    return obj[keyValue.key] === keyValue.value;
}
function keys(obj) {
    if (Object.keys) {
        return Object.keys(obj);
    }
    var keys = [];
    for (var key in obj) {
        keys.push(key);
    }
    return keys;
}

function values(obj) {
    if (Object.values) {
        return Object.values(obj);
    }
    var values = [];
    for (var key in obj) {
        values.push(obj[key]);
    }
    return values;
}

function keyCount(obj) {
    return keys(obj).length;
}