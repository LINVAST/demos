public class JavaMoveZerosEnd {
    public static void java_move_zeros_end(int[] arr) {
        int write = 0;
        for (int i = 0; i < arr.length; i = i + 1) {
            if (arr[i] != 0) {
                arr[write] = arr[i];
                write = write + 1;
            }
        }
        while (write < arr.length) {
            arr[write] = 0;
            write = write + 1;
        }
    }
}
