// Knapsack algorithm
fun max(a: Int, b: Int): Int {
    if (a > b) {
        return a
    } else {
        return b
    }
}

fun knapsack(values: Array<Int>, weights: Array<Int>, capacity: Int): Int {
    val n: Int = values.size
    val dp: Array<Array<Int>> = Array(n + 1) { Array(capacity + 1) { 0 } }
    var i: Int = 1
    while (i <= n) {
        var w: Int = 0
        while (w <= capacity) {
            if (weights[i - 1] <= w) {
                dp[i][w] = max(dp[i - 1][w], dp[i - 1][w - weights[i - 1]] + values[i - 1])
            } else {
                dp[i][w] = dp[i - 1][w]
            }
            w = w + 1
        }
        i = i + 1
    }
    return dp[n][capacity]
}