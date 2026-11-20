static void c_quicksort(int arr[], int low, int high) {
    if (low < high) {
        int pivot = arr[high];
        int i = low - 1;
        int j = low;
        while (j < high) {
            if (arr[j] <= pivot) {
                i = i + 1;
                int temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
            }
            j = j + 1;
        }
        int temp = arr[i + 1];
        arr[i + 1] = arr[high];
        arr[high] = temp;
        int pi = i + 1;
        c_quicksort(arr, low, pi - 1);
        c_quicksort(arr, pi + 1, high);
    }
}
