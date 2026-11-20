public class JavaCountZero {
    public static int java_count_zero(int[] arr) {
        int count = 0;
        for (int v : arr) {
            if (v == 0) {
                count = count + 1;
            }
        }
        return count;
    }
}
