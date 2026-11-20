fun kt_convolution_sum(a: Array<Int>, b: Array<Int>): Int {
    var total = 0
    for (i in a.indices) {
        for (j in b.indices) {
            total = total + a[i] * b[j]
        }
    }
    return total
}
