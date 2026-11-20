// Bubble sort algorithm
fun swap(arr: Array<Int>, i: Int, j: Int) {
    val temp: Int = arr[i]
    arr[i] = arr[j]
    arr[j] = temp
}

fun bubblesort(arr: Array<Int>) {
    val n: Int = arr.size
    var i: Int = 1
    var swapped: Boolean = true
    while (i < n && swapped) {
        swapped = false
        var j: Int = 1
        while (j <= n - i) {
            if (arr[j - 1] > arr[j]) {
                swap(arr, j - 1, j)
                swapped = true
            }
            j = j + 1
        }
        i = i + 1
    }
}