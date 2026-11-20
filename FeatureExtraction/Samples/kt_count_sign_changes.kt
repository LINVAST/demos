fun kt_count_sign_changes(arr: Array<Int>): Int {
    var count = 0
    for (i in 1 until arr.size) {
        var prevNeg = arr[i - 1] < 0
        var curNeg = arr[i] < 0
        if (prevNeg != curNeg) {
            count = count + 1
        }
    }
    return count
}
