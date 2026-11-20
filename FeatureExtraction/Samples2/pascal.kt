// Pascal triangle algorithm
fun pascal(rows: Int): Array<Int> {
    val triangle: Array<Array<Int>> = Array(rows + 1) { Array(rows + 1) { 0 } }
    var i: Int = 1
    while (i <= rows) {
        var j: Int = 1
        while (j <= i) {
            if (j == 1 || j == i) {
                triangle[i][j] = 1
            } else {
                val prev: Array<Int> = triangle[i - 1]
                triangle[i][j] = prev[j - 1] + prev[j]
            }
            j = j + 1
        }
        i = i + 1
    }
    return triangle[rows]
}