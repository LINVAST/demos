static void c_selection_sort(int arr[], int n) {
    int i = 0;
    while (i < n - 1) {
        int min_idx = i;
        int j = i + 1;
        while (j < n) {
            if (arr[j] < arr[min_idx]) {
                min_idx = j;
            }
            j = j + 1;
        }
        int temp = arr[min_idx];
        arr[min_idx] = arr[i];
        arr[i] = temp;
        i = i + 1;
    }
}
