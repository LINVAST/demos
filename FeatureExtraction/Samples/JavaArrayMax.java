public class JavaArrayMax {
    public static int java_array_max(int[] arr) {
        int best = arr[0];
        for (int v : arr) {
            if (v > best) {
                best = v;
            }
        }
        return best;
    }
}
