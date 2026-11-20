fun kt_contains_duplicate(arr: Array<Int>): Boolean {
    for (i in arr.indices) {
        for (j in i + 1 until arr.size) {
            if (arr[i] == arr[j]) {
                return true
            }
        }
    }
    return false
}
