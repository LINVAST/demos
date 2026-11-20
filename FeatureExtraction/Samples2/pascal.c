static void pascal(int rows, int out[]) {
    int triangle[10][10];
    int i = 1;
    while (i <= rows) {
        int j = 1;
        while (j <= i) {
            if (j == 1 || j == i) {
                triangle[i][j] = 1;
            } else {
                triangle[i][j] = triangle[i - 1][j - 1] + triangle[i - 1][j];
            }
            j = j + 1;
        }
        i = i + 1;
    }
    int k = 1;
    while (k <= rows) {
        out[k - 1] = triangle[rows][k];
        k = k + 1;
    }
}