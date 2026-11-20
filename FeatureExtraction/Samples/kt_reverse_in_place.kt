fun kt_reverse_in_place(arr: Array<Int>) {
    var i = 0
    var j = arr.size - 1
    while (i < j) {
        val temp = arr[i]
        arr[i] = arr[j]
        arr[j] = temp
        i = i + 1
        j = j - 1
    }
}
