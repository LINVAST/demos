public class coinchange {
    static int min(int a, int b) {
        if (a < b) {
            return a;
        } else {
            return b;
        }
    }

    static int coinchange(int[] coins, int amount) {
        int maxval = amount + 1;
        int[] dp = new int[amount + 1];
        for (int i = 0; i <= amount; i++) {
            dp[i] = maxval;
        }
        dp[0] = 0;
        for (int c = 0; c < coins.length; c++) {
            int a = coins[c];
            while (a <= amount) {
                dp[a] = min(dp[a], dp[a - coins[c]] + 1);
                a = a + 1;
            }
        }
        if (dp[amount] > amount) {
            return -1;
        }
        return dp[amount];
    }
}