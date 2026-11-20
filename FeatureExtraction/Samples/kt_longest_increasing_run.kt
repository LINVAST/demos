fun kt_longest_increasing_run(arr: Array<Int>): Int {
    if (arr.size == 0) {
        return 0
    }
    var best = 1
    var cur = 1
    for (i in 1 until arr.size) {
        if (arr[i] > arr[i - 1]) {
            cur = cur + 1
        } else {
            cur = 1
        }
        if (cur > best) {
            best = cur
        }
    }
    return best
}
