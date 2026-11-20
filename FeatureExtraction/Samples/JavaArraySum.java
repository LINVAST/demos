public class JavaArraySum {
    public static int java_array_sum(int[] arr) {
        int total = 0;
        for (int v : arr) {
            total = total + v;
        }
        return total;
    }
}
