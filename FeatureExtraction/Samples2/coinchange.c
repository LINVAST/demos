static int min(int a, int b) {
    if (a < b) {
        return a;
    } else {
        return b;
    }
}

static int coinchange(int coins[], int ncoins, int amount) {
    int dp[100];
    int i = 0;
    while (i <= amount) {
        dp[i] = amount + 1;
        i = i + 1;
    }
    dp[0] = 0;
    int c = 0;
    while (c < ncoins) {
        int a = coins[c];
        while (a <= amount) {
            dp[a] = min(dp[a], dp[a - coins[c]] + 1);
            a = a + 1;
        }
        c = c + 1;
    }
    if (dp[amount] > amount) {
        return -1;
    }
    return dp[amount];
}