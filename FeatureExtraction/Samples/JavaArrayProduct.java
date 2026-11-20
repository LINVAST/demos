public class JavaArrayProduct {
    public static int java_array_product(int[] arr) {
        int total = 1;
        for (int v : arr) {
            total = total * v;
        }
        return total;
    }
}
