-- Knapsack problem using dynamic programming

local function max(a, b)
    if a > b then
        return a
    else
        return b
    end
end

local function knapsack(values, weights, capacity)
    local n = #values
    local dp = {}
    for i = 0, n do
        dp[i] = {}
        for w = 0, capacity do
            dp[i][w] = 0
        end
    end
    for i = 1, n do
        for w = 0, capacity do
            if weights[i] <= w then
                dp[i][w] = max(dp[i-1][w], dp[i-1][w-weights[i]] + values[i])
            else
                dp[i][w] = dp[i-1][w]
            end
        end
    end
    return dp[n][capacity]
end
