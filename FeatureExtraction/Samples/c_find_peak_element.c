static int c_find_peak_element(int arr[], int n) {
    int i = 0;
    while (i < n) {
        int left_ok = (i == 0) || (arr[i] > arr[i - 1]);
        int right_ok = (i == n - 1) || (arr[i] > arr[i + 1]);
        if (left_ok && right_ok) {
            return i;
        }
        i = i + 1;
    }
    return -1;
}
