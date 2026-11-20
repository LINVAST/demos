fun kt_running_product_last(arr: Array<Int>): Int {
    var total = 1
    for (v in arr) {
        total = total * v
    }
    return total
}
