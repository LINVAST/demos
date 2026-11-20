public class JavaShiftRight {
    public static void java_shift_right(int[] arr) {
        if (arr.length <= 1) {
            return;
        }
        int last = arr[arr.length - 1];
        for (int i = arr.length - 1; i > 0; i = i - 1) {
            arr[i] = arr[i - 1];
        }
        arr[0] = last;
    }
}
