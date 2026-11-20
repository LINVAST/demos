public class JavaFindLastIndex {
    public static int java_find_last_index(int[] arr, int value) {
        for (int i = arr.length - 1; i >= 0; i = i - 1) {
            if (arr[i] == value) {
                return i;
            }
        }
        return -1;
    }
}
