public class JavaArrayMin {
    public static int java_array_min(int[] arr) {
        int best = arr[0];
        for (int v : arr) {
            if (v < best) {
                best = v;
            }
        }
        return best;
    }
}
