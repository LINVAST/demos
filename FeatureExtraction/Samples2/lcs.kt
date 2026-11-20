// Longest common subsequence algorithm
fun max(a: Int, b: Int): Int {
    if (a > b) {
        return a
    } else {
        return b
    }
}

fun lcs(a: Array<Int>, b: Array<Int>): Int {
    val m: Int = a.size
    val n: Int = b.size
    val dp: Array<Array<Int>> = Array(m + 1) { Array(n + 1) { 0 } }
    var i: Int = 1
    while (i <= m) {
        var j: Int = 1
        while (j <= n) {
            if (a[i - 1] == b[j - 1]) {
                dp[i][j] = dp[i - 1][j - 1] + 1
            } else {
                dp[i][j] = max(dp[i - 1][j], dp[i][j - 1])
            }
            j = j + 1
        }
        i = i + 1
    }
    return dp[m][n]
}