public class JavaSecondLargest {
    public static int java_second_largest(int[] arr) {
        int first = arr[0];
        int second = arr[0];
        for (int v : arr) {
            if (v > first) {
                second = first;
                first = v;
            } else if (v > second && v < first) {
                second = v;
            }
        }
        return second;
    }
}
