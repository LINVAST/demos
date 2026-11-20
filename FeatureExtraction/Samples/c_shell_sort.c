static void c_shell_sort(int arr[], int n) {
    int gap = n / 2;
    while (gap > 0) {
        int i = gap;
        while (i < n) {
            int temp = arr[i];
            int j = i;
            while (j >= gap && arr[j - gap] > temp) {
                arr[j] = arr[j - gap];
                j = j - gap;
            }
            arr[j] = temp;
            i = i + 1;
        }
        gap = gap / 2;
    }
}
