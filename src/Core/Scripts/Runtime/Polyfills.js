if (!Object.keys) {
    Object.keys = function (obj) {
        var keys = [];

        for (var key in obj) {
            keys.push(key);
        }

        return keys;
    }
}

if (!Object.values) {
    Object.values = function (obj) {
        var values = [];

        for (var key in obj) {
            values.push(obj[key]);
        }

        return values;
    }
}