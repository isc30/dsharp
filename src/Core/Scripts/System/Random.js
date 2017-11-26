function Random(seed){
    this.seed = seed || Date.now();
}

var Random$ = {
    nextInt: function (min, max) {
        min = Math.ceil(min || 0);
        max = Math.floor(max || Number.MAX_INT_VALUE);

        return Math.floor(Math.random() * (max - min + 1)) + min;
    },
    nextIntMax: function (max) {
        return this.nextInt(null, max);
    },
    nextDouble: function () {
        return Math.random();
    }
}