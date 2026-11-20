public class JavaCountNegative {
    public static int java_count_negative(int[] arr) {
        int count = 0;
        for (int v : arr) {
            if (v < 0) {
                count = count + 1;
            }
        }
        return count;
    }
}
