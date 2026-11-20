static void swap(int arr[], int i, int j) {
    int temp = arr[i];
    arr[i] = arr[j];
    arr[j] = temp;
}

static void bubblesort(int arr[], int n) {
    int i = 1;
    int swapped = 1;
    while (i < n && swapped) {
        swapped = 0;
        int j = 1;
        while (j <= n - i) {
            if (arr[j - 1] > arr[j]) {
                swap(arr, j - 1, j);
                swapped = 1;
            }
            j = j + 1;
        }
        i = i + 1;
    }
}