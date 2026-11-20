public class JavaShiftLeft {
    public static void java_shift_left(int[] arr) {
        if (arr.length <= 1) {
            return;
        }
        int first = arr[0];
        for (int i = 0; i < arr.length - 1; i = i + 1) {
            arr[i] = arr[i + 1];
        }
        arr[arr.length - 1] = first;
    }
}
