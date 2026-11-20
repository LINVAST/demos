-- Longest common subsequence algorithm

local function max(a, b)
    if a > b then
        return a
    else
        return b
    end
end

local function lcs(a, b)
    local m = #a
    local n = #b
    local dp = {}
    local i = 0
    while i <= m do
        dp[i] = {}
        local j = 0
        while j <= n do
            dp[i][j] = 0
            j = j + 1
        end
        i = i + 1
    end
    i = 1
    while i <= m do
        local j = 1
        while j <= n do
            if a[i] == b[j] then
                dp[i][j] = dp[i - 1][j - 1] + 1
            else
                dp[i][j] = max(dp[i - 1][j], dp[i][j - 1])
            end
            j = j + 1
        end
        i = i + 1
    end
    return dp[m][n]
end