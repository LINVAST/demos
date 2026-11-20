public class JavaArrayAverage {
    public static int java_array_average(int[] arr) {
        int total = 0;
        for (int v : arr) {
            total = total + v;
        }
        if (arr.length == 0) {
            return 0;
        }
        return total / arr.length;
    }
}
