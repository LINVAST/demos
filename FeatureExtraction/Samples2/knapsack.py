def max(a: int, b: int) -> int:
    if a > b:
        return a
    else:
        return b


def knapsack(values: list, weights: list, capacity: int) -> int:
    n = len(values)
    dp = []
    for i in range(n + 1):
        row = []
        for w in range(capacity + 1):
            row.append(0)
        dp.append(row)
    for i in range(1, n + 1):
        for w in range(capacity + 1):
            if weights[i - 1] <= w:
                dp[i][w] = max(dp[i - 1][w], dp[i - 1][w - weights[i - 1]] + values[i - 1])
            else:
                dp[i][w] = dp[i - 1][w]
    return dp[n][capacity]