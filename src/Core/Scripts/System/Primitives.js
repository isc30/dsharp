Object.defineProperties(Number, {
    "MIN_SHORT_VALUE": {
        value: 0 - 0x8000
    },
    "MAX_SHORT_VALUE": {
        value: 0x7FFF
    },
    "MAX_USHORT_VALUE": {
        value: 0xFFFF
    },
    "MIN_INT_VALUE": {
        value: 0x80000000 | 0
    },
    "MAX_INT_VALUE": {
        value: 0x7fffffff
    },
    "MAX_UINT_VALUE": {
        value: 0xffffffff
    }
});

