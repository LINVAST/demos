public class JavaHasAdjacentDup {
    public static boolean java_has_adjacent_dup(int[] arr) {
        for (int i = 0; i < arr.length - 1; i = i + 1) {
            if (arr[i] == arr[i + 1]) {
                return true;
            }
        }
        return false;
    }
}
