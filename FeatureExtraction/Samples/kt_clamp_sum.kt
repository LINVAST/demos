fun kt_clamp_sum(arr: Array<Int>, lo: Int, hi: Int): Int {
    var total = 0
    for (v in arr) {
        var c = v
        if (c < lo) {
            c = lo
        }
        if (c > hi) {
            c = hi
        }
        total = total + c
    }
    return total
}
