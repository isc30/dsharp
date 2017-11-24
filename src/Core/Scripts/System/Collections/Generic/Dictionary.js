function Dictionary() {
    var instance = {};

    createPropertyGet(this, "values", function () {
        return Object.values(instance);
    });

    createPropertyGet(this, "keys", function () {
        return Object.keys(instance);
    });
}

var Dictionary$ = {
    add: function (key, value) {
    },
    remove: function (key) {
    },
    clear: function () {
    },
    contains: function (key) {
    },
    tryGetValue: function (key, valueContainer) {
    }
}