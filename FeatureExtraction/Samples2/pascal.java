public class pascal {
    static int[] pascal(int rows) {
        int[][] triangle = new int[rows + 1][];
        for (int i = 1; i <= rows; i++) {
            int[] row = new int[i + 1];
            for (int j = 1; j <= i; j++) {
                if (j == 1 || j == i) {
                    row[j] = 1;
                } else {
                    int[] prev = triangle[i - 1];
                    row[j] = prev[j - 1] + prev[j];
                }
            }
            triangle[i] = row;
        }
        return triangle[rows];
    }
}