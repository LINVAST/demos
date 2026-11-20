fun kt_equilibrium_index(arr: Array<Int>): Int {
    var total = 0
    for (v in arr) {
        total = total + v
    }
    var left = 0
    for (i in arr.indices) {
        var right = total - left - arr[i]
        if (left == right) {
            return i
        }
        left = left + arr[i]
    }
    return -1
}
