-- Coin change algorithm

local function min(a, b)
    if a < b then
        return a
    else
        return b
    end
end

local function coinchange(coins, amount)
    local maxval = amount + 1
    local dp = {}
    local i = 0
    while i <= amount do
        dp[i] = maxval
        i = i + 1
    end
    dp[0] = 0
    local c = 1
    while c <= #coins do
        local a = coins[c]
        while a <= amount do
            dp[a] = min(dp[a], dp[a - coins[c]] + 1)
            a = a + 1
        end
        c = c + 1
    end
    if dp[amount] > amount then
        return -1
    end
    return dp[amount]
end