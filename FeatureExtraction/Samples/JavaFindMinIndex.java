public class JavaFindMinIndex {
    public static int java_find_min_index(int[] arr) {
        int best = 0;
        for (int i = 1; i < arr.length; i = i + 1) {
            if (arr[i] < arr[best]) {
                best = i;
            }
        }
        return best;
    }
}
