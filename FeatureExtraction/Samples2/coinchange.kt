// Coin change algorithm
fun min(a: Int, b: Int): Int {
    if (a < b) {
        return a
    } else {
        return b
    }
}

fun coinchange(coins: Array<Int>, amount: Int): Int {
    val maxval: Int = amount + 1
    val dp: Array<Int> = Array(amount + 1) { maxval }
    dp[0] = 0
    var c: Int = 0
    while (c < coins.size) {
        var a: Int = coins[c]
        while (a <= amount) {
            dp[a] = min(dp[a], dp[a - coins[c]] + 1)
            a = a + 1
        }
        c = c + 1
    }
    if (dp[amount] > amount) {
        return -1
    }
    return dp[amount]
}