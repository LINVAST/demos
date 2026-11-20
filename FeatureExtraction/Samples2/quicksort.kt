// Quicksort algorithm
fun swap(arr: Array<Int>, i: Int, j: Int) {
    val temp: Int = arr[i]
    arr[i] = arr[j]
    arr[j] = temp
}

fun partition(arr: Array<Int>, low: Int, high: Int): Int {
    val pivot: Int = arr[high]
    var i: Int = low - 1
    var j: Int = low
    while (j <= high - 1) {
        if (arr[j] < pivot) {
            i = i + 1
            swap(arr, i, j)
        }
        j = j + 1
    }
    swap(arr, i + 1, high)
    return i + 1
}

fun quicksort(arr: Array<Int>, low: Int, high: Int) {
    if (low < high) {
        val pi: Int = partition(arr, low, high)
        quicksort(arr, low, pi - 1)
        quicksort(arr, pi + 1, high)
    }
}