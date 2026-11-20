fun kt_running_min_last(arr: Array<Int>): Int {
    if (arr.size == 0) {
        return 0
    }
    var best = arr[0]
    for (v in arr) {
        if (v < best) {
            best = v
        }
    }
    return best
}
