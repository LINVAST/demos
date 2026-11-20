public class bubblesort {
    static void swap(int[] arr, int i, int j) {
        int temp = arr[i];
        arr[i] = arr[j];
        arr[j] = temp;
    }

    static void bubblesort(int[] arr) {
        int n = arr.length;
        int i = 1;
        boolean swapped = true;
        while (i < n && swapped) {
            swapped = false;
            int j = 1;
            while (j <= n - i) {
                if (arr[j - 1] > arr[j]) {
                    swap(arr, j - 1, j);
                    swapped = true;
                }
                j = j + 1;
            }
            i = i + 1;
        }
    }
}