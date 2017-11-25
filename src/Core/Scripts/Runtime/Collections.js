// Collections

function toArray(obj) {
  return obj ? (typeof obj == 'string' ? JSON.parse('(' + obj + ')') : Array.prototype.slice.call(obj)) : null;
}
function removeItem(a, item) {
    var index = a.indexOf(item);
    return index >= 0 ? (a.splice(index, 1), true) : false;
}

function clearKeys(obj) {
  for (var key in obj) {
    delete obj[key];
  }
}

function getValues(obj) {
    return Object.values(obj);
}

function keyExists(obj, key) {
  return obj[key] !== undefined;
}

function keys(obj) {
    return Object.keys(obj);
}

function keyCount(obj) {
  return keys(obj).length;
}

function removeKey(obj, key) {
    var value = obj[key] !== undefined;
    delete obj[key];
    return !!value;
}

function Enumerator(obj, keys) {
  var index = -1;
  var length = keys ? keys.length : obj.length;
  var lookup = keys ? function() { return { key: keys[index], value: obj[keys[index]] }; } :
                      function() { return obj[index]; };

  this.current = null;
  this.moveNext = function() {
    index++;
    this.current = lookup();
    return index < length;
  };
  this.reset = function() {
    index = -1;
    this.current = null;
  };
}
var _nopEnumerator = {
  current: null,
  moveNext: function() { return false; },
  reset: _nop
};

function enumerate(o) {
  if (!isValue(o)) {
    return _nopEnumerator;
  }
  if (o.getEnumerator) {
    return o.getEnumerator();
  }
  if (o.length !== undefined) {
    return new Enumerator(o);
  }
  return new Enumerator(o, keys(o));
}

