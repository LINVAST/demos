fun kt_alternating_sum(arr: Array<Int>): Int {
    var total = 0
    var sign = 1
    for (v in arr) {
        total = total + sign * v
        sign = -sign
    }
    return total
}
