def py_max_profit(prices):
    if len(prices) == 0:
        return 0
    min_price = prices[0]
    max_profit = 0
    for v in prices:
        if v < min_price:
            min_price = v
        profit = v - min_price
        if profit > max_profit:
            max_profit = profit
    return max_profit


