static int max(int a, int b) {
    if (a > b) {
        return a;
    } else {
        return b;
    }
}

static int knapsack(int values[], int weights[], int n, int capacity) {
    int dp[10][10];
    int i = 0;
    while (i <= n) {
        int w = 0;
        while (w <= capacity) {
            dp[i][w] = 0;
            w = w + 1;
        }
        i = i + 1;
    }
    i = 1;
    while (i <= n) {
        int w = 0;
        while (w <= capacity) {
            if (weights[i - 1] <= w) {
                dp[i][w] = max(dp[i - 1][w], dp[i - 1][w - weights[i - 1]] + values[i - 1]);
            } else {
                dp[i][w] = dp[i - 1][w];
            }
            w = w + 1;
        }
        i = i + 1;
    }
    return dp[n][capacity];
}