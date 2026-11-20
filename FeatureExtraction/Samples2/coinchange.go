package main

func min(a int, b int) int {
	if a < b {
		return a
	}
	return b
}

func coinchange(coins []int, amount int) int {
	maxval := amount + 1
	dp := make([]int, amount+1)
	for i := 0; i <= amount; i++ {
		dp[i] = maxval
	}
	dp[0] = 0
	for c := 0; c < len(coins); c++ {
		a := coins[c]
		for a <= amount {
			dp[a] = min(dp[a], dp[a-coins[c]]+1)
			a = a + 1
		}
	}
	if dp[amount] > amount {
		return -1
	}
	return dp[amount]
}