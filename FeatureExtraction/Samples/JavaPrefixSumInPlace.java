public class JavaPrefixSumInPlace {
    public static void java_prefix_sum_in_place(int[] arr) {
        for (int i = 1; i < arr.length; i = i + 1) {
            arr[i] = arr[i] + arr[i - 1];
        }
    }
}
