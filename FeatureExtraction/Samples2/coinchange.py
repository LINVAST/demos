def min(a: int, b: int) -> int:
    if a < b:
        return a
    else:
        return b


def coinchange(coins: list, amount: int) -> int:
    maxval = amount + 1
    dp = []
    i = 0
    while i <= amount:
        dp.append(maxval)
        i = i + 1
    dp[0] = 0
    for c in range(len(coins)):
        a = coins[c]
        while a <= amount:
            dp[a] = min(dp[a], dp[a - coins[c]] + 1)
            a = a + 1
    if dp[amount] > amount:
        return -1
    return dp[amount]