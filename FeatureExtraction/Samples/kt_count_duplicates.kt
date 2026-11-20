fun kt_count_duplicates(arr: Array<Int>): Int {
    var count = 0
    for (i in arr.indices) {
        for (j in i + 1 until arr.size) {
            if (arr[i] == arr[j]) {
                count = count + 1
            }
        }
    }
    return count
}
