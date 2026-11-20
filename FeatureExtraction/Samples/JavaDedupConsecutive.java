public class JavaDedupConsecutive {
    public static int java_dedup_consecutive(int[] arr) {
        if (arr.length == 0) {
            return 0;
        }
        int write = 1;
        for (int i = 1; i < arr.length; i = i + 1) {
            if (arr[i] != arr[write - 1]) {
                arr[write] = arr[i];
                write = write + 1;
            }
        }
        return write;
    }
}
