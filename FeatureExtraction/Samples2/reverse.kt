// Reverse array algorithm
fun reverse(arr: Array<Int>) {
    var left: Int = 0
    var right: Int = arr.size - 1
    while (left < right) {
        val temp: Int = arr[left]
        arr[left] = arr[right]
        arr[right] = temp
        left = left + 1
        right = right - 1
    }
}