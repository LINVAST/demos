public class JavaCountEven {
    public static int java_count_even(int[] arr) {
        int count = 0;
        for (int v : arr) {
            if (v % 2 == 0) {
                count = count + 1;
            }
        }
        return count;
    }
}
