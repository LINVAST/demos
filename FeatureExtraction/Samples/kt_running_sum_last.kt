fun kt_running_sum_last(arr: Array<Int>): Int {
    var total = 0
    for (v in arr) {
        total = total + v
    }
    return total
}
