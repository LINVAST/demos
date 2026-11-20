static int max(int a, int b) {
    if (a > b) {
        return a;
    } else {
        return b;
    }
}

static int lcs(int a[], int b[], int m, int n) {
    int dp[10][10];
    int i = 0;
    while (i <= m) {
        int j = 0;
        while (j <= n) {
            dp[i][j] = 0;
            j = j + 1;
        }
        i = i + 1;
    }
    i = 1;
    while (i <= m) {
        int j = 1;
        while (j <= n) {
            if (a[i - 1] == b[j - 1]) {
                dp[i][j] = dp[i - 1][j - 1] + 1;
            } else {
                dp[i][j] = max(dp[i - 1][j], dp[i][j - 1]);
            }
            j = j + 1;
        }
        i = i + 1;
    }
    return dp[m][n];
}