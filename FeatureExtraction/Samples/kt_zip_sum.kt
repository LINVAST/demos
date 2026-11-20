fun kt_zip_sum(a: Array<Int>, b: Array<Int>): Int {
    var total = 0
    var n = a.size
    if (b.size < n) {
        n = b.size
    }
    for (i in 0 until n) {
        total = total + a[i] + b[i]
    }
    return total
}
