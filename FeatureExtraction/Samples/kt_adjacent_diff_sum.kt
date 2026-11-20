fun kt_adjacent_diff_sum(arr: Array<Int>): Int {
    var total = 0
    for (i in 1 until arr.size) {
        val d = arr[i] - arr[i - 1]
        if (d < 0) {
            total = total - d
        } else {
            total = total + d
        }
    }
    return total
}
