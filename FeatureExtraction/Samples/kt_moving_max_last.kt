fun kt_moving_max_last(arr: Array<Int>, k: Int): Int {
    if (arr.size == 0) {
        return 0
    }
    var start = arr.size - k
    if (start < 0) {
        start = 0
    }
    var best = arr[start]
    for (i in start until arr.size) {
        if (arr[i] > best) {
            best = arr[i]
        }
    }
    return best
}
